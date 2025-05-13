using UnityEngine;

namespace PadlockSystem
{
    public class KeyInteraction : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private PadlockTrigger padlockTrigger;
        [SerializeField] private GameObject keyPromptUI;

        [Header("Settings")]
        [SerializeField] private float interactionDistance = 2f;

        public bool HaveKey { get; private set; } = false;

        private Transform player;
        private Camera playerCamera;

        private void Start()
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
                playerCamera = Camera.main;
            }
        }

        private void Update()
        {
            if (HaveKey || player == null || padlockTrigger == null || !padlockTrigger.HasUnlocked)
            {
                keyPromptUI?.SetActive(false);
                return;
            }

            bool isLookingAtKey = false;

            Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, interactionDistance))
            {
                if (hit.collider != null && hit.collider.gameObject == gameObject)
                {
                    isLookingAtKey = true;
                }
            }

            keyPromptUI?.SetActive(isLookingAtKey);

            if (isLookingAtKey && Input.GetKeyDown(KeyCode.E))
            {
                HaveKey = true;

                keyPromptUI?.SetActive(false);
                gameObject.SetActive(false); // Hide the key object
            }
        }
    }
}
