using UnityEngine;

public class SquareWallMark : MonoBehaviour
{
    public float sideLength = 10f;
    public float wallHeight = 1f;
    public float wallThickness = 0.2f;
    public float flightHeight = 10f;
    public Color wallColor = new Color(1, 0, 0, 0.3f);

    void Start()
    {
        Vector3 center = transform.position;
        float half = sideLength / 2f;
        float halfThick = wallThickness / 2f;

        CreateWall(
            center + new Vector3(-halfThick, 0, -half - halfThick),
            new Vector3(0, 0, -1)
        );
        CreateWall(
            center + new Vector3(halfThick, 0, half + halfThick),
            new Vector3(0, 0, 1)
        );
        CreateWall(
            center + new Vector3(-half - halfThick, 0, halfThick),
            new Vector3(-1, 0, 0)
        );
        CreateWall(
            center + new Vector3(half + halfThick, 0, -halfThick),
            new Vector3(1, 0, 0)
        );
    }

    void CreateWall(Vector3 position, Vector3 direction)
    {
        GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wall.transform.position = new Vector3(position.x, flightHeight, position.z);
        wall.transform.localScale = new Vector3(sideLength + wallThickness, wallHeight, wallThickness);
        wall.transform.LookAt(wall.transform.position + direction);

        Renderer renderer = wall.GetComponent<Renderer>();
        renderer.material.color = wallColor;
        renderer.material.SetFloat("_Mode", 3);
        renderer.material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        renderer.material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        renderer.material.SetInt("_ZWrite", 0);
        renderer.material.DisableKeyword("_ALPHATEST_ON");
        renderer.material.EnableKeyword("_ALPHABLEND_ON");
        renderer.material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        renderer.material.renderQueue = 3000;

        Destroy(wall.GetComponent<Collider>());
    }
}