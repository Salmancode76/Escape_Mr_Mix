using UnityEngine;
using UnityEngine.SceneManagement;

public class ReturnToMainMenu : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Layer index assigned to the Player")]
    [SerializeField] private int playerLayer = 8;

    [Tooltip("Index of the scene to load (0 = first scene in Build Settings)")]
    [SerializeField] private int sceneIndex = 0;

    private void OnTriggerEnter(Collider other)
    {
        // Only respond to objects on the Player layer
        if (other.gameObject.layer != playerLayer)
            return;

        // Transfer the player to the specified scene
        SceneManager.LoadScene(sceneIndex);
    }
}
