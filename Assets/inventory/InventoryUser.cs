using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUser : BaseInteractable
{
	public InventoryItem triggerItem;

	protected override void onInteract()
	{
		base.onInteract();

		if (inventory.selectedItem == triggerItem)
		{
			director.Play();
		}
	}
}
