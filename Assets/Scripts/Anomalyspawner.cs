using UnityEngine;

public enum AnomalyType
{
    None,
    StaticTanaman,
    StaticKeranjangSampah,
    StaticPantry,
    StaticMeetingRoom,
    StaticFotokopiKebuka,
    MovingFotokopi,
    MovingDispenser
}

public class AnomalySpawner : MonoBehaviour
{
    public static AnomalySpawner Instance { get; private set; }

    [Header("Background per State")]
    [SerializeField] private SpriteRenderer groundRenderer;
    [SerializeField] private Sprite normalSprite;
    [SerializeField] private Sprite tanamanSprite;
    [SerializeField] private Sprite keranjangSampahSprite;
    [SerializeField] private Sprite pantrySprite;
    [SerializeField] private Sprite meetingRoomSprite;
    [SerializeField] private Sprite fotokopiKebukaSprite;
    [SerializeField] private Sprite fotokopiGerakBackgroundSprite;
    [SerializeField] private Sprite dispenserGerakBackgroundSprite;

    [Header("Moving Anomaly Entities (drag di sini)")]
    [SerializeField] private GameObject movingMarkerFotokopi;     // enable/disable
    [SerializeField] private GameObject movingMarkerDispenser;    // enable/disable

    [Header("Config")]
    [Tooltip("Peluang static anomaly muncul per lantai (index 0 = lantai 9, urut turun sampai lantai 1). Dipakai kalau panjangnya cocok, kalau tidak fallback ke defaultAnomalyChance.")]
    [SerializeField] private float[] anomalyChancePerFloor;
    [Range(0f, 1f)]
    [SerializeField] private float defaultAnomalyChance = 0.5f;
    [SerializeField] private int movingAnomalyMinFloor = 2;
    [SerializeField] private int movingAnomalyMaxFloor = 5;

    [Header("Debug Testing (matiin kalau udah gak dipake)")]
    [Tooltip("Paksa lantai 9 langsung ada Mesin Fotokopi gerak, skip nunggu random roll.")]
    [SerializeField] private bool debugForceFloor9Fotokopi = false;
    [Tooltip("Paksa lantai 9 langsung ada Dispenser Galon gerak, skip nunggu random roll.")]
    [SerializeField] private bool debugForceFloor9Dispenser = false;

    public AnomalyType CurrentAnomalyType { get; private set; } = AnomalyType.None;

    private bool movingAnomalyAlreadySpawned;
    private int movingAnomalyTargetFloor;

    private static readonly AnomalyType[] StaticTypes =
    {
        AnomalyType.StaticTanaman,
        AnomalyType.StaticKeranjangSampah,
        AnomalyType.StaticPantry,
        AnomalyType.StaticMeetingRoom,
        AnomalyType.StaticFotokopiKebuka
    };

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void OnDisable()
    {
        if (FloorManager.Instance != null)
            FloorManager.Instance.OnFloorChanged -= HandleFloorChanged;
    }

    private void Start()
    {
        FloorManager.Instance.OnFloorChanged += HandleFloorChanged; // pindah ke sini
        PickMovingAnomalyFloor();
        ApplyState(AnomalyType.None);
    }

    private void PickMovingAnomalyFloor()
    {
        movingAnomalyTargetFloor = Random.Range(movingAnomalyMinFloor, movingAnomalyMaxFloor + 1);
        movingAnomalyAlreadySpawned = false;
        Debug.Log($"[AnomalySpawner] Moving anomaly run ini muncul di floor {movingAnomalyTargetFloor}");
    }

    private void HandleFloorChanged(int floor)
    {
        AnomalyType type = DecideAnomalyType(floor);
        CurrentAnomalyType = type;
        ApplyState(type);

        bool hasAnomaly = type != AnomalyType.None;
        FloorManager.Instance.SetCurrentFloorAnomaly(hasAnomaly);

        Debug.Log($"[AnomalySpawner] Floor {floor} -> {type}");
    }

    private AnomalyType DecideAnomalyType(int floor)
    {
        if (floor == 9)
        {
            PickMovingAnomalyFloor();

            if (debugForceFloor9Dispenser) return AnomalyType.MovingDispenser;
            if (debugForceFloor9Fotokopi) return AnomalyType.MovingFotokopi;

            return AnomalyType.None;
        }

        if (floor == 1)
        {
            return AnomalyType.None;
        }

        bool inMovingRange = floor >= movingAnomalyMinFloor && floor <= movingAnomalyMaxFloor;

        if (inMovingRange && !movingAnomalyAlreadySpawned && floor == movingAnomalyTargetFloor)
        {
            movingAnomalyAlreadySpawned = true;
            return Random.value < 0.5f ? AnomalyType.MovingFotokopi : AnomalyType.MovingDispenser;
        }

        if (Random.value < GetAnomalyChance(floor))
        {
            return StaticTypes[Random.Range(0, StaticTypes.Length)];
        }

        return AnomalyType.None;
    }

    private float GetAnomalyChance(int floor)
    {
        int index = 9 - floor; // lantai 9 -> index 0, lantai 1 -> index 8
        if (anomalyChancePerFloor != null && index >= 0 && index < anomalyChancePerFloor.Length)
            return anomalyChancePerFloor[index];

        return defaultAnomalyChance;
    }

    private void ApplyState(AnomalyType type)
    {
        if (groundRenderer != null)
            groundRenderer.sprite = GetBackgroundSprite(type);

        ResetAndSetActive(movingMarkerFotokopi, type == AnomalyType.MovingFotokopi);
        ResetAndSetActive(movingMarkerDispenser, type == AnomalyType.MovingDispenser);
    }

    private void ResetAndSetActive(GameObject marker, bool active)
    {
        if (marker == null) return;

        // Reset dulu SEBELUM SetActive: kalau objeknya udah aktif dari lantai sebelumnya
        // (misal abis caught terus lantai baru masih anomali yang sama), SetActive(true) ga
        // nge-trigger OnEnable lagi karena statusnya emang udah aktif -> tanpa reset manual ini
        // dia bakal nerusin state Chasing/Retreating lama dan nyangkut/ngejar ulang posisi baru player.
        var anomaly = marker.GetComponent<MovingAnomalyBase>();
        if (anomaly != null) anomaly.ResetToStart();

        marker.SetActive(active);
    }

    private Sprite GetBackgroundSprite(AnomalyType type)
    {
        switch (type)
        {
            case AnomalyType.StaticTanaman: return tanamanSprite;
            case AnomalyType.StaticKeranjangSampah: return keranjangSampahSprite;
            case AnomalyType.StaticPantry: return pantrySprite;
            case AnomalyType.StaticMeetingRoom: return meetingRoomSprite;
            case AnomalyType.StaticFotokopiKebuka: return fotokopiKebukaSprite;
            case AnomalyType.MovingFotokopi: return fotokopiGerakBackgroundSprite;
            case AnomalyType.MovingDispenser: return dispenserGerakBackgroundSprite;
            default: return normalSprite;
        }
    }
}
