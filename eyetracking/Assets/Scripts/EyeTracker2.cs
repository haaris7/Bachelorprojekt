using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class EyeTracker2 : MonoBehaviour
{
    public float nextActionTime = 0.0f;
    public float period = 1.0f;
    public List<GameObject> gazeTargets = new List<GameObject>();
    private LineRenderer lineRenderer;

    private GameObject currentGazeTarget;

    [SerializeField]
    private LayerMask gazeLayerMask;
    [SerializeField]
    private float gazeDistance = 10f;
    public Vector3 gazePoint;
    // public Vector3 gazeRot;
    public string TargetName = "";

    private void Start()
    {
        Debug.Log("Checking gaze...");
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.startWidth = 0.01f;
        lineRenderer.endWidth = 0.01f;
        lineRenderer.positionCount = 2;

        lineRenderer.material = new Material(Shader.Find("Standard"));
        lineRenderer.startColor = Color.red;
        lineRenderer.endColor = Color.red;
    }

    private void Update()
    {
        CheckGaze();
    }

    private void CheckGaze()
    {
        

        List<InputDevice> devices = new List<InputDevice>();
        InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.HeadMounted, devices);

        if (devices.Count > 0)
        {
            InputDevice device = devices[0];

            Vector3 leftEyePosition;
            Vector3 rightEyePosition;
            Quaternion leftEyeRotation;
            Quaternion rightEyeRotation;

            if (device.TryGetFeatureValue(CommonUsages.leftEyePosition, out leftEyePosition) &&
                device.TryGetFeatureValue(CommonUsages.rightEyePosition, out rightEyePosition) &&
                device.TryGetFeatureValue(CommonUsages.leftEyeRotation, out leftEyeRotation) &&
                device.TryGetFeatureValue(CommonUsages.rightEyeRotation, out rightEyeRotation))
            {
                Vector3 eyeCenterPosition = (leftEyePosition + rightEyePosition) / 2f;
                Vector3 leftEyeForward = leftEyeRotation * Vector3.forward;
                Vector3 rightEyeForward = rightEyeRotation * Vector3.forward;
                Vector3 gazeDirection = (leftEyeForward + rightEyeForward) / 2f;
                

                // Transform eyeCenterPosition and gazeDirection from local space to world space
                eyeCenterPosition = transform.TransformPoint(eyeCenterPosition);
                gazeDirection = transform.TransformDirection(gazeDirection);
                gazePoint = (gazeDirection*gazeDistance);

                Ray ray = new Ray(eyeCenterPosition, gazeDirection);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, gazeDistance, gazeLayerMask))
                {
                    GameObject hitObject = hit.collider.gameObject;

                    if (gazeTargets.Contains(hitObject))
                    {
                        TargetName = hitObject.name;
                        //Debug.Log("Raycast hit: " + hitObject.name + " at distance: " + hit.distance + " at point: " + hit.point);

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
                        //Debug.Log("Not looking at any object");
                        if (currentGazeTarget != null)
                        {
                            currentGazeTarget.SendMessage("OnGazeExit", SendMessageOptions.DontRequireReceiver);
                            currentGazeTarget = null;
                        }
                    }
                }
                else
                {
                    //Debug.Log("gaze pos: "+ (gazeDirection*gazeDistance));

                    //Debug.Log("Not looking at any object");
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
    }
}
