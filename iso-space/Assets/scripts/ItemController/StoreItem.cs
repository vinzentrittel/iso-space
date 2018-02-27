// disable this for auto collect
#define COLLECT_ON_KEYPRESS

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Database = ItemDatabase;
using Item = ItemDatabase.Item;

/* ItemController
 *  Attach this script to every collectible item.
 *  The item must have a BoxCollider attached
 */
public class StoreItem : MonoBehaviour {
	
	private const string PLAYER_NAME = "Player";
	private Item item;
	private Inventory inventory;


#if COLLECT_ON_KEYPRESS
	[Tooltip("Key binding to collect this item")]
	public /*const*/ KeyCode actionKey = KeyCode.E;
#endif
	[Tooltip("Fetch the object data by it's database id")]
	public int itemID = Item.INVALID_ID;


	// Use this for initialization
	void Start () {
		// get the corresponding item stats from database
		item = FetchItem();

		// set name
		gameObject.name = item.Name;

		inventory  = FindObjectOfType(typeof(Inventory)) as Inventory;
		if(inventory == null)
			Debug.LogError("Cannot find InventoryController in scene.");
	}


	// Gives player chance to collect items, when in touch
	// with it
	public void OnTriggerStay(Collider other) {
#if COLLECT_ON_KEYPRESS
		if(Input.GetKeyDown(actionKey) && other.CompareTag(PLAYER_NAME))
#else
		if(other.gameObject.name == "Player")
#endif
		// try to store item in inventory
		if(inventory.AddToInventory(item))
			Destroy(gameObject);
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
			return item;
		} catch (KeyNotFoundException) {
			// Log exeption
			Debug.LogFormat("ItemController: Item ID {0} not found in Database.", itemID);
			item = new Item();
			return item;
		}
	}
}