using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Checker : MonoBehaviour
{
    public GameObject win;
    public GameObject lose;
    public GameObject Drone;
    public LevelLoader ll;
    private string prs;
    [SerializeField] private GameObject summary;
    [SerializeField] private TMP_Text sumt;
    [SerializeField] private GameObject bt;
    [SerializeField] private GameObject Ask;
    [SerializeField] private Timer timer;


    public int time;
    public int chalenge1 = 0;
    public int chalenge2 = 0;
    public int chalenge3 = 0;
    public int chalenge4 = 0;
    public int balls = 0;
    public int asks = 0;
    private bool iA = false;
    private bool iTP = false;

    void Awake()
    {
        prs = SceneManager.GetActiveScene().name;
        Drone = GameObject.Find("Drone");
        win = GameObject.Find("/Drone/Frame/Main Camera/Canvas/win");
        lose = GameObject.Find("/Drone/Frame/Main Camera/Canvas/lose");
        ll = GetComponent<LevelLoader>();
        DontDestroyOnLoad(this);
    }

    void Update()
    {
        if (prs != SceneManager.GetActiveScene().name)
        {
            Ask.SetActive(true);
            iTP = false;
            iA = false;
            bt.SetActive(false);
            win = GameObject.Find("/Drone/Frame/Main Camera/Canvas/win");
            lose = GameObject.Find("/Drone/Frame/Main Camera/Canvas/lose");
            Drone = GameObject.Find("Drone");
            prs = SceneManager.GetActiveScene().name;
        }
        ll = GetComponent<LevelLoader>();
        if (win.activeSelf && !iA)
        {
            balls++;
            asks++;
            iA = true;
            UpdateData(SceneManager.GetActiveScene().name);
            if (SceneManager.GetActiveScene().name == "FindBuildFinal") time = (int)timer.time;
            SendData();
        }
        if (lose.activeSelf && !iA)
        {
            asks++;
            iA = true;
            if (SceneManager.GetActiveScene().name == "FindBuildFinal") time = (int)timer.time;
            SendData();
        }
        if (asks == 4)
        {
            sumt.text = "Результат: " + balls.ToString() + "/4";
            summary.SetActive(true);
            
        }
        else if (iA)
        {
            bt.SetActive(true);
        }
    }

    private void UpdateData(string SceneName)
    {
        if (SceneName == "FinalRoom1") chalenge2 = 1;
        else if (SceneName == "FinalRoom2") chalenge3 = 1;
        else if (SceneName == "FinalRoom3") chalenge4 = 1;
        else chalenge1 = 1;
    }

    private void SendData()
    {
        if (SceneManager.GetActiveScene().name == "FinalRoom3")
        {
            UserData ud = GameObject.Find("UD").GetComponent<UserData>();
            ud.SendData(chalenge1, chalenge2, chalenge3, chalenge4, time);
        }
    }

    public void TP()
    {
        if (!iTP)
        {
            ll.LoadLevel(3 + asks);
            iTP = true;
        }
    }

    public void Return()
    {
        if (!iTP)
        {
            ll.LoadLevel(0, true);
            iTP = true;
        }
    }
}
