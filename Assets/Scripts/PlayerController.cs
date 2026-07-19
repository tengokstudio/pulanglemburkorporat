using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float walkSpeed = 3f;
    [SerializeField] private float runSpeed = 6f;

    [Header("References")]
    [SerializeField] private Animator animator; // optional, aman kalau null

    private Rigidbody2D rb;
    private float moveInput;
    private bool isRunning;
    private bool isHiding;

    public bool IsHiding => isHiding;
    public bool FacingRight { get; private set; } = true;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Interact();
        }

        if (isHiding)
        {
            moveInput = 0f;
            UpdateAnimator(0f);
            return;
        }

        moveInput = 0f;
        if (Input.GetKey(KeyCode.A)) moveInput -= 1f;
        if (Input.GetKey(KeyCode.D)) moveInput += 1f;

        isRunning = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

        if (moveInput > 0 && !FacingRight) Flip();
        else if (moveInput < 0 && FacingRight) Flip();

        UpdateAnimator(Mathf.Abs(moveInput));
    }

    private void FixedUpdate()
    {
        if (isHiding) return;

        float speed = isRunning ? runSpeed : walkSpeed;
        rb.linearVelocity = new Vector2(moveInput * speed, rb.linearVelocity.y);
    }

    private void UpdateAnimator(float inputMagnitude)
    {
        if (animator == null) return;

        bool isMoving = inputMagnitude > 0.01f;
        animator.SetBool("IsWalking", isMoving && !isRunning);
        animator.SetBool("IsRunning", isMoving && isRunning);
    }

    private void Flip()
    {
        FacingRight = !FacingRight;
        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * (FacingRight ? 1f : -1f);
        transform.localScale = scale;
    }

    private HidingSpot currentHidingSpot;

    private void OnTriggerEnter2D(Collider2D other)
    {
        var spot = other.GetComponent<HidingSpot>();
        if (spot != null) currentHidingSpot = spot;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        var spot = other.GetComponent<HidingSpot>();
        if (spot != null && spot == currentHidingSpot)
            currentHidingSpot = null;
    }

    private void Interact()
    {
        if (currentHidingSpot != null)
        {
            SetHiding(!isHiding);
            Debug.Log(isHiding ? "Mulai sembunyi" : "Keluar dari sembunyi");
        }
    }

    public void TeleportTo(Vector3 position)
    {
        if (isHiding) SetHiding(false); // cegah stuck kalau direset sambil sembunyi
        rb.position = position;
        rb.linearVelocity = Vector2.zero;
        moveInput = 0f;
        Physics2D.SyncTransforms();
    }

    public void SetHiding(bool value)
    {
        isHiding = value;
        rb.linearVelocity = Vector2.zero; // biar ga nyisa momentum pas mulai hide
        if (animator != null)
        {
            animator.SetBool("IsWalking", false);
            animator.SetBool("IsRunning", false);
        }
    }

    /// <summary>Dipanggil FloorManager buat teleport player ke spawn point tiap floor mulai.
    /// Pakai rb.position (bukan transform.position) + SyncTransforms supaya physics
    /// collider langsung ke-update saat itu juga, ga nunggu FixedUpdate berikutnya.
    /// Ini fix dari bug sebelumnya: teleport mid-trigger-callback via transform.position
    /// bikin collider ForwardExitZone/BackwardExitZone berhenti fire event.</summary>
}