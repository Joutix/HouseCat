using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ventilateur : BaseInteractable
{
	public GameObject conditionBefore;

	public float maxSpeed = 360;
	public float speedImpulse = 90;
	//public float acceleration = 90;
	public float deceleration = 30;
	float speed;

	protected override void onInteract()
	{
		base.onInteract();

		if (conditionBefore && !conditionBefore.activeInHierarchy)
		{
			return;
		}

		speed += speedImpulse;
		speed = Mathf.Min(speed, maxSpeed);
	}

	protected override void Update()
	{
		base.Update();

		if (speed > 0)
		{
			//transform.rotation *= Quaternion.AngleAxis(venti, Vector3.up);
			float dt = Time.deltaTime;
			transform.Rotate(Vector3.forward, speed * dt, Space.Self);
			speed -= (deceleration * dt);
		}
	}
}
