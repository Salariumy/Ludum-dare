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
    private void Awake()
    {
        text = GetComponent<TMP_Text>();
        //SendMessage("ApplyText", "Like most people, I was born in a hospital..."); 
    }
    // Existing direct call
    public void ApplyText(string text)
    {
        StartTyping(text);
    }

    // Allow Unity SendMessage / BroadcastMessage to trigger typing with an object payload
    // e.g. SendMessage("ApplyText", "hello world");
    public void ApplyText(object message)
    {
        if (message == null) return;
        string s = message as string;
        if (s == null)
        {
            // try ToString fallback
            s = message.ToString();
        }
        StartTyping(s);
    }

    private void StartTyping(string s)
    {
        isTyping = true;
        fullText = s ?? "";
        typingTimer = 0f;
    }
    private void Update()
    {
        if (isTyping)
        {
            typingTimer += Time.deltaTime;
            if(typingTimer * typingSpeedFerSec >= fullText.Length)
            {
                isTyping = false;
                text.SetText(fullText);
                return;
            }
            text.SetText(fullText.Substring(0, Mathf.FloorToInt(typingTimer * typingSpeedFerSec)));
        }
    }
}
