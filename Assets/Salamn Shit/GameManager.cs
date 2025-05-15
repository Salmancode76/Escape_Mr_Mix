using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public Text loadingText;
    public GameObject foneLoad, loadParentObject;
    
    // Add this flag to prevent player from dying during scene transition
    private bool isTransitioning = false;
    
    void Awake()
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
    
    public void RestartLevel(AudioClip clickSound, Transform soundSource)
    {
        // Prevent multiple restarts
        if (isTransitioning) return;
        isTransitioning = true;
        
        // Play sound if needed
        if (SoundFXManager.instance != null && clickSound != null)
        {
            SoundFXManager.instance.playSoundFXClip(clickSound, soundSource, 1f);
        }
        
        // Disable player immediately
        DisablePlayer();
        
        // Setup loading screen
        if (foneLoad != null) foneLoad.SetActive(true);
        if (loadParentObject != null) loadParentObject.SetActive(true);
        
        // Start loading process
        StartCoroutine(LoadAsync(SceneManager.GetActiveScene().buildIndex));
    }
    
    // Add this method to disable the player
    private void DisablePlayer()
    {
        // Find and disable player GameObject
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            // Disable player movement scripts
            MonoBehaviour[] components = player.GetComponents<MonoBehaviour>();
            foreach (MonoBehaviour comp in components)
            {
                comp.enabled = false;
            }
            
            // Or completely disable the player GameObject if appropriate
            // player.SetActive(false);
            
            // If the player has a rigidbody, freeze it
            Rigidbody rb = player.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = true;
                rb.velocity = Vector3.zero;
            }
            
            // If the player has a character controller, disable it
            CharacterController cc = player.GetComponent<CharacterController>();
            if (cc != null)
            {
                cc.enabled = false;
            }
        }
    }
    
    // Rest of the code remains the same...
    public void LoadMainMenu(AudioClip clickSound, Transform soundSource)
    {
        if (isTransitioning) return;
        isTransitioning = true;
        
        // Play sound if needed
        if (SoundFXManager.instance != null && clickSound != null)
        {
            SoundFXManager.instance.playSoundFXClip(clickSound, soundSource, 1f);
        }
        
        // Disable player
        DisablePlayer();
        
        // Setup loading screen
        if (foneLoad != null) foneLoad.SetActive(true);
        if (loadParentObject != null) loadParentObject.SetActive(true);
        
        // Start loading process
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
            if (loadingText != null)
                loadingText.text = "Loading... " + percent + "%";
                
            yield return null;
        }
        
        if (loadingText != null)
            loadingText.text = "Loading... 100%";
            
        yield return new WaitForSecondsRealtime(0.5f);
        
        // Reset transition flag right before loading the new scene
        isTransitioning = false;
        loadOperation.allowSceneActivation = true;
    }
}