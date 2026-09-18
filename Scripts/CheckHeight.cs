using UnityEngine;
using TMPro;

public class CheckHeight : MonoBehaviour
{
    public LevelLoader ll;
    public int nextLevel = 9;

    public float targetHeight = 10f;
    public float testDuration = 5f;
    public GameObject Frame;

    public float perfectThreshold = 0.2f;
    public float goodThreshold = 0.4f;
    public float okThreshold = 0.7f;
    public float badThreshold = 1.0f;
    public float awfulThreshold = 1.4f;

    public TMP_Text taskText;
    public TMP_Text resultText;

    private SkillResult sr;
    private float errorSum = 0f;
    private int samples = 0;
    private float timer = 0f;
    private bool testing = false;
    private bool testCompleted = false;
    private bool readyToTest = false;

    void Start()
    {
        sr = FindObjectOfType<SkillResult>();
        taskText.text = $"Поднимитесь на высоту {targetHeight} м";
        resultText.text = "Ожидание подъёма...";
    }

    void FixedUpdate()
    {
        float currentHeight = Frame.transform.position.y;

        if (!testCompleted)
        {
            if (testing)
                resultText.text = $"Высота: {currentHeight:F2} м | Время: {timer:F1}с";
            else
                resultText.text = $"Текущая высота: {currentHeight:F2} м";
        }

        if (!readyToTest && currentHeight >= targetHeight - 0.5f)
        {
            readyToTest = true;
            testing = true;
            taskText.text = $"Удерживайте высоту {targetHeight} м {testDuration} секунд";
            timer = 0f;
            errorSum = 0f;
            samples = 0;
        }

        if (!testing) return;

        timer += Time.fixedDeltaTime;

        if (timer >= testDuration)
        {
            testing = false;
            testCompleted = true;
            float avgError = errorSum / samples;
            (string quality, float score) = GetQuality(avgError);

            if (sr != null && score > sr.result2)
            {
                sr.result3 = score;
            }

            resultText.text = $"Ошибка: {avgError:F3} м\nОценка: {quality}\n{score}/5. Для продолжения нажмите Space";
            return;
        }

        errorSum += Mathf.Abs(currentHeight - targetHeight);
        samples++;
    }

    void Update()
    {
        if (testCompleted && Input.GetKeyDown(KeyCode.Space))
            ll.LoadLevel(nextLevel);
    }

    (string quality, float score) GetQuality(float error)
    {
        if (error <= perfectThreshold) return ("Идеально", 5f);
        if (error <= goodThreshold) return ("Отлично", 4f);
        if (error <= okThreshold) return ("Хорошо", 3f);
        if (error <= badThreshold) return ("Средне", 2f);
        if (error <= awfulThreshold) return ("Плохо", 1f);
        return ("Ужасно", 0f);
    }
}