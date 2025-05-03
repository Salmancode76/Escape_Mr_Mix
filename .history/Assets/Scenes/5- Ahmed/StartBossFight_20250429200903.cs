using System.Collections;
using UnityEngine;

public class StartBossFight : MonoBehaviour
{
    [Header("References")]
    [SerializeField] MonoBehaviour playerController;
    [SerializeField] GameObject cutScenePlayer;
    [SerializeField] Camera mainCamera;
    [SerializeField] Camera cutSceneCamera;
    [SerializeField] GameObject cutsceneMixEnemy;
    [SerializeField] GameObject mixEnemy;

    [Header("Door Setup")]
    [SerializeField] GameObject openedDoor;
    [SerializeField] GameObject closedDoor;
    [SerializeField] AudioClip doorCloseClip;

    [Header("Audio")]
    [SerializeField] AudioClip bossIntroClip;

    [SerializeField] int playerLayer = 8;
    const float sfxDelay = 1f;
    const float cutsceneDuration = 3f;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != playerLayer) return;

        StartCoroutine(PlayCutsceneAndStartBoss());
        GetComponent<Collider>().enabled = false;
    }

    IEnumerator PlayCutsceneAndStartBoss()
    {
        // --- Door logic ---
        SoundFXManager.instance.playSoundFXClip(doorCloseClip, transform, 1f);
        openedDoor.SetActive(false);
        closedDoor.SetActive(true);

        // --- Disable player control ---
        playerController.enabled = false;

        // --- Activate cutscene characters ---
        cutScenePlayer.SetActive(true);
        cutsceneMixEnemy.SetActive(true);

        // --- Switch camera ---
        mainCamera.enabled = false;
        cutSceneCamera.enabled = true;

        // --- Delay before boss SFX ---
        yield return new WaitForSeconds(sfxDelay);
        SoundFXManager.instance.playSoundFXClip(bossIntroClip, transform, 1f);

        // --- Wait for cutscene to finish (fixed time) ---
        yield return new WaitForSeconds(cutsceneDuration);

        // --- End cutscene ---
        cutScenePlayer.SetActive(false);
        cutsceneMixEnemy.SetActive(false);
        cutSceneCamera.enabled = false;
        mainCamera.enabled = true;

        // --- Re-enable gameplay ---
        playerController.enabled = true;
        mixEnemy.SetActive(true);
    }
}
