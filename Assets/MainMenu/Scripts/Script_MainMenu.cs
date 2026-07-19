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
    public GameObject Tutor1;
    public GameObject Tutor2;

    bool Check_Tutor = false;


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
        Script_UISlider.instance.SlideToTutorial();
    }

    public void Credits()
    {
        Script_UISlider.instance.SlideToCredits();
    }

    public void Home()
    {
        Script_UISlider.instance.SlideToMainMenu();
        if (Check_Tutor == true)
        {
            Tutor1.SetActive(true);
            Tutor2.SetActive(false);
        }
    }

    public void btn_tutorial()
    {
        Check_Tutor = true;
        Tutor1.SetActive(false);
        Tutor2.SetActive(true);
    }
}
