using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void StartGame()
    {
        StartCoroutine(LoadSceneAsyncCoroutine("Main"));
    }

    private IEnumerator LoadSceneAsyncCoroutine(string scene)
    {
        AsyncOperation ao = SceneManager.LoadSceneAsync(scene);
        while (!ao.isDone)
        {
            yield return null;
        }
    }
}
