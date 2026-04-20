using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.UI;

public class TutorUI : MonoBehaviour
{
    float timer = 0f;
    bool time=false;
    [SerializeField] Image UIObject1;
    [SerializeField] Image UIObject2;
    [SerializeField] Image UIObject3;


    [SerializeField] List<KeyWithTime> keyWithTimes;
    int index = 0;
    private void Start()
    {
        StartTimer();
    }
    void Update()
    {
        if (time)
        {
            
            if(timer>= keyWithTimes.ElementAt(index).time)
            {
                WaitForClick(keyWithTimes.ElementAt(index).key);
                index++;
            }
            timer += Time.deltaTime;

        }
    }
    void StartTimer()
    {
        time = true;
    }
    void WaitForClick(KeyCode   key)
    {
        Time.timeScale = 0f;
        time = false;
        StartCoroutine(WaitClick(key));
    }
    IEnumerator WaitClick(KeyCode key)
    {
        StartCoroutine(alternatingExist());
        
        UIObject1.sprite = keyWithTimes.ElementAt(index).UI1;
        UIObject2.sprite = keyWithTimes.ElementAt(index).UI2;
        UIObject3.sprite = keyWithTimes.ElementAt(index).UI3;
        Debug.Log("WaitFor" + key);
        while (!Input.GetKeyDown(key))
        {
            yield return null;
        }
        UIObject1.gameObject.SetActive(false);
        UIObject2.gameObject.SetActive(false);
        UIObject3.gameObject.SetActive(false);
        Time.timeScale = 1f;
        time = true;
        if (index >= keyWithTimes.Count)
        {
            Destroy(gameObject);
            Time.timeScale = 1f;
        }
        StopAllCoroutines();
    }
    IEnumerator alternatingExist()
    {
        while (true)
        {
            UIObject3.gameObject.SetActive(false);
            UIObject1.gameObject.SetActive(true);
            yield return new WaitForSecondsRealtime(0.1f);
            UIObject1.gameObject.SetActive(false);
            UIObject2.gameObject.SetActive(true);
            yield return new WaitForSecondsRealtime(0.1f);
            UIObject2.gameObject.SetActive(false);
            UIObject3.gameObject.SetActive(true);
            yield return new WaitForSecondsRealtime(0.1f);
        }
    }
}
[System.Serializable]public class KeyWithTime
{
    public KeyCode key;
    public float time;
    public Sprite UI1;
    public Sprite UI2;
    public Sprite UI3;
}