using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;

    [Header("Regras de Distância")]
    [Tooltip("Distância mínima das paredes (m). Menor → volta ao início.")]
    public float wallMinDistance     = 1.0f;
    [Tooltip("Distância mínima dos obstáculos (m). Menor → volta ao início.")]
    public float obstacleMinDistance = 0.5f;

    [HideInInspector] public Transform exitTransform;

    private CharacterController cc;
    private Vector3 spawnPosition;
    private float verticalVelocity;

    private const float Gravity       = -15f;
    private const float GraceDuration = 0.5f; // seconds of immunity after spawn / reset
    private float graceTimer;

    // ─── Lifecycle ─────────────────────────────────────────────────────────────

    void Start()
    {
        cc            = GetComponent<CharacterController>();
        spawnPosition = transform.position;
        graceTimer    = GraceDuration;
    }

    void Update()
    {
        Move();

        if (graceTimer > 0f)
        {
            graceTimer -= Time.deltaTime;
            return;
        }

        CheckDistanceRules();
        CheckExit();
    }

    // ─── Movement ──────────────────────────────────────────────────────────────

    void Move()
    {
        var kb = Keyboard.current;
        if (kb == null) return;

        float h = 0f, v = 0f;
        if (kb.wKey.isPressed || kb.upArrowKey.isPressed)    v += 1f;
        if (kb.sKey.isPressed || kb.downArrowKey.isPressed)  v -= 1f;
        if (kb.aKey.isPressed || kb.leftArrowKey.isPressed)  h -= 1f;
        if (kb.dKey.isPressed || kb.rightArrowKey.isPressed) h += 1f;

        Vector3 dir = new Vector3(h, 0f, v);
        if (dir.sqrMagnitude > 1f) dir.Normalize();
        cc.Move(dir * moveSpeed * Time.deltaTime);

        verticalVelocity = cc.isGrounded ? -0.5f : verticalVelocity + Gravity * Time.deltaTime;
        cc.Move(Vector3.up * verticalVelocity * Time.deltaTime);
    }

    // ─── Distance Rules ────────────────────────────────────────────────────────

    void CheckDistanceRules()
    {
        Vector3 checkPos = transform.position + cc.center;
        
        // Valida a colizao do jogador com as paredes e obstaculos
        if (IsNearType(checkPos, MazeObjectType.Wall,     wallMinDistance)     ||
            IsNearType(checkPos, MazeObjectType.Obstacle, obstacleMinDistance))
        {
            ResetToStart();
        }
    }

    bool IsNearType(Vector3 center, MazeObjectType type, float radius)
    {
        var hits = Physics.OverlapSphere(center, radius);
        foreach (var col in hits)
            if (col.TryGetComponent<MarkerComponent>(out var mc) && mc.objectType == type)
                return true;
        return false;
    }

    // ─── Exit Detection ────────────────────────────────────────────────────────

    void CheckExit()
    {
        if (exitTransform == null) return;

        // Compare on XZ plane to ignore height differences
        Vector3 pos  = transform.position; pos.y = 0f;
        Vector3 exit = exitTransform.position; exit.y = 0f;

        if (Vector3.Distance(pos, exit) < 2f)
            GameManager.Instance?.Win();
    }

    // ─── Reset ─────────────────────────────────────────────────────────────────

    void ResetToStart()
    {
        // CharacterController must be disabled to teleport
        cc.enabled = false;
        transform.position = spawnPosition;
        verticalVelocity   = 0f;
        cc.enabled = true;
        graceTimer = GraceDuration;
        Debug.Log("[Labirinto] Muito perto! Voltou ao início.");
    }
}
