using UnityEngine;

/// <summary>
/// 金币：被玩家碰到后收集
/// Tag 设为 "Coin"，Collider2D 设为 Trigger
/// </summary>
public class Coin : MonoBehaviour
{
    [SerializeField] private float bobSpeed  = 2f;
    [SerializeField] private float bobHeight = 0.15f;

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        // 上下浮动小动画
        float offset = Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        transform.position = startPos + Vector3.up * offset;
    }

    // 碰撞在 PlayerController.OnTriggerEnter2D 中处理
}
