using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSController : MonoBehaviour
{
    private float fps;
    public UnityEngine.UI.Text FPSCounterText;

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("GetFPS", 1,1);
    }

    void GetFPS(){
        fps = (int)(1f / Time.unscaledDeltaTime);
        FPSCounterText.text = "Fps " + fps.ToString();
    }
    
}
