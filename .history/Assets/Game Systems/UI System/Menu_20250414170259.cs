using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Menu : MonoBehaviour {
	public Image Obvod, Obvod2;
	public GameObject MenuParentObject, LoadSave, FoneLoad, LoadParentObject, OptionsParentObject, CreditsParentObject, SaveLoadParentObject;
	public Slider SliderMasterSound, SliderFXSound, SliderMusicSound, SliderGraph, SliderResolut;
	public Text TextTime;
	public AudioClip UnderLineClip, ClickClip;
	public Text[] ButtonsSaved;
	
	// Add reference to the AudioSource component
	private AudioSource _AudioSource;
	
	// Use this for initialization
	void Start () {
		// Get reference to the AudioSource component
		_AudioSource = GetComponent<AudioSource>();
		if (_AudioSource == null) {
			// Add AudioSource component if it doesn't exist
			_AudioSource = gameObject.AddComponent<AudioSource>();
		}
		
		//int NmSav = PlayerPrefs.GetInt ("NmSav");
		for (int i = 0; i < ButtonsSaved.Length; i++) {
			if(PlayerPrefs.GetInt ("Saved"+System.Convert.ToString(i)) != 0)
			{
				ButtonsSaved[i].text = "Saved: "+PlayerPrefs.GetString ("TimeSave"+System.Convert.ToString(i));
			}
		}
		
		SliderMasterSound.value = PlayerPrefs.GetFloat ("SliderSound");
		SliderGraph.value = PlayerPrefs.GetFloat ("SliderGraph");
		SliderResolut.value = PlayerPrefs.GetFloat ("SliderResolut");
		
		AudioListener.volume = PlayerPrefs.GetFloat ("SliderSound");
		QualitySettings.SetQualityLevel(System.Convert.ToInt32(PlayerPrefs.GetFloat ("SliderGraph")+1));
		if (PlayerPrefs.GetFloat ("SliderResolut") == 0) {
			Screen.SetResolution(800, 600, true);
		}
		if (PlayerPrefs.GetFloat ("SliderResolut") == 1) {
			Screen.SetResolution(1280, 720, true);
		}
		if (PlayerPrefs.GetFloat ("SliderResolut") == 2) {
			Screen.SetResolution(1600, 1900, true);
		}
		if (PlayerPrefs.GetFloat ("SliderResolut") == 3) {
			Screen.SetResolution(1920, 1080, true);
		}
	}
	
	// Update is called once per frame
	void Update () {
		TextTime.text = System.DateTime.Now.Hour + ":" + System.DateTime.Now.Minute;
	}
	
	// Method to play sound FX at a specific location with volume control
	public void playSoundFXClip(AudioClip audioClip, Transform spawnTransform, float volume) {
		if (audioClip == null) return;
		
		// Option 1: Use the existing AudioSource
		_AudioSource.clip = audioClip;
		_AudioSource.volume = volume;
		_AudioSource.Play();
		
		// Option 2: Create audio at position (uncomment if you want to spawn audio at the transform position)
		// AudioSource.PlayClipAtPoint(audioClip, spawnTransform.position, volume);
	}
	
	public void OnLoadLevel () {
		playSoundFXClip(ClickClip, transform, 1.0f);
		MenuParentObject.SetActive (false);
		SaveLoadParentObject.SetActive (false);
		CreditsParentObject.SetActive (false);
		OptionsParentObject.SetActive (false);
		playSoundFXClip(ClickClip, transform, 1.0f);
		MenuParentObject.SetActive (false);
		LoadParentObject.SetActive (true);
		FoneLoad.SetActive (true);
		// _LoadScene.isLoading = true;
		// PlayerPrefs.SetInt ("Load", 0);
		// _LoadScene.StartCoroutine("_Start");
	}
	
	public void OnOptions() {
		playSoundFXClip(ClickClip, transform, 1.0f);
		MenuParentObject.SetActive (false);
		OptionsParentObject.SetActive (true);
	}
	
	public void OnCredits() {
		playSoundFXClip(ClickClip, transform, 1.0f);
		MenuParentObject.SetActive (false);
		CreditsParentObject.SetActive (true);
	}
	
	public void OnExit () {
		playSoundFXClip(ClickClip, transform, 1.0f);
		Application.Quit ();
	}
	
	public void OnExitCredits () {
		Obvod.rectTransform.anchoredPosition = new Vector2 (0, 0);
		playSoundFXClip(ClickClip, transform, 1.0f);
		CreditsParentObject.SetActive (false);
		MenuParentObject.SetActive (true);
	}
	
	public void SaveOptions () {
		Obvod.rectTransform.anchoredPosition = new Vector2 (0, 0);
		PlayerPrefs.SetFloat ("SliderSound", SliderMasterSound.value);
		PlayerPrefs.SetFloat ("SliderGraph", SliderGraph.value);
		PlayerPrefs.SetFloat ("SliderResolut", SliderResolut.value);
		AudioListener.volume = PlayerPrefs.GetFloat ("SliderSound");
		QualitySettings.SetQualityLevel(System.Convert.ToInt32(PlayerPrefs.GetFloat ("SliderGraph")+1));
		if (PlayerPrefs.GetFloat ("SliderResolut") == 0) {
			Screen.SetResolution(800, 600, true);
		}
		if (PlayerPrefs.GetFloat ("SliderResolut") == 1) {
			Screen.SetResolution(1280, 720, true);
		}
		if (PlayerPrefs.GetFloat ("SliderResolut") == 2) {
			Screen.SetResolution(1600, 1900, true);
		}
		if (PlayerPrefs.GetFloat ("SliderResolut") == 3) {
			Screen.SetResolution(1920, 1080, true);
		}
		playSoundFXClip(ClickClip, transform, 1.0f);
		OptionsParentObject.SetActive (false);
		MenuParentObject.SetActive (true);
	}
	
	public void OnObvodka (Text _text) {
		Obvod.rectTransform.anchoredPosition = new Vector2 (_text.rectTransform.anchoredPosition.x, _text.rectTransform.anchoredPosition.y);
		playSoundFXClip(UnderLineClip, transform, 1.0f);
	}
	
	public void OnObvodka2 (Text _text) {
		Obvod2.rectTransform.anchoredPosition = new Vector2 (_text.rectTransform.anchoredPosition.x, _text.rectTransform.anchoredPosition.y);
		playSoundFXClip(UnderLineClip, transform, 1.0f);
	}
	
	public void OnSaveLoad () {
		playSoundFXClip(ClickClip, transform, 1.0f);
		MenuParentObject.SetActive (false);
		SaveLoadParentObject.SetActive (true);
	}
	
	public void ExitSaveLoad () {
		playSoundFXClip(ClickClip, transform, 1.0f);
		SaveLoadParentObject.SetActive (false);
		MenuParentObject.SetActive (true);
	}
	
	public void OnLoadGame (int num) {
		if (PlayerPrefs.GetInt ("Saved" + System.Convert.ToString (num)) != 0) {
			MenuParentObject.SetActive (false);
			SaveLoadParentObject.SetActive (false);
			CreditsParentObject.SetActive (false);
			OptionsParentObject.SetActive (false);
			PlayerPrefs.SetInt ("Load", 1);
			PlayerPrefs.SetInt ("NmSav", num);
			playSoundFXClip(ClickClip, transform, 1.0f);
			MenuParentObject.SetActive (false);
			LoadParentObject.SetActive (true);
			FoneLoad.SetActive (true);
			// _LoadScene.isLoading = true;
			// _LoadScene.StartCoroutine ("_Start");
		}
	}
}