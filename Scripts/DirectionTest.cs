using UnityEngine;
using TMPro;

public class DirectionTest : MonoBehaviour
{
    public LevelLoader ll;
    public int nextLevel = 6;

    public Transform drone;
    public float droneSize = 2f;
    public float requiredDistance = 2f;
    public float minStartHeight = 2f;

    public float perfectThreshold = -1f;
    public float goodThreshold = -1f;
    public float okThreshold = -1f;
    public float badThreshold = -1f;
    public float awfulThreshold = -1f;

    public TMP_Text taskText;
    public TMP_Text resultText;

    public KeyCode startKey = KeyCode.F1;

    private SkillResult sr;
    private Vector3 startPos;
    private float startHeight;
    private float maxForward = 0f;
    private float maxBackward = 0f;
    private float maxLeft = 0f;
    private float maxRight = 0f;

    private float errorSum = 0f;
    private int sampleCount = 0;
    private float timer = 0f;
    private bool testing = false;
    private bool testCompleted = false;

    private float usedPerfect, usedGood, usedOk, usedBad, usedAwful;

    void Start()
    {
        sr = FindObjectOfType<SkillResult>();
        usedPerfect = (perfectThreshold < 0) ? droneSize * 0.1f : perfectThreshold;
        usedGood = (goodThreshold < 0) ? droneSize * 0.2f : goodThreshold;
        usedOk = (okThreshold < 0) ? droneSize * 0.3f : okThreshold;
        usedBad = (badThreshold < 0) ? droneSize * 0.5f : badThreshold;
        usedAwful = (awfulThreshold < 0) ? droneSize * 0.7f : awfulThreshold;

        taskText.text = $"Поднимитесь на удобную высоту (минимум {minStartHeight} м), затем нажмите {startKey}";
        resultText.text = $"Текущая высота: {drone.position.y:F2} м";
    }

    void Update()
    {
        if (!testing && !testCompleted)
        {
            resultText.text = $"Текущая высота: {drone.position.y:F2} м";
        }

        if (Input.GetKeyDown(startKey) && !testing && !testCompleted)
        {
            float currentHeight = drone.position.y;
            if (currentHeight < minStartHeight)
            {
                resultText.text = $"Слишком низко! Поднимитесь минимум на {minStartHeight} м (сейчас {currentHeight:F2} м)";
                return;
            }
            StartTest();
        }

        if (testCompleted && Input.GetKeyDown(KeyCode.Space))
        {
            ll.LoadLevel(nextLevel);
        }
    }

    void StartTest()
    {
        testing = true;
        startPos = drone.position;
        startHeight = drone.position.y;

        maxForward = maxBackward = maxLeft = maxRight = 0f;
        errorSum = 0f;
        sampleCount = 0;
        timer = 0f;

        taskText.text = $"Пролетите {requiredDistance} м в каждую сторону (вперёд, назад, влево, вправо)";
        resultText.text = "Тест начат!";
    }

    void FixedUpdate()
    {
        if (!testing || drone == null) return;

        timer += Time.fixedDeltaTime;

        Vector3 currentPos = drone.position;
        float heightError = Mathf.Abs(currentPos.y - startHeight);

        errorSum += heightError;
        sampleCount++;

        float dz = currentPos.z - startPos.z;
        float dx = currentPos.x - startPos.x;
        if (dz > maxForward) maxForward = dz;
        if (-dz > maxBackward) maxBackward = -dz;
        if (-dx > maxLeft) maxLeft = -dx;
        if (dx > maxRight) maxRight = dx;

        int completedDirs = 0;
        if (maxForward >= requiredDistance) completedDirs++;
        if (maxBackward >= requiredDistance) completedDirs++;
        if (maxLeft >= requiredDistance) completedDirs++;
        if (maxRight >= requiredDistance) completedDirs++;
        float progress = completedDirs / 4f;

        resultText.text = $"Прогресс: {progress * 100:F0}%\n" +
                          $"F:{maxForward:F1} B:{maxBackward:F1} L:{maxLeft:F1} R:{maxRight:F1}\n" +
                          $"Высота: {currentPos.y:F2} м";

        if (completedDirs >= 4)
        {
            testing = false;
            testCompleted = true;
            float avgError = errorSum / sampleCount;
            (string quality, float score) = GetQuality(avgError);

            if (sr != null && score > sr.result2)
            {
                sr.result2 = score;
            }

            resultText.text = $"Ошибка: {avgError:F3} м\nОценка: {quality}\n{score}/5. Для продолжения нажмите Space";
            taskText.text = "Тест завершён!";
            return;
        }

        if (timer > 60f)
        {
            testing = false;
            testCompleted = true;
            resultText.text = "Тест прерван по таймауту. Для продолжения нажмите Space";
            taskText.text = "Таймаут!";
        }
    }

    (string quality, float score) GetQuality(float error)
    {
        if (error < usedPerfect) return ("Идеально", 5f);
        if (error < usedGood) return ("Отлично", 4f);
        if (error < usedOk) return ("Хорошо", 3f);
        if (error < usedBad) return ("Средне", 2f);
        if (error < usedAwful) return ("Плохо", 1f);
        return ("Ужасно", 0f);
    }
}