using UnityEngine;
using UnityEngine.InputSystem;

public class CameraLogic : MonoBehaviour
{
    public Transform player;

    [Header("Distancia e Sensibilidade")]
    public float distance         = 7f;
    public float mouseSensitivity = 0.2f;
    public float arrowRotateSpeed = 120f;

    [Header("Limite Vertical (graus)")]
    [Tooltip("Angulo minimo — camera quase no nivel do chao")]
    public float minPitch = 15f;
    [Tooltip("Angulo maximo — camera acima das paredes")]
    public float maxPitch = 40f;

    private float yaw   = 180f; // comeca atras do player
    private float pitch =  25f;

    void LateUpdate()
    {
        if (player == null) return;

        // Setas esquerda/direita: girar camera horizontalmente
        Keyboard kb = Keyboard.current;
        if (kb != null)
        {
            if (kb.leftArrowKey.isPressed)  yaw -= arrowRotateSpeed * Time.deltaTime;
            if (kb.rightArrowKey.isPressed) yaw += arrowRotateSpeed * Time.deltaTime;
        }

        // Botao direito do mouse: girar camera livremente
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
