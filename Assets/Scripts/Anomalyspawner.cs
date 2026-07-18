using UnityEngine;

public enum AnomalyType
{
    None,
    MesinFotokopi,
    DispenserGalon,
    Static // tanaman/sampah/plang/pintu pantry — anomali visual doang, ga ngejar
}

public class AnomalySpawner : MonoBehaviour
{
    public static AnomalySpawner Instance { get; private set; }

    [Header("Config")]
    [Range(0f, 1f)]
    [SerializeField] private float anomalyChance = 0.5f; // peluang tiap floor punya anomali
    [SerializeField] private int movingAnomalyMinFloor = 2;
    [SerializeField] private int movingAnomalyMaxFloor = 5;

    public AnomalyType CurrentAnomalyType { get; private set; } = AnomalyType.None;
    public event System.Action<AnomalyType> OnAnomalyTypeChanged;

    private bool movingAnomalyAlreadySpawned;
    private int movingAnomalyTargetFloor; // floor yang ditentuin di awal run buat moving anomaly

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void OnDisable()
    {
        if (FloorManager.Instance != null)
            FloorManager.Instance.OnFloorChanged -= HandleFloorChanged;
    }

    private void Start()
    {
        // FloorManager nunda BeginFloor() 1 frame, jadi subscribe di sini udah aman —
        // dijamin kedaftar sebelum event floor pertama ditembak.
        FloorManager.Instance.OnFloorChanged += HandleFloorChanged;
        PickMovingAnomalyFloor();
    }

    private void PickMovingAnomalyFloor()
    {
        movingAnomalyTargetFloor = Random.Range(movingAnomalyMinFloor, movingAnomalyMaxFloor + 1);
        movingAnomalyAlreadySpawned = false;
        Debug.Log($"[AnomalySpawner] Moving anomaly run ini bakal muncul di floor {movingAnomalyTargetFloor}");
    }

    private void HandleFloorChanged(int floor)
    {
        AnomalyType type = DecideAnomalyType(floor);
        CurrentAnomalyType = type;

        bool hasAnomaly = type != AnomalyType.None;
        FloorManager.Instance.SetCurrentFloorAnomaly(hasAnomaly);

        OnAnomalyTypeChanged?.Invoke(type);
        Debug.Log($"[AnomalySpawner] Floor {floor} -> {type}");
    }

    private AnomalyType DecideAnomalyType(int floor)
    {
        // Reset dan pastikan Lantai 9 selalu normal sebagai referensi awal
        if (floor == 9)
        {
            PickMovingAnomalyFloor();
            return AnomalyType.None;
        }

        // Cek apakah lantai saat ini berada di area anomali bergerak (Lantai 5-2)
        bool inMovingRange = floor >= movingAnomalyMinFloor && floor <= movingAnomalyMaxFloor;

        if (inMovingRange && !movingAnomalyAlreadySpawned && floor == movingAnomalyTargetFloor)
        {
            movingAnomalyAlreadySpawned = true;
            return Random.value < 0.5f ? AnomalyType.MesinFotokopi : AnomalyType.DispenserGalon;
        }

        // Jika tidak ada anomali bergerak di lantai ini, berikan peluang untuk anomali statis
        // (Berlaku untuk semua lantai selain Lantai 9)
        if (Random.value < anomalyChance)
        {
            return AnomalyType.Static;
        }

        // Jika lolos dari semua kondisi di atas, lantai normal
        return AnomalyType.None;
    }
}
