using System.Collections;
using UnityEngine;

public class ChangingLights : MonoBehaviour
{
    [SerializeField] private Light[] spotLights; // Array to hold multiple Light components
    [SerializeField] private float minIntensity = 10000f;
    [SerializeField] private float maxIntensity = 100000f;
    [SerializeField] private float frequency = 1f;

    private void Start()
    {
        // Ensure all lights are assigned in the inspector; otherwise, get the Light components in child objects
        if (spotLights.Length == 0)
        {
            spotLights = GetComponentsInChildren<Light>();
        }

        StartCoroutine(OscillateIntensity());
    }

    private IEnumerator OscillateIntensity()
    {
        while (true)
        {
            float time = Time.time * frequency;
            float intensity = Mathf.Lerp(minIntensity, maxIntensity, (Mathf.Sin(time) + 1) / 2); // Sine wave oscillation

            foreach (Light light in spotLights)
            {
                if (light != null)
                {
                    light.intensity = intensity; // Set intensity for each light
                }
            }

            yield return null; // Wait until the next frame
        }
    }
}