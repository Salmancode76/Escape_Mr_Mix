using UnityEngine;
using AuroraFPSRuntime.SystemModules.HealthModules;

public class HealthBridge : MonoBehaviour
{
    [SerializeField] private HealthHandller healthHandler;
    private ObjectHealth objectHealth; // Using ObjectHealth as base class to be more generic
    private float previousHealth;
    private float previousMaxHealth;
    private bool hasPrintedDeadMessage = false;

    void Start()
    {
        // Try to get any type of health component
        objectHealth = GetComponent<ObjectHealth>();
        
        if (objectHealth == null)
        {
            Debug.LogError("No ObjectHealth component found on this GameObject!");
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
        previousHealth = objectHealth.GetHealth();
        previousMaxHealth = objectHealth.GetMaxHealth();
        
        healthHandler.health = previousHealth;
        healthHandler.maxHealth = previousMaxHealth;
        
        // Initialize UI sliders
        if (healthHandler.healthSlider != null)
        {
            healthHandler.healthSlider.maxValue = previousMaxHealth;
            healthHandler.healthSlider.value = previousHealth;
        }
        
        if (healthHandler.easeHealthSlider != null)
        {
            healthHandler.easeHealthSlider.maxValue = previousMaxHealth;
            healthHandler.easeHealthSlider.value = previousHealth;
        }
        
        // Reset the dead message flag when starting
        hasPrintedDeadMessage = false;
    }

    void Update()
    {
        if (objectHealth == null || healthHandler == null) return;

        // Check if health has changed
        float currentHealth = objectHealth.GetHealth();
        if (currentHealth != previousHealth)
        {
            // Update the health handler with the new health value
            healthHandler.health = currentHealth;
            previousHealth = currentHealth;
            
            // Check if health reached zero and print "dead" message
            if (currentHealth <= 0 && !hasPrintedDeadMessage)
            {
                Debug.Log("Dead");
                hasPrintedDeadMessage = true;
            }
            // Reset flag if health is above zero again (revived)
            else if (currentHealth > 0 && hasPrintedDeadMessage)
            {
                hasPrintedDeadMessage = false;
            }
        }
        
        // Also check if max health changed
        float currentMaxHealth = objectHealth.GetMaxHealth();
        if (currentMaxHealth != previousMaxHealth)
        {
            healthHandler.maxHealth = currentMaxHealth;
            previousMaxHealth = currentMaxHealth;
            
            // Update slider max values if needed
            if (healthHandler.healthSlider != null)
                healthHandler.healthSlider.maxValue = currentMaxHealth;
                
            if (healthHandler.easeHealthSlider != null)
                healthHandler.easeHealthSlider.maxValue = currentMaxHealth;
        }
    }
}