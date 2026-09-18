using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Restart : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.R))
        {
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            transform.position = new Vector3 (24.6000004f, 3.66000009f, -77f);
        }
    }
}
