using System.Collections.Generic;
using UnityEngine;

public class EyeTracker : MonoBehaviour
{
    public GameObject leftEye;
    public GameObject rightEye;
    public List<GameObject> gazeTargets = new List<GameObject>();
    private LineRenderer lineRenderer;

    private GameObject currentGazeTarget;

    [Range(0.0f, 1.0f)]
    public float blendFactor = 0.5f;

    [SerializeField]
    private LayerMask gazeLayerMask;
    [SerializeField]
    private float gazeDistance = 10f;


    private void Start()
    {
    lineRenderer = gameObject.AddComponent<LineRenderer>();
    lineRenderer.startWidth = 0.01f; // Set the start width of the line
    lineRenderer.endWidth = 0.01f; // Set the end width of the line
    lineRenderer.positionCount = 2; // Set the number of positions to 2 (start and end points)

    // Optional: Set the material and color for the line
    lineRenderer.material = new Material(Shader.Find("Standard"));
    lineRenderer.startColor = Color.red;
    lineRenderer.endColor = Color.red;
    }

    private void Update()
    {
        Vector3 averageGazeTargetsPosition = GetAverageGazeTargetsPosition();

        Vector3 leftEyeToTarget = averageGazeTargetsPosition - leftEye.transform.position;
        Vector3 rightEyeToTarget = averageGazeTargetsPosition - rightEye.transform.position;

        Quaternion leftEyeRotation = Quaternion.LookRotation(leftEyeToTarget);
        Quaternion rightEyeRotation = Quaternion.LookRotation(rightEyeToTarget);

        leftEye.transform.rotation = Quaternion.Slerp(leftEye.transform.rotation, leftEyeRotation, blendFactor);
        rightEye.transform.rotation = Quaternion.Slerp(rightEye.transform.rotation, rightEyeRotation, blendFactor);

        CheckGaze();
    }

    private Vector3 GetAverageGazeTargetsPosition()
    {
        Vector3 averagePosition = Vector3.zero;

        if (gazeTargets.Count == 0)
        {
            return averagePosition;
        }

        foreach (GameObject target in gazeTargets)
        {
            averagePosition += target.transform.position;
        }

        averagePosition /= gazeTargets.Count;
        return averagePosition;
    }

private void CheckGaze()
{
    Debug.Log("Checking gaze...");
    Vector3 eyeCenterPosition = (leftEye.transform.position + rightEye.transform.position) / 2f;
    Vector3 gazeDirection = (leftEye.transform.forward + rightEye.transform.forward) / 2f; // Updated line

    //Vector3 gazeDirection = transform.forward;

    Ray ray = new Ray(eyeCenterPosition, gazeDirection);
    RaycastHit hit;

    if (Physics.Raycast(ray, out hit, gazeDistance, gazeLayerMask))
    {
        GameObject hitObject = hit.collider.gameObject;

        if (gazeTargets.Contains(hitObject))
        {
            Debug.Log("Raycast hit: " + hitObject.name + " at distance: " + hit.distance + " at point: " + hit.point);

            if (currentGazeTarget != hitObject)
            {
                if (currentGazeTarget != null)
                {
                    currentGazeTarget.SendMessage("OnGazeExit", SendMessageOptions.DontRequireReceiver);
                }

                currentGazeTarget = hitObject;
                currentGazeTarget.SendMessage("OnGazeEnter", SendMessageOptions.DontRequireReceiver);
            }
        }
        else
        {
            Debug.Log("Not looking at any object");
            if (currentGazeTarget != null)
            {
                currentGazeTarget.SendMessage("OnGazeExit", SendMessageOptions.DontRequireReceiver);
                currentGazeTarget = null;
            }
        }
    }
    else
    {
        Debug.Log("Not looking at any object");
        if (currentGazeTarget != null)
        {
            currentGazeTarget.SendMessage("OnGazeExit", SendMessageOptions.DontRequireReceiver);
            currentGazeTarget = null;
        }
    }
    lineRenderer.SetPosition(0, eyeCenterPosition);
    lineRenderer.SetPosition(1, eyeCenterPosition + gazeDirection * gazeDistance);
}





}
