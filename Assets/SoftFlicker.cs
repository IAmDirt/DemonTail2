using UnityEngine;

[RequireComponent(typeof(Light))]
public class SoftFlicker : MonoBehaviour
{
    private float minIntensity =0.7f;
    private float maxIntensity = 1.4f;
    private float startIntensity;
    private float random;
    private Light light;
    void Start()
    {
        light = GetComponent<Light>();
        startIntensity = light.intensity;
        random = Random.Range(0.0f, 65535.0f);
    }
    void Update()
    {
        float noise = Mathf.PerlinNoise(random, Time.time *2);
        light.intensity = Mathf.Lerp(startIntensity *minIntensity, startIntensity*maxIntensity, noise);
    }
}