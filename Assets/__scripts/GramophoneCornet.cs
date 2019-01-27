using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GramophoneCornet : BaseInteractable
{
	public float smoothSpeed1 = 0.2f;
	public float smoothSpeed2 = 0.05f;

	bool turn;
	float current;
	float intermediate;

	protected override void onInteract()
	{
		base.onInteract();

		turn = true;
		current = 0;
		intermediate = 0;
	}

	protected override void Update()
	{
		base.Update();

		if (!turn)
		{
			return;
		}

		const float target = 1;
		intermediate += (target - intermediate) * smoothSpeed1;
		current += (intermediate - current) * smoothSpeed2;
		if (Mathf.Abs(current - target) < 0.01f)
		{
			turn = false;
			current = target;
		}

		var euler = transform.localEulerAngles;
		euler.z = current * 360;
		transform.localEulerAngles = euler;
	}
}
