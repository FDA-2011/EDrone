using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    public float time = 0f;
    private bool isStop = false;
    public TMP_Text txt; 

    private void Update()
    {
        if (!isStop)
        {
            time += Time.deltaTime;
            int times = (int)time;
            int minutes = times / 60;
            int seconds = times % 60;
            int ms = (int)(time * 100) % 100;
            txt.text = $"Время: {minutes}:{seconds}:{ms}";  
        }
    }

    public void Stop()
    {
        isStop = true;
    }
}
