using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// Overlay hitam full-screen buat transisi ganti lantai. fadeImage harus Image UI
// full-stretch di atas Canvas (Screen Space - Overlay), alpha awal 0 (transparan).
public class ScreenFader : MonoBehaviour
{
    public static ScreenFader Instance { get; private set; }

    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadeInTime = 0.15f;
    [SerializeField] private float fadeOutTime = 0.25f;

    private void Awake()
    {
        Instance = this;
        SetAlpha(0f);
    }

    /// <summary>Fade ke hitam, jalanin onBlackout pas layar full hitam, baru fade balik transparan.</summary>
    public void PlayTransition(Action onBlackout)
    {
        StartCoroutine(TransitionRoutine(onBlackout));
    }

    private IEnumerator TransitionRoutine(Action onBlackout)
    {
        yield return Fade(0f, 1f, fadeInTime);
        onBlackout?.Invoke();
        yield return Fade(1f, 0f, fadeOutTime);
    }

    private IEnumerator Fade(float from, float to, float duration)
    {
        if (fadeImage == null) yield break;

        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            SetAlpha(Mathf.Lerp(from, to, duration > 0f ? t / duration : 1f));
            yield return null;
        }
        SetAlpha(to);
    }

    private void SetAlpha(float a)
    {
        if (fadeImage == null) return;
        Color c = fadeImage.color;
        c.a = a;
        fadeImage.color = c;
    }
}
