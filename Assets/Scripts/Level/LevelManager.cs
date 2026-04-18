using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 关卡流程控制器
/// 管理 5 个关卡的推进，驱动 TerrainChunker 和 ParallaxBackground 切换场景段
/// </summary>
public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [Header("关卡配置（按顺序放入 5 个 LevelData SO）")]
    [SerializeField] private LevelData[] levels;

    [Header("衔接画面对象（场景中一个带 SpriteRenderer 的物体，默认隐藏）")]
    [SerializeField] private SpriteRenderer transitionRenderer;

    [Header("不可摧毁障碍物概率")]
    [SerializeField, Range(0f, 1f)] private float indestructibleChance = 0.25f;

    /// <summary>当前关卡索引 (0~4)</summary>
    public int CurrentLevelIndex { get; private set; }

    /// <summary>当前场景段 (A=0, Transition=1, B=2)</summary>
    public int CurrentSegmentPhase { get; private set; }

    /// <summary>当前场景段已循环的次数</summary>
    public int CurrentCycleCount { get; private set; }

    /// <summary>当前生效的场景段数据</summary>
    public SceneSegment CurrentSegment { get; private set; }

    /// <summary>当前关卡数据</summary>
    public LevelData CurrentLevel => (levels != null && CurrentLevelIndex < levels.Length)
        ? levels[CurrentLevelIndex] : null;

    private int targetCycleCount;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        CurrentLevelIndex = 0;
        StartLevel();
    }

    void StartLevel()
    {
        if (CurrentLevel == null) return;

        CurrentSegmentPhase = 0;
        CurrentCycleCount = 0;
        CurrentSegment = CurrentLevel.segmentA;
        targetCycleCount = CurrentSegment.cycleCount;

        if (transitionRenderer) transitionRenderer.enabled = false;

        EventBus.Publish(GameEvents.SceneSegmentChanged, CurrentSegment);
    }

    /// <summary>
    /// 由 TerrainChunker 每次回收地形块时调用，推进循环计数
    /// </summary>
    public void OnChunkRecycled()
    {
        if (CurrentLevel == null) return;

        CurrentCycleCount++;

        if (CurrentCycleCount >= targetCycleCount)
        {
            AdvancePhase();
        }
    }

    void AdvancePhase()
    {
        if (CurrentSegmentPhase == 0)
        {
            // A 段结束 → 显示衔接画面
            CurrentSegmentPhase = 1;

            // 通知衔接（ParallaxBackground 会在最右面板右侧放一个衔接画面）
            EventBus.Publish(GameEvents.TransitionStart,
                new TransitionInfo(CurrentLevel.transitionSprite, CurrentLevel.transitionWidth));

            // 衔接只占 1 个 chunk，之后自动进入 B 段
            CurrentSegmentPhase = 2;
            CurrentCycleCount = 0;
            CurrentSegment = CurrentLevel.segmentB;
            targetCycleCount = CurrentSegment.cycleCount;

            EventBus.Publish(GameEvents.SceneSegmentChanged, CurrentSegment);
        }
        else if (CurrentSegmentPhase == 2)
        {
            // B 段结束 → 关卡完成
            CurrentLevelIndex++;
            if (CurrentLevelIndex < levels.Length)
            {
                EventBus.Publish(GameEvents.LevelCompleted, CurrentLevelIndex);
                StartLevel();
            }
            else
            {
                // 全部通关
                EventBus.Publish(GameEvents.GameCompleted, null);
            }
        }
    }

    /// <summary>获取当前场景段的障碍物 Sprite（随机选可摧毁或不可摧毁）</summary>
    public void GetObstacleConfig(out Sprite sprite, out bool indestructible)
    {
        indestructible = false;
        sprite = null;

        if (CurrentSegment == null) return;

        bool hasDestructible = CurrentSegment.destructibleObstacleSprites != null
                            && CurrentSegment.destructibleObstacleSprites.Length > 0;
        bool hasIndestructible = CurrentSegment.indestructibleObstacleSprites != null
                              && CurrentSegment.indestructibleObstacleSprites.Length > 0;

        if (hasIndestructible && Random.value < indestructibleChance)
        {
            indestructible = true;
            sprite = CurrentSegment.indestructibleObstacleSprites[
                Random.Range(0, CurrentSegment.indestructibleObstacleSprites.Length)];
        }
        else if (hasDestructible)
        {
            sprite = CurrentSegment.destructibleObstacleSprites[
                Random.Range(0, CurrentSegment.destructibleObstacleSprites.Length)];
        }
    }

    /// <summary>获取当前背景 Sprite</summary>
    public Sprite GetCurrentBackgroundSprite()
    {
        return CurrentSegment?.backgroundSprite;
    }

    /// <summary>是否还有下一关</summary>
    public bool HasNextLevel => levels != null && CurrentLevelIndex < levels.Length - 1;

    /// <summary>重新开始当前关卡</summary>
    public void ReloadLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    /// <summary>加载下一关</summary>
    public void LoadNextLevel()
    {
        if (!HasNextLevel) return;
        CurrentLevelIndex++;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    /// <summary>加载指定关卡</summary>
    public void LoadLevel(int index)
    {
        if (levels == null || index < 0 || index >= levels.Length) return;
        CurrentLevelIndex = index;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
