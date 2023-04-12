using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class EyeTracker : MonoBehaviour
{
    public GameObject leftEye;
    public GameObject rightEye;
    public GameObject gazeTarget;

    [Range(0.0f, 1.0f)]
    public float blendFactor = 0.5f;

    [SerializeField]
    private LayerMask gazeLayerMask;
    [SerializeField]
    private float gazeDistance = 10f;

    void Update()
    {
        Vector3 leftEyeToTarget = gazeTarget.transform.position - leftEye.transform.position;
        Vector3 rightEyeToTarget = gazeTarget.transform.position - rightEye.transform.position;

        Quaternion leftEyeRotation = Quaternion.LookRotation(leftEyeToTarget);
        Quaternion rightEyeRotation = Quaternion.LookRotation(rightEyeToTarget);

        leftEye.transform.rotation = Quaternion.Slerp(leftEye.transform.rotation, leftEyeRotation, blendFactor);
        rightEye.transform.rotation = Quaternion.Slerp(rightEye.transform.rotation, rightEyeRotation, blendFactor);

        CheckGaze();
    }


void CheckGaze()
{
    Vector3 eyeCenterPosition = (leftEye.transform.position + rightEye.transform.position) / 2f;
    Vector3 gazeDirection = (gazeTarget.transform.position - eyeCenterPosition).normalized;

    Ray ray = new Ray(eyeCenterPosition, gazeDirection);
    RaycastHit hit;

    if (Physics.Raycast(ray, out hit, gazeDistance, gazeLayerMask))
    {
        if (hit.collider.gameObject == gazeTarget)
        {
            gazeTarget.SendMessage("OnGazeEnter", SendMessageOptions.DontRequireReceiver);
        }
        else
        {
            gazeTarget.SendMessage("OnGazeExit", SendMessageOptions.DontRequireReceiver);
        }
    }
    else
    {
        gazeTarget.SendMessage("OnGazeExit", SendMessageOptions.DontRequireReceiver);
    }
}

}
