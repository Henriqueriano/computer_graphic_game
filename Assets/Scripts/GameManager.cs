using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private bool won;
    private bool playing = true;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(this); return; }
        Instance = this;
        Cursor.lockState = CursorLockMode.Locked; // trava o cursor no centro da tela
        Cursor.visible = false; // esconde o cursor
    }

    public void Win()
    {
        if (won) return;
        won = true;
        playing = false;
        Cursor.lockState = CursorLockMode.None; // destrava o cursor
        Cursor.visible = true; // mostra o cursor
        Debug.Log("[Labirinto] Voce saiu do labirinto!");
    }

    void OnGUI()
    {

        float sw = Screen.width;
        float sh = Screen.height;

        if (playing) 
        {

            GUIStyle dotStyle = new GUIStyle(GUI.skin.label);
            dotStyle.fontSize = 25;
            dotStyle.alignment = TextAnchor.MiddleCenter;
            dotStyle.fontStyle = FontStyle.Bold;
            dotStyle.normal.textColor = Color.cyan;
            GUI.Label(new Rect(sw * 0.4f, sh * 0.4f, sw * 0.24f, 48f), "+", dotStyle);
            return;
        }
        else if (!won) return;
        
        GUIStyle titleStyle = new GUIStyle(GUI.skin.label);
        titleStyle.fontSize  = Mathf.RoundToInt(sw * 0.055f);
        titleStyle.alignment = TextAnchor.MiddleCenter;
        titleStyle.fontStyle = FontStyle.Bold;
        titleStyle.normal.textColor = Color.green;

        GUIStyle btnStyle = new GUIStyle(GUI.skin.button);
        btnStyle.fontSize = 22;

        GUI.Label(new Rect(0, sh * 0.35f, sw, sh * 0.15f),
                  "VOCE SAIU DO LABIRINTO!", titleStyle);

        if (GUI.Button(new Rect(sw * 0.38f, sh * 0.53f, sw * 0.24f, 48f),
                       "Jogar Novamente", btnStyle))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
} 