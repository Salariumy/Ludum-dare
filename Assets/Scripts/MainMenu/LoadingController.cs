using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingController : MonoBehaviour
{
    [SerializeField] private GameObject LoadingPage;
    [SerializeField] private List<PictureAndText> pictureAndTexts = new List<PictureAndText>();

    private void Start()
    {
        if (LoadingPage != null) LoadingPage.SetActive(true);

        int reached = SaveAPI.GetReachedLevel();
        int nextScene = reached;
        StartCoroutine(LoadLevelAsync(nextScene));
    }

    private IEnumerator LoadLevelAsync(int sceneIndex)
    {
        if (LoadingPage != null) LoadingPage.SetActive(true);

        var loadingCtrl = LoadingPage != null ? LoadingPage.GetComponent<LoadingPageController>() : null;

        // apply picture/text if available
        if (loadingCtrl != null && sceneIndex >= 0 && sceneIndex < pictureAndTexts.Count)
        {
            var pat = pictureAndTexts[sceneIndex];
            loadingCtrl.applyImage(pat.picture);
            loadingCtrl.applyText(pat.text);
        }

        yield return new WaitForSeconds(0.2f);

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex+2);
        operation.allowSceneActivation = false;

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);

            if (loadingCtrl != null)
                loadingCtrl.applyPercent(progress);

            // when loading reached threshold, wait for any key to activate
            if (operation.progress >= 0.9f && Input.anyKeyDown)
                operation.allowSceneActivation = true;

            yield return null;
        }
    }
}

[System.Serializable]
class PictureAndText
{
    public Sprite picture;
    public string text;
}
