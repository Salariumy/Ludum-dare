using UnityEngine;

/// <summary>
/// 关卡终点线：玩家碰到后触发通关
/// Tag 设为 "LevelEnd"，Collider2D 设为 Trigger
/// 放在每关地图最右端
/// </summary>
public class LevelEndTrigger : MonoBehaviour
{
    // 碰撞在 PlayerController.OnTriggerEnter2D 中处理
    // 此处可加通关特效

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            EventBus.Publish(GameEvents.LevelCompleted);
    }
}
