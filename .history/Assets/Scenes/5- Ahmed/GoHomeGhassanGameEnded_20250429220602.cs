using System.Collections;
using UnityEngine;
using AuroraFPSRuntime.SystemModules.HealthModules;  // for ObjectHealth

public class GoHomeGhassanGameEnded : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ObjectHealth mixObjectHealth;
    [SerializeField] private GameObject healthUI;

    [Header("Door Objects")]
    [Tooltip("Enabled by default")]
    [SerializeField] private GameObject closedDoor;
    [SerializeField] private GameObject openedDoor;

    [Header("Audio Clips")]
    [SerializeField] private AudioClip lockOpenClip;
    [SerializeField] private AudioClip doorOpenClip;

    private bool hasOpened = false;

    void Start()
    {
        if (mixObjectHealth == null)
        {
            mixObjectHealth = GetComponent<ObjectHealth>();
            if (mixObjectHealth == null)
            {
                Debug.LogError("No ObjectHealth found!");
                enabled = false;
            }
        }
        // ensure initial door state
        if (closedDoor != null) closedDoor.SetActive(true);
        if (openedDoor != null) openedDoor.SetActive(false);
    }

    void Update()
    {
        if (hasOpened) return;

        if (mixObjectHealth.GetHealth() <= 0f)
        {
            StartCoroutine(OnKillOpenDoor());
            hasOpened = true;
        }
    }

    private IEnumerator OnKillOpenDoor()
    {
        // hide health UI
        if (healthUI != null)
            healthUI.SetActive(false);

        // play lock-open
        if (SoundFXManager.instance != null && lockOpenClip != null)
            SoundFXManager.instance.playSoundFXClip(lockOpenClip, transform, 1f);

        // wait before door-open sound
        yield return new WaitForSeconds(0.6f);

        // play door-open
        if (SoundFXManager.instance != null && doorOpenClip != null)
            SoundFXManager.instance.playSoundFXClip(doorOpenClip, transform, 1f);

        // swap door objects
        if (closedDoor != null) closedDoor.SetActive(false);
        if (openedDoor != null) openedDoor.SetActive(true);
    }
}
