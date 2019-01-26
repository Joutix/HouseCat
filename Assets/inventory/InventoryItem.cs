using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryItem : BaseInteractable
{
	public GameObject conditionBefore;
	public GameObject conditionAfter;
	public enum State
	{
		Idle,
		InInventory,
		Dragging,
	}
	public State state;
	public int slotIndex = -1;

	protected override void onInteract()
	{
		base.onInteract();

		if (conditionBefore && !conditionBefore.activeInHierarchy)
		{
			return;
		}

		if (conditionAfter)
		{
			conditionAfter.SetActive(true);
		}

		//Debug.Log($"onInteract {state} {name}");
		switch (state)
		{
			case State.Idle:
			{
				state = State.InInventory;
				inventory.addItem(this);
				break;
			}
			case State.InInventory:
			{
				state = State.Dragging;
				collider.enabled = false;
				//gameObject.layer = Physics.IgnoreRaycastLayer;
				inventory.selectItem(this);
				break;
			}
			case State.Dragging:
			{
				break;
			}
			default: throw new ArgumentOutOfRangeException();
		}
	}

	public float distanceFromCamera = 5;
	protected override void Update()
	{
		base.Update();

		if (state != State.Dragging)
		{
			return;
		}

		var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		transform.position = ray.GetPoint(distanceFromCamera);
	}
}
