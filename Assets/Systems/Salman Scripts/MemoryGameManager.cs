using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

// Remove these if they cause errors in your project
// using UnityTutorial.Manager;
// using UnityTutorial.PlayerControl;

public class MemoryGameManager : MonoBehaviour
{
    public static MemoryGameManager instance;
    public TextMeshProUGUI instructionText;
    public TextMeshProUGUI timerText;

    public PuseMenu puseMenu;
    public List<press_btn> buttons;
    public AudioClip successSound;
    public AudioClip defused;
    public AudioClip explosionSound;
    public AudioClip manScreem;
    
    // Death screen reference
    public GameObject deathUI;
    
    // Add this to reference the death UI handler
    private DeathUIHandler deathUIHandler;

    public int roundsToWin = 5;
    private int currentRound = 0;

    private List<press_btn> sequence = new List<press_btn>();
    private int currentInputIndex = 0;
    private bool waitingForPlayer = false;

    public float playerTimeLimit = 5f;
    private float playerTimer = 0f;
    private bool timerRunning = false;

    private bool gameStarted = false;
    private bool showIntro = false;

    void Start()
    {
        instance = this;
        
        // Find the global death UI
        if (deathUI == null)
        {
            // Try to find the death UI in the scene
            deathUI = GameObject.Find("DeathUI");
        }
        
        // Get the DeathUIHandler component
        if (deathUI != null)
        {
            deathUIHandler = deathUI.GetComponent<DeathUIHandler>();
            deathUI.SetActive(false);
        }
    }

    void Update()
    {
        if (timerRunning)
        {
            playerTimer -= Time.deltaTime;
            if (playerTimer <= 0f)
            {
                timerRunning = false;
                PlayerFailedTimeout();
            }
            UpdateTimerDisplay();
        }
    }

    public void StartIntroSequence()
    {
        if (!gameStarted)
        {
            gameStarted = true;
            StartCoroutine(ShowIntroAndStartGame());
        }
    }

    IEnumerator ShowIntroAndStartGame()
    {
        showIntro = true;

        instructionText.gameObject.SetActive(false);
        instructionText.gameObject.SetActive(true);
        instructionText.color = Color.red;
        instructionText.ForceMeshUpdate();
        instructionText.text = "Press the correct buttons to stop the bomb!";

        if (timerText != null)
        {
            timerText.color = Color.red;
            timerText.text = "";
        }

        yield return new WaitForSeconds(10f);
        showIntro = false;
        StartCoroutine(StartGameWithDelay());
    }

    IEnumerator StartGameWithDelay()
    {
        if (!showIntro)
        {
            instructionText.color = Color.red;
            instructionText.text = "Get Ready...";
        }

        if (timerText != null)
        {
            timerText.color = Color.red;
            timerText.text = "";
        }

        yield return new WaitForSeconds(2f);
        StartCoroutine(StartNewRound());
    }

    IEnumerator StartNewRound()
    {
        waitingForPlayer = false;
        currentInputIndex = 0;

        if (currentRound >= roundsToWin)
        {
            LevelCompleted();
            yield break;
        }

        press_btn newButton = buttons[Random.Range(0, buttons.Count)];
        sequence.Add(newButton);

        yield return StartCoroutine(FlashSequence());

        waitingForPlayer = true;
        if (!showIntro)
        {
            instructionText.color = Color.red;
            instructionText.text = "Your Turn!";
        }

        playerTimer = playerTimeLimit;
        timerRunning = true;
        UpdateTimerDisplay();
    }

    IEnumerator FlashSequence()
    {
        if (!showIntro)
        {
            instructionText.color = Color.red;
            instructionText.text = "Watch Carefully!";
        }

        if (timerText != null)
        {
            timerText.color = Color.red;
            timerText.text = "";
        }

        foreach (var btn in sequence)
        {
            if (btn.pointLight != null)
            {
                btn.pointLight.enabled = true;
                btn.UpdateBulbMaterial();
                yield return new WaitForSeconds(0.5f);

                btn.pointLight.enabled = false;
                btn.UpdateBulbMaterial();
                yield return new WaitForSeconds(0.2f);
            }
        }
    }

    public void PlayerPressedButton(press_btn pressedButton)
    {
        if (!waitingForPlayer) return;
        StartCoroutine(HandlePlayerPress(pressedButton));
    }

    IEnumerator HandlePlayerPress(press_btn pressedButton)
    {
        if (pressedButton.pointLight != null)
        {
            pressedButton.pointLight.enabled = true;
            pressedButton.UpdateBulbMaterial();
        }

        yield return new WaitForSeconds(0.5f);

        if (pressedButton.pointLight != null)
        {
            pressedButton.pointLight.enabled = false;
            pressedButton.UpdateBulbMaterial();
        }

        if (pressedButton == sequence[currentInputIndex])
        {
            currentInputIndex++;
            if (currentInputIndex >= sequence.Count)
            {
                waitingForPlayer = false;
                timerRunning = false;
                StartCoroutine(SuccessSequence());
            }
        }
        else PlayerFailed();
    }

    void PlayerFailedTimeout()
    {
        if (!waitingForPlayer) return;
        PlayerFailed();
    }

    void PlayerFailed()
    {
        if (!showIntro)
        {
            instructionText.color = Color.red;
            instructionText.text = "❌ Wrong! Game Over!";
        }

        if (timerText != null)
        {
            timerText.color = Color.red;
            timerText.text = "";
        }

        waitingForPlayer = false;
        timerRunning = false;

        foreach (AudioSource audio in FindObjectsOfType<AudioSource>())
        {
            if (audio.isPlaying) audio.Stop();
        }

        // Play explosion sounds
        PlaySound2D(manScreem);
        PlaySound2D(explosionSound);
        sequence.Clear();
        
        // Try to trigger player death directly
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            // Try to find a health component and kill the player
            var healthComponents = player.GetComponents<MonoBehaviour>();
            bool playerKilled = false;
            
            foreach (var component in healthComponents)
            {
                string typeName = component.GetType().Name;
                if (typeName.Contains("Health") || typeName.Contains("Death") || typeName.Contains("Damage"))
                {
                    // Try common death method names
                    player.SendMessage("TakeDamage", 100f, SendMessageOptions.DontRequireReceiver);
                    player.SendMessage("Die", SendMessageOptions.DontRequireReceiver);
                    playerKilled = true;
                    break;
                }
            }
            
            // If we couldn't find a way to kill the player, use our own death screen
            if (!playerKilled)
            {
                ShowDeathScreen();
            }
        }
        else
        {
            // No player found, use our own death screen
            ShowDeathScreen();
        }
    }

    void ShowDeathScreen()
    {
     
     Cursor.lockState = 0; // This is equivalent to CursorLockState.None
    Cursor.visible = true;
       // Try to find the global death UI if not assigned
        if (deathUI == null)
        {
            deathUI = GameObject.Find("DeathUI");
        }
        
        // Show the death UI
        if (deathUI != null)
        {
            deathUI.SetActive(true);
            
            // Make sure time is running so buttons can be clicked
            Time.timeScale = 1f;
            
            // Disable game UI elements
            if (instructionText != null)
                instructionText.gameObject.SetActive(false);
                
            if (timerText != null)
                timerText.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogError("Death UI not found!");
            RestartGame();
        }
        
        // Disable player movement
        DisablePlayerMovement();
    }
    
    void DisablePlayerMovement()
    {
        // Find and disable all possible player control scripts
        MonoBehaviour[] allScripts = FindObjectsOfType<MonoBehaviour>();
        foreach (MonoBehaviour script in allScripts)
        {
            string typeName = script.GetType().Name;
            if (typeName.Contains("Controller") || 
                typeName.Contains("Input") || 
                typeName.Contains("Player"))
            {
                script.enabled = false;
            }
        }
        
        // Try to freeze the player's rigidbody
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Rigidbody rb = player.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = true;
                rb.velocity = Vector3.zero;
            }
            
            // Also try to disable character controller
            CharacterController cc = player.GetComponent<CharacterController>();
            if (cc != null)
            {
                cc.enabled = false;
            }
        }
    }

    IEnumerator SuccessSequence()
    {
        waitingForPlayer = false;
        if (!showIntro)
        {
            instructionText.color = Color.red;
            instructionText.text = "✅ Well Done!";
        }

        if (timerText != null)
        {
            timerText.color = Color.red;
            timerText.text = "";
        }

        currentRound++;

        foreach (var btn in buttons)
        {
            if (btn.pointLight != null)
            {
                btn.pointLight.enabled = false;
                btn.UpdateBulbMaterial();
            }
        }

        PlaySound2D(successSound);
        yield return new WaitForSeconds(1f);
        StartCoroutine(StartNewRound());
    }

    void RestartGame()
    {
        currentRound = 0;
        sequence.Clear();
        StartCoroutine(StartGameWithDelay());
    }

    void LevelCompleted()
    {
            foreach (AudioSource audio in FindObjectsOfType<AudioSource>())
    {
        if (audio.isPlaying)
        {
            audio.Stop();
        }
    }
        PlaySound2D(defused, 1.5f);
        if (!showIntro)
        {
            instructionText.color = Color.green;
            instructionText.text = "You Passed the Level!";
        }

        if (timerText != null)
        {
            timerText.color = Color.red;
            timerText.text = "";
        }
         waitingForPlayer = false;
    timerRunning = false;
    gameStarted = false;
     StopAllCoroutines();
    sequence.Clear();
       foreach (var btn in buttons)
    {
        if (btn.pointLight != null)
        {
            btn.pointLight.enabled = false;
            btn.UpdateBulbMaterial();
        }
    }
      // Optional: Create a victory effect
    StartCoroutine(VictoryEffect());

        StartCoroutine(LoadNextSceneAfterDelay());

}
    IEnumerator LoadNextSceneAfterDelay()
    {
        // Wait for victory effects and sound to play
        yield return new WaitForSeconds(3f);

        // Load Rehan's scene (scene index 2)
        //    SceneManager.LoadScene("4- Rehan");

        puseMenu.LoadNextLevel();
    
}
// Add this for a cool victory effect
IEnumerator VictoryEffect()
{
    // Flash all buttons in celebration
    for (int i = 0; i < 3; i++)
    {
        // Turn all lights on
        foreach (var btn in buttons)
        {
            if (btn.pointLight != null)
            {
                btn.pointLight.enabled = true;
                btn.UpdateBulbMaterial();
            }
        }
        yield return new WaitForSeconds(0.2f);
        
        // Turn all lights off
        foreach (var btn in buttons)
        {
            if (btn.pointLight != null)
            {
                btn.pointLight.enabled = false;
                btn.UpdateBulbMaterial();
            }
        }
        yield return new WaitForSeconds(0.2f);
    }
}
    void PlaySound2D(AudioClip clip, float volume = 1f)
    {
        if (clip == null) return;
        GameObject tempGO = new GameObject("TempAudio");
        AudioSource aSource = tempGO.AddComponent<AudioSource>();
        aSource.clip = clip;
        aSource.volume = volume;
        aSource.spatialBlend = 0f;
        aSource.Play();
        Destroy(tempGO, clip.length);
    }

    void UpdateTimerDisplay()
    {
        if (timerText != null)
        {
            timerText.color = Color.red;
            timerText.text = "Time Left: " + Mathf.Ceil(playerTimer).ToString() + "s";
        }
    }
}