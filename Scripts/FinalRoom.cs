using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class FinalRoom : MonoBehaviour
{
    public bool isSp;
    public int isSp2;
    [SerializeField] private GameObject human;
    [SerializeField] private GameObject win;
    [SerializeField] private GameObject lose;

    private void Start()
    {
        string now = SceneManager.GetActiveScene().name;
        if (now.Contains("FinalRoom"))
        {
            isSp2 = Random.Range(0, 101) % 2;
            if (isSp2 == 1)
            {
                isSp = true;
            }
            else
            {
                isSp = false;
            }
            if (isSp)
            {
                human.SetActive(true);
            }
        }
    }

    public void Click(bool state)
    {
        if (state == isSp)
        {
            win.SetActive(true);
        }
        else
        {
            lose.SetActive(true);
        }
    }
}
