using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Babah : MonoBehaviour
{
    public CheckWin TriggersT;
    public Obval gg;
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnTriggerEnter (Collider rb)
    {
        print(TriggersT.pisWin);
        print(rb.name);
        if(TriggersT.pisWin && rb.name == "Frame")
        {
            print("gg");
            gg.isObv = true;
        }
    }
}
