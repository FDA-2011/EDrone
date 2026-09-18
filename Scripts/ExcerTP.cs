using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ExcerTP : MonoBehaviour
{
    public TMP_Text task;
    public TMP_Text status;
    public float requiredDistance = 2f;
    public float errorThreshold = 0.5f;
    public bool invertDirections = true;
    public KeyCode startKey = KeyCode.F2;
    public bool testing = false;
    public bool testFinished = false;
    public GameObject Drone;

    public SkillResult sr;
    public LevelLoader ll;
    public int menuLevel = 0;

    private string[] baseTasks = { "прямо", "назад", "влево", "вправо" };
    private List<string> remainingTasks;
    private string currentTask;
    private Vector3 startPos;
    private bool taskActive = false;
    private bool taskCompleted = false;
    private bool waitingForConfirm = false;

    private int correctCount = 0;
    private int errorCount = 0;
    private int taskCount = 0;

    private void Start()
    {
        sr = FindObjectOfType<SkillResult>();
        ll = FindObjectOfType<LevelLoader>();

        task.text = "Камера развернута в сторону дрона. Вы должны выполнять инструкции относительно вашего взгляда";
        remainingTasks = new List<string>(baseTasks);
        testing = false;
        testFinished = false;
        if (status != null) status.text = "Готов к тесту (Space)";
    }

    private void Update()
    {
        if (Input.GetKeyDown(startKey) && !testing && !testFinished)
        {
            StartTest();
        }

        if (waitingForConfirm && Input.GetKeyDown(KeyCode.Space))
        {
            waitingForConfirm = false;
            taskActive = true;
            taskCompleted = false;
            startPos = Drone.transform.position;
            if (status != null) status.text = "Выполняйте!";
        }

        if (testing && taskActive && !taskCompleted)
        {
            CheckTaskAutomatic();
        }

        if (testing && status != null)
        {
            if (waitingForConfirm)
                status.text = "Нажмите Space для начала выполнения";
            else if (taskActive && !taskCompleted)
                status.text = "Выполняйте задание";
        }

        if (testFinished && Input.GetKeyDown(KeyCode.Space))
        {
            if (ll != null)
                ll.LoadLevel(menuLevel);
        }
    }

    private void StartTest()
    {
        List<string> shuffled = new List<string>(baseTasks);
        for (int i = shuffled.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            string temp = shuffled[i];
            shuffled[i] = shuffled[j];
            shuffled[j] = temp;
        }

        string extra = baseTasks[Random.Range(0, baseTasks.Length)];
        shuffled.Add(extra);

        remainingTasks = shuffled;

        correctCount = 0;
        errorCount = 0;
        taskCount = 0;
        testing = true;
        testFinished = false;
        if (status != null) status.text = "Ожидайте задание";
        NextTask();
    }

    private void NextTask()
    {
        if (remainingTasks.Count == 0)
        {
            FinishTest();
            return;
        }

        int index = Random.Range(0, remainingTasks.Count);
        currentTask = remainingTasks[index];
        remainingTasks.RemoveAt(index);
        task.text = "Летите " + currentTask;

        startPos = Drone.transform.position;
        taskActive = false;
        taskCompleted = false;
        waitingForConfirm = true;
        if (status != null) status.text = "Нажмите Space для начала выполнения";
    }

    private void CheckTaskAutomatic()
    {
        Vector3 delta = Drone.transform.position - startPos;
        float sign = invertDirections ? -1f : 1f;

        float projection = 0f;
        string direction = currentTask;

        if (direction == "прямо")
            projection = delta.z * sign;
        else if (direction == "назад")
            projection = -delta.z * sign;
        else if (direction == "влево")
            projection = -delta.x * sign;
        else if (direction == "вправо")
            projection = delta.x * sign;

        float opposite = -projection;

        if (projection >= requiredDistance)
        {
            correctCount++;
            taskCount++;
            if (status != null) status.text = $"Правильно! (+1)   Всего: {correctCount} верных, {errorCount} ошибок";
            taskCompleted = true;
            taskActive = false;
            NextTask();
            return;
        }

        if (opposite > errorThreshold)
        {
            errorCount++;
            taskCount++;
            if (status != null) status.text = $"Ошибка! (неправильное направление)   Верных: {correctCount}, Ошибок: {errorCount}";
            taskCompleted = true;
            taskActive = false;
            NextTask();
            return;
        }
    }

    private void FinishTest()
    {
        testing = false;
        taskActive = false;
        waitingForConfirm = false;
        testFinished = true;

        float score = 0f;
        if (errorCount == 0) score = 5f;
        else if (errorCount == 1) score = 4f;
        else if (errorCount == 2) score = 3f;
        else if (errorCount == 3) score = 2f;
        else if (errorCount == 4) score = 1f;
        else score = 0f;

        if (sr != null && score > sr.result6)
            sr.result6 = score;

        string grade;
        if (errorCount == 0) grade = "Идеально";
        else if (errorCount <= 1) grade = "Отлично";
        else if (errorCount <= 2) grade = "Хорошо";
        else if (errorCount <= 3) grade = "Средне";
        else if (errorCount <= 4) grade = "Плохо";
        else grade = "Ужасно";

        task.text = $"Тест завершён! Оценка: {grade} ({score}/5)";
        if (status != null) status.text = $"Верных: {correctCount}, Ошибок: {errorCount}. Нажмите Space для выхода в меню";
    }
}