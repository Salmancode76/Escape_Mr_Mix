using UnityEngine;
using System.Collections;

public class Lightning : MonoBehaviour{
	
	public float lightningOffMin = 5.0f;
	public float lightningOffMax = 30.0f;
	public float lightningOnMin = 0.05f;
	public float lightningOnMax = 0.1f;
	public float soundDelayMin = 0.25f;
	public float soundDelayMax = 2.0f;
	public GameObject lightning;
	public AudioClip[] Thunder;
	
	void OnEnable()
	{
		StartCoroutine(lighter());
	}
	
	IEnumerator lighter()
{
	while (true)
	{
		// Wait before lightning appears
		yield return new WaitForSeconds(Random.Range(lightningOffMin, lightningOffMax));

		// Show lightning
		lightning.SetActive(true);

		// Wait before playing thunder
		yield return new WaitForSeconds(Random.Range(soundDelayMin, soundDelayMax));

		// Select random thunder clip
		AudioClip clipToPlay = Thunder[Random.Range(0, Thunder.Length)];

		// Play thunder using SoundFXManager
		SoundFXManager.instance.playSoundFXClip(clipToPlay, transform, 0.65f);

		// Lightning stays on for a short time
		yield return new WaitForSeconds(Random.Range(lightningOnMin, lightningOnMax));

		// Turn lightning off
		lightning.SetActive(false);
	}
}

}