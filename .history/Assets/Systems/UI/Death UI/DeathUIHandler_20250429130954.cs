using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DeathUIHandler : MonoBehaviour
{
    public AudioClip ClickClip;
    public Text LoadingText;
    public GameObject FoneLoad, LoadParentObject;

    public void OnRetry()
    {
        SoundFXManager.instance.playSoundFXClip(ClickClip, transform, 1f);

        // Prepare UI
        FoneLoad.SetActive(true);
        LoadParentObject.SetActive(true);

        // Start async reload
        StartCoroutine(LoadAsync(SceneManager.GetActiveScene().buildIndex));
    }

    public void OnMainMenu()
    {
        SoundFXManager.instance.playSoundFXClip(ClickClip, transform, 1f);

        // Prepare UI
        FoneLoad.SetActive(true);
        LoadParentObject.SetActive(true);

        // Start async load of main menu (scene 0)
        StartCoroutine(LoadAsync(0));
    }

    private IEnumerator LoadAsync(int sceneIndex)
    {
        // Unfreeze time
        Time.timeScale = 1f;

        yield return new WaitForSecondsRealtime(0.5f);

        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(sceneIndex);
        loadOperation.allowSceneActivation = false;

        while (loadOperation.progress < 0.9f)
        {
            float progress = Mathf.Clamp01(loadOperation.progress / 0.9f);
            int percent = Mathf.RoundToInt(progress * 100f);
            if (LoadingText != null)
                LoadingText.text = "Loading... " + percent + "%";

            yield return null;
        }

        if (LoadingText != null)
            LoadingText.text = "Loading... 100%";

        yield return new WaitForSecondsRealtime(0.5f);

        loadOperation.allowSceneActivation = true;
    }
}
