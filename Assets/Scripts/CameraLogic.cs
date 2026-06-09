using UnityEngine;
using UnityEngine.InputSystem;

public class CameraLogic : MonoBehaviour
{
    public Transform player;

    [Header("Distancia e Sensibilidade")]
    public float distance         = 7f;
    public float height           = 1f;
    public float mouseSensitivity = 0.1f;
    public float arrowRotateSpeed = 120f;

    [Header("Mouse Lookup")]
    public float xAxis;
    public float yAxis;

    void LateUpdate()
    {
        if (player == null) return;


        // Botao direito do mouse: girar camera livremente
        Mouse mouse = Mouse.current;
        if (mouse != null)
        {
            Vector2 delta = mouse.delta.ReadValue();

            yAxis += delta.y * mouseSensitivity;
            xAxis += delta.x * mouseSensitivity;
            Debug.Log($"{xAxis},{yAxis}");
            yAxis = Mathf.Clamp(yAxis, -5, 40);

            transform.localRotation = Quaternion.Euler(-yAxis, xAxis, 0);
            Vector3 pivot = player.position + Vector3.up * height;
            transform.position  = pivot + transform.localRotation * new Vector3(0f, 0f, -distance);
            // transform.LookAt(pivot);
        }
    }
}
