using UnityEngine;

public class ScatterSpheres : MonoBehaviour
{
    public Vector3 minPosition = new Vector3(-10, -10, -10);
    public Vector3 maxPosition = new Vector3(10, 10, 10);

    void Start()
    {
        // Loop through each child of this GameObject
        for (int i = 0; i < transform.childCount; i++)
        {
            // Get the i-th child GameObject
            Transform child = transform.GetChild(i);

            // Calculate a random position within the specified range
            Vector3 randomPosition = new Vector3(
                Random.Range(minPosition.x, maxPosition.x),
                Random.Range(minPosition.y, maxPosition.y),
                Random.Range(minPosition.z, maxPosition.z));

            // Set the child's position to the calculated random position
            child.position = randomPosition;
        }
    }
}