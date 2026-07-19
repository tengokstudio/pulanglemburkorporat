using UnityEngine;

// Hub SFX/musik. Script lain manggil AudioManager.Instance.PlayXxx() / SetXxxLooping()
// alih-alih masing-masing punya AudioSource sendiri. Semua clip kosong by default —
// drag file audio ke slot yang sesuai di Inspector, gak perlu ubah kode apapun.
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource sfxSource;           // one-shot SFX umum
    [SerializeField] private AudioSource musicSource;         // ambience layer 1, loop
    [SerializeField] private AudioSource musicSource2;        // ambience layer 2, loop
    [SerializeField] private AudioSource musicSource3;        // ambience layer 3, loop
    [SerializeField] private AudioSource footstepSource;      // loop, jalan/lari player
    [SerializeField] private AudioSource fotokopiSlideSource; // loop, selama Mesin Fotokopi gerak

    [Header("Player")]
    [SerializeField] private AudioClip footstepWalkClip;
    [SerializeField] private AudioClip footstepRunClip;
    [SerializeField] private AudioClip lockerOpenClip;
    [SerializeField] private AudioClip lockerCloseClip;

    [Header("Mesin Fotokopi")]
    [SerializeField] private AudioClip fotokopiSlideClip;  // loop selama gerak (geser)
    [SerializeField] private AudioClip fotokopiFlipClip;   // "klek" pas kepala/lengan buka-tutup

    [Header("Dispenser Galon")]
    [SerializeField] private AudioClip dispenserHopClip;   // "tak tak tak" tiap landing lompat

    [Header("Floor / Game State")]
    [SerializeField] private AudioClip liftClip;           // lift ketutup/naik lift, tiap ganti lantai
    [SerializeField] private AudioClip correctChoiceClip;  // pilihan bener (maju/mundur)
    [SerializeField] private AudioClip wrongChoiceClip;    // pilihan salah, reset ke lantai 9
    [SerializeField] private AudioClip caughtClip;         // ketangkep anomali
    [SerializeField] private AudioClip winClip;            // berhasil keluar lantai 1

    [Header("Ambience (layering — semua nyala bareng)")]
    [SerializeField] private AudioClip backgroundMusicClip;
    [SerializeField] private AudioClip backgroundMusicClip2;
    [SerializeField] private AudioClip backgroundMusicClip3;

    private bool footstepIsRunning;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start()
    {
        PlayAmbienceLayer(musicSource, backgroundMusicClip);
        PlayAmbienceLayer(musicSource2, backgroundMusicClip2);
        PlayAmbienceLayer(musicSource3, backgroundMusicClip3);
    }

    private void PlayAmbienceLayer(AudioSource source, AudioClip clip)
    {
        if (source == null || clip == null) return;
        source.clip = clip;
        source.loop = true;
        source.Play();
    }

    // ===== Player =====
    public void SetFootstepLooping(bool playing, bool running)
    {
        if (footstepSource == null) return;

        if (!playing)
        {
            footstepSource.Stop();
            return;
        }

        AudioClip clip = running ? footstepRunClip : footstepWalkClip;
        if (clip == null) { footstepSource.Stop(); return; }

        if (!footstepSource.isPlaying || footstepSource.clip != clip)
        {
            footstepSource.clip = clip;
            footstepSource.loop = true;
            footstepSource.Play();
        }
        footstepIsRunning = running;
    }

    public void PlayLockerOpen() => PlayOneShot(lockerOpenClip);
    public void PlayLockerClose() => PlayOneShot(lockerCloseClip);

    // ===== Mesin Fotokopi =====
    public void SetFotokopiSlideLooping(bool playing)
    {
        if (fotokopiSlideSource == null || fotokopiSlideClip == null) return;

        if (playing)
        {
            if (!fotokopiSlideSource.isPlaying)
            {
                fotokopiSlideSource.clip = fotokopiSlideClip;
                fotokopiSlideSource.loop = true;
                fotokopiSlideSource.Play();
            }
        }
        else
        {
            fotokopiSlideSource.Stop();
        }
    }

    public void PlayFotokopiFlip() => PlayOneShot(fotokopiFlipClip);

    // ===== Dispenser Galon =====
    public void PlayDispenserHop() => PlayOneShot(dispenserHopClip);

    // ===== Floor / Game State =====
    public void PlayLiftSound() => PlayOneShot(liftClip);
    public void PlayCorrectChoice() => PlayOneShot(correctChoiceClip);
    public void PlayWrongChoice() => PlayOneShot(wrongChoiceClip);
    public void PlayCaught() => PlayOneShot(caughtClip);
    public void PlayWin() => PlayOneShot(winClip);

    private void PlayOneShot(AudioClip clip)
    {
        if (sfxSource == null || clip == null) return;
        sfxSource.PlayOneShot(clip);
    }
}
