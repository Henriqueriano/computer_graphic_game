using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerLogic : MonoBehaviour
{
    private float current_speed = 1f;
    private void Start()
    {
        GameObject player = GameObject.Find("Player");
    }

    private void Update()
    {
        Keyboard keyboard = Keyboard.current;
        HasKeyInteract(keyboard);

    }

    private void HasKeyInteract(Keyboard keyboard)
    {
        Vector3 d_move = Vector3.forward * this.current_speed * Time.deltaTime;
        Vector3 s_move = Vector3.right * this.current_speed * Time.deltaTime;
        Vector3 a_move = Vector3.back * this.current_speed * Time.deltaTime;
        Vector3 w_move = Vector3.left * this.current_speed * Time.deltaTime;
        Vector3 wa_move = new Vector3(-1,0,-1) * this.current_speed * Time.deltaTime;
        Vector3 wd_move = new Vector3(1,0,1) * this.current_speed * Time.deltaTime;
        Vector3 sd_move = new Vector3(-1,0,1) * this.current_speed * Time.deltaTime;
        Vector3 sa_move = new Vector3(1,0,-1) * this.current_speed * Time.deltaTime;

        // combinated key presses
        if (keyboard.wKey.isPressed && keyboard.aKey.isPressed) 
        {
            transform.Translate(wa_move);
        }
        else if (keyboard.wKey.isPressed && keyboard.dKey.isPressed)
        { 
            transform.Translate(wd_move);
        }
        else if (keyboard.sKey.isPressed && keyboard.aKey.isPressed)
        {
            transform.Translate(sa_move);
        }
        else if (keyboard.sKey.isPressed && keyboard.dKey.isPressed)
        {
            transform.Translate(sd_move);
        }

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

