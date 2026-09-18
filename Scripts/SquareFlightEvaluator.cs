using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class SquareFlightEvaluator : MonoBehaviour
{
    public SkillResult sr;
    public LevelLoader ll;
    public int nextLevel = 11;

    public TMP_Text statusText;
    public TMP_Text errorText;
    public TMP_Text progressText;

    public Transform drone;
    public Rigidbody droneRigidbody;
    public MonoBehaviour droneController;


    public float sideLength = 20f;
    public float wallThikness = 5f;
    public float wallHeight = 5f;
    public float targetHeight = 10f;
    public int segments = 30;

    public float perfectDeviation = 0.5f;
    public float goodDeviation = 1f;
    public float okDeviation = 2f;
    public float badDeviation = 3f;
    public float awfulDeviation = 4.5f; 

    public KeyCode startKey = KeyCode.Space;
    public float ignoreErrorBelowSpeed = 0.1f;
    public int neighborRadius = 5;

    public bool drawTrail = true;
    public Color trailColor = Color.white;
    public float trailWidth = 0.15f;

    public bool isTesting = false;
    public bool testComplete = false;
    public float averageError = 0f;
    public string quality = "";
    public float progress = 0f;

    private List<Vector3> waypoints;
    private bool[] visited;
    private int visitedCount = 0;

    private float errorSum = 0f;
    private int errorSamples = 0;

    private LineRenderer pathLine;
    private int pathPoints = 0;
    private int maxPathPoints = 1000;

    private float innerHalf;
    private float outerHalf;

    void Start()
    {
        sr = FindObjectOfType<SkillResult>();

        float innerSize = sideLength;
        float outerSize = sideLength + 2 * wallThikness;

        innerHalf = innerSize / 2f;
        outerHalf = outerSize / 2f;

        GenerateWaypoints();
        pathLine = gameObject.AddComponent<LineRenderer>();
        pathLine.startWidth = trailWidth;
        pathLine.endWidth = trailWidth;
        pathLine.material = new Material(Shader.Find("Sprites/Default"));
        pathLine.startColor = trailColor;
        pathLine.endColor = trailColor;
        pathLine.positionCount = 0;
        pathLine.enabled = drawTrail;
    }

    void GenerateWaypoints()
    {
        waypoints = new List<Vector3>();
        Vector3 center = transform.position;
        Vector3[] corners = new Vector3[]
        {
            new Vector3(center.x - innerHalf - wallThikness / 2, targetHeight, center.z - innerHalf - wallThikness / 2),
            new Vector3(center.x + innerHalf + wallThikness / 2, targetHeight, center.z - innerHalf - wallThikness / 2),
            new Vector3(center.x + innerHalf + wallThikness / 2, targetHeight, center.z + innerHalf + wallThikness / 2),
            new Vector3(center.x - innerHalf - wallThikness / 2, targetHeight, center.z + innerHalf + wallThikness / 2)
        };
        float perimeter = (sideLength + 2 * wallThikness) * 4f;
        float step = perimeter / segments;
        for (int i = 0; i < 4; i++)
        {
            Vector3 a = corners[i];
            Vector3 b = corners[(i + 1) % 4];
            float dist = Vector3.Distance(a, b);
            int steps = Mathf.CeilToInt(dist / step);
            for (int j = 0; j < steps; j++)
            {
                float t = (float)j / steps;
                Vector3 pt = Vector3.Lerp(a, b, t);
                pt.y = targetHeight;
                waypoints.Add(pt);
            }
        }
        while (waypoints.Count > segments)
            waypoints.RemoveAt(waypoints.Count - 1);
        while (waypoints.Count < segments)
            waypoints.Add(waypoints[waypoints.Count - 1]);

        visited = new bool[waypoints.Count];
    }

    void Update()
    {
        if (Input.GetKeyDown(startKey) && !isTesting && !testComplete)
            StartTest();

        if (testComplete && Input.GetKeyDown(KeyCode.Space))
        {
            ll.LoadLevel(nextLevel);
        }
    }

    void FixedUpdate()
    {
        if (!isTesting || testComplete) return;

        Vector3 flatPos = new Vector3(drone.position.x, drone.position.y, drone.position.z);
        Vector3 center = transform.position;

        float localX = flatPos.x - center.x;
        float localZ = flatPos.z - center.z;
        float localY = flatPos.y - targetHeight;

        float absX = Mathf.Abs(localX);
        float absZ = Mathf.Abs(localZ);
        float absY = Mathf.Abs(localY);

        float totalError = 0f;

        if (absX > outerHalf)
        {
            float errX = absX - outerHalf;
            totalError = errX * errX;
            totalError = Mathf.Sqrt(totalError);
        }

        if(absZ > outerHalf)
        {
            float errZ = absZ - outerHalf;
            totalError = totalError * totalError + errZ * errZ;
            totalError = Mathf.Sqrt(totalError);
        }

        if (absX < innerHalf && absZ < innerHalf)
        {
            float errX = innerHalf - absX;
            float errZ = innerHalf - absZ;
            totalError = Mathf.Min(errX, errZ);
        }

        if (absY > wallHeight / 2)
        {
            totalError = totalError * totalError + absY * absY;
            totalError = Mathf.Sqrt(totalError);
        }


        float minDist = float.MaxValue;
        int closestIdx = 0;
        for (int i = 0; i < waypoints.Count; i++)
        {
            float d = Vector3.Distance(flatPos, waypoints[i]);
            if (d < minDist)
            {
                minDist = d;
                closestIdx = i;
            }
        }

        float speed = droneRigidbody.velocity.magnitude;
        if (speed > ignoreErrorBelowSpeed)
        {
            if (!visited[closestIdx])
            {
                visited[closestIdx] = true;
                visitedCount++;
                errorSum += totalError;
                errorSamples++;

                for (int offset = 1; offset <= neighborRadius; offset++)
                {
                    int idxPlus = (closestIdx + offset) % waypoints.Count;
                    int idxMinus = (closestIdx - offset + waypoints.Count) % waypoints.Count;
                    if (!visited[idxPlus])
                    {
                        visited[idxPlus] = true;
                        visitedCount++;
                        errorSum += totalError;
                        errorSamples++;
                    }
                    if (!visited[idxMinus])
                    {
                        visited[idxMinus] = true;
                        visitedCount++;
                        errorSum += totalError;
                        errorSamples++;
                    }
                }
            }
        }

        progress = (float)visitedCount / waypoints.Count;

        if (drawTrail)
            AddPathPoint(drone.position);

        Debug.DrawLine(drone.position, new Vector3(waypoints[closestIdx].x, drone.position.y, waypoints[closestIdx].z), Color.red);

        if (progress >= 1f)
            CompleteTest();

        if (isTesting && !testComplete && Time.frameCount % 60 == 0)
        {
            float currentAvg = (errorSamples > 0) ? errorSum / errorSamples : 0f;
        }
    }

    public void StartTest()
    {
        Vector3 center = transform.position;
        float outerSize = sideLength + wallThikness;
        float outerHalfLocal = outerSize / 2f;
        Vector3 startPos = new Vector3(center.x - 10 - wallThikness, targetHeight + 1, center.z - 10 - wallThikness);

        if (droneController != null)
            droneController.enabled = false;


        if (droneController != null)
            droneController.enabled = true;

        visited = new bool[waypoints.Count];
        visitedCount = 0;
        errorSum = 0f;
        errorSamples = 0;
        averageError = 0f;
        quality = "";
        testComplete = false;
        isTesting = true;
        progress = 0f;

        pathLine.positionCount = 0;
        pathPoints = 0;
        pathLine.startColor = trailColor;
        pathLine.endColor = trailColor;

    }

    void CompleteTest()
    {
        isTesting = false;
        testComplete = true;

        if (errorSamples > 0)
            averageError = errorSum / errorSamples;
        else
            averageError = 999f;

        (string qualityText, float score) = GetQuality(averageError);
        quality = qualityText;

        sr.result4 = score;

        statusText.text = $"Ошибка: {averageError:F2} м\nОценка: {quality}\n{score}/5\nНажмите Space для продолжения";

        pathLine.startColor = Color.yellow;
        pathLine.endColor = Color.yellow;
    }

    void ResetTest()
    {
        testComplete = false;
        averageError = 0f;
        quality = "";
        progress = 0f;
        visitedCount = 0;
        visited = new bool[waypoints.Count];
        errorSum = 0f;
        errorSamples = 0;
        pathLine.positionCount = 0;
        pathPoints = 0;
        pathLine.startColor = trailColor;
        pathLine.endColor = trailColor;
    }

    void AddPathPoint(Vector3 point)
    {
        if (pathPoints >= maxPathPoints) return;
        pathLine.positionCount = ++pathPoints;
        pathLine.SetPosition(pathPoints - 1, point);
    }

    (string quality, float score) GetQuality(float error)
    {
        if (error <= perfectDeviation) return ("Идеально", 5f);
        if (error <= goodDeviation) return ("Отлично", 4f);
        if (error <= okDeviation) return ("Хорошо", 3f);
        if (error <= badDeviation) return ("Средне", 2f);
        if (error <= awfulDeviation) return ("Плохо", 1f);
        return ("Ужасно", 0f);
    }

    public float GetCurrentError()
    {
        return errorSamples > 0 ? errorSum / errorSamples : 0f;
    }

    void OnDrawGizmosSelected()
    {
        if (waypoints == null) GenerateWaypoints();
        Gizmos.color = Color.green;
        foreach (var p in waypoints)
            Gizmos.DrawWireSphere(p, 0.1f);
    }
}