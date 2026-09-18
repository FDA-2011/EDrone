using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    //UI
    public Slider power;
    public Text copterAngles;

    //Scripts
    public quadrocopterScript copter;



    void Start()
    {
        power.interactable = false;
    }

    // Update is called once per frame
    void Update()
    {
        power.value = (float)copter.throttle;
        copterAngles.text = $"Pitch/TPitch: {Math.Round(copter.pitch) % 360}/{Math.Round(copter.targetPitch) % 360}\nRoll/TRoll: {Math.Round(copter.roll) % 360}/{Math.Round(copter.targetRoll) % 360}\nYaw/TYaw: {Math.Round(copter.yaw) % 360}/{Math.Round(copter.targetYaw) % 360}";
    }
}
