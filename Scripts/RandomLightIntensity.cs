using UnityEngine;

public class RandomLightIntensity : MonoBehaviour
{
    [SerializeField] private float minIntensity = 0.25f;
    [SerializeField] private float maxIntensity = 2f;
    [SerializeField] private bool includeInactiveLights = false;

    private void Start()
    {
        SetRandomIntensityToAllLights();
    }

    public void SetRandomIntensityToAllLights()
    {
        Light[] lights = FindObjectsOfType<Light>(includeInactiveLights);
        float randomIntensity = Random.Range(minIntensity, maxIntensity);
        RenderSettings.ambientIntensity = randomIntensity;
        print($"Устанавливаем интенсивность {randomIntensity} для {lights.Length} источников света.");
        foreach (Light light in lights)
        {
            light.intensity = randomIntensity;
        }
    }
}