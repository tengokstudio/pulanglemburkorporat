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
    public event Action OnPlayerCaught;
    public event Action OnGameWon;

    private bool hasLeftSpawnZone;
    private bool hasWon;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // Fail fast: kalau reference lupa di-assign di Inspector, ketauan
        // langsung di Console pas Play, bukan diem-diem null pas runtime.
        if (spawnPoint == null)
            Debug.LogError("[FloorManager] spawnPoint belum di-assign di Inspector!");
        if (player == null)
            Debug.LogError("[FloorManager] player belum di-assign di Inspector!");
    }

    private void Start()
    {
        CurrentFloor = startingFloor;
        StartCoroutine(BeginFloorNextFrame());
    }

    private System.Collections.IEnumerator BeginFloorNextFrame()
    {
        // Tunda 1 frame biar semua script lain (AnomalySpawner, dll) udah selesai
        // subscribe ke event ini sebelum floor pertama ditembak.
        yield return null;
        BeginFloor();
    }

    private void BeginFloor()
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

    /// <summary>Dipanggil dari Anomaly AI kalau player ketangkep.</summary>
    public void PlayerCaught()
    {
        if (hasWon) return; // guard biar ga kepanggil dobel abis menang

        CurrentFloor = startingFloor;
        OnPlayerCaught?.Invoke(); // trigger circle wipe / UI dulu

        // TIDAK manggil BeginFloor() di sini secara langsung — sengaja.
        // Alasan: BeginFloor() teleport player & reset visual floor SEKARANG,
        // padahal circle wipe animation (listener OnPlayerCaught) butuh waktu
        // sebelum balik ke gameplay. Kalau BeginFloor() dipanggil di sini,
        // player bakal keliatan teleport balik ke lift SEBELUM wipe animation selesai.
        //
        // Sebagai gantinya: script yang handle circle wipe (nanti, GameStateManager
        // atau semacamnya) WAJIB manggil FloorManager.Instance.BeginFloor() setelah
        // animasi wipe selesai. Kalau lupa dipasang, game bakal macet total di floor 9
        // dengan HasAnomaly = false selamanya — jadi gampang ketauan pas testing,
        // bukan silent bug.
    }
}