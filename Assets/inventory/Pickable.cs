using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickable : BaseInteractable
{
	public enum State
	{
		Idle,
		Dragging,
	}
	public State state;

	public float distanceFromCamera = 5;
	public float distanceFromCollision = 1;
	public float moveSpeed = 1;
	public bool debugHit;

	public bool doubleSmooth;
	public float smoothSpeed = 0.1f;
	public float smoothSpeed1 = 0.1f;
	public float smoothSpeed2 = 0.1f;

	public Vector3 targetPos;
	public Vector3 initialPosition;
	public Vector3 initialScreenPosition;
	public Vector3 lastScreenPosition;

	protected override void onInteract()
	{
		base.onInteract();
		//Debug.Log($"onInteract {state} {name}");

		switch (state)
		{
			case State.Idle:
			{
				state = State.Dragging;
				collider.enabled = false;
				gameObject.layer = Physics.IgnoreRaycastLayer;

				initialPosition = transform.position;
				initialScreenPosition = Camera.main.WorldToScreenPoint(initialPosition);
				lastScreenPosition = initialScreenPosition;
				break;
			}
			case State.Dragging:
			{
				break;
			}
			default: throw new ArgumentOutOfRangeException();
		}
	}

	protected override void Update()
	{
		base.Update();

		if (state != State.Dragging)
		{
			return;
		}

		var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		Vector3 finalTargetPos;
		if (Physics.Raycast(ray, out var hit))
		{
			finalTargetPos = hit.point - ray.direction * distanceFromCollision;
			if (debugHit)
			{
				Debug.DrawLine(hit.point, hit.point + hit.normal, Color.red, 0.1f);
			}
		}
		else
		{
			finalTargetPos = ray.GetPoint(initialScreenPosition.z);
		}
		lastHit = new SerializableRaycastHit(hit);

		if (doubleSmooth)
		{
			targetPos += (finalTargetPos - targetPos) * smoothSpeed1;
			transform.position += (targetPos - transform.position) * smoothSpeed2;
		}
		else
		{
			transform.position += (finalTargetPos - transform.position) * smoothSpeed;
		}

		//transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
		//transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, moveSpeed * Time.deltaTime);
	}

	public SerializableRaycastHit lastHit;
	/*void OnDrawGizmos()
	{
		if (!lastHit.collider)
		{
			return;
		}

		var hit = lastHit;
		Debug.DrawLine(hit.point, hit.normal, Color.red, 0.1f);
	}*/
}
