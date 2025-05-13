using UnityEngine;
using System.Collections;

namespace PadlockSystem
{
    public class DoorInteraction : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private KeyInteraction keyInteraction;
        [SerializeField] private Animator doorAnimator;
        [SerializeField] private GameObject doorPromptUI;

        [Header("Audio Clips")]
        [SerializeField] private AudioClip insertKeyClip;
        [SerializeField] private AudioClip unlockClip;
        [SerializeField] private AudioClip openDoorClip;
        [SerializeField] private AudioClip openDoorCrackClip;
        [SerializeField] private float volume = 1f;

        [Header("Settings")]
        [SerializeField] private float interactionDistance = 2f;

        private Transform player;
        private bool doorOpened = false;

        private void Start()
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
        }

        private void Update()
        {
            if (doorOpened || player == null || keyInteraction == null || !keyInteraction.HaveKey)
            {
                doorPromptUI?.SetActive(false);
                return;
            }

            float distance = Vector3.Distance(transform.position, player.position);
            bool isClose = distance <= interactionDistance;

            doorPromptUI?.SetActive(isClose);

            if (isClose && Input.GetKeyDown(KeyCode.E))
            {
                doorOpened = true;
                doorPromptUI?.SetActive(false);
                StartCoroutine(PlayUnlockSequence());
            }
        }

        private IEnumerator PlayUnlockSequence()
        {
            // Insert Key Sound
            if (insertKeyClip != null)
            {
                SoundFXManager.instance.playSoundFXClip(insertKeyClip, transform, volume);
                yield return new WaitForSeconds(insertKeyClip.length);
            }

            // Unlock Sound
            if (unlockClip != null)
            {
                SoundFXManager.instance.playSoundFXClip(unlockClip, transform, volume);
                yield return new WaitForSeconds(unlockClip.length);
            }

            // Open Door Sound and Animation
            if (openDoorClip != null)
            {
                SoundFXManager.instance.playSoundFXClip(openDoorClip, transform, volume);
                yield return new WaitForSeconds(openDoorClip.length);
                SoundFXManager.instance.playSoundFXClip(openDoorCrackClip, transform, volume);
            }

            doorAnimator.Play("DoorOpen");
        }
    }
}
