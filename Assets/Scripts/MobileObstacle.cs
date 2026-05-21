using UnityEngine;

/// <summary>
/// Patrols between random points within a radius of its origin cell center.
/// The NavMeshObstacle component (added by MazeGenerator) marks it for the NavMesh.
/// </summary>
public class MobileObstacle : MonoBehaviour
{
    [HideInInspector] public Vector3 origin;
    [HideInInspector] public float   patrolRadius = 1.2f;
    public float moveSpeed = 1.2f;

    private Vector3 target;

    void Start()  => PickTarget();

    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
        if (Vector3.Distance(transform.position, target) < 0.05f)
            PickTarget();
    }

    void PickTarget()
    {
        Vector2 circle = Random.insideUnitCircle * patrolRadius;
        target = new Vector3(origin.x + circle.x, transform.position.y, origin.z + circle.y);
    }
}
