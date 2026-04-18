using System.Collections;
using UnityEngine;

public enum SignalFrequency { High, Low }

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("自动移动")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("跳跃")]
    [SerializeField] private float     jumpForce          = 11f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float     groundCheckRadius  = 0.15f;
    [SerializeField] private int       maxJumpCount       = 2;
    [SerializeField] private float     fallMultiplier     = 2.5f;
    [SerializeField] private float     lowJumpMultiplier  = 2f;

    [Header("信号")]
    [SerializeField] private float signalRadius   = 8f;
    [SerializeField] private float signalInterval = 0.8f;

    [Header("射击")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform  firePoint;
    [SerializeField] private float      fireRate = 0.3f;

    [Header("血量")]
    [SerializeField] private int   maxHealth       = 4;
    [SerializeField] private float invincibleTime  = 1f;

    public SignalFrequency CurrentFrequency { get; private set; } = SignalFrequency.High;
    public int   CurrentHealth { get; private set; }
    public int   CoinCount     { get; private set; }
    public float Distance      { get; private set; }

    private Rigidbody2D    rb;
    private SpriteRenderer sr;
    private VisionManager  visionManager;
    private bool  isGrounded;
    private int   jumpCount;
    private float signalTimer;
    private float fireTimer;
    private bool  isInvincible;
    private float startX;

    void Awake()
    {
        rb            = GetComponent<Rigidbody2D>();
        sr            = GetComponentInChildren<SpriteRenderer>();
        visionManager = FindObjectOfType<VisionManager>();
        CurrentHealth = maxHealth;
        startX        = transform.position.x;
    }

    void Update()
    {
        CheckGround();
        HandleJump();
        HandleFrequencySwitch();

        if (CurrentFrequency == SignalFrequency.High)
            HandleHighFrequency();
        else
            HandleLowFrequency();

        UpdateDistance();
        fireTimer -= Time.deltaTime;
    }

    void FixedUpdate()
    {
        rb.velocity = new Vector2(moveSpeed, rb.velocity.y);
        ApplyBetterGravity();
    }

    void ApplyBetterGravity()
    {
        if (rb.velocity.y < 0f)
            rb.velocity += Vector2.up * Physics2D.gravity.y
                               * (fallMultiplier - 1f) * Time.fixedDeltaTime;
        else if (rb.velocity.y > 0f && !Input.GetButton("Jump"))
            rb.velocity += Vector2.up * Physics2D.gravity.y
                               * (lowJumpMultiplier - 1f) * Time.fixedDeltaTime;
    }

    void CheckGround()
    {
        bool was = isGrounded;
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        if (!was && isGrounded) jumpCount = 0;
    }

    void HandleJump()
    {
        if (Input.GetButtonDown("Jump") && (isGrounded || jumpCount < maxJumpCount))
        {
            rb.velocity = new Vector2(rb.velocity.x, 0f);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            jumpCount++;
        }
    }

    // ───── E 切换频段 ─────
    void HandleFrequencySwitch()
    {
        if (!Input.GetKeyDown(KeyCode.E)) return;

        CurrentFrequency = CurrentFrequency == SignalFrequency.High
            ? SignalFrequency.Low
            : SignalFrequency.High;

        EventBus.Publish(GameEvents.FrequencyChanged, CurrentFrequency);

        if (CurrentFrequency == SignalFrequency.Low && visionManager)
            visionManager.SetSignalActive(false);
    }

    // ───── 高频：自动发信号照亮场景 ─────
    void HandleHighFrequency()
    {
        signalTimer -= Time.deltaTime;
        if (signalTimer <= 0f)
        {
            signalTimer = signalInterval;
            if (visionManager) visionManager.EmitPulse(transform.position, signalRadius);
        }
    }

    // ───── 低频：Q 射子弹 ─────
    void HandleLowFrequency()
    {
        if (Input.GetKeyDown(KeyCode.Q) && fireTimer <= 0f)
        {
            fireTimer = fireRate;
            if (bulletPrefab && firePoint)
                Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        }
    }

    void UpdateDistance()
    {
        Distance = transform.position.x - startX;
        EventBus.Publish(GameEvents.DistanceUpdated, Distance);
    }

    public void CollectCoin()
    {
        CoinCount++;
        EventBus.Publish(GameEvents.CoinCollected, CoinCount);
    }

    public void TakeDamage()
    {
        if (isInvincible) return;
        CurrentHealth--;
        EventBus.Publish(GameEvents.PlayerHit, CurrentHealth);
        if (RunnerCamera.Instance) RunnerCamera.Instance.Shake();

        if (CurrentHealth <= 0)
        {
            EventBus.Publish(GameEvents.PlayerDied);
            return;
        }
        StartCoroutine(InvincibleRoutine());
    }

    IEnumerator InvincibleRoutine()
    {
        isInvincible = true;
        float timer = 0f;
        while (timer < invincibleTime)
        {
            sr.enabled = !sr.enabled;
            yield return new WaitForSeconds(0.1f);
            timer += 0.1f;
        }
        sr.enabled   = true;
        isInvincible = false;
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Obstacle"))
            TakeDamage();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Coin"))
        {
            CollectCoin();
            Destroy(other.gameObject);
        }
        else if (other.CompareTag("LevelEnd"))
        {
            EventBus.Publish(GameEvents.LevelCompleted);
        }
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}
