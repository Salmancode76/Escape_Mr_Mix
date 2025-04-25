using UnityEngine;

public class ReplaceWalls : MonoBehaviour
{
    public GameObject newWallPrefab;

    void Start()
    {
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();

        foreach (GameObject obj in allObjects)
        {
            if (obj.name.Contains("wall_01_m")) // or whatever pattern all your old walls use
            {
                Vector3 pos = obj.transform.position;
                Quaternion rot = obj.transform.rotation;
                Transform parent = obj.transform.parent;

                DestroyImmediate(obj); // replace in editor instantly
                GameObject newWall = Instantiate(newWallPrefab, pos, rot, parent);
            }
        }

        Debug.Log("✅ Old walls replaced!");
    }
}
