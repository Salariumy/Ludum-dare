using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 关卡前序介绍面板
/// 每关开始前展示：动画图片、介绍文字、章节编号，点击 Next 后加载关卡
/// </summary>
public class LevelIntroUI : MonoBehaviour
{
    public static LevelIntroUI Instance { get; private set; }

    [Header("面板")]
    [SerializeField] private GameObject introPanel;

    [Header("UI 元素")]
    [SerializeField] private Image     introImage;
    [SerializeField] private Animator  introImageAnimator;
    [SerializeField] private TextMeshProUGUI introTextField;
    [SerializeField] private TextMeshProUGUI chapterLabel;
    [SerializeField] private Button    nextButton;

    private System.Action onNextCallback;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        if (introPanel) introPanel.SetActive(false);
        if (nextButton) nextButton.onClick.AddListener(OnNextClicked);
    }

    /// <summary>
    /// 显示指定关卡的介绍面板
    /// </summary>
    /// <param name="levelData">关卡数据</param>
    /// <param name="onNext">点击 Next 后的回调</param>
    public void Show(LevelData levelData, System.Action onNext)
    {
        if (levelData == null) return;

        onNextCallback = onNext;

        // 暂停游戏
        Time.timeScale = 0f;

        // 章节编号
        if (chapterLabel)
            chapterLabel.text = $"NO.{levelData.levelIndex}";

        // 介绍文字
        if (introTextField)
            introTextField.text = levelData.introText ?? "";

        // 图片 / 动画
        if (introImage && levelData.introImage)
        {
            introImage.sprite = levelData.introImage;
            introImage.raycastTarget = false;   // 防止图片拦截按钮点击
        }

        if (introImageAnimator)
        {
            if (levelData.introAnimator != null)
            {
                introImageAnimator.runtimeAnimatorController = levelData.introAnimator;
                introImageAnimator.updateMode = AnimatorUpdateMode.UnscaledTime; // timeScale=0 时仍能播放
                introImageAnimator.enabled = true;
                introImageAnimator.Update(0f); // 强制刷新第一帧
            }
            else
            {
                introImageAnimator.enabled = false;
            }
        }

        if (introPanel) introPanel.SetActive(true);
    }

    /// <summary>隐藏面板</summary>
    public void Hide()
    {
        if (introPanel) introPanel.SetActive(false);
    }

    private void OnNextClicked()
    {
        Hide();
        Time.timeScale = 1f;
        onNextCallback?.Invoke();
    }
}
