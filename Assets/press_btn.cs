using UnityEngine;

public class press_btn : MonoBehaviour
{
    public Light pointLight;             // Assign in Inspector
    public float interactionRange = 5f;  // Interaction distance
    public Camera playerCamera;          // Assign the player's camera

    void Start()
    {
        if (pointLight != null)
        {
            pointLight.enabled = false; // Make sure light starts off
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
            RaycastHit hit;

            // Check if we're looking at something
            if (Physics.Raycast(ray, out hit, interactionRange))
            {
                Debug.Log("Looking at: " + hit.transform.name);

                // Check if we're looking at THIS object
                if (hit.transform == this.transform)
                {
                    Debug.Log("PRESSED");

                    if (pointLight != null)
                    {
                        pointLight.enabled = !pointLight.enabled; // Toggle the light
                    }
                }
            }
        }
    }
}
