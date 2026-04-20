using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 设置面板 —— 控制 BGM / SFX 音量
/// 面板上有两个 Slider + 对应的静音/非静音图标叠加层
/// 以及 Close / Save / Reset 三个按钮
/// </summary>
public class SettingsUI : MonoBehaviour
{
    public static SettingsUI Instance { get; private set; }

    [Header("面板根节点")]
    [SerializeField] private GameObject settingsPanel;

    [Header("BGM")]
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Image  bgmIconActive;    // 蓝色轮廓（音量 > 0）
    [SerializeField] private Image  bgmIconMuted;     // 红色斜杠（音量 = 0）

    [Header("SFX")]
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Image  sfxIconActive;    // 蓝色轮廓（音量 > 0）
    [SerializeField] private Image  sfxIconMuted;     // 红色斜杠（音量 = 0）

    [Header("按钮")]
    [SerializeField] private Button closeButton;
    [SerializeField] private Button saveButton;
    [SerializeField] private Button resetButton;

    private bool wasTimePaused;
    private System.Action onCloseCallback;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        if (settingsPanel) settingsPanel.SetActive(false);

        if (bgmSlider)  bgmSlider.onValueChanged.AddListener(OnBgmSliderChanged);
        if (sfxSlider)  sfxSlider.onValueChanged.AddListener(OnSfxSliderChanged);
        if (closeButton) closeButton.onClick.AddListener(OnClose);
        if (saveButton)  saveButton.onClick.AddListener(OnSave);
        if (resetButton) resetButton.onClick.AddListener(OnReset);
    }

    /// <summary>打开设置面板</summary>
    /// <param name="pauseGame">是否在打开时暂停游戏</param>
    /// <param name="onClose">关闭面板后的回调</param>
    public void Show(bool pauseGame = false, System.Action onClose = null)
    {
        onCloseCallback = onClose;
        wasTimePaused = Time.timeScale == 0f;

        if (pauseGame && !wasTimePaused)
            Time.timeScale = 0f;

        // 停止打字机（防止打字音效与UI音效混杂）
        if (AudioManager.Instance) AudioManager.Instance.StopTyping();
        var typingEffect = FindObjectOfType<TypingEffect>(true);
        if (typingEffect) typingEffect.Clear();

        // 同步 Slider 到当前音量
        var am = AudioManager.Instance;
        if (am != null)
        {
            if (bgmSlider) bgmSlider.SetValueWithoutNotify(am.RuntimeBgmVolume);
            if (sfxSlider) sfxSlider.SetValueWithoutNotify(am.RuntimeSfxVolume);
        }

        RefreshIcons();

        if (settingsPanel) settingsPanel.SetActive(true);
    }

    /// <summary>隐藏设置面板</summary>
    public void Hide()
    {
        if (settingsPanel) settingsPanel.SetActive(false);
    }

    // ─── Slider 回调 ───

    private void OnBgmSliderChanged(float val)
    {
        if (AudioManager.Instance) AudioManager.Instance.SetBgmVolume(val);
        RefreshIcons();
    }

    private void OnSfxSliderChanged(float val)
    {
        if (AudioManager.Instance) AudioManager.Instance.SetSfxVolume(val);
        RefreshIcons();
    }

    // ─── 按钮回调 ───

    private void OnClose()
    {
        Hide();

        // 如果是本面板自己暂停的游戏，就恢复
        if (!wasTimePaused)
            Time.timeScale = 1f;

        onCloseCallback?.Invoke();
        onCloseCallback = null;
    }

    private void OnSave()
    {
        if (AudioManager.Instance) AudioManager.Instance.SaveVolumeSettings();
    }

    private void OnReset()
    {
        var am = AudioManager.Instance;
        if (am == null) return;

        am.ResetVolumeToDefault();

        if (bgmSlider) bgmSlider.SetValueWithoutNotify(am.RuntimeBgmVolume);
        if (sfxSlider) sfxSlider.SetValueWithoutNotify(am.RuntimeSfxVolume);

        RefreshIcons();
    }

    // ─── 图标刷新 ───

    private void RefreshIcons()
    {
        float bgm = bgmSlider ? bgmSlider.value : 1f;
        float sfx = sfxSlider ? sfxSlider.value : 1f;

        if (bgmIconActive) bgmIconActive.enabled = bgm > 0f;
        if (bgmIconMuted)  bgmIconMuted.enabled  = bgm <= 0f;
        if (sfxIconActive) sfxIconActive.enabled  = sfx > 0f;
        if (sfxIconMuted)  sfxIconMuted.enabled   = sfx <= 0f;
    }
}
