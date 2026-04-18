using UnityEngine;

/// <summary>
/// 动画驱动器：读取 PlayerController 和 Rigidbody2D 状态，同步给 Animator
/// 挂在 Player 根节点上（和 PlayerController 同一 GameObject）
/// </summary>
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(PlayerController))]
public class PlayerAnimator : MonoBehaviour
{
    private Animator         anim;
    private PlayerController controller;
    private Rigidbody2D      rb;

    // Animator 参数哈希（用哈希比字符串查找更快）
    private static readonly int HashSpeed      = Animator.StringToHash("Speed");
    private static readonly int HashYVelocity  = Animator.StringToHash("YVelocity");
    private static readonly int HashGrounded   = Animator.StringToHash("Grounded");
    private static readonly int HashShoot      = Animator.StringToHash("Shoot");
    private static readonly int HashHit        = Animator.StringToHash("Hit");
    private static readonly int HashDead       = Animator.StringToHash("Dead");

    private SignalFrequency lastFrequency;
    private int             lastHealth;

    void Awake()
    {
        anim       = GetComponent<Animator>();
        controller = GetComponent<PlayerController>();
        rb         = GetComponent<Rigidbody2D>();

        lastFrequency = controller.CurrentFrequency;
        lastHealth    = controller.CurrentHealth;
    }

    void Update()
    {
        // ── 持续更新的参数 ──
        anim.SetFloat(HashSpeed,     Mathf.Abs(rb.velocity.x));
        anim.SetFloat(HashYVelocity, rb.velocity.y);

        // Grounded：用 YVelocity 接近 0 且速度不向下来近似判断（也可直接在 PlayerController 暴露 isGrounded）
        bool grounded = Mathf.Abs(rb.velocity.y) < 0.05f;
        anim.SetBool(HashGrounded, grounded);

        // ── 死亡 ──
        if (controller.CurrentHealth <= 0)
        {
            anim.SetBool(HashDead, true);
            enabled = false;   // 死后不再更新
            return;
        }

        // ── 受伤（血量减少时触发一次）──
        if (controller.CurrentHealth < lastHealth)
        {
            anim.SetTrigger(HashHit);
        }
        lastHealth = controller.CurrentHealth;

        // ── 射击（低频模式下按 Q）──
        if (controller.CurrentFrequency == SignalFrequency.Low
            && Input.GetKeyDown(KeyCode.Q))
        {
            anim.SetTrigger(HashShoot);
        }
    }
}
