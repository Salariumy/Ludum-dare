using UnityEngine;

/// <summary>
/// 深渊检测：玩家掉出地图底部时扣血并复位
/// 放在地图下方一个长条 Trigger Collider
/// </summary>
public class FallZone : MonoBehaviour
{
    [SerializeField] private float respawnYOffset = 5f;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        var player = other.GetComponent<PlayerController>();
        if (player == null) return;

        player.TakeDamage();

        // 把玩家传送回安全高度
        var rb = other.GetComponent<Rigidbody2D>();
        if (rb) rb.velocity = Vector2.zero;
        other.transform.position = new Vector3(
            other.transform.position.x,
            transform.position.y + respawnYOffset,
            0f
        );
    }
}
