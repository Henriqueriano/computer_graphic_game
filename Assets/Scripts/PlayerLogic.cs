using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerLogic : MonoBehaviour
{
    private void Start()
    {
        GameObject player = GameObject.Find("Player");
    }

    private void Update()
    {
        Keyboard keyboard = Keyboard.current;
        MovePlayer(keyboard);
    }

    private void MovePlayer(Keyboard keyboard)
    {
        float movement_speed = 0.88f;
        Vector3 d_move = Vector3.forward * movement_speed * Time.deltaTime;
        Vector3 s_move = Vector3.right * movement_speed * Time.deltaTime;
        Vector3 a_move = Vector3.back * movement_speed * Time.deltaTime;
        Vector3 w_move = Vector3.left * movement_speed * Time.deltaTime;
        Vector3 wa_move = new Vector3(-1,0,-1) * movement_speed * Time.deltaTime;
        Vector3 wd_move = new Vector3(1,0,1) * movement_speed * Time.deltaTime;
        Vector3 sd_move = new Vector3(-1,0,1) * movement_speed * Time.deltaTime;
        Vector3 sa_move = new Vector3(1,0,-1) * movement_speed * Time.deltaTime;

        // combinated key presses
        if (keyboard.wKey.isPressed && keyboard.aKey.isPressed) 
        {
            transform.Translate(wa_move);
        }
        if (keyboard.wKey.isPressed && keyboard.dKey.isPressed)
        { 
            transform.Translate(wd_move);
        }
        if (keyboard.sKey.isPressed && keyboard.aKey.isPressed)
        {
            transform.Translate(sa_move);
        }
        if (keyboard.sKey.isPressed && keyboard.dKey.isPressed)
        {
            transform.Translate(sd_move);
        }

        // single key presses
        if (keyboard.wKey.isPressed)
        {
            transform.Translate(w_move);
        }
        else if (keyboard.sKey.isPressed)
        {
            transform.Translate(s_move);
        }
        else if (keyboard.aKey.isPressed)
        {
            transform.Translate(a_move);
        }
        else if (keyboard.dKey.isPressed)
        {
            transform.Translate(d_move);
        }
    }
}

