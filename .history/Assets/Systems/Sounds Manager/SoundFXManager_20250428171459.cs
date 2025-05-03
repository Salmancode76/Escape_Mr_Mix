using System;
using Unity.Mathematics;
using UnityEngine;

public class SoundFXManager : MonoBehaviour
{
    public static SoundFXManager instance;

    [SerializeField] private AudioSource soundFXObject;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void playSoundFXClip(AudioClip audioClip, Transform spawnTransform, float volume)
    {
        // Spawn the audio source
        AudioSource audioSource = Instantiate(soundFXObject, spawnTransform.position, quaternion.identity);

        // Assign the audioClip
        audioSource.clip = audioClip;

        // Assign volume
        audioSource.volume = volume;

        // Play sound
        audioSource.Play();

        // Get length of sound FX clip
        float clipLength = audioSource.clip.length;

        // Destroy the clip after it finishes
        Destroy(audioSource.gameObject, clipLength);
    }

    public AudioSource playSoundFXClipLooped(AudioClip audioClip, Transform spawnTransform, float volume)
    {
        // Spawn the audio source
        AudioSource audioSource = Instantiate(soundFXObject, spawnTransform.position, quaternion.identity);

        // Configure audio source to loop and play the sound
        audioSource.clip = audioClip;
        audioSource.volume = volume;
        audioSource.loop = true;
        audioSource.Play();

        // Return the audio source for manual control of stopping
        return audioSource;
    }

    // New function to handle footstep sound 3D audio based on player distance
    public void PlayFootstepsBasedOnDistance(Transform mixTransform, Transform playerTransform, AudioClip[] footstepClips, AudioClip bassClip, float minDistance = 2f, float maxDistance = 6f)
    {
        // Calculate the distance between Mix and the player
        float distance = Vector3.Distance(mixTransform.position, playerTransform.position);

        // Ensure that volume only plays between minDistance and maxDistance
        float volume = 0f;

        // If the player is within range
        if (distance <= maxDistance)
        {
            // Calculate volume based on distance
            if (distance <= minDistance)
            {
                volume = 1f; // Full volume if very close
            }
            else
            {
                volume = Mathf.Lerp(1f, 0f, (distance - minDistance) / (maxDistance - minDistance)); // Fade out as Mix gets farther
            }

            // Play footstep sound if within range
            if (footstepClips.Length > 0)
            {
                AudioClip step = footstepClips[UnityEngine.Random.Range(0, footstepClips.Length)];
                playSoundFXClip(step, mixTransform, volume); // Adjusted volume
            }

            // Play bass sound if within range
            if (bassClip != null)
            {
                playSoundFXClip(bassClip, mixTransform, volume); // Adjusted volume
            }
        }
    }

}
