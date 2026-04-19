using UnityEngine;


/// <summary>
/// 一个关卡内的单个场景段配置
/// </summary>
[System.Serializable]
public class SceneSegment
{
    [Tooltip("该场景段的背景 Sprite")]
    public Sprite backgroundSprite;

    [Tooltip("可摧毁障碍物的 Sprite 变体")]
    public Sprite[] destructibleObstacleSprites;

    [Tooltip("不可摧毁障碍物的 Sprite 变体")]
    public Sprite[] indestructibleObstacleSprites;

    [Tooltip("该场景段地形块循环次数（之后切换到下一段或下一关）")]
    public int cycleCount = 10;
}

/// <summary>
/// 单个关卡的完整配置（ScriptableObject）
/// 只包含 2 个场景段，无过渡画面
/// </summary>
[CreateAssetMenu(fileName = "Level_01", menuName = "Game/Level Data")]
public class LevelData : ScriptableObject
{
    [Tooltip("关卡编号（1~5）")]
    public int levelIndex;

    [Tooltip("场景段 A（先播放）")]
    public SceneSegment segmentA;

    [Tooltip("场景段 B（衔接后播放）")]
    public SceneSegment segmentB;

    [Tooltip("该关卡的生成序列（控制障碍物/金币/宝箱/皇冠出现顺序）")]
    public SpawnPattern spawnPattern;
}
