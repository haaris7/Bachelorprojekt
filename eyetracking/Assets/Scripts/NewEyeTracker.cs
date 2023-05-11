using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class NewEyeTracker : MonoBehaviour
{
    public float logTimeInterval = 0.15f;
    public List<GameObject> gazeTargets = new List<GameObject>();
    private LineRenderer lineRenderer;

    private GameObject currentGazeTarget;

    [SerializeField]
    private LayerMask gazeLayerMask;
    [SerializeField]
    private float gazeDistance = 10f;
    public Vector3 gazePoint;
    public Vector3 gazePointOnQuad;
    public string TargetName = "";
    private float previoustime;

    public bool usePositionThreshold = true;
    public float positionThreshold = 0.05f;
    private Vector2 lastLoggedPosition;

    private void Start()
    {
        previoustime = Time.time;
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

                eyeCenterPosition = transform.TransformPoint(eyeCenterPosition);
                gazeDirection = transform.TransformDirection(gazeDirection);
                gazePoint = (gazeDirection * gazeDistance);

                Ray ray = new Ray(eyeCenterPosition, gazeDirection);
                if (ShouldCheck())
                {
                    gazePointOnQuad = GetTextureCoord(ray);
                }
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, gazeDistance, gazeLayerMask))
                {
                    GameObject hitObject = hit.collider.gameObject;

                    bool isValidTarget = false;

                    foreach (GameObject parent in gazeTargets)
                    {
                        if (parent.transform == hitObject.transform || hitObject.transform.IsChildOf(parent.transform))
                        {
                            isValidTarget = true;
                            break;
                        }
                    }

                    if (isValidTarget)
                    {
                        TargetName = hitObject.name;
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
                        if (currentGazeTarget != null)
                        {
                            currentGazeTarget.SendMessage("OnGazeExit", SendMessageOptions.DontRequireReceiver);
                            currentGazeTarget = null;
                        }
                    }
                }
                else
                {
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

    public bool ShouldCheck()
    {
        float now = Time.time;

        if (usePositionThreshold)
        {
            RaycastHit hit;
            Ray ray = new Ray(transform.position, transform.forward);

            if (Physics.Raycast(ray, out hit, gazeDistance, gazeLayerMask))
            {
                Vector2 currentTextureCoord = hit.textureCoord;

                if (Vector2.Distance(currentTextureCoord, lastLoggedPosition) > positionThreshold)
                {
                    lastLoggedPosition = currentTextureCoord;
                    return true;
                }
            }
        }
        else
        {
            if (now - previoustime > logTimeInterval)
            {
                previoustime = now;
                return true;
            }
        }

        return false;
    }

    public Vector2 GetTextureCoord(Ray ray)
    {
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, gazeDistance, LayerMask.GetMask("HeatLayer")))
        {
            Debug.Log("Coordinate: (" + hit.textureCoord.x + "," + hit.textureCoord.y + ")");
            return hit.textureCoord;
        }
        else
        {
            return Vector2.zero;
        }
    }
}