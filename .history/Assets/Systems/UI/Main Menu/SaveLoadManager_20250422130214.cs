using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class SaveLoadManager : MonoBehaviour
{
    public static SaveLoadManager instance;

    public Text LoadingText;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Save current scene index to a slot
    public void SaveGame(int slotIndex)
    {
        int sceneIndex = SceneManager.GetActiveScene().buildIndex;
        PlayerPrefs.SetInt("Saved" + slotIndex, 1);
        PlayerPrefs.SetInt("SceneIndex" + slotIndex, sceneIndex);
        PlayerPrefs.SetString("TimeSave" + slotIndex, System.DateTime.Now.ToString("HH:mm"));
        PlayerPrefs.SetInt("CurrentSlot", slotIndex);
    }

    // Load the scene from the slot
    public void LoadGame(int slotIndex)
    {
        if (PlayerPrefs.GetInt("Saved" + slotIndex) == 1)
        {
            int sceneIndex = PlayerPrefs.GetInt("SceneIndex" + slotIndex);
            StartCoroutine(LoadSceneAsync(sceneIndex));
        }
    }

    // Async loader with UI update
    public IEnumerator LoadSceneAsync(int sceneIndex)
    {
        LoadingText.text = "Loading... 0%";
        yield return new WaitForSecondsRealtime(0.5f);

        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(sceneIndex);
        loadOperation.allowSceneActivation = false;

        while (loadOperation.progress < 0.9f)
        {
            float progress = Mathf.Clamp01(loadOperation.progress / 0.9f);
            int percent = Mathf.RoundToInt(progress * 100f);
            LoadingText.text = "Loading... " + percent + "%";
            yield return null;
        }

        LoadingText.text = "Loading... 100%";
        yield return new WaitForSecondsRealtime(0.5f);

        loadOperation.allowSceneActivation = true;
    }
}
