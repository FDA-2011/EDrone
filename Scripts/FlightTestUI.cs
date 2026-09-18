using UnityEngine;
using TMPro;

public class FlightTestUI : MonoBehaviour
{
    public SquareFlightEvaluator evaluator;
    public TMP_Text statusText;
    public TMP_Text errorText;
    public TMP_Text progressText;

    void Update()
    {
        if (evaluator == null) return;

        if (evaluator.isTesting)
        {
            statusText.text = "Летите по квадрату";
            errorText.text = "Ошибка: " + evaluator.GetCurrentError().ToString("F2") + " м";
            progressText.text = "Прогресс: " + (evaluator.progress * 100f).ToString("F0") + "%";
        }
        else if (evaluator.testComplete)
        {
            statusText.text = "Тест завершён: " + evaluator.quality;
            errorText.text = "Ср. ошибка: " + evaluator.averageError.ToString("F2") + " м";
            progressText.text = "Пройдено: " + (evaluator.progress * 100f).ToString("F0") + "%";
        }
        else
        {
            statusText.text = "Пролетите по периметру квадрата";
            errorText.text = "Нажмите Space для старта";
            progressText.text = "";
        }
    }
}