using UnityEngine;

public class KeyPickup : MonoBehaviour
{
    public Collect_Keys collectKeysScript; // Drag in Inspector
    public GameObject pressUIPrompt;       // Drag your KeyPromptCanvas
    private bool playerNearby = false;

    void Start()
    {
        if (pressUIPrompt != null)
        {
            Debug.Log("Prompt hidden at start");
            pressUIPrompt.SetActive(false);
        }
    }

    void Update()
    {
        if (playerNearby && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("E key pressed near key");
            if (collectKeysScript != null)
            {
                collectKeysScript.AddKey();
                Debug.Log("Key collected!");
                Destroy(gameObject);
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered key trigger!");
            playerNearby = true;
            if (pressUIPrompt != null)
                pressUIPrompt.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player exited key trigger!");
            playerNearby = false;
            if (pressUIPrompt != null)
                pressUIPrompt.SetActive(false);
        }
    }
}
