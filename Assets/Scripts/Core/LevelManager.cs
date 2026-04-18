using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 关卡管理器：5 个关卡的加载、重载、切换
/// 场景命名：Level_01 ~ Level_05
/// </summary>
public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [SerializeField] private string[] levelSceneNames = {
        "Level_01", "Level_02", "Level_03", "Level_04", "Level_05"
    };

    private int currentLevelIndex;

    public bool HasNextLevel => currentLevelIndex < levelSceneNames.Length - 1;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // 根据当前场景名判断是第几关
        string sceneName = SceneManager.GetActiveScene().name;
        for (int i = 0; i < levelSceneNames.Length; i++)
        {
            if (levelSceneNames[i] == sceneName)
            {
                currentLevelIndex = i;
                break;
            }
        }
    }

    public void ReloadLevel()
    {
        SceneManager.LoadScene(levelSceneNames[currentLevelIndex]);
    }

    public void LoadNextLevel()
    {
        if (!HasNextLevel) return;
        currentLevelIndex++;
        SceneManager.LoadScene(levelSceneNames[currentLevelIndex]);
    }

    public void LoadLevel(int index)
    {
        if (index < 0 || index >= levelSceneNames.Length) return;
        currentLevelIndex = index;
        SceneManager.LoadScene(levelSceneNames[currentLevelIndex]);
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
