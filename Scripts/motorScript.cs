using UnityEngine;
using System.Collections;

public class motorScript : MonoBehaviour
{

	public float power = 0.0f;
	public float yaw = 0f;
	public float kf = 0f;

	void FixedUpdate()
	{
		GetComponent<Rigidbody>().AddRelativeForce(0, power, 0);
		GetComponent<Rigidbody>().AddRelativeTorque(0, yaw, 0);
	}
}