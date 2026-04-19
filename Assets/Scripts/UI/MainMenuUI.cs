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

    [SerializeField] private Button startButton;
    [SerializeField] private Button[] levelButtons;   // 5 个关卡按钮

    void Start()
    {
        // 进入游戏时暂停，等待玩家从菜单选择
        if (menuPanel) menuPanel.SetActive(true);
        Time.timeScale = 0f;

        startButton.onClick.AddListener(() => StartGame(0));

        for (int i = 0; i < levelButtons.Length; i++)
        {
            int idx = i;
            levelButtons[i].onClick.AddListener(() => StartGame(idx));
        }
    }

    void StartGame(int levelIndex)
    {
        if (menuPanel) menuPanel.SetActive(false);
        Time.timeScale = 1f;
        if (LevelManager.Instance) LevelManager.Instance.LoadLevel(levelIndex);
    }
}
