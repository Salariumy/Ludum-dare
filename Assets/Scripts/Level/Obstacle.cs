using UnityEngine;

/// <summary>
/// 障碍物：玩家碰到扣血，子弹碰到被摧毁
/// Tag 设为 "Obstacle"，碰撞体非 Trigger
/// </summary>
public class Obstacle : MonoBehaviour
{
    // 子弹通过 Bullet.cs 的 OnTriggerEnter2D 处理
    // 玩家通过 PlayerController 的 OnCollisionEnter2D 处理
    // 此脚本可扩展（如不同类型障碍物有不同血量）
}
