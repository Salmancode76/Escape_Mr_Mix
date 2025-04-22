using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Menu : MonoBehaviour {
	public Image Obvod, Obvod2, Obvod3;
	public GameObject MenuParentObject, LoadSave, FoneLoad, LoadParentObject, OptionsParentObject, CreditsParentObject, SaveLoadParentObject;
	public Slider SliderMasterSound, SliderFXSound, SliderMusicSound, SliderGraph, SliderResolut;
	public Text TextTime, TextMR_MIX, TextEscape;
	public AudioClip UnderLineClip, ClickClip;
	public Text[] ButtonsSaved;
	public Text[] LoadSaved;
	
	// Use this for initialization
	void Start () {
		
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

		float red = Mathf.PingPong(Time.time * (100f / 3f), 100f) + 100f;
		Color newColor = new Color(red / 255f, TextMR_MIX.color.g, TextMR_MIX.color.b);
		TextMR_MIX.color = newColor;
	}
	
	public void OnNewGamePick(int slotIndex) {
		SoundFXManager.instance.playSoundFXClip(ClickClip, transform, 1f);

		if (PlayerPrefs.GetInt("Saved" + slotIndex) == 0) {
			// Save first level index directly (not main menu)
			PlayerPrefs.SetInt("SceneIndex" + slotIndex, 1);
			PlayerPrefs.SetString("TimeSave" + slotIndex, System.DateTime.Now.ToString("HH:mm"));
			PlayerPrefs.SetInt("Saved" + slotIndex, 1);

			// Save selected slot for future saves
			PlayerPrefs.SetInt("CurrentSlot", slotIndex);

			// Load first level
			SaveLoadManager.instance.LoadSceneAsync(1); 
		} else {
			OnBack(); // Slot already used
		}
	}


	public void OnLoadGame(int slotIndex) {
		SoundFXManager.instance.playSoundFXClip(ClickClip, transform, 1f);

		if (PlayerPrefs.GetInt("Saved" + slotIndex) != 0) {
			// Show loading UI
			MenuParentObject.SetActive(false);
			SaveLoadParentObject.SetActive(false);
			LoadSave.SetActive(false);
			CreditsParentObject.SetActive(false);
			OptionsParentObject.SetActive(false);
			LoadParentObject.SetActive(true);
			FoneLoad.SetActive(true);

			SaveLoadManager.instance.LoadGame(slotIndex); // Load saved scene
		} else {
			OnBack(); // No saved file, go back
		}
	}

	
	public void OnOptions() {
		SoundFXManager.instance.playSoundFXClip(ClickClip, transform, 1f);
		MenuParentObject.SetActive (false);
		OptionsParentObject.SetActive (true);
		TextEscape.enabled = false;
		TextMR_MIX.enabled = false;
	}
	
	public void OnCredits() {
		SoundFXManager.instance.playSoundFXClip(ClickClip, transform, 1f);
		MenuParentObject.SetActive (false);
		CreditsParentObject.SetActive (true);
		TextEscape.enabled = false;
		TextMR_MIX.enabled = false;
	}
	
	public void OnExit() {
    	SoundFXManager.instance.playSoundFXClip(ClickClip, transform, 1f);
		//if played on editor will also close
		#if UNITY_EDITOR
			UnityEditor.EditorApplication.isPlaying = false; 
		#else
			Application.Quit(); 
		#endif
	}
	
	public void OnBack () {
		SoundFXManager.instance.playSoundFXClip(ClickClip, transform, 1f);
		

		SaveLoadParentObject.SetActive (false);
		CreditsParentObject.SetActive (false);
		OptionsParentObject.SetActive (false);
		LoadSave.SetActive (false);

		Obvod.rectTransform.anchoredPosition = new Vector2 (0, 0);
		MenuParentObject.SetActive (true);
		TextEscape.enabled = true;
		TextMR_MIX.enabled = true;
	
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
		SoundFXManager.instance.playSoundFXClip(ClickClip, transform, 1f);
		OptionsParentObject.SetActive (false);
		MenuParentObject.SetActive (true);
		TextEscape.enabled = true;
		TextMR_MIX.enabled = true;
	}
	
	public void OnObvodka (Text _text) {
		Obvod.rectTransform.anchoredPosition = new Vector2 (_text.rectTransform.anchoredPosition.x, _text.rectTransform.anchoredPosition.y);
		SoundFXManager.instance.playSoundFXClip(UnderLineClip, transform, 1f);
	}
	
	public void OnObvodka2 (Text _text) {
		Obvod2.rectTransform.anchoredPosition = new Vector2 (_text.rectTransform.anchoredPosition.x, _text.rectTransform.anchoredPosition.y);
		SoundFXManager.instance.playSoundFXClip(UnderLineClip, transform, 1f);
	}

	public void OnObvodka3 (Text _text) {
		Obvod3.rectTransform.anchoredPosition = new Vector2 (_text.rectTransform.anchoredPosition.x, _text.rectTransform.anchoredPosition.y);
		SoundFXManager.instance.playSoundFXClip(UnderLineClip, transform, 1f);
	}
	
	void UpdateSaveSlotDisplay() {
		for (int i = 0; i < ButtonsSaved.Length; i++) {
			if (PlayerPrefs.GetInt("Saved" + i) == 1) {
				string time = PlayerPrefs.GetString("TimeSave" + i);
				ButtonsSaved[i].text = "Saved: " + time;
			} else {
				ButtonsSaved[i].text = "Empty Save";
			}
		}
	}

	void UpdateLoadSlotDisplay() {
		for (int i = 0; i < LoadSaved.Length; i++) {
			if (PlayerPrefs.GetInt("Saved" + i) == 1) {
				string time = PlayerPrefs.GetString("TimeSave" + i);
				LoadSaved[i].text = "Saved: " + time;
			} else {
				LoadSaved[i].text = "Empty Save";
			}
		}
	}

	public void OnSaveLoad () {
		SoundFXManager.instance.playSoundFXClip(ClickClip, transform, 1f);
		MenuParentObject.SetActive (false);
		SaveLoadParentObject.SetActive (true);
		TextEscape.enabled = false;
		TextMR_MIX.enabled = false;
		UpdateSaveSlotDisplay();
	}

	public void OnLoadSave () {
		SoundFXManager.instance.playSoundFXClip(ClickClip, transform, 1f);
		MenuParentObject.SetActive (false);
		LoadSave.SetActive (true);
		TextEscape.enabled = false;
		TextMR_MIX.enabled = false;
		UpdateLoadSlotDisplay();
	}
	
}