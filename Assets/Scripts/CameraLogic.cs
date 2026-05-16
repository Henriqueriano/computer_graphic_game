using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameObject player = GameObject.Find("Player");
        GameObject camera = GameObject.Find("Main Camera");
    }

    // Update is called once per frame
    void Update()
    {
        // GetComponent<Camera>().transform.position = player.transform.position + new Vector3(0, 5, -10);
    }
}
