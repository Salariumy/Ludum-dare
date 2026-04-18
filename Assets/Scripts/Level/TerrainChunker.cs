using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 地形块循环复用器
/// 挂在任意空 GameObject 上，把 2~3 个地形块 Grid 拖进 chunks 列表
/// 当最左块完全离开屏幕后，自动移到最右块的右边，实现无限地形
/// </summary>
public class TerrainChunker : MonoBehaviour
{
    [Header("地形块列表（2~3 个 Grid GameObject）")]
    [SerializeField] private List<Transform> chunks;

    [Header("单块宽度（单位：Unity 米）")]
    [Tooltip("= 你画的 Tilemap 格子列数 × 每格大小（默认每格 1 米，画了 32 列就填 32）")]
    [SerializeField] private float chunkWidth = 32f;

    [Header("提前回收的缓冲距离")]
    [SerializeField] private float screenBuffer = 2f;

    private Camera cam;

    void Start()
    {
        cam = Camera.main;
    }

    void LateUpdate()
    {
        if (chunks == null || chunks.Count < 2) return;

        // 摄像机左边缘（再减 buffer，块完全看不见才回收）
        float camLeft = cam.transform.position.x
                        - cam.orthographicSize * cam.aspect
                        - screenBuffer;

        Transform leftmost  = null;
        Transform rightmost = null;
        float minX =  float.MaxValue;
        float maxX = -float.MaxValue;

        foreach (var c in chunks)
        {
            if (c == null) continue;
            float x = c.position.x;
            if (x < minX) { minX = x; leftmost  = c; }
            if (x > maxX) { maxX = x; rightmost = c; }
        }

        if (leftmost == null || rightmost == null) return;

        // 最左块的右边缘已经在屏幕左边缘左边 → 移到最右块右侧
        if (leftmost.position.x + chunkWidth < camLeft)
        {
            leftmost.position = new Vector3(
                rightmost.position.x + chunkWidth,
                leftmost.position.y,
                leftmost.position.z);
        }
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        if (chunks == null) return;
        Gizmos.color = Color.cyan;
        foreach (var c in chunks)
        {
            if (c == null) continue;
            // 画出每块的范围框
            Vector3 center = c.position + new Vector3(chunkWidth * 0.5f, 2f, 0f);
            Gizmos.DrawWireCube(center, new Vector3(chunkWidth, 4f, 0f));
        }
    }
#endif
}
