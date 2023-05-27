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
    private LayerMask boundsLayerMask;
    [SerializeField]
    private float gazeDistance = 10f;

    private float def;
    public Vector2 prevgazePointOnQuad;
    public Vector3 gazePoint;
    public Vector2 gazePointOnQuad;
    public string TargetName = "";
    private float previoustime;
    public float duration;

    public bool usePositionThreshold = true;
    public float positionThreshold;
    private Vector2 lastLoggedPosition;
    private Ray ray;

    public float aimAssistStrength = 0.175f; // New variable for aim assist strength
    public bool hasLight = false;
    public bool hasVelocity = false;
    public float intensity = 0f;
    public float velocity = 0f;

    private void Start()
    {
        previoustime = Time.time;
        Debug.Log("Checking gaze...");
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.startWidth = 0.01f;
        lineRenderer.endWidth = 0.01f;
        lineRenderer.positionCount = 2;

        lineRenderer.material = new Material(Shader.Find("Unlit/Color"));
        lineRenderer.material.SetColor("_Color", Color.red);
        prevgazePointOnQuad = new Vector2(0.5f, 0.5f);
        def = gazeDistance;
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

                // Aim assist code
                GameObject nearestTarget = null;
                float nearestDistance = Mathf.Infinity;
                foreach (GameObject target in gazeTargets)
                {
                    float distance = Vector3.Distance(eyeCenterPosition, target.transform.position);
                    if (distance < nearestDistance)
                    {
                        nearestTarget = target;
                        nearestDistance = distance;
                    }
                }

                if (nearestTarget != null)
                {
                    Vector3 targetDirection = (nearestTarget.transform.position - eyeCenterPosition).normalized;
                    gazeDirection = Vector3.Lerp(gazeDirection, targetDirection, aimAssistStrength);
                }

                gazePoint = (gazeDirection * gazeDistance);

                ray = new Ray(eyeCenterPosition, gazeDirection);
                gazePointOnQuad = GetTextureCoord(ray);
                RaycastHit hit;
                AdjustGazeDistance();
                if (Physics.Raycast(ray, out hit, gazeDistance, gazeLayerMask))
                {
                    GameObject hitObject = hit.collider.gameObject;


                    bool isValidTarget = false;
                    Transform pointLight = hitObject.transform.Find("Point Light");
                    if (hasLight != true && pointLight != null)
                    {
                        hasLight = true;
                    }
                    if(hitObject.GetComponent<Rigidbody>())
                    {
                        hasVelocity = true;
                    }

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
                        if (hasLight)
                        {
                            intensity = hitObject.transform.Find("Point Light").GetComponent<Light>().intensity;
                        }
                        if(hasVelocity)
                        {
                            velocity = hitObject.GetComponent<Rigidbody>().velocity.magnitude;
                        }
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
    public Vector3 GetEyePosition()
    {
        List<InputDevice> devices = new List<InputDevice>();
        InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.HeadMounted, devices);

        if (devices.Count > 0)
        {
            InputDevice device = devices[0];

            Vector3 leftEyePosition;
            Vector3 rightEyePosition;

            if (device.TryGetFeatureValue(CommonUsages.leftEyePosition, out leftEyePosition) &&
                device.TryGetFeatureValue(CommonUsages.rightEyePosition, out rightEyePosition))
            {
                Vector3 eyeCenterPosition = (leftEyePosition + rightEyePosition) / 2f;
                return eyeCenterPosition;
            }
        }
        // Return zero vector if eye positions can't be determined.
        return Vector3.zero;
    }

    public void AdjustGazeDistance()
    {
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, gazeDistance, boundsLayerMask))
        {
            gazeDistance = Vector3.Distance(hit.point, this.transform.position);
        }
        else if (gazeDistance != def)
        {
            gazeDistance = def;
        }
    }

    public bool Check()
    {
        float now = Time.time;

        if (usePositionThreshold)
        {
            Vector2 tmp = GetTextureCoord(ray);
            if (gazePointOnQuad != null && Vector2.Distance(tmp, prevgazePointOnQuad) > positionThreshold)
            {
                prevgazePointOnQuad = tmp;
                duration = now - previoustime;
                previoustime = now;
                return true;
            }
            else return false;
        }
        else
        {
            if (now - previoustime > logTimeInterval)
            {
                duration = now - previoustime;
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
            return hit.textureCoord;
        }
        else
        {
            return Vector2.zero;
        }
    }
}
