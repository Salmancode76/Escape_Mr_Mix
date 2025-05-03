using UnityEngine;
using AuroraFPSRuntime.SystemModules.HealthModules;

public class HealthBridge : MonoBehaviour
{
    [SerializeField] private HealthHandller healthHandler;
    private CharacterHealth characterHealth;
    private float previousHealth;

    void Start()
    {
        // Get the CharacterHealth component (or AICharacterHealth which inherits from it)
        characterHealth = GetComponent<CharacterHealth>();
        
        if (characterHealth == null)
        {
            Debug.LogError("No CharacterHealth component found on this GameObject!");
            enabled = false;
            return;
        }

        if (healthHandler == null)
        {
            Debug.LogError("HealthHandler reference is not assigned!");
            enabled = false;
            return;
        }

        // Initialize health values
        previousHealth = characterHealth.GetHealth();
        healthHandler.health = previousHealth;
        healthHandler.maxHealth = characterHealth.GetMaxHealth();
        
        // Initialize UI sliders
        healthHandler.healthSlider.maxValue = characterHealth.GetMaxHealth();
        healthHandler.easeHealthSlider.maxValue = characterHealth.GetMaxHealth();
    }

    void Update()
    {
        if (characterHealth == null || healthHandler == null) return;

        // Check if health has changed
        float currentHealth = characterHealth.GetHealth();
        if (currentHealth != previousHealth)
        {
            // Update the health handler with the new health value
            healthHandler.health = currentHealth;
            previousHealth = currentHealth;
        }
    }
}