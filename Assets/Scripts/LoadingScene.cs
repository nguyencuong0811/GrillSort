using System.Collections;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class LoadingScene : MonoBehaviour
{
    public static LoadingScene Instance;
    public CanvasGroup pnlLoading;

    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void GoToGame()
    {
        StartCoroutine(LoadAsyncScene("Main"));
    }
    public void BackToHome()
    {
        StartCoroutine(LoadAsyncScene("Home"));
    }
    IEnumerator LoadAsyncScene(string nameScene)
    {
        pnlLoading.gameObject.SetActive(true);
        pnlLoading.alpha = 0f;
        pnlLoading.DOFade(1f, 0.5f);

        yield return new WaitForSeconds(0.5f);

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(nameScene);

        asyncLoad.allowSceneActivation = false;

        while (asyncLoad.progress < 0.9f)
        {
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);

        asyncLoad.allowSceneActivation = true;

        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        pnlLoading.DOFade(0f, 1f).OnComplete(() =>
        {
            pnlLoading.gameObject.SetActive(false);
        });


    }
}
