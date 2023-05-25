using System.Collections;
using UnityEngine;

public class LightFader : MonoBehaviour
{
    public Material sphereMaterial;
    private Light light;
    private float targetIntensity;
    private float initialIntensity;
    private float fadeSpeed;
    private float lerpTime;

    void Start()
    {
        sphereMaterial = new Material(sphereMaterial);
        GetComponent<MeshRenderer>().material = sphereMaterial;
        light = transform.Find("Point Light").GetComponent<Light>();
        initialIntensity = light.intensity;
        targetIntensity = Random.Range(0f, 1f);
        fadeSpeed = Random.Range(0.02f, 0.2f);
        lerpTime = 0;

        StartCoroutine(StartAfterDelay(Random.Range(1, 4)));
    }

    IEnumerator StartAfterDelay(int delay)
    {
        yield return new WaitForSeconds(delay);

        while (true)
        {
            light.intensity = Mathf.SmoothStep(initialIntensity, targetIntensity, lerpTime);
            sphereMaterial.color = new Color(light.intensity, light.intensity, light.intensity, 1);

            lerpTime += Time.deltaTime * fadeSpeed;

            if (lerpTime >= 1)
            {
                lerpTime = 0;
                initialIntensity = light.intensity;
                targetIntensity = Random.Range(0f, 1f);
                fadeSpeed = Random.Range(0.02f, 0.2f);
            }
            
            yield return null;
        }
    }
}
