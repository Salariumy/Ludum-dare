using System.Collections;
using UnityEngine;

/// <summary>
/// 宝箱：固定位置奖励，碰到后给玩家金币 + 短暂护盾
/// Tag: "Chest"，Collider2D: Trigger
/// 自身只负责视觉和碰撞检测，通过 EventBus 广播
/// </summary>
public class Book : MonoBehaviour
{

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

    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (isOpened) return;
        if (!other.CompareTag("Player")) return;


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

    }
}