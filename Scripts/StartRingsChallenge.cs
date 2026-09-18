using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StartRingsChallenge : MonoBehaviour
{
    public GameObject rings;
    public TMP_Text task;

    void Start()
    {
        rings.SetActive(false);
        task.text = "Пролетите по кольцам. Для старта нажмите Space";
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && task.text == "Пролетите по кольцам. Для старта нажмите Space")
        {
            rings.SetActive(true);
            task.text = "Пролетите по кольцам";
        }
    }
}
