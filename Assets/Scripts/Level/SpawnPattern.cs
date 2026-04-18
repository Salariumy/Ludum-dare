using UnityEngine;

public enum SpawnType { Obstacle, CoinCluster, Chest, Crown }

[System.Serializable]
public class SpawnEntry
{
    public SpawnType type;

    [Tooltip("被雾覆盖的概率 (0=无雾, 1=必定有雾)")]
    [Range(0f, 1f)] public float fogChance;

    [Header("金币簇参数（仅 CoinCluster 类型生效）")]
    public int   coinCount  = 5;
    public float arcHeight  = 2f;
}

/// <summary>
/// 生成序列配置 —— 定义关卡中物体出现的顺序与概率
/// 在 Inspector 中逐条设计出现顺序，TerrainChunker 按序读取
/// </summary>
[CreateAssetMenu(fileName = "SpawnPattern", menuName = "Game/Spawn Pattern")]
public class SpawnPattern : ScriptableObject
{
    [Tooltip("到末尾后是否从头循环")]
    public bool loop = true;

    [Tooltip("非金币物体之间的最小间距")]
    public float minSpacing = 11f;

    [Tooltip("非金币物体之间的最大间距")]
    public float maxSpacing = 20f;

    [Tooltip("金币簇内金币间距")]
    public float coinSpacing = 1.5f;

    [Tooltip("生成序列（按顺序出现）")]
    public SpawnEntry[] entries;
}
