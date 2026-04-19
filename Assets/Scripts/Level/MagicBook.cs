using UnityEngine;

/// <summary>
/// 通关书本物体
/// 玩家触碰到书后，播放书的打开动画，并广播进入关卡结束状态
/// </summary>
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Animator))]
public class MagicBook : MonoBehaviour
{
    private Animator animator;
    private bool hasTriggered = false;

    void Awake()
    {
        animator = GetComponent<Animator>();
        // 禁用 Animator，防止默认状态自动播放开书动画
        animator.enabled = false;
        
        // 确保 Collider 是触发器
        var col = GetComponent<Collider2D>();
        if (col) col.isTrigger = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (hasTriggered) return;

        if (other.CompareTag("Player"))
        {
            hasTriggered = true;
            
            // 触碰时才启用 Animator 并播放开书动画
            animator.enabled = true;
            animator.SetTrigger("Open");

            // 播放翻书音效
            if (AudioManager.Instance)
                AudioManager.Instance.PlayBookFlip();

            // 广播关卡完成事件
            EventBus.Publish(GameEvents.LevelCompleted);
        }
    }
}
