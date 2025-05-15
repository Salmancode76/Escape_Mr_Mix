using UnityEngine;
using TMPro;

public class press_btn : MonoBehaviour
{
    public Light pointLight;
    public Renderer bulbRenderer;
    public Material onMaterial;
    public Material offMaterial;
    public GameObject player;
    public TextMeshProUGUI promptText;
    public float interactDistance = 2f;
    public AudioClip clickSound;
    public float buttonPressDepth = 0.03f;
    public float buttonPressSpeed = 5f;
    public string lightColorName = "red"; // specify color name (red, green, blue)

    private Vector3 originalPosition;
    private Vector3 pressedPosition;
    private bool isMovingDown = false;
    private Camera mainCam;
    private bool isPromptActive = false;

    void Start()
    {
        if (player == null)
            player = GameObject.Find("New Ghassan Prefab");

        mainCam = Camera.main;
        originalPosition = transform.localPosition;
        pressedPosition = originalPosition - new Vector3(0, buttonPressDepth, 0);

        if (pointLight != null)
            pointLight.enabled = false;

        if (promptText != null)
            promptText.gameObject.SetActive(false);

        UpdateBulbMaterial();
    }

    void Update()
    {
        if (player == null || mainCam == null)
            return;

        float distance = Vector3.Distance(player.transform.position, transform.position);

        if (distance <= interactDistance && IsLookingAtButton())
        {
            if (!isPromptActive && promptText != null)
            {
                promptText.gameObject.SetActive(true);
                UpdatePromptColor();
                isPromptActive = true;
            }

            UpdatePromptText();

            if (Input.GetKeyDown(KeyCode.E))
            {
                PressButton(); // ✅ Press the button (light on for 2 seconds)
                AnimateButtonPress();
            }
        }
        else
        {
            if (isPromptActive && promptText != null)
            {
                promptText.gameObject.SetActive(false);
                isPromptActive = false;
            }
        }

        if (isMovingDown)
            transform.localPosition = Vector3.Lerp(transform.localPosition, pressedPosition, Time.deltaTime * buttonPressSpeed);
        else
            transform.localPosition = Vector3.Lerp(transform.localPosition, originalPosition, Time.deltaTime * buttonPressSpeed);
    }

    bool IsLookingAtButton()
    {
        Ray ray = new Ray(mainCam.transform.position, mainCam.transform.forward);
        Debug.DrawRay(ray.origin, ray.direction * interactDistance, Color.red);

        if (Physics.SphereCast(ray, 0.3f, out RaycastHit hit, interactDistance))
        {
            return hit.transform == transform;
        }
        return false;
    }

    void PressButton()
    {
        if (pointLight != null)
        {
            pointLight.enabled = true;
            UpdateBulbMaterial();
            UpdatePromptText();
            Invoke(nameof(AutoTurnOffLight), 1f); // ✅ Light off after 2 seconds automatically
        }

        if (clickSound != null)
            AudioSource.PlayClipAtPoint(clickSound, transform.position);

        if (MemoryGameManager.instance != null)
        {
            MemoryGameManager.instance.PlayerPressedButton(this);
        }
    }

    void AutoTurnOffLight()
    {
        if (pointLight != null)
            pointLight.enabled = false;

        UpdateBulbMaterial();
        UpdatePromptText();
    }

    void UpdatePromptText()
    {
        if (promptText != null)
        {
            string formattedColorName = CapitalizeFirstLetter(lightColorName);

            if (pointLight.enabled)
                promptText.text = $"Press E to turn OFF {formattedColorName} light";
            else
                promptText.text = $"Press E to turn ON {formattedColorName} light";
        }
    }

    void UpdatePromptColor()
    {
        if (promptText != null)
        {
            promptText.color = GetColorFromName(lightColorName);
        }
    }

    public bool IsLightInState(bool shouldBeOn)
    {
        return pointLight != null && pointLight.enabled == shouldBeOn;
    }

    Color GetColorFromName(string colorName)
    {
        switch (colorName.ToLower())
        {
            case "red": return Color.red;
            case "green": return new Color(0f, 1f, 0f); // bright green
            case "blue": return Color.blue;
            case "yellow": return Color.yellow;
            case "white": return Color.white;
            case "cyan": return Color.cyan;
            case "magenta": return Color.magenta;
            default: return Color.white;
        }
    }

    string CapitalizeFirstLetter(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;
        return char.ToUpper(input[0]) + input.Substring(1).ToLower();
    }

    public void UpdateBulbMaterial()
    {
        if (bulbRenderer != null)
        {
            if (pointLight.enabled)
            {
                if (onMaterial != null)
                    bulbRenderer.material = onMaterial;

                Color onColor = GetColorFromName(lightColorName);
                bulbRenderer.material.color = onColor;
                bulbRenderer.material.SetColor("_EmissionColor", onColor);
            }
            else
            {
                if (offMaterial != null)
                    bulbRenderer.material = offMaterial;

                Color offColor = new Color(0.3f, 0.3f, 0.3f); // dark gray
                bulbRenderer.material.color = offColor;
                bulbRenderer.material.SetColor("_EmissionColor", offColor);
            }
        }
    }

    void AnimateButtonPress()
    {
        isMovingDown = true;
        Invoke(nameof(ResetButton), 0.2f);
    }

    void ResetButton()
    {
        isMovingDown = false;
    }
}
