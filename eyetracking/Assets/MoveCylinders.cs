using UnityEngine;

public class MoveCylinders : MonoBehaviour
{
    public float minSpeed = 1f;
    public float maxSpeed = 5f;
    public int repeatTimes = 3; // Number of times to repeat
    private int currentRepeat = 0; // Current repeat count
    private int completedCylinders = 0; // Number of cylinders that have completed their movement
    private Vector3[] originalPositions; // Array to store original positions

    void Start()
    {
        // Initialize original positions array
        originalPositions = new Vector3[transform.childCount];

        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            originalPositions[i] = child.position;

            // Make sure the child has a Rigidbody component
            Rigidbody rb = child.GetComponent<Rigidbody>();
            if (rb == null)
            {
                Debug.LogError("Child object does not have a Rigidbody component", child);
                continue;
            }

            // Give the cylinder a random velocity towards -5 in the Z axis
            float speed = Random.Range(minSpeed, maxSpeed);
            rb.velocity = new Vector3(0, 0, -5) * speed;
        }
    }

    void Update()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);

            // Check if cylinder has reached -5 in Z axis
            if (child.position.z <= -5)
            {
                // Reset position of cylinder
                child.position = originalPositions[i];

                // Increment completed cylinders count
                completedCylinders++;

                // Check if all cylinders have completed their movement
                if (completedCylinders >= transform.childCount)
                {
                    // Increment repeat count
                    currentRepeat++;

                    // Reset completed cylinders count
                    completedCylinders = 0;
                }

                // Check if repeat limit has been reached
                if (currentRepeat >= repeatTimes)
                {
                    // Stop movement of cylinder
                    Rigidbody rb = child.GetComponent<Rigidbody>();
                    rb.velocity = Vector3.zero;
                }
                else
                {
                    // Give the cylinder a random velocity towards -5 in the Z axis
                    float speed = Random.Range(minSpeed, maxSpeed);
                    Rigidbody rb = child.GetComponent<Rigidbody>();
                    rb.velocity = new Vector3(0, 0, -5) * speed;
                }
            }
        }
    }
}