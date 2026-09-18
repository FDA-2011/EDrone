using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.SceneManagement;

public class Control : MonoBehaviour
{
    
    public quadrocopterScript copter;
    public double speedT, speedR, yawSpeed;
    public Transform Frame;
    public int k;
    private bool isClick = false;
    public map m;
    public Gamepad gamepad;
    public Transform Cam;
    public float speedCRX;
    public float speedCRY;
    [SerializeField] private int ind;
    [SerializeField] private LevelLoader ll;
    private string now;

    private void Awake()
    {
        now = SceneManager.GetActiveScene().name;
    }

    private void FixedUpdate()
    {
        if (now.Contains("FindBuild"))
        {
            if (Input.GetKey(KeyCode.M))
            {
                isClick = true;
            }
            if (!Input.GetKey(KeyCode.M) && isClick)
            {
                m.isWindow = !m.isWindow;
                isClick = false;
            }
        }
        if (ind > -1 && Input.GetKeyDown(KeyCode.Space))
        {
            ll.LoadLevel(ind);
        }
        gamepad = Gamepad.current;
        if (gamepad == null)
        {
            KeyBoard();
        }else
        {
            GamePad();
        }
    }

    private void KeyBoard()
    {
        if (Input.GetKey(KeyCode.UpArrow))
        {
            copter.throttle += speedT;
            copter.throttle = copter.throttle > 200 ? 200 : copter.throttle;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            copter.throttle -= speedT;
            copter.throttle = copter.throttle < 0 ? 0 : copter.throttle;
        }
        if (Input.GetKey(KeyCode.W))
        {
            copter.targetPitch += speedR;
            copter.targetPitch = copter.targetPitch > 180 ? 180 : copter.targetPitch;
        }
        if (Input.GetKey(KeyCode.S))
        {
            copter.targetPitch -= speedR;
            copter.targetPitch = copter.targetPitch < -180 ? -180 : copter.targetPitch;
        }
        if (Input.GetKey(KeyCode.A))
        {
            copter.targetRoll += speedR;
            copter.targetRoll = copter.targetRoll > 180 ? 180 : copter.targetRoll;
        }
        if (Input.GetKey(KeyCode.D))
        {
            copter.targetRoll -= speedR;
            copter.targetRoll = copter.targetRoll < -180 ? -180 : copter.targetRoll;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            copter.targetYaw += yawSpeed;
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            copter.targetYaw -= yawSpeed;
        }
    }

    private void GamePad()
    {
        //считываем значение стиков Джойстика
        Vector2 leftStick = gamepad.leftStick.ReadValue();
        Vector2 rightStick = gamepad.rightStick.ReadValue();
        //считываем значение с кнопок и бамперов джойстика
        float lt = gamepad.leftTrigger.ReadValue();
        float rt = gamepad.rightTrigger.ReadValue();
        bool lb = gamepad.leftShoulder.isPressed;
        bool rb = gamepad.rightShoulder.isPressed;

        //поворот камеры вверх/вниз
        if (lb)
        {
            Cam.Rotate(0, -speedCRX, 0, Space.World);
        }
        else if (rb)
        {
            Cam.Rotate(0, speedCRX, 0, Space.World);
        }

        //поворот камеры влево/вправо
        Cam.Rotate(speedCRY * (lt - rt), 0, 0);
        
        //газ
        copter.throttle = leftStick[1] > 0 ? leftStick[1] * 150 : 0;
        
        //тангаж
        copter.targetPitch = rightStick[1] * 50;

        //крен
        copter.targetRoll = (rightStick[0] * 50) * -1;

        //рысканье
        copter.targetYaw += yawSpeed * leftStick[0];
    }
}
