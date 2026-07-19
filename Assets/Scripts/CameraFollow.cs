using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraFollow : MonoBehaviour
{
    public static CameraFollow Instance { get; private set; }

    [Header("Target")]
    [SerializeField] private Transform target; // Player

    [Header("Follow")]
    [SerializeField] private float smoothTime = 0.15f;
    [SerializeField] private float offsetX = 0f;

    [Header("Level Bounds")]
    [Tooltip("Drag renderer ground di sini — batas kamera diambil otomatis dari lebar objek ini. Kosongin buat pakai levelMinX/levelMaxX manual.")]
    [SerializeField] private Renderer groundRenderer;
    [SerializeField] private float levelMinX = -49f;
    [SerializeField] private float levelMaxX = 49f;

    private Camera cam;
    private Vector3 velocity;
    private Vector3 followPosition;

    private float shakeDuration;
    private float shakeTimeRemaining;
    private float shakeMagnitude;

    private void Awake()
    {
        Instance = this;
        cam = GetComponent<Camera>();
        followPosition = transform.position;

        if (groundRenderer != null)
        {
            levelMinX = groundRenderer.bounds.min.x;
            levelMaxX = groundRenderer.bounds.max.x;
        }
    }

    /// <summary>Trigger shake kecil, dipanggil dari luar (misal anomali lompat landing).</summary>
    public void Shake(float duration, float magnitude)
    {
        shakeDuration = duration;
        shakeTimeRemaining = duration;
        shakeMagnitude = magnitude;
    }

    private void LateUpdate()
    {
        if (target == null) return;

        float halfWidth = cam.orthographicSize * cam.aspect;
        float minX = levelMinX + halfWidth;
        float maxX = levelMaxX - halfWidth;

        float desiredX = target.position.x + offsetX;
        float clampedX = maxX >= minX ? Mathf.Clamp(desiredX, minX, maxX) : (levelMinX + levelMaxX) * 0.5f;

        Vector3 desired = new Vector3(clampedX, followPosition.y, followPosition.z);
        followPosition = Vector3.SmoothDamp(followPosition, desired, ref velocity, smoothTime);

        Vector3 shakeOffset = Vector3.zero;
        if (shakeTimeRemaining > 0f)
        {
            shakeTimeRemaining -= Time.deltaTime;
            float falloff = Mathf.Clamp01(shakeTimeRemaining / shakeDuration);
            shakeOffset = (Vector3)(Random.insideUnitCircle * shakeMagnitude * falloff);
        }

        transform.position = followPosition + shakeOffset;
    }
}
