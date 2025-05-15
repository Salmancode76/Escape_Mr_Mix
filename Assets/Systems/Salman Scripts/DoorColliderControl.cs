using UnityEngine;

public class DoorColliderControl : MonoBehaviour
{
    public Collider doorCollider;

    public void DisableCollider()
    {
        if (doorCollider != null)
        {
            doorCollider.enabled = false;
        }
    }

    public void EnableCollider()
    {
        if (doorCollider != null)
        {
            doorCollider.enabled = true;
        }
    }
}
