using System;
using UnityEngine;
using TMPro;

public class Softness : MonoBehaviour
{
    public SkillResult sr;

    public Rigidbody drone;
    public Transform droneTrans;
    public TMP_Text softT;
    public TMP_Text hT;

    public int nextLevel = 8;

    public LevelLoader ll;

    public float result;

    private float prevVel = 0f;
    private float avgChange = 0f;
    private int count = 0;

    public bool isComplete = false;

    public bool isTested  = true;

    void Start()
    {
        isTested = true;
        InvokeRepeating("SoftUpdate", 0f, 0.1f);
    }

    void FixedUpdate()
    {
        if (!isComplete)
        {
            if (droneTrans.position.y > 5f) isComplete = true;
        }
        else
        {
            if (result <= 0.001f) isTested = false;
        }
        if (!isTested)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                ll.LoadLevel(nextLevel);
            }
        }
        if (!isTested) finish();
        if (!isComplete) hT.text = $"Плавно поднимитесь на высоту 5 метров. Текущая высота: {droneTrans.position.y.ToString("F1")}м";
        else hT.text = $"Теперь медленно опуститесь";
        float vel = drone.velocity.magnitude;
        avgChange += Mathf.Abs(vel - prevVel);
        count++;
        prevVel = vel;
    }

    void SoftUpdate()
    {
        if (isTested)
        {
            result = Mathf.Clamp((avgChange / count) * 15f, 0f, 10f);
            string quality = GetQuality(result).quality;
            sr.result1 = (GetQuality(result).score < sr.result1 || sr.result1 == -1) ? GetQuality(result).score : sr.result1;
            softT.text = $"{Math.Round(result, 2)} - {quality}";

            avgChange = 0f;
            count = 0;
        }
    }

    (string quality, float score) GetQuality(float value)
    {
        if (value < 0.5f) return ("Идеально", 5f);
        if (value < 1.5f) return  ("Отлично", 4f);
        if (value < 3f) return ("Хорошо", 3f);
        if (value < 5f) return ("Средне", 2f);
        if (value < 7.5f) return ("Плохо", 1f);
        return ("Ужасно", 0f);
    }
        
    void finish()
    {
        softT.text = $"{sr.result1}/5. Для продолжения нажмите Space";
    }
}