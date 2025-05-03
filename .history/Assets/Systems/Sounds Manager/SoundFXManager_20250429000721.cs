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
    public void PlayFootstepsBasedOnDistance(AudioClip audioClip, Transform spawnTransform, float volume)
    {
        // Spawn the audio source
        AudioSource audioSource = Instantiate(soundFXObject, spawnTransform.position, quaternion.identity);

        // Assign the audioClip
        audioSource.clip = audioClip;

        // Assign volume
        audioSource.volume = volume;
        audioSource.spatialBlend = 1.1f; // Set to 3D sound
        audioSource.maxDistance = 10f; // Set max distance for 3D sound
        audioSource.minDistance = 2f; // Set min distance for 3D sound
        // Play sound
        audioSource.Play();

        // Get length of sound FX clip
        float clipLength = audioSource.clip.length;

        // Destroy the clip after it finishes
        Destroy(audioSource.gameObject, clipLength);
    }
}
