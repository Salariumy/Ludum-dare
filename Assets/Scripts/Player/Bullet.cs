using UnityEngine;

/// <summary>
/// 子弹：向右飞行，碰到障碍物摧毁双方
/// 挂在 BulletPrefab 上（SpriteRenderer + CircleCollider2D(Trigger) + Rigidbody2D(Kinematic)）
/// </summary>
public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed    = 12f;
    [SerializeField] private float lifeTime = 3f;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        transform.Translate(Vector3.right * speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Obstacle"))
        {
            // 摧毁障碍物
            Destroy(other.gameObject);
            Destroy(gameObject);
        }
    }
}
