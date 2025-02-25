using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour {

    public void LoadScene(string newScene) {
        StartCoroutine(LoadSceneAsync(newScene));
    }

    private IEnumerator LoadSceneAsync(string newScene) {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(newScene, LoadSceneMode.Single);

        if (asyncLoad == null) {
            Debug.LogError("Scene " + newScene + " not found.");
            yield break;
        }

        while (!asyncLoad.isDone) {
            yield return null;
        }

    }

}
