using UnityEngine;

/// <summary>
/// 背景无限滚动：多张背景图拼接循环
/// 挂在一个空父物体上，子物体放 2~3 张相同宽度的背景 SpriteRenderer
/// </summary>
public class ParallaxBackground : MonoBehaviour
{
    [Header("背景块（按从左到右顺序放入）")]
    [SerializeField] private Transform[] bgPanels;

    [Header("单张背景宽度（和 Sprite 实际世界宽度一致）")]
    [SerializeField] private float panelWidth = 20f;

    [Header("视差系数（0=不动，1=和摄像机同速）")]
    [SerializeField] private float parallaxFactor = 0.5f;

    private Camera cam;
    private float lastCamX;

    void Start()
    {
        cam = Camera.main;
        lastCamX = cam.transform.position.x;
    }

    void LateUpdate()
    {
        float camX = cam.transform.position.x;
        float deltaX = camX - lastCamX;
        lastCamX = camX;

        // 按视差系数移动背景
        for (int i = 0; i < bgPanels.Length; i++)
        {
            bgPanels[i].position += Vector3.right * deltaX * parallaxFactor;
        }

        // 找到最左和最右的面板
        float camLeft = camX - cam.orthographicSize * cam.aspect - panelWidth;

        Transform leftmost = null;
        Transform rightmost = null;
        float minX = float.MaxValue;
        float maxX = float.MinValue;

        for (int i = 0; i < bgPanels.Length; i++)
        {
            float x = bgPanels[i].position.x;
            if (x < minX) { minX = x; leftmost = bgPanels[i]; }
            if (x > maxX) { maxX = x; rightmost = bgPanels[i]; }
        }

        // 最左面板超出屏幕左边时，移到最右面板右边
        if (leftmost != null && rightmost != null)
        {
            if (leftmost.position.x + panelWidth * 0.5f < camLeft)
            {
                float newX = rightmost.position.x + panelWidth;
                leftmost.position = new Vector3(newX, leftmost.position.y, leftmost.position.z);
            }
        }
    }
}
