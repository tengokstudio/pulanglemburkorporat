using UnityEngine;

// Geraknya slide/geser lurus ke arah player, dikasih geter halus (efek friction) selagi jalan.
public class MesinFotokopiAnomaly : MovingAnomalyBase
{
    [Header("Shake (efek friction)")]
    [SerializeField] private float shakeAmount = 0.03f;
    [SerializeField] private float shakeFrequency = 25f;

    protected override void MoveTowardsTarget(Vector3 target)
    {
        AudioManager.Instance?.SetFotokopiSlideLooping(true);

        Vector3 baseTarget = new Vector3(target.x, startPosition.y, target.z);
        Vector3 baseCurrent = new Vector3(transform.position.x, startPosition.y, transform.position.z);
        Vector3 next = Vector3.MoveTowards(baseCurrent, baseTarget, moveSpeed * Time.deltaTime);

        float shake = (Mathf.PerlinNoise(Time.time * shakeFrequency, 0f) - 0.5f) * 2f * shakeAmount;
        transform.position = new Vector3(next.x, startPosition.y + shake, next.z);
    }

    protected override void OnBecameIdle()
    {
        AudioManager.Instance?.SetFotokopiSlideLooping(false);
    }
}
