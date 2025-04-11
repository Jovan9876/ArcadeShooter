using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{

    public GameObject pauseMenu;

    private bool hasLoaded = false;

    public string cutscene = "TrevorCutscene";

    private void Start()
    {
        Application.targetFrameRate = 60;
        Time.timeScale = 1f;

        if (pauseMenu != null)
        {
            pauseMenu.SetActive(false);
        }
    }

    public void LoadSceneBasedOnFirstPress(string gameScene)
    {
        if (!hasLoaded && gameScene == "GameScene")
        {
            hasLoaded = true;
            StartCoroutine(LoadSceneAsync(cutscene));
        }
        else
        {
            StartCoroutine(LoadSceneAsync(gameScene));
        }
    }

    private IEnumerator LoadSceneAsync(string newScene)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(newScene, LoadSceneMode.Single);

        if (asyncLoad == null)
        {
            Debug.LogError("Scene " + newScene + " not found.");
            yield break;
        }

        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

    public void ShowPauseMenu()
    {
        if (pauseMenu.activeSelf)
        {
            Time.timeScale = 1f;
            pauseMenu.SetActive(false);
        }
        else
        {
            Time.timeScale = 0f;
            pauseMenu.SetActive(true);
        }
    }
}
