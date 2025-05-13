using UnityEngine;

namespace PadlockSystem
{
    public class PLDoorAnimation : MonoBehaviour
    {
        [Header("Door Animation Settings")]
        [SerializeField] private string doorAnimation = "DoorOpen";

        [Header("Optional Locker Sound")]
        [SerializeField] private AudioClip lockerSoundClip;
        [SerializeField] private float volume = 1f;

        private Animator anim;

        private void Start()
        {
            anim = GetComponent<Animator>();
        }

        public void PlayAnimation()
        {
            if (anim != null)
            {
                anim.Play(doorAnimation, 0, 0.0f);

                if (gameObject.name == "Locker 1" || gameObject.name == "Locker 2")
                {
                    if (lockerSoundClip != null)
                    {
                        SoundFXManager.instance.playSoundFXClip(lockerSoundClip, transform, volume);
                    }
                }
            }
        }
    }
}
