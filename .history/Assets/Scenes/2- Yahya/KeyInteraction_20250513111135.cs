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

        private void Start()
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }

        private void Update()
        {
            if (!padlockTrigger || !padlockTrigger.HasUnlocked)
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
            }
        }
    }
}
