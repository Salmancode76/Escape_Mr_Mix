using UnityEngine;
using System.Collections;
using TMPro;

public class CloseDoorTrigger : MonoBehaviour
{
    public GameObject doorObject2; // The first door to close
    public GameObject doorToOpen;  // The second door to open after delay
    public AudioClip closeDoorSound;
    public AudioClip bomb_beeb;
    public AudioClip evilLaugh;
    public AudioClip newThemeMusic;
    public Collider doorCollider; // This should be the collider on doorObject2
    public TextMeshProUGUI trapMessageText;
    public GameObject objectToDestroy;
    
    private bool hasClosed = false;

    void OnTriggerEnter(Collider other)
    {
        if (hasClosed) return;

        if (other.CompareTag("Player"))
        {
            Destroy(objectToDestroy);
            Animation anim = doorObject2.GetComponent<Animation>();

            if (anim != null && anim.GetClip("Close") != null)
            {
                // Stop all current music/audio
                foreach (AudioSource audio in FindObjectsOfType<AudioSource>())
                {
                    if (audio.isPlaying)
                        audio.Stop();
                }

                Debug.Log("🔇 All music stopped.");

                // Play evil laugh
                if (evilLaugh != null)
                    AudioSource.PlayClipAtPoint(evilLaugh, Camera.main.transform.position);

                // Loop bomb_beeb from BombAudio object
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

                // Loop new theme music from GlobalAudio object
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

                // Close door animation & sound
                if (closeDoorSound != null)
                    AudioSource.PlayClipAtPoint(closeDoorSound, transform.position);

                anim[anim.GetClip("Close").name].speed = 1.5f;
                anim.Play("Close");

                Debug.Log("🚪 First door closed!");

                // Enable the collider on the first door when it closes
                // This ensures player can't walk through the closed door
                if (doorCollider != null)
                {
                    doorCollider.enabled = true;
                    Debug.Log("✅ First door collider enabled - blocking passage!");
                }

                // Start coroutine to open second door after delay
                StartCoroutine(OpenNextDoorAfterDelay());

                hasClosed = true;

                // Show trap message
                if (trapMessageText != null)
                {
                    trapMessageText.text = "Explosives surround you. One way out. One chance. Win my game… or paint the room red.";
                    trapMessageText.enabled = true;
                    Debug.Log("💀 Trap message shown.");

                    StartCoroutine(HideTrapMessageAfterDelay());
                }
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
                
                // Disable the collider on the second door when it opens
                // so player can walk through
                Collider secondDoorCollider = doorToOpen.GetComponent<Collider>();
                if (secondDoorCollider != null)
                {
                    secondDoorCollider.enabled = false;
                    Debug.Log("🚪 Second door opened - collider disabled!");
                }
            }
            else
            {
                Debug.LogWarning("❌ Second door animation missing or no 'Open' clip.");
            }
        }
    }

    IEnumerator HideTrapMessageAfterDelay()
    {
        yield return new WaitForSeconds(5f);
        if (trapMessageText != null)
        {
            trapMessageText.text = "";
            Debug.Log("🧹 Trap message cleared.");
        }
    }
}