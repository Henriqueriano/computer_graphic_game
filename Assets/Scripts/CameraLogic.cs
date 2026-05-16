using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    public Transform player;
    public Vector3 offset;
    public float smoothSpeed = 0.125f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameObject camera = GameObject.Find("Main Camera");
    }

    // Update is called once per frame
    void Update()
    {
        offset = new Vector3(1, 0.8f, -1.2f);
        Vector3 desiredPosition = player.position + offset;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
    }
}
