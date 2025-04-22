using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class PuseMenu : MonoBehaviour {
	public Image Obvod;
	public GameObject MenuParentObject, FoneLoad, LoadParentObject, OptionsParentObject, CreditsParentObject;
	public Slider SliderMasterSound, SliderFXSound, SliderMusicSound, SliderGraph, SliderResolut;
	public AudioClip UnderLineClip, ClickClip;
	public Text[] ButtonsSaved;
	
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
		
	}
	
	
	public void OnOptions() {
		SoundFXManager.instance.playSoundFXClip(ClickClip, transform, 1f);
		MenuParentObject.SetActive (false);
		OptionsParentObject.SetActive (true);

	}
	
	public void OnCredits() {
		SoundFXManager.instance.playSoundFXClip(ClickClip, transform, 1f);
		MenuParentObject.SetActive (false);
		CreditsParentObject.SetActive (true);

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
		CreditsParentObject.SetActive (false);
		OptionsParentObject.SetActive (false);
		Obvod.rectTransform.anchoredPosition = new Vector2 (0, 0);
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
		SoundFXManager.instance.playSoundFXClip(ClickClip, transform, 1f);
		OptionsParentObject.SetActive (false);
		MenuParentObject.SetActive (true);
	}
	
	public void OnObvodka (Text _text) {
		Obvod.rectTransform.anchoredPosition = new Vector2 (_text.rectTransform.anchoredPosition.x, _text.rectTransform.anchoredPosition.y);
		SoundFXManager.instance.playSoundFXClip(UnderLineClip, transform, 1f);
	}

    public void OnSaveGame (int num) {
		
	}
	
	

    public void OnGoToMainMenu()
    {
        SoundFXManager.instance.playSoundFXClip(ClickClip, transform, 1f);

        // Show loading screen
        MenuParentObject.SetActive(false);
        CreditsParentObject.SetActive(false);
        OptionsParentObject.SetActive(false);
        LoadParentObject.SetActive(true);
        FoneLoad.SetActive(true);

        // Start loading scene
        StartCoroutine(LoadMainMenuAsync());
    }

    private IEnumerator LoadMainMenuAsync()
    {
        yield return new WaitForSeconds(1f); // Optional delay before loading

        AsyncOperation loadOperation = SceneManager.LoadSceneAsync("MainMenu");

        // Optional: wait until scene is fully loaded
        while (!loadOperation.isDone)
        {
            yield return null;
        }
    }

}