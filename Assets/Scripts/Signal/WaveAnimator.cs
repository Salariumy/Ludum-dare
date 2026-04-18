using UnityEngine;

/// <summary>
/// 跟随 Player 的波动画管理器。
/// 根据频段切换播放 attack wave / signal wave 动画，
/// 同时执行摧毁障碍物（attack）和清除迷雾（signal）的功能逻辑。
/// </summary>
[RequireComponent(typeof(Animator))]
public class WaveAnimator : MonoBehaviour
{

    [Header("Attack（低频）")]
    [SerializeField] private float     attackRange = 20f;
    [SerializeField] private LayerMask attackHitLayers;

    [Header("Signal（高频）")]
    [SerializeField] private float signalRadius = 8f;

    private Animator   anim;
    private FogManager fogManager;

    // Animator parameter hashes
    private static readonly int HashShoot = Animator.StringToHash("Shoot");
    private static readonly int HashPulse = Animator.StringToHash("Pulse");

    void Awake()
    {
        anim       = GetComponent<Animator>();
        fogManager = FindObjectOfType<FogManager>();
    }


    /// <summary>低频 Q：播放 attack wave 动画 + 瞬间射线摧毁障碍物</summary>
    public bool Shoot(Vector3 origin)
    {
        anim.SetTrigger(HashShoot);
        DoAttackRaycast(origin);
        return true;
    }

    /// <summary>高频 Q：播放 signal wave 动画 + 清除迷雾</summary>
    public void Pulse(Vector3 origin)
    {
        anim.SetTrigger(HashPulse);
        DoFogClear(origin);
    }

    void DoAttackRaycast(Vector3 origin)
    {
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.right, attackRange, attackHitLayers);

        if (hit.collider != null && hit.collider.CompareTag("Obstacle"))
        {
            var fog = hit.collider.GetComponent<FogCover>();
            if (fog == null || fog.IsRevealed)
            {
                var obs = hit.collider.GetComponent<Obstacle>();
                if (obs != null)
                    obs.Shatter();
            }
        }
    }

    void DoFogClear(Vector3 origin)
    {
        if (fogManager != null)
            fogManager.EmitPulse(origin, signalRadius);
    }
}
