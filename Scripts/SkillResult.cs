using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillResult : MonoBehaviour
{
    public float result1;
    public float result2;
    public float result3;
    public float result4;
    public float result5;
    public float result6;

    void Awake()
    {
        DontDestroyOnLoad(this);
    }
}
