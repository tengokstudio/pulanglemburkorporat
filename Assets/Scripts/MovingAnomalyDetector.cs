using UnityEngine;

// Trigger ngejar bukan lagi pas player fisik nabrak collider ini, tapi pas
// sebagian besar area collider ini ("detector") udah masuk viewport kamera —
// simulasi "player ngeliat anomali di layar".
public class MovingAnomalyDetector : MonoBehaviour
{
    [Range(0f, 1f)]
    [SerializeField] private float visibleFractionThreshold = 0.5f;

    private MovingAnomalyBase parentAnomaly;
    private Collider2D detectorCollider;
    private Camera cam;

    private void Awake()
    {
        parentAnomaly = GetComponentInParent<MovingAnomalyBase>();
        if (parentAnomaly == null)
            Debug.LogError("[MovingAnomalyDetector] Ga nemu MovingAnomalyBase di parent!");

        detectorCollider = GetComponent<Collider2D>();
        cam = Camera.main;
    }

    private void Update()
    {
        if (parentAnomaly == null || detectorCollider == null || cam == null) return;

        if (GetVisibleFraction() >= visibleFractionThreshold)
            parentAnomaly.StartChasing();
    }

    private float GetVisibleFraction()
    {
        Bounds bounds = detectorCollider.bounds;
        Vector3 viewMin = cam.WorldToViewportPoint(bounds.min);
        Vector3 viewMax = cam.WorldToViewportPoint(bounds.max);

        float left = Mathf.Min(viewMin.x, viewMax.x);
        float right = Mathf.Max(viewMin.x, viewMax.x);
        float totalWidth = right - left;
        if (totalWidth <= 0f) return 0f;

        float visibleWidth = Mathf.Clamp01(right) - Mathf.Clamp01(left);
        return Mathf.Max(0f, visibleWidth) / totalWidth;
    }
}
