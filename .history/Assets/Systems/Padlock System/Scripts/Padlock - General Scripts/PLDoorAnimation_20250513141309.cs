using UnityEngine;
using System.Collections;

namespace PadlockSystem
{
    public class PLDoorAnimation : MonoBehaviour
    {
        [Header("Door Animation Settings")]
        [SerializeField] private string doorAnimation = "DoorOpen";

        [Header("Locker Sound (Locker 1 & 2)")]
        [SerializeField] private AudioClip lockerSoundClip;
        [SerializeField] private float lockerVolume = 1f;

        [Header("DoorTwo Sounds")]
        [SerializeField] private AudioClip openDoorClip;
        [SerializeField] private AudioClip doorCrackClip;
        [SerializeField] private float doorVolume = 1f;

        private Animator anim;

        private void Start()
        {
            anim = GetComponent<Animator>();
        }

        public void PlayAnimation()
        {
            if (anim == null)
                return;

            string objName = gameObject.name;

            if (objName == "Locker 1" || objName == "Locker 2")
            {
                if (lockerSoundClip != null)
                {
                    SoundFXManager.instance.playSoundFXClip(lockerSoundClip, transform, lockerVolume);
                }

                anim.Play(doorAnimation, 0, 0.0f);
            }
            else if (objName == "DoorTwo")
            {
                StartCoroutine(PlayDoorTwoSequence());
            }
            else
            {
                anim.Play(doorAnimation, 0, 0.0f);
            }
        }

        private IEnumerator PlayDoorTwoSequence()
        {
            if (openDoorClip != null)
            {
                float openDuration = openDoorClip.length;
                SoundFXManager.instance.playSoundFXClip(openDoorClip, transform, doorVolume);
                yield return new WaitForSeconds(openDuration);
            }

            anim.Play(doorAnimation, 0, 0.0f);

            if (doorCrackClip != null)
            {
                SoundFXManager.instance.playSoundFXClip(doorCrackClip, transform, doorVolume);
            }
        }
    }
}
