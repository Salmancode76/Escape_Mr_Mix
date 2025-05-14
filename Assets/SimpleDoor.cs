using UnityEngine;

public class SimpleDoor : MonoBehaviour
{
    public GameObject player;
    public float interactDistance = 2f;
    public GameObject pressUIPrompt;
    public Collect_Keys keyManager;
    public Collider doorCollider;
    public GameObject doorObject2; // Door with Animation component
    public AudioClip openDoor;

    private bool opened = false;

    void Start()
    {
        // Make sure door and key manager are set
        if (doorObject2 == null)
            Debug.LogWarning("🚪 doorObject2 not assigned!");

        if (player == null)
            Debug.LogWarning("🧍 Player reference is missing!");

        if (keyManager == null)
            Debug.LogWarning("🔑 Key manager is not set!");

        // Hide press UI prompt at start
        if (pressUIPrompt != null)
        {
            pressUIPrompt.SetActive(false);
            Debug.Log("✅ Hiding 'Press E' prompt on Start");
        }
    }

    void Update()
    {
        // Skip if door already opened or not enough keys
        if (opened || keyManager == null || keyManager.CollectedKeys < keyManager.totalKeys)
            return;

        // Check distance and view direction
        float distance = Vector3.Distance(player.transform.position, transform.position);
        Vector3 dirToDoor = (transform.position - player.transform.position).normalized;
        float dot = Vector3.Dot(player.transform.forward, dirToDoor);
        bool lookingAtDoor = dot > 0.5f;

        if (distance <= interactDistance && lookingAtDoor)
        {
            if (pressUIPrompt != null && !pressUIPrompt.activeSelf)
                pressUIPrompt.SetActive(true);

            if (Input.GetKeyDown(KeyCode.E))
            {
                opened = true;

                if (pressUIPrompt != null)
                    pressUIPrompt.SetActive(false);

                if (doorCollider != null)
                    doorCollider.enabled = false;

                Animation anim = doorObject2.GetComponent<Animation>();
                if (anim != null && anim.GetClip("Open") != null)
                {
                    anim.Play("Open");
                    Debug.Log("🚪 Playing door 'Open' animation");

                    if (openDoor != null)
                    {
                        AudioSource audioSource = gameObject.AddComponent<AudioSource>();
                        audioSource.clip = openDoor;
                        audioSource.Play();
                    }
                }
                else
                {
                    Debug.LogWarning("⚠️ Missing Animation or 'Open' clip");
                }
            }
        }
        else
        {
            if (pressUIPrompt != null && pressUIPrompt.activeSelf)
                pressUIPrompt.SetActive(false);
        }
    }
}
