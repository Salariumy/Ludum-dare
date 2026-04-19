using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private GameObject coverPage;
    [SerializeField] private GameObject tutorialPage;
    [SerializeField] private int sceneIndex = 1; // LoadingPage scene index

    private void Start()
    {
        if (coverPage != null) coverPage.SetActive(true);
        if (tutorialPage != null) tutorialPage.SetActive(false);
        StartCoroutine(WaitForPress());
        //SaveAPI.ClearAll(); // clear saved data for testing purposes, remove this line in production
    }

    private IEnumerator WaitForPress()
    {
        // wait for key on cover page
        while (!Input.anyKeyDown)
            yield return null;

        if (coverPage != null) coverPage.SetActive(false);
        if (tutorialPage != null) tutorialPage.SetActive(true);

        // wait for key on tutorial page
        while (!Input.anyKeyDown)
            yield return null;

        // start loading the LoadingPage scene (sceneIndex)
        yield return StartCoroutine(LoadLevelAsync(sceneIndex));
    }

    private IEnumerator LoadLevelAsync(int sceneIndex)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);
        // let Unity activate the scene when loading finishes
        operation.allowSceneActivation = true;

        while (!operation.isDone)
        {
            // optionally log progress
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            Debug.Log("Loading scene " + sceneIndex + " progress: " + (progress * 100f) + "%");
            yield return null;
        }
    }
}
