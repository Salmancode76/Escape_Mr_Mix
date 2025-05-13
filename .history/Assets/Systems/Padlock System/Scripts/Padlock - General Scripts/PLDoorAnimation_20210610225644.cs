using UnityEngine;

namespace PadlockSystem
{
    public class PLDoorAnimation : MonoBehaviour
    {
        [Header("Door Animation Settings")]
        [SerializeField] private string doorAnimation = "DoorOpen";

        //Example for playing a sound when opening the door
        //[SerializeField] private string doorOpenSound = "DoorSound";

        private Animator anim;

        private void Start()
        {
            anim = GetComponent<Animator>();
        }

        public void PlayAnimation()
        {
            if (null != anim)
            {
                anim.Play(doorAnimation, 0, 0.0f);

                //Uncomment the line below if you want to add an audio sound when the door is opened
                //PLAudioManager.instance.Play(doorOpenSound);
            }
        }
    }
}
