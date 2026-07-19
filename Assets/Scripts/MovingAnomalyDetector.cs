using UnityEngine;

public class MovingAnomalyDetector : MonoBehaviour
{
    private MovingAnomaly parentAnomaly;

    private void Awake()
    {
        parentAnomaly = GetComponentInParent<MovingAnomaly>();
        if (parentAnomaly == null)
            Debug.LogError("[MovingAnomalyDetector] Ga nemu MovingAnomaly di parent!");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        parentAnomaly?.StartChasing();
    }
}