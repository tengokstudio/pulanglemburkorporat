using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class Script_MainMenu : MonoBehaviour
{
    [SerializeField] private string GameMenu;
    public static Script_MainMenu instance;
    public GameObject MainMenu_UI;
    public GameObject Logo;
    public GameObject Tutorial_UI;
    public GameObject Credits_UI;

    private void Awake()
    {
        if (instance == null) instance = this;
    }
    public void Play()
    {
        SceneManager.LoadScene(GameMenu);
        print("PLAYER TERDETEKSI KE GAME");
    }

    public void Tutorial()
    {
        MainMenu_UI.SetActive(false);

    }
}
