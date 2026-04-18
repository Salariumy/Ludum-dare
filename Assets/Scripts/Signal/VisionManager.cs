using System.Collections;
using UnityEngine;

/// <summary>
/// 视野遮罩系统：高频信号照亮场景，无信号时黑暗
/// 挂在一个全屏黑色 Sprite 上（覆盖整个关卡）
/// </summary>
public class VisionManager : MonoBehaviour
{
    [SerializeField] private SpriteMask   playerMask;       // 挂在 Player 子物体上的 SpriteMask
    [SerializeField] private float        baseVisionRadius = 1.5f;  // 无信号时的极小视野
    [SerializeField] private float        fadeSpeed = 4f;

    private float currentRadius;
    private float targetRadius;

    void Awake()
    {
        currentRadius = baseVisionRadius;
        targetRadius  = baseVisionRadius;
    }

    void Update()
    {
        currentRadius = Mathf.Lerp(currentRadius, targetRadius, fadeSpeed * Time.deltaTime);
        if (playerMask)
            playerMask.transform.localScale = Vector3.one * currentRadius * 2f;

        // 没有新信号时持续收缩
        targetRadius = Mathf.MoveTowards(targetRadius, baseVisionRadius, Time.deltaTime * 2f);
    }

    /// <summary>高频信号发射一个脉冲，扩大视野</summary>
    public void EmitPulse(Vector3 position, float radius)
    {
        targetRadius = radius;
    }

    /// <summary>切到低频时关闭信号，视野立即收缩</summary>
    public void SetSignalActive(bool active)
    {
        if (!active) targetRadius = baseVisionRadius;
    }
}
