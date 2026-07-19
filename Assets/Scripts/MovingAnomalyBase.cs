using UnityEngine;

public enum AnomalyChaseState
{
    Idle,
    Chasing,
    Retreating
}

public abstract class MovingAnomalyBase : MonoBehaviour
{
    [Header("References")]
    [SerializeField] protected PlayerController playerController;

    [Header("Config")]
    [SerializeField] protected float moveSpeed = 2f;
    [SerializeField] protected float catchDistance = 0.5f;

    [Header("Retreat (pas player ngumpet)")]
    [Tooltip("Anomali jalan terus ke kiri (ga peduli player lagi) sampai X ini, baru menghilang. Sesuaiin ke tepi kiri background di scene.")]
    [SerializeField] protected float leftExitX = -50f;

    protected Vector3 startPosition;
    public AnomalyChaseState State { get; private set; } = AnomalyChaseState.Idle;

    protected virtual void Awake()
    {
        startPosition = transform.position;
    }

    protected virtual void OnEnable()
    {
        transform.position = startPosition;
        State = AnomalyChaseState.Idle;
        Physics2D.SyncTransforms(); // collider (termasuk Detector di child) langsung update, ga nunggu physics step berikutnya
    }

    public void StartChasing()
    {
        if (State != AnomalyChaseState.Idle) return;
        State = AnomalyChaseState.Chasing;
        Debug.Log($"[{GetType().Name}] Player kedeteksi, mulai ngejar!");
    }

    /// <summary>Dipanggil AnomalySpawner tiap ganti lantai (caught, wrong choice, atau lanjut normal)
    /// biar anomali yang lagi Chasing/Retreating ga nyangkut state lama pas dipake lagi.</summary>
    public void ResetToStart()
    {
        transform.position = startPosition;
        State = AnomalyChaseState.Idle;
        Physics2D.SyncTransforms(); // sama kayak di OnEnable — cegah Detector baca posisi collider yang lama
    }

    private void Update()
    {
        if (playerController == null)
        {
            Debug.LogWarning($"[{GetType().Name}] PlayerController reference NULL!");
            return;
        }

        switch (State)
        {
            case AnomalyChaseState.Idle:
                return;

            case AnomalyChaseState.Chasing:
                if (playerController.IsHiding)
                {
                    // Player ngumpet -> anomali ga lagi ngejar player, cuma jalan terus ke kiri sampai hilang.
                    State = AnomalyChaseState.Retreating;
                    return;
                }
                MoveTowardsTarget(new Vector3(playerController.transform.position.x, transform.position.y, transform.position.z));
                CheckCatch();
                return;

            case AnomalyChaseState.Retreating:
                Vector3 leftTarget = new Vector3(leftExitX, transform.position.y, transform.position.z);
                MoveTowardsTarget(leftTarget);
                if (transform.position.x <= leftExitX + 0.05f)
                {
                    gameObject.SetActive(false); // menghilang; OnEnable bakal reset posisi & state kalau dipake lagi nanti
                }
                return;
        }
    }

    private void CheckCatch()
    {
        float distanceX = Mathf.Abs(transform.position.x - playerController.transform.position.x);
        if (distanceX <= catchDistance)
        {
            Debug.Log($"[{GetType().Name}] JARAK MASUK, manggil PlayerCaught()");
            FloorManager.Instance?.PlayerCaught();
        }
    }

    protected abstract void MoveTowardsTarget(Vector3 target);
}
