using UnityEngine;

namespace PadlockSystem
{
    public class KeyInteraction : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private PadlockTrigger padlockTrigger;
        [SerializeField] private GameObject keyPromptUI;
        [SerializeField] private AudioClip TakeKeyClip;

        [Header("Settings")]
        [SerializeField] private float interactionDistance = 2f;

        public bool HaveKey { get; private set; } = false;

        private Transform player;

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
            if (HaveKey || player == null || padlockTrigger == null || !padlockTrigger.HasUnlocked)
            {
                keyPromptUI?.SetActive(false);
                return;
            }

            float distance = Vector3.Distance(transform.position, player.position);
            bool isClose = distance <= interactionDistance;

            keyPromptUI?.SetActive(isClose);

            if (isClose && Input.GetKeyDown(KeyCode.E))
            {
                HaveKey = true;
                SoundFXManager.instance.playSoundFXClip(TakeKeyClip , transform , 1f);
                keyPromptUI?.SetActive(false);
                gameObject.SetActive(false); // Hides the key object
            }
        }
    }
}
