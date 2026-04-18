using UnityEngine;

/// <summary>
/// 子弹：向右飞行，碰到障碍物摧毁双方
/// 挂在 BulletPrefab 上（SpriteRenderer + CircleCollider2D(Trigger) + Rigidbody2D(Kinematic)）
/// </summary>
public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed    = 12f;
    [SerializeField] private float lifeTime = 3f;

    private float timer;

    void OnEnable()
    {
        timer = lifeTime;
    }

    void ReturnToPool()
    {
        if (ObjectPool.Instance)
            ObjectPool.Instance.Return(gameObject);
        else
            Destroy(gameObject);
    }

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f) { ReturnToPool(); return; }
        transform.Translate(Vector3.right * speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Obstacle"))
        {
            // 有雾的障碍物不会被子弹摧毁
            var fog = other.GetComponent<FogCover>();
            if (fog != null && !fog.IsRevealed)
            {
                ReturnToPool();
                return;
            }

            var obs = other.GetComponent<Obstacle>();
            if (obs) obs.onDestr();
            ReturnToPool();
        }
    }
}
