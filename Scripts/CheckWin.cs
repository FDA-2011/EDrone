using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CheckWin : MonoBehaviour
{
    
    public TriggersArea[] rooms;
    public int n;
    public bool pisWin = false;
    void Update()
    {
        bool isWin = true;
        for (int i = 0; i < rooms.Length; ++i)
        {
            if (!rooms[i].isVisit)
            {
                isWin = false;
                break;
            }
        }
        if (isWin)
        {
            pisWin = isWin;
            print("Ïî÷òèWin");
        }
    }
}
