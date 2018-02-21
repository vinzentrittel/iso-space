#define COLLECT_ON_KEYPRESS

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Database = ItemDatabase;
using Item = ItemDatabase.Item;

public class ItemController : MonoBehaviour {

#if COLLECT_ON_KEYPRESS
	private const KeyCode KEY_COLLECT = KeyCode.E;
#endif

	public int itemID = Item.INVALID_ID;
	public string itemName;

	private Item item;

	// Use this for initialization
	void Start () {
		// get the corresponding item stats from database
		item = FetchItem();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerStay(Collider other) {
#if COLLECT_ON_KEYPRESS
		if(Input.GetKeyDown(KEY_COLLECT)) {
			// equip item
			Destroy(gameObject);
		}
#else
		// equip item
		Destroy(gameObject);
#endif		
	}


	// Fetches the item corresponding to itemID or itemName
	// Hence itemID has higher priority, the resulting Item
	// might not have the name specified in itemName.
	// If neither itemId, nor itemName are found, the
	// function returns the defaulf Item
	private Item FetchItem() {
		// first try fetching by set id
		try {
			item = ItemDatabase.FetchItemById(itemID);
		} catch (KeyNotFoundException eId) {
			// if ID is not valid, try fetching by set name
			try {
				item = ItemDatabase.FetchItemByName(itemName);
			} catch (KeyNotFoundException eName) {
				// if name is invalid, create empy
				item = new Item();

				// Log exeption
				Debug.Log("Item ID " + eId.Message + " not found in Database.");
				Debug.Log("Item name '" + eName.Message + "' not found in Database.");
			}
		}

		return item;
	}
}