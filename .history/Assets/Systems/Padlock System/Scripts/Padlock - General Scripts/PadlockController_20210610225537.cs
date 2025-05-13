using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace PadlockSystem
{
    public class PadlockController : MonoBehaviour
    {
        [Header("Your Inputs")]
        [SerializeField] private string yourCombination = "1234";
        private string playerCombi;
        private bool hasUnlocked;
        private bool isShowing;
        private Camera mainCamera;

        //Hiddie from the inspector because these are only integers to hold some information for later.
        [HideInInspector] public int combinationRow1;
        [HideInInspector] public int combinationRow2;
        [HideInInspector] public int combinationRow3;
        [HideInInspector] public int combinationRow4;

        [Header("Interactive Padlock")]
        [SerializeField] private GameObject interactableLock = null;

        [Header("Camera GameObject References")]
        [SerializeField] private GameObject cameraPadlock = null;
        private Animator lockAnim;

        [Header("Audio Names")]
        [SerializeField] private string padlockInteractSound = "PadlockInteract";
        [SerializeField] private string padlockSpinSound = "PadlockSpin";
        [SerializeField] private string padlockUnlockSound = "PadlockUnlock";

        [Header("Trigger Type - ONLY if using a trigger event")]
        [SerializeField] private PadlockTrigger triggerObject = null;
        [SerializeField] private bool isPadlockTrigger = false;

        [Header("Unlock Events")]
        [SerializeField] private UnityEvent unlock = null;

        void Awake()
        {
            lockAnim = cameraPadlock.GetComponentInChildren<Animator>();
            mainCamera = Camera.main;
            combinationRow1 = 1;
            combinationRow2 = 1;
            combinationRow3 = 1;
            combinationRow4 = 1;
        }

        void UnlockPadlock()
        {
            unlock.Invoke();
        }

        public void ShowPadlock()
        {
            PLDisableManager.instance.DisablePlayer(true);
            cameraPadlock.SetActive(true);
            isShowing = true;    
            mainCamera.transform.localEulerAngles = new Vector3(0, 0, 0);
            InteractSound();

            if (isPadlockTrigger)
            {
                triggerObject.interactPrompt.SetActive(false);
                triggerObject.enabled = false;
            }
        }

        void DisablePadlock()
        {
            PLDisableManager.instance.DisablePlayer(false);
            cameraPadlock.SetActive(false);
            isShowing = false;

            if (isPadlockTrigger)
            {
                triggerObject.interactPrompt.SetActive(true);
                triggerObject.enabled = true;
            }
        }

        public void CheckCombination()
        {
            playerCombi = combinationRow1.ToString("0") + combinationRow2.ToString("0") + combinationRow3.ToString("0") + combinationRow4.ToString("0");

            if (playerCombi == yourCombination)
            {
                if (!hasUnlocked)
                {
                    StartCoroutine(CorrectCombination());
                    hasUnlocked = true;
                }
            }
        }

        IEnumerator CorrectCombination()
        {
            lockAnim.Play("LockOpen");
            UnlockSound();

            const float waitDuration = 1.2f;
            yield return new WaitForSeconds(waitDuration);

            cameraPadlock.SetActive(false);
            interactableLock.SetActive(false);
            UnlockPadlock();

            PLDisableManager.instance.DisablePlayer(false);
            gameObject.SetActive(false);
        }

        void Update()
        {
            if (isShowing)
            {
                if (Input.GetKeyDown(PLInputManager.instance.closeKey))
                {
                    DisablePadlock();
                }
            }
        }

        void InteractSound()
        {
            PLAudioManager.instance.Play(padlockInteractSound);
        }

        public void SpinSound()
        {
            PLAudioManager.instance.Play(padlockSpinSound);
        }

        public void UnlockSound()
        {
            PLAudioManager.instance.Play(padlockUnlockSound);
        }
    }
}
