using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HangingBody : MonoBehaviour
{
    public AudioSource audioSource;     // Assign in Inspector
    public AudioClip[] clips;           // Assign clips in Inspector
    public float delayBetweenSequences = 3f;

    private void Start()
    {
        StartCoroutine(PlaySequenceLoop());
    }

    private IEnumerator PlaySequenceLoop()
    {
        while (true)
        {
            ShuffleClips();

            foreach (AudioClip clip in clips)
            {
                audioSource.clip = clip;
                audioSource.Play();
                yield return new WaitForSeconds(clip.length);
            }

            yield return new WaitForSeconds(delayBetweenSequences);
        }
    }

    private void ShuffleClips()
    {
        for (int i = 0; i < clips.Length; i++)
        {
            int randIndex = Random.Range(i, clips.Length);
            AudioClip temp = clips[i];
            clips[i] = clips[randIndex];
            clips[randIndex] = temp;
        }
    }
}
