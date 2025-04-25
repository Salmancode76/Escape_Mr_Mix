using UnityEngine;
using System.Collections;

public class CloseDoorTrigger : MonoBehaviour
{
    public GameObject doorObject2; // The first door to close
    public GameObject doorToOpen;  // The second door to open after delay
    public AudioClip closeDoorSound;
    public AudioClip bomb_beeb;
    public AudioClip evilLaugh;
    public AudioClip newThemeMusic;
        public Collider doorCollider;


    private bool hasClosed = false;

    void OnTriggerEnter(Collider other)
    {
        if (hasClosed) return;

        if (other.CompareTag("Player"))
        {
            Animation anim = doorObject2.GetComponent<Animation>();

            if (anim != null && anim.GetClip("Close") != null)
            {
                // 🔇 Stop all current music/audio
                foreach (AudioSource audio in FindObjectsOfType<AudioSource>())
                {
                    if (audio.isPlaying)
                        audio.Stop();
                }

                Debug.Log("🔇 All music stopped.");

                // 🔊 Immediately play evil laugh one-shot
                if (evilLaugh != null)
                    AudioSource.PlayClipAtPoint(evilLaugh, Camera.main.transform.position);

                // 💣 Loop bomb_beeb (from BombAudio)
                if (bomb_beeb != null)
                {
                    GameObject bombAudioObj = GameObject.Find("BombAudio");
                    if (bombAudioObj != null)
                    {
                        AudioSource bombSource = bombAudioObj.GetComponent<AudioSource>();
                        bombSource.clip = bomb_beeb;
                        bombSource.loop = true;
                        bombSource.Play();
                        Debug.Log("💣 Bomb beep looping.");
                    }
                }

                // 🎵 Loop newThemeMusic (from GlobalAudio)
                if (newThemeMusic != null)
                {
                    GameObject globalAudioObj = GameObject.Find("GlobalAudio");
                    if (globalAudioObj != null)
                    {
                        AudioSource themeSource = globalAudioObj.GetComponent<AudioSource>();
                        themeSource.clip = newThemeMusic;
                        themeSource.loop = true;
                        themeSource.Play();
                        Debug.Log("🎵 New theme music looping.");
                    }
                }

                // 🚪 Door closing animation & sound
                if (closeDoorSound != null)
                    AudioSource.PlayClipAtPoint(closeDoorSound, transform.position);

                anim[anim.GetClip("Close").name].speed = 1.5f;
                anim.Play("Close");

                Debug.Log("🚪 First door closed!");

                // Start coroutine to open second door after 3 seconds
                StartCoroutine(OpenNextDoorAfterDelay());

                hasClosed = true;
            }
            else
            {
                Debug.LogWarning("❌ 'Close' animation not found on door.");
            }
        }
    }

    IEnumerator OpenNextDoorAfterDelay()
    {
        yield return new WaitForSeconds(5f);

        if (doorToOpen != null)
        {
            Animation secondAnim = doorToOpen.GetComponent<Animation>();
            if (secondAnim != null && secondAnim.GetClip("Open") != null)
            {
                secondAnim.Play("Open");
                  doorCollider.enabled = false;
                Debug.Log("🚪 Second door opened after delay!");
            }
            else
            {
                Debug.LogWarning("❌ Second door animation missing or no 'Open' clip.");
            }
        }
    }
}
