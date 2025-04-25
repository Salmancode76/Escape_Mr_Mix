using UnityEngine;

public class SimpleKey : MonoBehaviour
{
    public GameObject player;              // Drag your player here
    public float pickupDistance = 2f;      // Distance to trigger collection
    public Collect_Keys keyManager;        // Your key manager with AddKey()
    public GameObject pressUIPrompt;       // The world space UI prompt
    public AudioClip pickupSound;          // The sound played on pickup

    private bool collected = false;

    void Start()
    {
        if (pressUIPrompt != null)
            pressUIPrompt.SetActive(false);
    }

    void Update()
    {
        if (collected) return;

        float distance = Vector3.Distance(player.transform.position, transform.position);
        Vector3 directionToKey = (transform.position - player.transform.position).normalized;
        float dot = Vector3.Dot(player.transform.forward, directionToKey); // 1 = looking directly

        bool lookingAtKey = dot > 0.5f;

        if (distance <= pickupDistance && lookingAtKey)
        {
            if (pressUIPrompt != null && !pressUIPrompt.activeSelf)
                pressUIPrompt.SetActive(true);

            if (Input.GetKeyDown(KeyCode.E))
            {
                collected = true;
                CollectKey();
            }
        }
        else
        {
            if (pressUIPrompt != null && pressUIPrompt.activeSelf)
                pressUIPrompt.SetActive(false);
        }
    }

    void CollectKey()
    {
        keyManager.AddKey();

        // Play the sound at the key's position, even after it's destroyed
        if (pickupSound != null)
        {
            AudioSource.PlayClipAtPoint(pickupSound, transform.position);
        }

        Destroy(gameObject); // Destroy the key immediately
    }
}
