using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Item = ItemDatabase.Item;

public class InventoryController : MonoBehaviour {

	public int stackSize = 5;

	private Dictionary<int, Item> items;
	private Dictionary<int, int> numItems;

	// Use this for initialization
	void Start () {
		items = new Dictionary<int, Item>();
		numItems = new Dictionary<int, int>();
	}


	public bool AddToInventory(Item item) {
		// if an equal item is in this inventory
		if(numItems.ContainsKey(item.ID)) {
			// and their count exceeds the maximum size
			if(numItems[item.ID] >= stackSize) {
				// the item will not be stored
				// TODO: print to UI
				Debug.LogFormat("You cannot store more than {0} {1}s in this inventory", stackSize, item.Name);
				return false;
			}
		// if no other instancec of item is found
		} else
			// make a new slot for item
			AddItem(item);

		// another item for the stack
		++numItems[item.ID];
		return true;
	}

	public bool GetFromInventory(int id, out Item item) {
		// find item
		item = items.ContainsKey(id) ? items[id] : new Item();
		// remove it
		return items.Remove(id);
	}

	// Add the item data to the inventory
	// The initial instance count in numItems is 0
	void AddItem(Item item) {
		items.Add(item.ID, item);
		numItems.Add(item.ID, 0);
	}
}
