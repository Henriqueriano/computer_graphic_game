using UnityEngine;

public class CameraLogic : MonoBehaviour
{
    public Transform player;
    public Vector3   offset      = new Vector3(0f, 8f, -2f);
    public float     smoothSpeed = 0.1f;

    void LateUpdate()
    {
        if (player == null) return;
        Vector3 desired = player.position + offset;
        transform.position = Vector3.Lerp(transform.position, desired, smoothSpeed);
        transform.LookAt(player.position + Vector3.up);
    }
}
