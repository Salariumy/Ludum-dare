using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 游戏内开始/选关面板
/// 不依赖场景切换；通过 LevelManager.LoadLevel() 在当前场景内切换SO关卡
/// </summary>
public class MainMenuUI : MonoBehaviour
{
    [Header("面板根节点（点击开始后隐藏）")]
    [SerializeField] private GameObject menuPanel;

    [Header("关卡按钮")]
    [SerializeField] private Button startButton;
    [SerializeField] private Button[] levelButtons;   // 5 个关卡按钮

    [Header("教程/引导页面（点击开始后先展示教学）")]
    [SerializeField] private GameObject guidePanel;
    [SerializeField] private Button guideCloseButton;

    [Header("设置")]
    [SerializeField] private Button settingsButton;

    void Start()
    {
        // 进入游戏时暂停，等待玩家从菜单选择
        if (menuPanel) menuPanel.SetActive(true);
        if (guidePanel) guidePanel.SetActive(false);
        Time.timeScale = 0f;

        startButton.onClick.AddListener(() => StartGame(0));

        for (int i = 0; i < levelButtons.Length; i++)
        {
            int idx = i;
            levelButtons[i].onClick.AddListener(() => StartGame(idx));
        }

        if (settingsButton)
            settingsButton.onClick.AddListener(() =>
            {
                if (SettingsUI.Instance) SettingsUI.Instance.Show(false);
            });
    }

    void StartGame(int levelIndex)
    {
        if (menuPanel) menuPanel.SetActive(false);

        // 选择第一关（或直接点 Start）时，如果有教学面板，则先引导玩家
        if (levelIndex == 0 && guidePanel != null)
        {
            guidePanel.SetActive(true);
            if (guideCloseButton)
            {
                guideCloseButton.onClick.RemoveAllListeners();
                guideCloseButton.onClick.AddListener(() => 
                {
                    guidePanel.SetActive(false);
                    // 关闭教学面版后恢复时间，进入 IntroPanel（因为 LoadLevel 会自动暂停拉起面版）
                    Time.timeScale = 1f;
                    if (LevelManager.Instance) LevelManager.Instance.LoadLevel(levelIndex);
                });
            }
        }
        else
        {
            // 无教学面板，或跳关直接开始该关卡
            Time.timeScale = 1f;
            if (LevelManager.Instance) LevelManager.Instance.LoadLevel(levelIndex);
        }
    }
}
