using UnityEngine;

public class MoveCylinders : MonoBehaviour
{
    public float minSpeed = 1f;
    public float maxSpeed = 5f;

    void Start()
    {
        foreach (Transform child in transform)
        {
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
}
