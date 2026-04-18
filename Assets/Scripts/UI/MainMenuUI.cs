using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

/// <summary>
/// 主菜单：选关 + 开始游戏
/// </summary>
public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private Button[] levelButtons;   // 5 个关卡按钮

    void Start()
    {
        startButton.onClick.AddListener(() =>
        {
            if (LevelManager.Instance) LevelManager.Instance.LoadLevel(0);
            else SceneManager.LoadScene("Level_01");
        });

        for (int i = 0; i < levelButtons.Length; i++)
        {
            int idx = i;
            levelButtons[i].onClick.AddListener(() =>
            {
                if (LevelManager.Instance) LevelManager.Instance.LoadLevel(idx);
                else SceneManager.LoadScene("Level_0" + (idx + 1));
            });
        }
    }
}
