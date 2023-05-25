using UnityEngine;
using System.Collections;

public class LightFader : MonoBehaviour
{
    public Material sphereMaterial; // The material of the sphere
    private Light light;
    private float targetIntensity;
    private float fadeSpeed;  // The fading speed, which will be randomized
    public float minFadeSpeed = 0.05f*1000f;  // The minimum possible fade speed
    public float maxFadeSpeed = 0.5f*1000f;  // The maximum possible fade speed
    public float minStartDelay = 0f*1000f;  // The minimum start delay
    public float maxStartDelay = 5f*1000f;  // The maximum start delay
    public float minHoldTime = 0.5f*1000f; // Minimum hold time before changing intensity
    public float maxHoldTime = 5f*1000f; // Maximum hold time before changing intensity
    public float minIntensity = 0f*1000f; // Minimum light intensity
    public float maxIntensity = 1f*1000f; // Maximum light intensity

    void Start()
    {
        light = GetComponent<Light>();

        // Change the random seed based on the sphere's initial position
        Random.InitState(transform.position.GetHashCode());

        // Randomize the initial intensity and target intensity
        float initialIntensity = Random.Range(minIntensity, maxIntensity);
        light.intensity = initialIntensity;
        targetIntensity = Random.Range(minIntensity, maxIntensity);

        // Randomize the fade speed
        fadeSpeed = Random.Range(minFadeSpeed, maxFadeSpeed);

        // Start the ChangeIntensity coroutine with a random delay
        float startDelay = Random.Range(minStartDelay, maxStartDelay);
        StartCoroutine(StartChangeIntensity(startDelay));
    }

    IEnumerator StartChangeIntensity(float delay)
    {
        // Wait for the specified delay
        yield return new WaitForSeconds(delay);

        // Then start changing the intensity
        while (true)
        {
            // Smoothly change the light's intensity towards the target intensity.
            light.intensity = Mathf.Lerp(light.intensity, targetIntensity, Time.deltaTime * fadeSpeed);

            // Set the material's color based on the light's intensity.
            sphereMaterial.color = new Color(light.intensity, light.intensity, light.intensity, 1);

            // If we've reached the target intensity, wait a random amount of time before changing the target.
            if (Mathf.Approximately(light.intensity, targetIntensity))
            {
                targetIntensity = Random.Range(minIntensity, maxIntensity);
                float holdTime = Random.Range(minHoldTime, maxHoldTime);
                yield return new WaitForSeconds(holdTime);
            }

            // Also randomly decide to change the target intensity before reaching it.
            if (Random.value < 0.01f)
            {
                targetIntensity = Random.Range(minIntensity, maxIntensity);
            }

            // Wait for the next frame
            yield return null;
        }
    }
}