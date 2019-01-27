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
		Falling,
		GoingBack,
	}
	public State state;

	public float distanceFromCamera = 5;
	public float distanceFromCollision = 1;
	public float moveSpeed = 1;
	public bool debugHit;

	public float maxDragTime = 3;
	public float maxFallTime = 3;

	public bool doubleSmooth;
	public float smoothSpeed = 0.1f;
	public float smoothSpeed1 = 0.1f;
	public float smoothSpeed2 = 0.1f;

	public Vector3 initialPosition;
	public Vector3 lastScreenPosition;
	public Vector3 targetPos;

	[Serializable]
	public struct DebugInfo
	{
		public Vector3 finalTargetPos;
		public Vector3 currentPos;
	}
	public DebugInfo debug;

	float timer;
	protected override void onInteract()
	{
		base.onInteract();
		//Debug.Log($"onInteract {state} {name}");

		switch (state)
		{
			case State.Idle:
			{
				setState(State.Dragging);
				break;
			}

			case State.Dragging:
			case State.GoingBack:
			{
				break;
			}

			default: throw new ArgumentOutOfRangeException();
		}
	}

	void enableCollider( bool _enabled, bool _dynamic = false )
	{
		collider.enabled = _enabled;
		rigidbody.isKinematic = !(_enabled && _dynamic);
		gameObject.layer = _enabled ? 0 : Physics.IgnoreRaycastLayer;
	}

	void setState( State newState )
	{
		timer = 0;

		state = newState;
		switch (newState)
		{
			case State.Idle:
			{
				enableCollider(true);
				break;
			}

			case State.Dragging:
			{
				enableCollider(false);
				initialPosition = transform.position;
				lastScreenPosition = camera.WorldToScreenPoint(initialPosition);
				targetPos = transform.position;
				break;
			}

			case State.Falling:
			{
				enableCollider(true, true);
				break;
			}

			case State.GoingBack:
			{
				enableCollider(false);
				break;
			}

			default: throw new ArgumentOutOfRangeException();
		}
	}

	protected override void Update()
	{
		base.Update();

		timer += Time.deltaTime;

		switch (state)
		{
			case State.Idle:
			{
				break;
			}

			case State.Dragging:
			{
				if (timer > maxDragTime)
				{
					setState(State.Falling);
					return;
				}
				updateDragging();
				break;
			}

			case State.Falling:
			{
				if (timer > maxFallTime || rigidbody.IsSleeping())
				{
					setState(State.GoingBack);
					return;
				}
				updateFalling();
				break;
			}

			case State.GoingBack:
			{
				updateGoingBack();
				break;
			}

			default: throw new ArgumentOutOfRangeException();
		}
	}

	void updateDragging()
	{
		Vector3 currentPos = transform.position;

		Vector3 finalTargetPos;
		var ray = camera.ScreenPointToRay(Input.mousePosition);
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
			finalTargetPos = ray.GetPoint(lastScreenPosition.z);
		}

		currentPos = Util.smooth(currentPos, ref targetPos, finalTargetPos,
		                         doubleSmooth, smoothSpeed, smoothSpeed1, smoothSpeed2);

		lastScreenPosition = camera.WorldToScreenPoint(currentPos);
		transform.position = currentPos;


		lastHit = new SerializableRaycastHit(hit);
		debug.finalTargetPos = finalTargetPos;
		debug.currentPos = currentPos;
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


	void updateFalling()
	{
	}

	void updateGoingBack()
	{
		Vector3 currentPos = transform.position;
		currentPos = Util.smooth(currentPos, initialPosition, smoothSpeed);

		if (Vector3.Distance(currentPos, initialPosition) < 0.1f)
		{
			currentPos = initialPosition;
			setState(State.Idle);
		}

		transform.position = currentPos;
	}
}

public static class Util
{
	public static Vector3 smooth( Vector3 current, Vector3 target, float smooth )
	{
		return current + (target - current) * smooth;
	}
	public static Vector3 smooth( Vector3 current, ref Vector3 midTarget, Vector3 finalTarget, float smooth1, float smooth2 )
	{
		midTarget = smooth(midTarget, finalTarget, smooth1);
		return smooth(current, midTarget, smooth2);
	}
	public static Vector3 smooth( Vector3 current, ref Vector3 midTarget, Vector3 finalTarget,
	                              bool doubleSmooth, float smooth, float smooth1, float smooth2 )
	{
		return doubleSmooth
			       ? Util.smooth(current, ref midTarget, finalTarget, smooth1, smooth2)
			       : Util.smooth(current, finalTarget, smooth);
	}
}
