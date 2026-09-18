using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obval : MonoBehaviour
{
    private Rigidbody rb;
    private Transform tr;
    public int i = 50;
    [SerializeField] private GameObject lose;
    public bool isObv = false;
    public bool isTr = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        tr = GetComponent<Transform>();
    }

    private void OnTriggerEnter(Collider rb2)
    {
        print(rb2.name);
        if (rb2.name == "Frame")
        {
            lose.SetActive(true);
            isTr = true;
        }
    }

    private void FixedUpdate()
    {
        if (isObv)
        {
            if (i > 0 && !isTr)
            {
                tr.Translate(0, -15f / 50f, 0);
                i--;
            }
            else
            {
                rb.isKinematic = false;
            }
        }
    }

    public void rboff()
    {
        print("gg");
        rb.constraints = RigidbodyConstraints.None;
        rb.useGravity = true;
        isObv = true;
    }
}
