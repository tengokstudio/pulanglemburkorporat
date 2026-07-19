using JetBrains.Annotations;
using System.Collections;
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

    [Header("Referensi Objek")]
    public GameObject Circle; // Pastikan ini sudah di-drag di Inspector

    [Header("Pengaturan Animasi Transisi")]
    public Animator animTransisi;
    public float durasiTransisi = 1f;

    bool Check_Tutor = false;

    private void Awake()
    {
        if (instance == null) instance = this;
    }

    // Fungsi Start akan berjalan otomatis saat game dimulai
    private void Start()
    {
        // Memastikan Circle tidak terlihat saat game pertama kali running
        if (Circle != null)
        {
            Circle.SetActive(false);
        }
    }

    public void Play()
    {
        // 1. Munculkan Circle-nya terlebih dahulu
        if (Circle != null)
        {
            Circle.SetActive(true);
        }

        // 2. Baru jalankan animasinya
        if (animTransisi != null)
        {
            animTransisi.SetTrigger("MulaiTransisi");
            StartCoroutine(ProsesPindahScene());
        }
        else
        {
            Debug.LogWarning("Animator transisi belum dimasukkan ke script!");
            SceneManager.LoadScene(GameMenu);
        }
    }

    private IEnumerator ProsesPindahScene()
    {
        yield return new WaitForSeconds(durasiTransisi);
        SceneManager.LoadScene(GameMenu);
    }

    // ... (Fungsi Tutorial, Credits, Home, btn_tutorial tetap sama) ...
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