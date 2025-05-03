using UnityEngine;
using UnityEngine.UI;
using AuroraFPSRuntime.SystemModules.HealthModules;

public class HealthHandler : MonoBehaviour
{
    public Slider healthSlider;
    public Slider easeHealthSlider;
    public float maxHealth = 100f;
    public float health;
    private float lerpSpeed = 0.05f;

    // Reference to the character's health
    public CharacterHealth characterHealth;

    void Start()
    {
        if (characterHealth != null)
        {
            maxHealth = characterHealth.GetMaxHealth();
            health = characterHealth.GetHealth();
        }
    }

    void Update()
    {
        if (characterHealth != null)
        {
            float characterCurrentHealth = characterHealth.GetHealth();
            if (characterCurrentHealth != health)
            {
                health = characterCurrentHealth;
            }
        }

        if (healthSlider.value != health)
        {
            healthSlider.value = health;
        }

        if (easeHealthSlider.value != health)
        {
            easeHealthSlider.value = Mathf.Lerp(easeHealthSlider.value, health, lerpSpeed);
        }
    }
}
