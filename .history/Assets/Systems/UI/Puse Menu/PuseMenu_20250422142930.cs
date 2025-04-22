using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class PuseMenu : MonoBehaviour {
	public static PuseMenu instance;
	public Image Obvod;
	public GameObject MenuParentObject, FoneLoad, LoadParentObject, OptionsParentObject, CreditsParentObject;
	public Slider SliderMasterSound, SliderFXSound, SliderMusicSound, SliderGraph, SliderResolut;
	public AudioClip UnderLineClip, ClickClip;
    public Text LoadingText;
	public Text[] ButtonsSaved;
	

	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
			DontDestroyOnLoad(gameObject); // this is fine now
			Debug.Log("PuseMenu Awake called");
		}
		else
		{
			Destroy(gameObject);
		}
	}



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
		//save progress before exit
		SaveLoadManager.instance.SaveCurrentProgress();
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

	
    public void OnSaveGame () {
		SaveLoadManager.instance.SaveCurrentProgress();
	}
	
	

    public void OnGoToMainMenu()
    {
        SoundFXManager.instance.playSoundFXClip(ClickClip, transform, 1f);

        MenuParentObject.SetActive(false);
        CreditsParentObject.SetActive(false);
        OptionsParentObject.SetActive(false);
        LoadParentObject.SetActive(true);
        FoneLoad.SetActive(true);

        StartCoroutine(LoadAsync(0));
    }

    private IEnumerator LoadAsync(int sceneIndex)
    {
        yield return new WaitForSecondsRealtime(0.5f);

        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(sceneIndex);
        loadOperation.allowSceneActivation = false;

        while (loadOperation.progress < 0.9f)
        {
            float progress = Mathf.Clamp01(loadOperation.progress / 0.9f);
            int percent = Mathf.RoundToInt(progress * 100f);
            LoadingText.text = "Loading... " + percent + "%";

            yield return null; // This is fine to leave
        }

        LoadingText.text = "Loading... 100%";
        yield return new WaitForSecondsRealtime(0.5f);

        loadOperation.allowSceneActivation = true;
    }

	public void LoadNextLevel(){
		Awake();
		MenuParentObject.SetActive(false);
        CreditsParentObject.SetActive(false);
        OptionsParentObject.SetActive(false);
        LoadParentObject.SetActive(true);
        FoneLoad.SetActive(true);
		StartCoroutine(LoadAsync(2));
	}


}