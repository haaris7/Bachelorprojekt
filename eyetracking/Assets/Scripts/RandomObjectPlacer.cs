using System.Collections.Generic;
using UnityEngine;

public class RandomObjectPlacer : MonoBehaviour
{
    public GameObject[] parentObjects;
    public GameObject[] objectAreas;

    void Start()
    {
        // Loop through all parent objects
        for (int i = 0; i < parentObjects.Length; i++)
        {
            // Get all child objects of the current parent object
            Transform[] childObjects = parentObjects[i].GetComponentsInChildren<Transform>();

            // Loop through all child objects
            for (int j = 1; j < childObjects.Length; j++)
            {
                // Get a random position within the current ObjectArea
                Vector3 randomPosition = RandomPositionInObjectArea(objectAreas[i]);

                // Set the child object's position to the random position
                childObjects[j].position = randomPosition;
            }
        }
    }

    // Returns a random position within the bounds of the given ObjectArea
    Vector3 RandomPositionInObjectArea(GameObject objectArea)
    {
        Bounds bounds = objectArea.GetComponent<Renderer>().bounds;
        Vector3 randomPosition = new Vector3(
            Random.Range(bounds.min.x, bounds.max.x),
            Random.Range(bounds.min.y, bounds.max.y),
            Random.Range(bounds.min.z, bounds.max.z)
        );
        return randomPosition;
    }
}