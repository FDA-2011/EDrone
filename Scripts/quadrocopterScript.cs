using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using static quadrocopterScript;


public class quadrocopterScript : MonoBehaviour
{

    public motorScript FR, FL, BR, BL;
	public Transform Frame;
	public int k;

	public float speedY;
	public float unspeedY;

	public double yawP;
	public double yawI;
	public double yawD;

	//фактические параметры
	public double pitch; //Тангаж
	public double roll; //Крен
	public double yaw; //Рыскание
	public double throttle; //Газ, газ мы задаем извне, поэтому он public

	//требуемые параметры
	public double targetPitch;
	public double targetRoll;
	public double targetYaw;

	//PID регуляторы, которые будут стабилизировать углы
	//каждому углу свой регулятор, класс PID определен ниже
	//константы подобраны на глаз :) пробуйте свои значения
	private PID pitchPID = new PID(100, 0, 20);
	private PID rollPID = new PID(100, 0, 20);
	private PID yawPID = new PID(500, 0, 1);

	void readRotation()
	{

		//фактическая ориентация нашего квадрокоптера,
		//в реальном квадрокоптере эти данные необходимо получать
		//из акселерометра-гироскопа-магнетометра, так же как делает это ваш
		//смартфон
		Vector3 rot = Frame.rotation.eulerAngles;
		pitch = rot.x;
		yaw = rot.y;
		roll = rot.z;

	}

	//функция стабилизации квадрокоптера
	//с помощью PID регуляторов мы настраиваем
	//мощность наших моторов так, чтобы углы приняли нужные нам значения
	void stabilize()
	{

		//нам необходимо посчитать разность между требуемым углом и текущим
		//эта разность должна лежать в промежутке [-180, 180] чтобы обеспечить
		//правильную работу PID регуляторов, так как нет смысла поворачивать на 350
		//градусов, когда можно повернуть на -10

		double dPitch = targetPitch - pitch;
		double dRoll = targetRoll - roll;
		double dYaw = targetYaw - yaw;

		dPitch -= Math.Ceiling(Math.Floor(dPitch / 180.0) / 2.0) * 360.0;
		dRoll -= Math.Ceiling(Math.Floor(dRoll / 180.0) / 2.0) * 360.0;
		dYaw -= Math.Ceiling(Math.Floor(dYaw / 180.0) / 2.0) * 360.0;

		//1 и 2 мотор впереди
		//3 и 4 моторы сзади
		double motor1power = throttle;
		double motor2power = throttle;
		double motor3power = throttle;
		double motor4power = throttle;

		double motor1powerR = speedY;
		double motor2powerR = -speedY;
		double motor3powerR = speedY;
		double motor4powerR = -speedY;

		//ограничитель на мощность подаваемую на моторы
		double powerLimit = throttle > 150 ? 150 : throttle;
		double rotLimit = Math.Abs(targetYaw - yaw);

		//управление тангажем:
		//на передние двигатели подаем возмущение от регулятора
		//на задние противоположное возмущение
		double pitchForce = -pitchPID.calc(0, dPitch / 180.0);
		pitchForce = pitchForce > powerLimit ? powerLimit : pitchForce;
		pitchForce = pitchForce < -powerLimit ? -powerLimit : pitchForce;
		motor1power += pitchForce;
		motor2power += pitchForce;
		motor3power += -pitchForce;
		motor4power += -pitchForce;

		//управление креном:
		//действуем по аналогии с тангажем, только регулируем боковые двигатели
		double rollForce = -rollPID.calc(0, dRoll / 180.0);
		rollForce = rollForce > powerLimit ? powerLimit : rollForce;
		rollForce = rollForce < -powerLimit ? -powerLimit : rollForce;
		motor1power += rollForce;
		motor2power += -rollForce;
		motor3power += -rollForce;
		motor4power += rollForce;


		unspeedY = (float)yawPID.calc(0, dYaw / 180.0);
		if (targetYaw < 0)
		{
			targetYaw += 360;
		}
		else if (targetYaw > 360)
		{
			targetYaw -= 360;
		}
		if (targetYaw > yaw)
		{
			if (targetYaw - yaw < 180)
			{
				motor1powerR += unspeedY;
				motor2powerR += unspeedY;
				motor3powerR += unspeedY;
				motor4powerR += unspeedY;
				print("r1");
			}
			else
			{
				motor1powerR += unspeedY;
				motor2powerR += unspeedY;
				motor3powerR += unspeedY;
				motor4powerR += unspeedY;
				print("l1");
			}
		}
		else
		{
			if (yaw - targetYaw < 180)
			{
				motor1powerR += unspeedY;
				motor2powerR += unspeedY;
				motor3powerR += unspeedY;
				motor4powerR += unspeedY;
				print("l2");
			}
			else
			{
				motor1powerR += unspeedY;
				motor2powerR += unspeedY;
				motor3powerR += unspeedY;
				motor4powerR += unspeedY;
				print("r2");
			}
		}


		FL.power = (float)motor1power;
		FR.power = (float)motor2power;
		BR.power = (float)motor3power;
		BL.power = (float)motor4power;
		FL.yaw = (float)motor1powerR;
		FR.yaw = (float)motor2powerR;
		BR.yaw = (float)motor3powerR;
		BL.yaw = (float)motor4powerR;
	}

	void FixedUpdate()
	{
		readRotation();
		stabilize();
	}

    

}

public class PID
{

	private double P;
	private double I;
	private double D;

	private double prevErr;
	private double sumErr;

    public void Reset()
    {
        prevErr = 0;
        sumErr = 0;
    }

    public PID(double P, double I, double D)
	{
		this.P = P;
		this.I = I;
		this.D = D;

	}

	public double calc(double current, double target)
	{

		double dt = Time.fixedDeltaTime;

		double err = target - current;
		this.sumErr += err;

		double force = this.P * err + this.I * this.sumErr * dt + this.D * (err - this.prevErr) / dt;

		this.prevErr = err;
		return force;
	}

};