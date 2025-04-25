using UnityEngine;

public class SimpleDoor : MonoBehaviour
{
    public GameObject player;
    public float interactDistance = 2f;
    public GameObject pressUIPrompt;
    public Collect_Keys keyManager;
    public Collider doorCollider;
    public GameObject doorObject2; // Door with Animation component
    public AudioClip openDoor;          // The sound played on pickup

    private bool opened = false;

    void Start()
    {
    

if (doorObject2 != null)
{

}

        if (pressUIPrompt != null)
            pressUIPrompt.SetActive(false);
    }

    void Update()
    {
        if (opened || keyManager == null || keyManager.CollectedKeys < keyManager.totalKeys)
            return;

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
        Debug.Log("🎥 Playing 'Door2_open' animation!");
                       AudioSource audioSource = gameObject.AddComponent<AudioSource>();
audioSource.clip = openDoor;
audioSource.Play();

    }
    else
    {
        Debug.LogWarning("⚠️ Animation component or 'Door2_open' clip is missing.");
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
