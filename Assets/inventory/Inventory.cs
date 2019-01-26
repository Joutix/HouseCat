using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
	public InventorySlot[] slots;
	public List<InventoryItem> items = new List<InventoryItem>();
	public InventoryItem selectedItem;

	public static Inventory instance;
	void Awake()
	{
		instance = this;
	}

	public void addItem( InventoryItem _item )
	{
		if (items.Contains(_item))
		{
			Debug.LogError("already in inventory!");
			return;
		}

		_item.slotIndex = items.Count;
		items.Add(_item);

		moveToSlot(_item);
	}
	void moveToSlot( InventoryItem _item )
	{
		var slot = slots[_item.slotIndex];
		_item.transform.position = slot.transform.position;
		_item.transform.rotation = slot.transform.rotation;
		_item.state = InventoryItem.State.InInventory;
	}

	public void removeItem( InventoryItem _item )
	{
		if (!items.Contains(_item))
		{
			Debug.LogError("was not in inventory!");
			return;
		}

		items.Remove(_item);
	}

	public void selectItem( InventoryItem _item)
	{
		if (selectedItem && selectedItem != _item)
		{
			moveToSlot(selectedItem);
		}

		selectedItem = _item;
	}
}
