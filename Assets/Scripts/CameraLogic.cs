using UnityEngine;
using UnityEngine.InputSystem;

public class CameraLogic : MonoBehaviour
{
    public Transform player;

    [Header("Distancia e Sensibilidade")]
    public float distance         = 3.5f;
    public float mouseSensitivity = 0.2f;

    [Header("Limite Vertical (graus)")]
    [Tooltip("Angulo minimo — camera quase no nivel do chao")]
    public float minPitch =  8f;
    [Tooltip("Angulo maximo — camera abaixo do topo das paredes (3 m)")]
    public float maxPitch = 28f;

    private float yaw   = 180f; // comeca atras do player
    private float pitch =  18f;

    void LateUpdate()
    {
        if (player == null) return;

        // Botao direito do mouse: girar camera
        Mouse mouse = Mouse.current;
        if (mouse != null && mouse.rightButton.isPressed)
        {
            Vector2 delta = mouse.delta.ReadValue();
            yaw   += delta.x * mouseSensitivity;
            pitch -= delta.y * mouseSensitivity;
            pitch  = Mathf.Clamp(pitch, minPitch, maxPitch);
        }

        // Ponto de foco: centro do personagem
        Vector3 pivot = player.position + Vector3.up * 0.9f;

        // Posicao orbital
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
        transform.position  = pivot + rotation * new Vector3(0f, 0f, -distance);
        transform.LookAt(pivot);
    }
}
