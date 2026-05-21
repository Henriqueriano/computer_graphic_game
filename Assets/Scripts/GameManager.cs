using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private bool won;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(this); return; }
        Instance = this;
    }

    public void Win()
    {
        if (won) return;
        won = true;
        Debug.Log("[Labirinto] Voce saiu do labirinto!");
    }

    void OnGUI()
    {
        if (!won) return;

        float sw = Screen.width;
        float sh = Screen.height;

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
