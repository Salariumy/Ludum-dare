using System.Collections;
using UnityEngine;

/// <summary>
/// 信号波视觉效果：高频发射信号时的扩散动画
/// 挂在 SignalWavePrefab 上（CircleCollider2D(Trigger) + SpriteRenderer 圆形）
/// </summary>
public class SignalWave : MonoBehaviour
{
    [SerializeField] private float expandSpeed = 8f;

    private float maxRadius;
    private float currentRadius;
    private SpriteRenderer sr;

    public void Init(float radius)
    {
        maxRadius     = radius;
        currentRadius = 0.05f;
        sr            = GetComponent<SpriteRenderer>();
        sr.color      = new Color(0f, 1f, 1f, 0.4f);
        StartCoroutine(Expand());
    }

    IEnumerator Expand()
    {
        while (currentRadius < maxRadius)
        {
            currentRadius += expandSpeed * Time.deltaTime;
            float scale = currentRadius * 2f;
            transform.localScale = new Vector3(scale, scale, 1f);

            Color c = sr.color;
            c.a = Mathf.Lerp(0.4f, 0f, currentRadius / maxRadius);
            sr.color = c;

            yield return null;
        }
        if (ObjectPool.Instance)
            ObjectPool.Instance.Return(gameObject);
        else
            Destroy(gameObject);
    }
}
