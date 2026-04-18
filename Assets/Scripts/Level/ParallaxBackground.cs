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

    [Header("衔接画面物体（场景中一个带 SpriteRenderer 的物体，默认隐藏）")]
    [SerializeField] private Transform transitionPanel;

    private Camera cam;
    private float lastCamX;
    private SpriteRenderer[] panelRenderers;
    private SpriteRenderer transitionSR;
    private float currentPanelWidth;       // 当前生效的背景宽度
    private bool transitionActive;         // 衔接画面是否正在显示
    private float transitionWidth;         // 衔接画面宽度

    void Awake()
    {
        panelRenderers = new SpriteRenderer[bgPanels.Length];
        for (int i = 0; i < bgPanels.Length; i++)
            panelRenderers[i] = bgPanels[i].GetComponent<SpriteRenderer>();

        if (transitionPanel)
        {
            transitionSR = transitionPanel.GetComponent<SpriteRenderer>();
            transitionPanel.gameObject.SetActive(false);
        }
    }

    void Start()
    {
        cam = Camera.main;
        lastCamX = cam.transform.position.x;
        currentPanelWidth = panelWidth;

        // 主动从 LevelManager 获取初始背景，防止错过事件
        if (LevelManager.Instance != null)
        {
            Sprite bg = LevelManager.Instance.GetCurrentBackgroundSprite();
            if (bg != null) SetAllPanelSprites(bg);
        }
    }

    void OnEnable()
    {
        EventBus.Subscribe(GameEvents.SceneSegmentChanged, OnSegmentChanged);
        EventBus.Subscribe(GameEvents.TransitionStart, OnTransition);
    }

    void OnDisable()
    {
        EventBus.Unsubscribe(GameEvents.SceneSegmentChanged, OnSegmentChanged);
        EventBus.Unsubscribe(GameEvents.TransitionStart, OnTransition);
    }

    void OnSegmentChanged(object data)
    {
        if (data is SceneSegment segment && segment.backgroundSprite != null)
        {
            SetAllPanelSprites(segment.backgroundSprite);
            currentPanelWidth = panelWidth;

            // 衔接结束，隐藏衔接物体
            if (transitionActive && transitionPanel)
            {
                transitionPanel.gameObject.SetActive(false);
                transitionActive = false;
            }
        }
    }

    void OnTransition(object data)
    {
        // 衔接画面只出现一次：放在最右面板右侧
        if (data is TransitionInfo info && info.sprite != null && transitionPanel)
        {
            transitionWidth = info.width;
            transitionSR.sprite = info.sprite;
            transitionPanel.gameObject.SetActive(true);
            transitionActive = true;

            // 定位到最右面板右侧
            float maxX = float.MinValue;
            foreach (var p in bgPanels)
            {
                if (p.position.x > maxX) maxX = p.position.x;
            }
            float transX = maxX + currentPanelWidth * 0.5f + transitionWidth * 0.5f;
            transitionPanel.position = new Vector3(transX, transitionPanel.position.y, transitionPanel.position.z);
        }
    }

    void SetAllPanelSprites(Sprite sprite)
    {
        if (panelRenderers == null) return;
        foreach (var sr in panelRenderers)
        {
            if (sr != null) sr.sprite = sprite;
        }
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
            if (leftmost.position.x + currentPanelWidth * 0.5f < camLeft)
            {
                // 如果衔接画面正在显示，接在衔接画面右侧
                float rightEdge;
                if (transitionActive && transitionPanel && transitionPanel.position.x > maxX)
                {
                    rightEdge = transitionPanel.position.x + transitionWidth * 0.5f + currentPanelWidth * 0.5f;
                }
                else
                {
                    rightEdge = rightmost.position.x + currentPanelWidth;
                }
                leftmost.position = new Vector3(rightEdge, leftmost.position.y, leftmost.position.z);
            }
        }

        // 衔接画面也随视差移动
        if (transitionActive && transitionPanel)
        {
            transitionPanel.position += Vector3.right * deltaX * parallaxFactor;

            // 衔接画面离开屏幕后自动隐藏
            float transLeft = transitionPanel.position.x + transitionWidth * 0.5f;
            if (transLeft < camLeft)
            {
                transitionPanel.gameObject.SetActive(false);
                transitionActive = false;
            }
        }
    }
}
