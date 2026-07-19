using System.Collections;
using UnityEngine;

public class Script_UISlider : MonoBehaviour
{
    [Header("Satu Panel Panjang (Berisi Main Menu & Credits)")]
    public RectTransform widePanel;
    public static Script_UISlider instance;

    [Header("Titik Koordinat (Sumbu X)")]
    // Posisi 0 = Tampilan awal (Main Menu di tengah, Credits tersembunyi di kanan)
    public float mainMenuPosX = 0f;

    // Posisi -1920 = Panel bergeser ke KIRI (Main Menu keluar ke kiri, Credits masuk dari kanan)
    public float creditPosX = -1920f;

    public float tutorialPosX = 1920f;

    [Header("Kecepatan Geser")]
    public float slideSpeed = 8f;

    private Coroutine slideCoroutine;
    private void Awake()
    {
        if (instance == null) instance = this;
    }



    // Panggil ini di tombol "Back / Main Menu"
    public void SlideToMainMenu()
    {
        if (slideCoroutine != null) StopCoroutine(slideCoroutine);

        // Mengembalikan keseluruhan panel ke tengah (0)
        slideCoroutine = StartCoroutine(SlideToPosition(mainMenuPosX));
    }

    public void SlideToTutorial()
    {
        if (slideCoroutine != null) StopCoroutine(slideCoroutine);
        // Au
        slideCoroutine = StartCoroutine(SlideToPosition(tutorialPosX));
    }
    public void SlideToCredits()
    {
        if (slideCoroutine != null) StopCoroutine(slideCoroutine);
        // Au
        slideCoroutine = StartCoroutine(SlideToPosition(creditPosX));
    }

    private IEnumerator SlideToPosition(float targetX)
    {
        Vector2 targetPosition = new Vector2(targetX, widePanel.anchoredPosition.y);

        while (Mathf.Abs(widePanel.anchoredPosition.x - targetX) > 0.5f)
        {
            widePanel.anchoredPosition = Vector2.Lerp(widePanel.anchoredPosition, targetPosition, Time.deltaTime * slideSpeed);
            yield return null;
        }

        widePanel.anchoredPosition = targetPosition;
    }
}