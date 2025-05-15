using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key_Script : MonoBehaviour
{
    public float rotationSpeed = 100f;      // Rotation speed in degrees per second
    public AudioClip pickupSound;           // Optional: assign in Inspector

    private AudioSource audioSource;
    private bool collected = false;

    void Start()
    {
        // Set up audio source if a sound is assigned
        if (pickupSound != null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.clip = pickupSound;
            audioSource.playOnAwake = false;
        }
    }

    void Update()
    {
        // Rotate only if not yet collected
        if (!collected)
        {
            transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
        }
    }
    

    private void OnTriggerEnter(Collider other)
    {
        if (collected) return;


        if (other.CompareTag("Player"))
        {

            collected = true;

            // Update key UI
            FindObjectOfType<Collect_Keys>()?.AddKey();

            // Disable collider immediately
            GetComponent<Collider>().enabled = false;

            // Optionally disable visuals (e.g., if mesh is a child)
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(false);
            }

            // Play sound + destroy after sound finishes
            if (audioSource != null)
            {
                audioSource.Play();
                StartCoroutine(DestroyAfterSound(pickupSound.length));
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }

    IEnumerator DestroyAfterSound(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }
}
