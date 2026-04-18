using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [Header("随机外观（在 Inspector 里拖入不同障碍物 Sprite）")]
    [SerializeField] private Sprite[] variants;

    private SpriteRenderer sr;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void OnEnable()
    {
        // 每次激活时随机选一个 Sprite
        if (sr != null && variants != null && variants.Length > 0)
            sr.sprite = variants[Random.Range(0, variants.Length)];
    }

    public void onDestr()
    {
        if (ObjectPool.Instance)
            ObjectPool.Instance.Return(gameObject);
        else
            Destroy(gameObject);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        PlayerController player = collision.collider.GetComponent<PlayerController>();
        if (player != null)
        {
            player.TakeDamage();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        PlayerController player = other.GetComponent<PlayerController>();
        if (player != null)
        {
            player.TakeDamage();
        }
    }
}
