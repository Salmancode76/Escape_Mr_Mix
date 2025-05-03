using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DeathUIHandler : MonoBehaviour
{
    public AudioClip ClickClip;
    public Text LoadingText;
    public GameObject DeathUI, FoneLoad, LoadParentObject;

    public MixAIController mixAI;

    public void OnRetry()
    {
        SoundFXManager.instance.playSoundFXClip(ClickClip, transform, 1f);

        // Disable the AI's detection logic and stop the death sound
        if (mixAI != null)
        {
            mixAI.isDead = true; // Prevent the death sound from being triggered
        }

        DisableOtherUI();

        DeathUI.SetActive(false);
        FoneLoad.SetActive(true);
        LoadParentObject.SetActive(true);

        StartCoroutine(LoadAsync(SceneManager.GetActiveScene().buildIndex));
    }



    public void OnMainMenu()
    {
        SoundFXManager.instance.playSoundFXClip(ClickClip, transform, 1f);

        DisableOtherUI();

        DeathUI.SetActive(false);
        FoneLoad.SetActive(true);
        LoadParentObject.SetActive(true);

        StartCoroutine(LoadAsync(0));
    }

    private IEnumerator LoadAsync(int sceneIndex)
    {
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

    private void DisableOtherUI()
    {
        Canvas[] allCanvases = FindObjectsOfType<Canvas>();

        foreach (Canvas canvas in allCanvases)
        {
            if (canvas.gameObject != this.gameObject)
            {
                canvas.gameObject.SetActive(false);
            }
        }
    }
}
