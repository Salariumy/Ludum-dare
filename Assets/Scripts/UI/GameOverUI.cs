using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 游戏结束面板（死亡/通关）
/// </summary>
public class GameOverUI : MonoBehaviour
{
    [Header("胜利面板")]
    [SerializeField] private GameObject      victoryPanel;
    [SerializeField] private TextMeshProUGUI victoryStatsText;
    [SerializeField] private Button          victoryNextButton;
    [SerializeField] private Button          victoryRetryButton;

    [Header("失败面板")]
    [SerializeField] private GameObject      defeatPanel;
    [SerializeField] private TextMeshProUGUI defeatStatsText;
    [SerializeField] private Button          defeatRetryButton;

    [Header("庆祝动画UI")]
    [SerializeField] private Animator celebrationAnimator;  // UI Image 上的 Animator，循环播放

    [Header("延迟设置")]
    [SerializeField] private float delayBeforeShow = 1f;

    void OnEnable()
    {
        EventBus.Subscribe(GameEvents.PlayerDied,     OnPlayerDied);
        EventBus.Subscribe(GameEvents.LevelCompleted, OnLevelCompleted);
    }

    void OnDisable()
    {
        EventBus.Unsubscribe(GameEvents.PlayerDied,     OnPlayerDied);
        EventBus.Unsubscribe(GameEvents.LevelCompleted, OnLevelCompleted);
    }

    void Start()
    {
        if (victoryPanel) victoryPanel.SetActive(false);
        if (defeatPanel) defeatPanel.SetActive(false);

        if (victoryNextButton) victoryNextButton.onClick.AddListener(OnNextLevel);
        if (victoryRetryButton) victoryRetryButton.onClick.AddListener(OnRetry);
        if (defeatRetryButton) defeatRetryButton.onClick.AddListener(OnRetry);
    }

    void OnPlayerDied(object data)
    {
        StartCoroutine(ShowPanelDelayed(false));
    }

    void OnLevelCompleted(object data)
    {
        StartCoroutine(ShowPanelDelayed(true));
    }

    System.Collections.IEnumerator ShowPanelDelayed(bool isVictory)
    {
        // 挂起一部分玩家控制，这里等待死亡或开书动画播完
        var player = FindObjectOfType<PlayerController>();
        if (player)
        {
            player.enabled = false; // 禁用玩家位移和输入
            var rb = player.GetComponent<Rigidbody2D>();
            if (rb) rb.velocity = new Vector2(0, rb.velocity.y); // 停下水平移动，允许落地
        }
        
        yield return new WaitForSeconds(delayBeforeShow);

        if (isVictory)
        {
            if (victoryPanel) victoryPanel.SetActive(true);
            // 在暂停游戏前启动庆祝动画
            if (celebrationAnimator)
            {
                celebrationAnimator.gameObject.SetActive(true);
                // 直接激活，让动画自动从默认状态播放（需确保 Animator Controller 的 Entry 连到循环动画）
                // 不使用 SetTrigger，避免参数不存在的问题
            }
            if (player && victoryStatsText)
                victoryStatsText.text = $"{player.CoinCount}";
            if (victoryNextButton)
                victoryNextButton.gameObject.SetActive(LevelManager.Instance != null && LevelManager.Instance.HasNextLevel);
        }
        else
        {
            if (defeatPanel) defeatPanel.SetActive(true);
            if (player && defeatStatsText)
                defeatStatsText.text = $"{player.CoinCount}";
        }

        // 最后再暂停游戏，此时动画已启动
        Time.timeScale = 0f;
    }

    void OnRetry()
    {
        if (victoryPanel) victoryPanel.SetActive(false);
        if (defeatPanel) defeatPanel.SetActive(false);
        LevelManager.Instance.ReloadLevel();
    }

    void OnNextLevel()
    {
        if (victoryPanel) victoryPanel.SetActive(false);
        LevelManager.Instance.LoadNextLevel();
    }
}
