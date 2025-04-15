using Unity.Mathematics;
using UnityEngine;

public class SoundFXManager : MonoBehaviour
{
    public static SoundFXManager instance;

    [SerializeField] private AudioSource soundFXObject;

    void Awake()
    {
        if(instance == null){
            instance = this;
        }

    }

    public void playSoundFXClip(AudioClip audioClip, Transform spawnTransform, float volume){
            // spawn the gameObject
            AudioSource audioSource = Instantiate(soundFXObject, spawnTransform.position , quaternion.identity);


            // assign the audioClip
            audioSource.clip = audioClip;

            // assign volume
            audioSource.volume = volume;

            // play sound
            audioSource.Play();

            // get kength of sound FX clip
            float clipLength = audioSource.clip.length;

            // destroy the clip 
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
}
