using System.Collections;
using UnityEngine;

public class StartBossFight : MonoBehaviour
{
    [Header("References")]
    [SerializeField] GameObject playerController;
    [SerializeField] GameObject cutScenePlayer;
    [SerializeField] GameObject mainCamera;
    [SerializeField] GameObject cutSceneCamera;
    [SerializeField] GameObject cutsceneMixEnemy;
    [SerializeField] GameObject mixEnemy;
    [SerializeField] GameObject bossUI;

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
        // Door sound and visuals
        if (SoundFXManager.instance != null && doorCloseClip != null)
        {
            SoundFXManager.instance.playSoundFXClip(doorCloseClip, transform, 1f);
        }
        else
        {
            Debug.LogWarning("Missing SoundFXManager or doorCloseClip.");
        }

        openedDoor.SetActive(false);
        closedDoor.SetActive(true);

        // Cutscene setup
        playerController.SetActive(false);
        cutScenePlayer.SetActive(true);
        cutsceneMixEnemy.SetActive(true);
        mainCamera.SetActive(false);
        cutSceneCamera.SetActive(true);

        yield return new WaitForSeconds(sfxDelay);

        if (SoundFXManager.instance != null && bossIntroClip != null)
        {
            SoundFXManager.instance.playSoundFXClip(bossIntroClip, transform, 1f);
        }
        else
        {
            Debug.LogWarning("Missing SoundFXManager or bossIntroClip.");
        }

        yield return new WaitForSeconds(cutsceneDuration);

        // Cutscene ends
        cutScenePlayer.SetActive(false);
        cutsceneMixEnemy.SetActive(false);
        cutSceneCamera.SetActive(false);
        mainCamera.SetActive(true);

        playerController.SetActive(true);
        mixEnemy.SetActive(true);

        // Show UI after cutscene
        if (bossUI != null)
            bossUI.SetActive(true);
    }
}
