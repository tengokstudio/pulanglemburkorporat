using UnityEngine;

public enum AnomalyType
{
    None,
    Static,
    Moving
}

public class AnomalySpawner : MonoBehaviour
{
    public static AnomalySpawner Instance { get; private set; }

    [Header("Prototype Markers (drag di sini)")]
    [SerializeField] private SpriteRenderer staticMarker;   // ganti warna
    [SerializeField] private GameObject movingMarker;       // enable/disable

    [Header("Config")]
    [Range(0f, 1f)]
    [SerializeField] private float anomalyChance = 0.5f;
    [SerializeField] private int movingAnomalyMinFloor = 2;
    [SerializeField] private int movingAnomalyMaxFloor = 5;

    public AnomalyType CurrentAnomalyType { get; private set; } = AnomalyType.None;

    private bool movingAnomalyAlreadySpawned;
    private int movingAnomalyTargetFloor;

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
        SetMarkers(AnomalyType.None);
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
        SetMarkers(type);

        bool hasAnomaly = type != AnomalyType.None;
        FloorManager.Instance.SetCurrentFloorAnomaly(hasAnomaly);

        Debug.Log($"[AnomalySpawner] Floor {floor} -> {type}");
    }

    private AnomalyType DecideAnomalyType(int floor)
    {
        if (floor == 9)
        {
            PickMovingAnomalyFloor();
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
            return AnomalyType.Moving;
        }

        if (Random.value < anomalyChance)
        {
            return AnomalyType.Static;
        }

        return AnomalyType.None;
    }

    private void SetMarkers(AnomalyType type)
    {
        if (staticMarker != null)
            staticMarker.color = (type == AnomalyType.Static) ? Color.red : Color.white;

        if (movingMarker != null)
            movingMarker.SetActive(type == AnomalyType.Moving);
    }
}