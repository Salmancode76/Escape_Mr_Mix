using System.Collections;
using UnityEngine;
using AuroraFPSRuntime.SystemModules.HealthModules;  // for ObjectHealth

public class GoHomeGhassanGameEnded : MonoBehaviour
{
    [Header("References")]
    [Tooltip("The Mix enemy's health component")]
    [SerializeField] private ObjectHealth mixObjectHealth;
    [Tooltip("UI GameObject showing Mix health")]
    [SerializeField] private GameObject healthUI;
    [Tooltip("Door that rotates on Z axis")]
    [SerializeField] private GameObject doorOne;
    [Tooltip("Door that rotates on Y axis")]
    [SerializeField] private GameObject doorTwo;

    [Header("Door Rotation Settings")]
    [SerializeField] private float doorOneTargetZ = 60f;
    [SerializeField] private float doorTwoTargetY = 120f;
    [SerializeField] private float rotationSpeed = 30f;

    [Header("Audio Clips")]
    [SerializeField] private AudioClip lockOpenClip;
    [SerializeField] private AudioClip doorOpenClip;

    private bool hasOpened = false;

    void Start()
    {
        // If you forgot to assign in inspector, try to grab it here
        if (mixObjectHealth == null)
        {
            mixObjectHealth = GetComponent<ObjectHealth>();
            if (mixObjectHealth == null)
            {
                Debug.LogError("MixDeathDoorOpener: No ObjectHealth found!");
                enabled = false;
                return;
            }
        }
    }

    void Update()
    {
        if (hasOpened) 
            return;

        // Check health via ObjectHealth.GetHealth()
        if (mixObjectHealth.GetHealth() <= 0f)
        {
            StartCoroutine(OpenDoorsSequence());
            hasOpened = true;
        }
    }

    private IEnumerator OpenDoorsSequence()
    {
        // 1) disable the health UI immediately
        if (healthUI != null)
            healthUI.SetActive(false);

        // 2) play the “lock open” sound
        if (SoundFXManager.instance != null && lockOpenClip != null)
            SoundFXManager.instance.playSoundFXClip(lockOpenClip, transform, 1f);

        // wait a bit before door creak
        yield return new WaitForSeconds(0.6f);

        // 3) play the “door open” sound
        if (SoundFXManager.instance != null && doorOpenClip != null)
            SoundFXManager.instance.playSoundFXClip(doorOpenClip, transform, 1f);

        // 4) over successive frames, rotate both doors until they hit targets
        while (!DoorsAtTarget())
        {
            RotateDoor(doorOne, Axis.Z, doorOneTargetZ);
            RotateDoor(doorTwo, Axis.Y, doorTwoTargetY);
            yield return null;
        }
    }

    private enum Axis { X, Y, Z }

    private void RotateDoor(GameObject door, Axis axis, float targetAngle)
    {
        if (door == null) return;
        Vector3 e = door.transform.localEulerAngles;
        float step = rotationSpeed * Time.deltaTime;
        switch (axis)
        {
            case Axis.Z:
                e.z = Mathf.MoveTowardsAngle(e.z, targetAngle, step);
                break;
            case Axis.Y:
                e.y = Mathf.MoveTowardsAngle(e.y, targetAngle, step);
                break;
        }
        door.transform.localEulerAngles = e;
    }

    private bool DoorsAtTarget()
    {
        if (doorOne == null || doorTwo == null) return true;
        float z = doorOne.transform.localEulerAngles.z;
        float y = doorTwo.transform.localEulerAngles.y;
        bool d1 = Mathf.Abs(Mathf.DeltaAngle(z, doorOneTargetZ)) < 0.5f;
        bool d2 = Mathf.Abs(Mathf.DeltaAngle(y, doorTwoTargetY)) < 0.5f;
        return d1 && d2;
    }
}
