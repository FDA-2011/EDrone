using UnityEngine;

public class StartPlatform : MonoBehaviour
{
    public float platformSize = 2f;
    public float platformHeight = 0.1f;
    public float spawnY = 0f;
    public float offsetX = 0f;
    public float offsetZ = 0f;
    public Color platformColor = Color.green;

    private GameObject platform;

    void Start()
    {
        SquareFlightEvaluator evaluator = FindObjectOfType<SquareFlightEvaluator>();
        Vector3 center = evaluator != null ? evaluator.transform.position : Vector3.zero;

        float spawnX = center.x - 10 - evaluator.wallThikness + offsetX;
        float spawnZ = center.z - 10 - evaluator.wallThikness + offsetZ;
        Vector3 spawnPos = new Vector3(spawnX, spawnY, spawnZ);

        CreatePlatform(spawnPos);
    }

    void CreatePlatform(Vector3 position)
    {
        platform = GameObject.CreatePrimitive(PrimitiveType.Cube);
        platform.transform.position = new Vector3(position.x, position.y + platformHeight / 2f, position.z);
        platform.transform.localScale = new Vector3(platformSize, platformHeight, platformSize);

        Renderer renderer = platform.GetComponent<Renderer>();
        renderer.material.color = platformColor;
    }

    void OnDestroy()
    {
        if (platform != null)
            Destroy(platform);
    }
}