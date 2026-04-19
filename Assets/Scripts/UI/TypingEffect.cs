using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TypingEffect : MonoBehaviour
{
    TMP_Text text;
    bool isTyping = false;
    [SerializeField] float typingSpeedFerSec = 10f;
    float typingTimer = 0f;
    string fullText = "";
    
    [SerializeField] private bool playTypingSFX = true;

    private void Awake()
    {
        text = GetComponent<TMP_Text>();
    }

    public void ApplyText(string text)
    {
        StartTyping(text);
    }

    public void ApplyText(object message)
    {
        if (message == null) return;
        string s = message as string;
        if (s == null)
            s = message.ToString();
        StartTyping(s);
    }

    /// <summary>清空当前显示的文字</summary>
    public void Clear()
    {
        isTyping = false;
        fullText = "";
        typingTimer = 0f;
        if (text) text.SetText("");
        
        // 停止打字音效
        if (playTypingSFX && AudioManager.Instance)
            AudioManager.Instance.StopTyping();
    }

    /// <summary>当前是否正在打字</summary>
    public bool IsTyping => isTyping;

    private void StartTyping(string s)
    {
        isTyping = true;
        fullText = s ?? "";
        typingTimer = 0f;
        if (text) text.SetText("");
        
        // 打字开始时播放音效一次，持续到打字完成
        if (playTypingSFX && AudioManager.Instance)
            AudioManager.Instance.PlayTypingStart();
    }

    private void Update()
    {
        if (isTyping)
        {
            typingTimer += Time.deltaTime;
            int charCount = Mathf.FloorToInt(typingTimer * typingSpeedFerSec);

            if (charCount >= fullText.Length)
            {
                isTyping = false;
                text.SetText(fullText);
                
                // 打字完成时停止音效
                if (playTypingSFX && AudioManager.Instance)
                    AudioManager.Instance.StopTyping();
                
                return;
            }

            text.SetText(fullText.Substring(0, charCount));
        }
    }
}
