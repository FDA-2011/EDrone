using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggersArea : MonoBehaviour
{
    private Rigidbody rb;
    public bool isVisit = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnTriggerEnter(Collider obj)
    {
        if (obj.name == "Frame")
        {
            isVisit = true;
        }
    }
}
