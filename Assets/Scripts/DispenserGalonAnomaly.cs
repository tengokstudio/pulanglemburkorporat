using UnityEngine;

// Geraknya lompat-lompat (hop) selagi ke arah player.
public class DispenserGalonAnomaly : MovingAnomalyBase
{
    [Header("Hop")]
    [SerializeField] private float hopHeight = 0.3f;
    [SerializeField] private float hopFrequency = 4f;

    [Header("Camera Shake (pas landing)")]
    [SerializeField] private float shakeDuration = 0.08f;
    [SerializeField] private float shakeMagnitude = 0.05f;

    private float previousHop;

    protected override void MoveTowardsTarget(Vector3 target)
    {
        Vector3 flatCurrent = new Vector3(transform.position.x, startPosition.y, transform.position.z);
        Vector3 flatTarget = new Vector3(target.x, startPosition.y, target.z);
        Vector3 nextFlat = Vector3.MoveTowards(flatCurrent, flatTarget, moveSpeed * Time.deltaTime);

        float hop = Mathf.Abs(Mathf.Sin(Time.time * hopFrequency)) * hopHeight;

        // Landing terdeteksi begitu hop turun balik nyentuh dasar -> shake kamera + SFX "tak".
        if (previousHop > 0.01f && hop <= 0.01f)
        {
            CameraFollow.Instance?.Shake(shakeDuration, shakeMagnitude);
            AudioManager.Instance?.PlayDispenserHop();
        }
        previousHop = hop;

        transform.position = new Vector3(nextFlat.x, startPosition.y + hop, nextFlat.z);
    }
}
