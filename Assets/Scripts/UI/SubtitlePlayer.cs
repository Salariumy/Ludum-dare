using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// 字幕播放控制器
/// 挂在含有 TypingEffect + TMP_Text 的 UI 物体上
/// 在 Inspector 中配置多句台词和每句之间的间隔
/// </summary>
public class SubtitlePlayer : MonoBehaviour
{
    [Serializable]
    public class SubtitleLine
    {
        [TextArea(1, 3)]
        [Tooltip("这句台词的内容")]
        public string text;

        [Tooltip("这句话打完后，等待多久再播放下一句（秒）")]
        public float delayAfter = 2f;
    }

    [Header("字幕台词序列")]
    [SerializeField] private SubtitleLine[] lines;

    [Header("打字完成后额外的等待时间（全部播完后）")]
    [SerializeField] private float endDelay = 1f;

    [Header("播完所有台词后是否自动隐藏")]
    [SerializeField] private bool hideOnComplete = true;

    [Header("是否在启用时自动开始播放")]
    [SerializeField] private bool autoPlay = true;

    private TypingEffect typingEffect;
    private Coroutine playRoutine;

    void Awake()
    {
        typingEffect = GetComponent<TypingEffect>();
    }

    void OnEnable()
    {
        if (autoPlay)
            Play();
    }

    /// <summary>从头开始播放字幕序列</summary>
    public void Play()
    {
        if (playRoutine != null)
            StopCoroutine(playRoutine);
        gameObject.SetActive(true);
        playRoutine = StartCoroutine(PlaySequence());
    }

    /// <summary>使用外部传入的台词数组播放</summary>
    public void Play(SubtitleLine[] customLines)
    {
        lines = customLines;
        Play();
    }

    /// <summary>立即停止并清空</summary>
    public void Stop()
    {
        if (playRoutine != null)
        {
            StopCoroutine(playRoutine);
            playRoutine = null;
        }
        if (typingEffect) typingEffect.Clear();
        if (hideOnComplete) gameObject.SetActive(false);
    }

    private IEnumerator PlaySequence()
    {
        if (typingEffect == null || lines == null || lines.Length == 0)
            yield break;

        for (int i = 0; i < lines.Length; i++)
        {
            var line = lines[i];
            if (string.IsNullOrEmpty(line.text)) continue;

            // 开始打字
            typingEffect.ApplyText(line.text);

            // 等待打字完成
            while (typingEffect.IsTyping)
                yield return null;

            // 等待这句话配置的间隔
            yield return new WaitForSeconds(line.delayAfter);

            // 清空文字，准备下一句
            if (i < lines.Length - 1)
                typingEffect.Clear();
        }

        // 全部播完后的额外等待
        yield return new WaitForSeconds(endDelay);

        playRoutine = null;

        if (hideOnComplete)
            gameObject.SetActive(false);
    }
}
