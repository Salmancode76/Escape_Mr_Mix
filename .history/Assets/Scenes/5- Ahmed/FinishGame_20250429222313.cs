using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishGame : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Layer index assigned to the Player")]
    [SerializeField] private int playerLayer = 8;

    [Tooltip("Index of the scene to load (0 = first scene in Build Settings)")]
    [SerializeField] private int sceneIndex = 0;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != playerLayer)
            return;

        // show cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // load main menu scene
        SceneManager.LoadScene(sceneIndex);
    }
}
