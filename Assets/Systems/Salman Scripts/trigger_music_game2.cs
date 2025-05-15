using UnityEngine;

public class trigger_music_game2 : MonoBehaviour
{
    public AudioSource musicSource; // Assign the AudioSource in Inspector
    public AudioClip musicClip;     // Assign your chosen music here
    private int temp = 0;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && temp ==0)
        {
            temp++;
            
            if (musicSource != null && musicClip != null)
            {
               // musicSource.clip = musicClip;
               // musicSource.Play();
            }
        }
    }
}
