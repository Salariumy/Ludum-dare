using System.Collections;
using UnityEngine;

/// <summary>
/// 宝箱：固定位置奖励，碰到后给玩家金币 + 短暂护盾
/// Tag: "Chest"，Collider2D: Trigger
/// 自身只负责视觉和碰撞检测，通过 EventBus 广播
/// </summary>
public class Chest : MonoBehaviour
{
    [Header("奖励")]
    [SerializeField] private int   coinReward    = 8;
    [SerializeField] private float shieldTime    = 3f;

    [Header("视觉")]
    [SerializeField] private Sprite closedSprite;
    [SerializeField] private Sprite openedSprite;

    private SpriteRenderer sr;
    private Animator animator;
    private bool isOpened;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    void OnEnable()
    {
        isOpened = false;
        
        // 如果有Animator，重置为关闭状态
        if (animator)
        {
            animator.SetBool("IsOpen", false);
        }

        if (sr)
        {
            sr.enabled = true;
            Color c = sr.color;
            c.a = 1f;
            sr.color = c;
            if (closedSprite)
                sr.sprite = closedSprite;
        }

        // 清理池复用时可能残留的雾子物体
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            var child = transform.GetChild(i);
            if (child.name == "Fog")
                DestroyImmediate(child.gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (isOpened) return;
        if (!other.CompareTag("Player")) return;

        // 被雾覆盖时由 PlayerController 统一处理伤害，不触发宝箱逻辑
        var fog = GetComponent<FogCover>();
        if (fog != null && !fog.IsRevealed) return;

        isOpened = true;

        // 播放打开动画
        if (animator)
        {
            animator.SetBool("IsOpen", true);
        }
        else if (sr && openedSprite)
        {
            // 如果没有Animator的降级处理：直接换图
            sr.sprite = openedSprite;
        }

        // 广播宝箱事件，传递奖励数据
        var reward = new ChestReward { coins = coinReward, shieldDuration = shieldTime };
        EventBus.Publish(GameEvents.ChestOpened, reward);

        // ※ 移除了原本的摧毁逻辑，宝箱现在只会打开并留在原地 ※
    }
}

/// <summary>宝箱奖励数据，通过 EventBus 传递</summary>
public class ChestReward
{
    public int   coins;
    public float shieldDuration;
}
