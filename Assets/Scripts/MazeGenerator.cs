using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;

/// <summary>
/// Entry point: attach to an empty GameObject in a blank scene and press Play.
/// Generates a random perfect maze (DFS), bakes a NavMesh on the floor,
/// places fixed and mobile obstacles, then spawns the player and camera.
/// </summary>
public class MazeGenerator : MonoBehaviour
{
    [Header("Maze Size")]
    public int width  = 7;
    public int height = 7;
    public float cellSize = 5f;

    [Header("Walls")]
    public float wallHeight    = 3f;
    public float wallThickness = 0.3f;

    [Header("Obstacles")]
    [Min(5)] public int fixedObstacleCount  = 6;
    [Min(5)] public int mobileObstacleCount = 6;

    // true = wall present; hWalls[row, col], vWalls[row, col]
    private bool[,] hWalls;   // horizontal segments [0..height, 0..width-1]
    private bool[,] vWalls;   // vertical segments   [0..height-1, 0..width]
    private bool[,] visited;

    private readonly List<Vector3> cellCenters = new List<Vector3>();
    private Transform exitTriggerTransform;

    [HideInInspector] public Vector3 startPosition;

    // ─── Bootstrap ─────────────────────────────────────────────────────────────

    void Awake()
    {
        if (GameManager.Instance == null)
            gameObject.AddComponent<GameManager>();

        if (transform.childCount == 0)
            GenerateGeometry();

        BakeNavMesh();      // bake BEFORE obstacles/player exist
        PlaceObstacles();
        SpawnPlayer();
    }

    [ContextMenu("Gerar Labirinto")]
    public void GenerateGeometry()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
            DestroyImmediate(transform.GetChild(i).gameObject);

        cellCenters.Clear();
        InitWalls();
        CarveMaze();
        BuildMaze();
        PlaceEntrance();
        PlaceExit();
    }

    // ─── Maze Generation (iterative DFS) ───────────────────────────────────────

    void InitWalls()
    {
        hWalls  = new bool[height + 1, width];
        vWalls  = new bool[height, width + 1];
        visited = new bool[height, width];

        for (int r = 0; r <= height; r++)
            for (int c = 0; c < width; c++)
                hWalls[r, c] = true;

        for (int r = 0; r < height; r++)
            for (int c = 0; c <= width; c++)
                vWalls[r, c] = true;
    }

    void CarveMaze()
    {
        int[] dr = { -1, 0, 1, 0 };
        int[] dc = { 0, 1, 0, -1 };

        Stack<Vector2Int> stack = new Stack<Vector2Int>();
        visited[0, 0] = true;
        cellCenters.Add(CellCenter(0, 0));
        stack.Push(new Vector2Int(0, 0));

        while (stack.Count > 0)
        {
            Vector2Int cur = stack.Peek();
            int row = cur.x;
            int col = cur.y;
            bool moved = false;

            foreach (int d in RandomDirs())
            {
                int nr = row + dr[d];
                int nc = col + dc[d];
                if (nr < 0 || nr >= height || nc < 0 || nc >= width || visited[nr, nc]) continue;

                // Remove the shared wall
                if      (d == 0) hWalls[row,     col]     = false;
                else if (d == 1) vWalls[row,     col + 1] = false;
                else if (d == 2) hWalls[row + 1, col]     = false;
                else if (d == 3) vWalls[row,     col]     = false;

                visited[nr, nc] = true;
                cellCenters.Add(CellCenter(nr, nc));
                stack.Push(new Vector2Int(nr, nc));
                moved = true;
                break;
            }

            if (!moved) stack.Pop();
        }
    }

    static int[] RandomDirs()
    {
        int[] d = { 0, 1, 2, 3 };
        for (int i = 3; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            int tmp = d[i]; d[i] = d[j]; d[j] = tmp;
        }
        return d;
    }

    Vector3 CellCenter(int row, int col)
        => new Vector3(col * cellSize + cellSize * 0.5f, 0f, row * cellSize + cellSize * 0.5f);

    // ─── Physical Construction ──────────────────────────────────────────────────

    void BuildMaze()
    {
        Material wallMat  = CreateMat(new Color(0.38f, 0.38f, 0.42f));
        Material floorMat = CreateMat(new Color(0.58f, 0.50f, 0.38f));

        // Floor tiles
        for (int r = 0; r < height; r++)
            for (int c = 0; c < width; c++)
            {
                Vector3 cc = CellCenter(r, c);
                var floor = SpawnCube(new Vector3(cc.x, -0.1f, cc.z),
                                      new Vector3(cellSize, 0.2f, cellSize), floorMat);
                floor.name = "Floor";
                floor.transform.parent = transform;
                floor.isStatic = true;
            }

        // Horizontal wall segments (run along X, at each row boundary on Z)
        for (int r = 0; r <= height; r++)
            for (int c = 0; c < width; c++)
                if (hWalls[r, c])
                    SpawnWall(
                        new Vector3(c * cellSize + cellSize * 0.5f, wallHeight * 0.5f, r * cellSize),
                        new Vector3(cellSize + wallThickness, wallHeight, wallThickness),
                        wallMat);

        // Vertical wall segments (run along Z, at each column boundary on X)
        for (int r = 0; r < height; r++)
            for (int c = 0; c <= width; c++)
                if (vWalls[r, c])
                    SpawnWall(
                        new Vector3(c * cellSize, wallHeight * 0.5f, r * cellSize + cellSize * 0.5f),
                        new Vector3(wallThickness, wallHeight, cellSize + wallThickness),
                        wallMat);

        // Wall count: (height+1)*width + height*(width+1) positions minus ~48 carved = ~64 walls
    }

    void SpawnWall(Vector3 pos, Vector3 scale, Material mat)
    {
        var w = SpawnCube(pos, scale, mat);
        w.name = "Wall";
        w.transform.parent = transform;
        w.isStatic = true;
        w.AddComponent<MarkerComponent>().objectType = MazeObjectType.Wall;
    }

    // ─── NavMesh ───────────────────────────────────────────────────────────────

    void BakeNavMesh()
    {
        // CollectObjects.Children → only bakes on walls/floors parented to this GO
        var surface = gameObject.AddComponent<NavMeshSurface>();
        surface.collectObjects = CollectObjects.Children;
        surface.useGeometry    = NavMeshCollectGeometry.PhysicsColliders;
        surface.BuildNavMesh();
    }

    // ─── Entrance / Exit ───────────────────────────────────────────────────────

    void PlaceEntrance()
    {
        startPosition = CellCenter(0, 0); // y = 0 (ground)

        var marker = SpawnCube(
            new Vector3(startPosition.x, 0.01f, startPosition.z),
            new Vector3(cellSize * 0.88f, 0.04f, cellSize * 0.88f),
            CreateMat(new Color(0.25f, 0.45f, 1f)));
        marker.name = "Entrada";
        marker.GetComponent<Collider>().enabled = false;
    }

    void PlaceExit()
    {
        Vector3 ec = CellCenter(height - 1, width - 1);

        // Coloured floor marker (visual only)
        var visual = SpawnCube(
            new Vector3(ec.x, 0.01f, ec.z),
            new Vector3(cellSize * 0.88f, 0.04f, cellSize * 0.88f),
            CreateMat(new Color(0.1f, 0.88f, 0.25f)));
        visual.name = "Saida";
        visual.GetComponent<Collider>().enabled = false;

        // Invisible trigger volume for win detection
        var triggerGO = new GameObject("ExitTrigger");
        triggerGO.transform.position = new Vector3(ec.x, 1f, ec.z);
        var bc = triggerGO.AddComponent<BoxCollider>();
        bc.isTrigger = true;
        bc.size = new Vector3(cellSize * 0.85f, 2.5f, cellSize * 0.85f);
        triggerGO.AddComponent<MarkerComponent>().objectType = MazeObjectType.Exit;
        exitTriggerTransform = triggerGO.transform;
    }

    // ─── Obstacles ─────────────────────────────────────────────────────────────

    void PlaceObstacles()
    {
        Material fixedMat  = CreateMat(new Color(0.85f, 0.18f, 0.10f));
        Material mobileMat = CreateMat(new Color(0.96f, 0.72f, 0.10f));

        Vector3 startCell = CellCenter(0, 0);
        Vector3 exitCell  = CellCenter(height - 1, width - 1);

        var available = cellCenters.FindAll(v =>
            Vector3.Distance(v, startCell) > 0.1f &&
            Vector3.Distance(v, exitCell)  > 0.1f);

        Shuffle(available);
        int idx = 0;

        // Obstaculos fixos com NavMeshObstacle
        for (int i = 0; i < fixedObstacleCount  && idx < available.Count; i++, idx++)
            SpawnFixedObstacle(available[idx], fixedMat);

        // Obstaculos moveis com NavMeshObstacle
        for (int i = 0; i < mobileObstacleCount && idx < available.Count; i++, idx++)
            SpawnMobileObstacle(available[idx], mobileMat);
    }

    void SpawnFixedObstacle(Vector3 cell, Material mat)
    {
        float r   = cellSize * 0.18f;
        Vector3 pos = cell + new Vector3(Random.Range(-r, r), 0f, Random.Range(-r, r));

        var obs = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        obs.name = "ObstaculoFixo";
        obs.transform.position   = pos + Vector3.up * 0.75f;
        obs.transform.localScale = new Vector3(0.5f, 0.75f, 0.5f);
        obs.GetComponent<Renderer>().sharedMaterial = mat;
        obs.isStatic = true;
        obs.AddComponent<MarkerComponent>().objectType = MazeObjectType.Obstacle;

        var nmo    = obs.AddComponent<NavMeshObstacle>();
        nmo.shape  = NavMeshObstacleShape.Capsule;
        nmo.radius = 0.25f;
        nmo.height = 1.5f;
        nmo.carving = true; // permanently carves floor NavMesh around this pillar
    }

    void SpawnMobileObstacle(Vector3 cell, Material mat)
    {
        var obs = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        obs.name = "ObstaculoMovel";
        obs.transform.position   = cell + Vector3.up * 0.6f;
        obs.transform.localScale = Vector3.one * 0.6f;
        obs.GetComponent<Renderer>().sharedMaterial = mat;
        obs.AddComponent<MarkerComponent>().objectType = MazeObjectType.Obstacle;

        var nmo    = obs.AddComponent<NavMeshObstacle>();
        nmo.shape  = NavMeshObstacleShape.Capsule;
        nmo.radius = 0.3f;
        nmo.height = 1.2f;
        nmo.carving = false; // moving obstacle — carving disabled for performance

        var mo          = obs.AddComponent<MobileObstacle>();
        mo.origin       = cell;
        mo.patrolRadius = 1.2f;
    }

    // ─── Player & Camera ───────────────────────────────────────────────────────

    void SpawnPlayer()
    {
        // Root: empty GO with CharacterController (pivot at feet level y = 0)
        var player = new GameObject("Player");
        player.transform.position = startPosition;

        var cc    = player.AddComponent<CharacterController>();
        cc.center = new Vector3(0f, 0.9f, 0f); // capsule center 0.9 m above pivot
        cc.height = 1.8f;
        cc.radius = 0.4f;

        var pc             = player.AddComponent<PlayerController>();
        pc.exitTransform   = exitTriggerTransform;

        // Child visual capsule (collider removed — CC handles physics)
        var visual = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        DestroyImmediate(visual.GetComponent<CapsuleCollider>());
        visual.GetComponent<Renderer>().material.color = new Color(0.22f, 0.50f, 1f);
        visual.name = "Visual";
        visual.transform.parent        = player.transform;
        visual.transform.localPosition = new Vector3(0f, 0.9f, 0f);
        visual.transform.localScale    = new Vector3(0.8f, 0.9f, 0.8f);

        SetupCamera(player.transform);
    }

    void SetupCamera(Transform playerTransform)
    {
        Camera cam = Camera.main;
        if (cam == null)
        {
            var camGO = new GameObject("Main Camera");
            camGO.tag = "MainCamera";
            cam = camGO.AddComponent<Camera>();
            camGO.AddComponent<AudioListener>();
        }

        var logic = cam.gameObject.GetComponent<CameraLogic>()
                    ?? cam.gameObject.AddComponent<CameraLogic>();
        logic.player = playerTransform;
    }

    // ─── Utilities ─────────────────────────────────────────────────────────────

    static GameObject SpawnCube(Vector3 pos, Vector3 scale, Material mat)
    {
        var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.transform.position   = pos;
        go.transform.localScale = scale;
        go.GetComponent<Renderer>().sharedMaterial = mat;
        return go;
    }

    static Material CreateMat(Color color)
    {
        var mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        mat.color = color;
        return mat;
    }

    static void Shuffle<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            T tmp = list[i]; list[i] = list[j]; list[j] = tmp;
        }
    }
}
