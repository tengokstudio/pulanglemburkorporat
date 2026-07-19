using UnityEngine;

public class MovingAnomaly : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerController playerController;

    [Header("Config")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float catchDistance = 0.5f;

    private Vector3 startPosition;
    private bool isChasing;

    private void Awake()
    {
        startPosition = transform.position;
    }

    private void OnEnable()
    {
        transform.position = startPosition;
        isChasing = false;
    }

    public void StartChasing()
    {
        if (isChasing) return;
        isChasing = true;
        Debug.Log("[MovingAnomaly] Player kedeteksi, mulai ngejar!");
    }


    private void Update()
    {
        if (playerController == null)
        {
            Debug.LogWarning("[MovingAnomaly] PlayerController reference NULL!");
            return;
        }

        if (!isChasing) return; // diam total sampai ke-trigger

        Vector3 target = playerController.IsHiding
            ? startPosition
            : new Vector3(playerController.transform.position.x, transform.position.y, transform.position.z);

        transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);

        if (playerController.IsHiding)
        {
            // Kalau udah balik ke start posisi sambil player masih sembunyi, matiin chase lagi.
            // Butuh trigger baru buat ngejar lagi nanti.
            if (Vector3.Distance(transform.position, startPosition) < 0.05f)
                isChasing = false;
            return;
        }

        float distanceX = Mathf.Abs(transform.position.x - playerController.transform.position.x);
        if (distanceX <= catchDistance)
        {
            Debug.Log("[MovingAnomaly] JARAK MASUK, manggil PlayerCaught()");
            FloorManager.Instance?.PlayerCaught();
        }
    }
}