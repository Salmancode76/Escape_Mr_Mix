using Unity.Mathematics;
using UnityEngine;

public class SoundFXManager : MonoBehaviour
{
    public static SoundFXManager instance;

    [SerializeField] private AudioSource soundFXObject;
    private AudioSource footstepAudioSource;
    private AudioSource bassAudioSource;

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    public void playSoundFXClip(AudioClip audioClip, Transform spawnTransform, float volume)
    {
        // spawn the gameObject
        AudioSource audioSource = Instantiate(soundFXObject, spawnTransform.position, quaternion.identity);

        // assign the audioClip
        audioSource.clip = audioClip;

        // assign volume
        audioSource.volume = volume;

        // play sound
        audioSource.Play();

        // get length of sound FX clip
        float clipLength = audioSource.clip.length;

        // destroy the clip after it finishes
        Destroy(audioSource.gameObject, clipLength);
    }

    public AudioSource playSoundFXClipLooped(AudioClip audioClip, Transform spawnTransform, float volume)
    {
        // Spawn the audio source
        AudioSource audioSource = Instantiate(soundFXObject, spawnTransform.position, quaternion.identity);

        // Configure audio source to loop, then play sound
        audioSource.clip = audioClip;
        audioSource.volume = volume;
        audioSource.loop = true;
        audioSource.Play();

        // Return the audio source for manual control of stopping
        return audioSource;
    }

    // Custom function to play Mix's footsteps
    public void PlayMixFootsteps(AudioClip footstepClip, AudioClip bassClip, Transform spawnTransform, Transform playerTransform)
    {
        // Calculate the distance between Mix and the player
        float distance = Vector3.Distance(spawnTransform.position, playerTransform.position);

        // If footstepAudioSource doesn't exist, instantiate it
        if (footstepAudioSource == null)
        {
            footstepAudioSource = Instantiate(soundFXObject, spawnTransform.position, quaternion.identity);
            footstepAudioSource.loop = true; // Loop the footstep sound
            footstepAudioSource.clip = footstepClip;
        }

        // If bassAudioSource doesn't exist, instantiate it
        if (bassAudioSource == null && bassClip != null)
        {
            bassAudioSource = Instantiate(soundFXObject, spawnTransform.position, quaternion.identity);
            bassAudioSource.loop = true; // Loop the bass sound
            bassAudioSource.clip = bassClip;
        }

        // Adjust volume based on distance
        float footstepVolume = Mathf.Clamp01(1 - (distance / 6f)); // Volume fades out beyond 6 units
        float bassVolume = Mathf.Clamp01(1 - (distance / 6f)); // Bass volume fades out similarly

        // Update footstep audio volume
        footstepAudioSource.volume = footstepVolume;

        // Update bass audio volume
        if (bassAudioSource != null)
        {
            bassAudioSource.volume = bassVolume;
        }

        // Ensure both audio sources are playing
        if (!footstepAudioSource.isPlaying)
        {
            footstepAudioSource.Play();
        }
        if (bassAudioSource != null && !bassAudioSource.isPlaying)
        {
            bassAudioSource.Play();
        }
        
        // If the player is too far, stop playing the sounds
        if (distance > 6f && footstepAudioSource.isPlaying)
        {
            footstepAudioSource.Stop();
            if (bassAudioSource != null)
            {
                bassAudioSource.Stop();
            }
        }
    }
}
