using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class map : MonoBehaviour
{
    public bool isWindow = false;
    public InputField inp;
    public GameObject wind;
    public string rightanswer;
    public GameObject fire;
    public Vector3[] pos;
    public Quaternion[] rot;
    public int i;
    public string now;
    private bool isDone = false;
    private int ind;
    [SerializeField] private LevelLoader ll;
    [SerializeField] private GameObject win;
    [SerializeField] private GameObject lose;
    [SerializeField] private GameObject butcont;
    [SerializeField] private Timer timer;


    void Start()
    {
        now = SceneManager.GetActiveScene().name;
        if (now == "FindBuild")
        {
            ind = 2;
            i = Random.Range(0, 5);
            Instantiate(fire, pos[i], rot[i]);
            if (i == 0)
            {
                rightanswer = "15";
            }
            if (i == 1)
            {
                rightanswer = "3";
            }
            if (i == 2)
            {
                rightanswer = "20";
            }
            if (i == 3)
            {
                rightanswer = "31";
            }
            if (i == 4)
            {
                rightanswer = "8";
            }
        }
        else if (now == "FindBuildFinal")
        {
            ind = 4;
            rightanswer = "3 15 20 31";
            for (int i = 0; i < 4; i++)
            {
                Instantiate(fire, pos[i], rot[i]);
            }
        }
    }

    void Update()
    {
        if (isDone && Input.GetKey(KeyCode.Space))
        {
            ll.LoadLevel(ind);
        }
        if (now.Contains("FindBuild"))
        {
            if (isWindow)
            {
                wind.SetActive(true);
            }
            else
            {
                wind.SetActive(false);
            }
        }
    }

    public void but ()
    {
        if (now.Contains("FindBuild"))
        {
            if (inp.text == rightanswer)
            {
                win.SetActive(true);    
            }
            else
            {
                lose.SetActive(true);
            }
            if (!now.Contains("Final")) butcont.SetActive(true);
            else timer.Stop();
            isDone = true;
        }
    }
}
