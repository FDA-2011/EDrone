using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    [SerializeField] public GameObject loadingScreen;
    [SerializeField] public Slider slider;
    private bool isOp = false;

    public void LoadLevel(int sceneIndex)
    {
        if (!isOp) StartCoroutine(LoadAsynchronously(sceneIndex));
    }

    public void LoadLevel(int sceneIndex, bool ds)
    {
        if (!isOp) StartCoroutine(LoadAsynchronously(sceneIndex, ds));
    }

    private IEnumerator LoadAsynchronously(int sceneIndex, bool ds=false)
    {
        isOp = true;
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);
        loadingScreen.SetActive(true);
        while (!operation.isDone)
        {
            float progress = operation.progress;
            slider.value = progress;
            yield return null;
        }
        isOp = false;
        loadingScreen.SetActive(false);
        if (ds) Destroy(gameObject);
    }
}
