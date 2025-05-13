using UnityEngine;

namespace PadlockSystem
{
    public class DoorInteraction : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private KeyInteraction keyInteraction;
        [SerializeField] private Animator doorAnimator;
        [SerializeField] private GameObject doorPromptUI;

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
                doorAnimator.Play("DoorOpen");
                doorOpened = true;
                doorPromptUI?.SetActive(false);
            }
        }
    }
}
