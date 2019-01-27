using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Pickable : BaseInteractable
{
	public NavMeshAgent agent;

	public enum State
	{
		Idle,
		Dragging,
		Falling,
		GoingBack,
		GoingBackNavMesh,
		GoingBackRotate,
	}
	public State state;

	public float distanceFromCollision = 1;

	public float maxDragTime = 3;
	public float maxFallTime = 3;

	public bool doubleSmooth;
	public float smoothSpeed = 0.1f;
	public float smoothSpeed1 = 0.1f;
	public float smoothSpeed2 = 0.1f;

	public Vector3 initialPosition;
	public Quaternion initialRotation;
	public Vector3 lastScreenPosition;
	public Vector3 targetPos;

	[Serializable]
	public struct DebugInfo
	{
		public bool drawHit;
		public SerializableRaycastHit lastHit;
		public Vector3 finalTargetPos;
		public Vector3 currentPos;
	}
	public DebugInfo debug;

	public static Pickable current;
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
		}
	}

	void enableCollider( bool _collider, bool _dynamic = false, bool _agent = false )
	{
		collider.enabled = _collider;

		if (rigidbody)
		{
			if (_collider && _dynamic)
			{
				rigidbody.isKinematic = false;
				rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
			}
			else
			{
				rigidbody.collisionDetectionMode = CollisionDetectionMode.Discrete;
				rigidbody.isKinematic = true;
			}
		}

		gameObject.layer = _collider ? 0 : Physics.IgnoreRaycastLayer;

		if (agent)
		{
			agent.enabled = _agent;
		}
	}

	void setState( State newState )
	{
		timer = 0;
		current = null;

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
				current = this;
				enableCollider(false);
				initialPosition = transform.position;
				initialRotation = transform.rotation;
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
				resetAgent();
				enableCollider(false, _agent: true);
				break;
			}

			case State.GoingBackNavMesh:
			{
				enableCollider(true, _agent: true);
				break;
			}

			case State.GoingBackRotate:
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
					setState(rigidbody ? State.Falling : State.GoingBack);
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

			case State.GoingBackNavMesh:
			{
				updateAgent();
				break;
			}

			case State.GoingBackRotate:
			{
				updateRotate();
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
			if (debug.drawHit)
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


		debug.lastHit = new SerializableRaycastHit(hit);
		debug.finalTargetPos = finalTargetPos;
		debug.currentPos = currentPos;
	}


	void updateFalling() { }


	void updateGoingBack()
	{
		if (tryAgent())
		{
			return;
		}

		Vector3 currentPos = transform.position;
		currentPos = Util.smooth(currentPos, initialPosition, smoothSpeed);

		if (Vector3.Distance(currentPos, initialPosition) < 0.1f)
		{
			currentPos = initialPosition;
			setState(State.Idle);
		}

		transform.position = currentPos;
	}


	void resetAgent()
	{
		if (agent)
		{
			agent.enabled = true;
			agent.ResetPath();
		}
	}
	bool tryAgent()
	{
		if (!agent || agent.pathPending)
		{
			return false;
		}

		if (!agent.hasPath || agent.pathStatus != NavMeshPathStatus.PathComplete)
		{
			if (agent.SetDestination(initialPosition))
			{
				setState(State.GoingBackNavMesh);
				return true;
			}
		}

		return false;
	}

	void updateAgent()
	{
		if (agent.remainingDistance <= agent.stoppingDistance)
		{
			setState(State.GoingBackRotate);
		}
	}

	void updateRotate()
	{
		Vector3 currentPos = transform.position;
		currentPos = Util.smooth(currentPos, initialPosition, smoothSpeed);

		Quaternion currentRot = transform.rotation;
		currentRot = Quaternion.Slerp(currentRot, initialRotation, smoothSpeed);

		if (Vector3.Distance(currentPos, initialPosition) < 0.1f &&
		    Quaternion.Angle(currentRot, initialRotation) < 1f)
		{
			currentPos = initialPosition;
			currentRot = initialRotation;
			setState(State.Idle);
		}

		transform.position = currentPos;
		transform.rotation = currentRot;
	}
}

public static class Util
{
	public static Vector3 smooth( Vector3 current, Vector3 target, float smooth )
	{
		return current + (target - current) * smooth;
	}
	public static Vector3 smooth( Vector3 current, ref Vector3 midTarget, Vector3 finalTarget,
	                              float smooth1, float smooth2 )
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
