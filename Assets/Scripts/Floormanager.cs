using UnityEngine;
using System;

public class FloorManager : MonoBehaviour
{
    public static FloorManager Instance { get; private set; }

    [Header("Floor State")]
    [SerializeField] private int startingFloor = 9;
    [SerializeField] private int finalFloor = 1; // floor exit (pintu kaca)

    [Header("Current Level Data")]
    [SerializeField] private bool currentFloorHasAnomaly;

    [Header("References")]
    [SerializeField] private Transform spawnPoint; // posisi lift ujung kiri
    [SerializeField] private PlayerController player; // drag manual di Inspector

    public int CurrentFloor { get; private set; }
    public bool CurrentFloorHasAnomaly => currentFloorHasAnomaly;

    public event Action<int> OnFloorChanged;
    public event Action OnGameWon;

    private bool hasLeftSpawnZone;
    private bool hasWon;
    private bool hasBegunOnce; // biar fade transition di-skip pas load awal scene (nanti transisi masuknya ditangani main menu)

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        CurrentFloor = startingFloor; // di-set di Awake, bukan Start — Unity ga jamin urutan Start() antar script,
                                       // jadi script lain (misal FloorNumberDisplay) yang baca CurrentFloor pas Start()
                                       // bisa kejalan duluan dan kebaca default 0 kalau ini nunggu Start().

        // Fail fast: kalau reference lupa di-assign di Inspector, ketauan
        // langsung di Console pas Play, bukan diem-diem null pas runtime.
        if (spawnPoint == null)
            Debug.LogError("[FloorManager] spawnPoint belum di-assign di Inspector!");
        if (player == null)
            Debug.LogError("[FloorManager] player belum di-assign di Inspector!");
    }

    private void Start()
    {
        StartCoroutine(BeginFloorNextFrame());
    }

    private System.Collections.IEnumerator BeginFloorNextFrame()
    {
        yield return null;
        BeginFloor();
    }

    public void BeginFloor()
    {
        bool skipTransition = !hasBegunOnce;
        hasBegunOnce = true;

        if (!skipTransition && ScreenFader.Instance != null)
        {
            ScreenFader.Instance.PlayTransition(DoBeginFloor);
        }
        else
        {
            DoBeginFloor();
        }
    }

    private void DoBeginFloor()
    {
        hasLeftSpawnZone = false;

        if (spawnPoint != null && player != null)
        {
            player.TeleportTo(spawnPoint.position);
        }

        OnFloorChanged?.Invoke(CurrentFloor);
        Debug.Log($"[Floor {CurrentFloor}] Mulai. HasAnomaly = {currentFloorHasAnomaly}");
    }

    public void SetCurrentFloorAnomaly(bool hasAnomaly)
    {
        currentFloorHasAnomaly = hasAnomaly;
    }

    public void MarkLeftSpawnZone()
    {
        hasLeftSpawnZone = true;
    }

    public void PlayerReachedForwardExit()
    {
        if (hasWon) return;

        if (CurrentFloor == finalFloor)
        {
            // Lantai terakhir: pintu kaca di kanan = satu-satunya jalan keluar/menang,
            // anomali "fixed" di lantai ini gak ngaruh ke arah yang bener.
            AdvanceFloor();
            return;
        }

        if (currentFloorHasAnomaly)
        {
            WrongChoice("Maju padahal ada anomali");
        }
        else
        {
            AdvanceFloor();
        }
    }

    public void PlayerReachedBackwardExit()
    {
        if (hasWon) return;
        if (!hasLeftSpawnZone) return;

        if (CurrentFloor == finalFloor)
        {
            // Lantai terakhir gak ada opsi mundur yang bener — harus lewat pintu kaca kanan.
            WrongChoice("Mundur padahal lantai terakhir, harusnya lewat pintu keluar kanan");
            return;
        }

        if (currentFloorHasAnomaly)
        {
            AdvanceFloor();
        }
        else
        {
            WrongChoice("Mundur padahal normal");
        }
    }

    private void AdvanceFloor()
    {
        Debug.Log($"Correct choice di floor {CurrentFloor} (HasAnomaly = {currentFloorHasAnomaly})");
        CurrentFloor--;

        if (CurrentFloor < finalFloor)
        {
            hasWon = true;
            OnGameWon?.Invoke();
            Debug.Log("MENANG! Player berhasil keluar dari looping room.");
            return;
        }

        BeginFloor();
    }
    private void WrongChoice(string reason)
    {
        Debug.Log($"Wrong choice: {reason} -> reset ke floor {startingFloor}");
        CurrentFloor = startingFloor;
        BeginFloor();
    }

    /// <summary>Dipanggil dari Anomaly AI kalau player ketangkep. Reset in-place ke lantai 9
    /// (sementara pengganti "balik ke main menu" sesuai GDD, sampai menu-nya dibikin).</summary>
    public void PlayerCaught()
    {
        if (hasWon) return;

        Debug.Log("[FloorManager] Player ketangkep! Reset ke lantai 9.");
        CurrentFloor = startingFloor;
        BeginFloor();
    }
}