using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* ItemContainerController
 *  Attach this script to every container for collectible items.
 *  The item must have a Rigidbody attached
 */
public class ItemContainerController : MonoBehaviour {

	private const string PLAYER_NAME = "Player";
	private List<Item> contentIds;
	private InventoryController inventory;

	[Tooltip("Key binding to collect contained items")]
	public /*const*/ KeyCode actionKey = KeyCode.E;


	/* Item
	 *  Data structure, to manage the children items
	 */
	struct Item { public int id; public GameObject obj; }


	void Start () {
		Item itemTuple;

		contentIds = new List<Item>();
		inventory  = FindObjectOfType(typeof(InventoryController)) as InventoryController;


		// check for items in this container
		foreach(Transform child in transform) {
			GameObject objChild = child.gameObject;
			// find game objects with ItemController
			foreach(StoreItem item in objChild.GetComponents<StoreItem>()) {
				itemTuple = new Item();
				itemTuple.id = item.itemID;
				itemTuple.obj = item.gameObject;

				contentIds.Add(itemTuple);
			}
		}
	} // end : Start
	

	/* OnCollisionStap()
	 *  A rigidbody must be attached to this game object
	 */
	public void OnTriggerStay(Collider other) {
		
		int id;
		ItemDatabase.Item item;

		// check on player collision for keystrokes
		if(other.gameObject.name == PLAYER_NAME) 
			if(Input.GetKeyDown(actionKey))
				// collect all items
				for(int i = 0; i < contentIds.Count; i++) {
					id = contentIds[i].id;
					item = ItemDatabase.FetchItemById(id);

					// move all items to inventory if enough space left
					if(inventory.AddToInventory(item))
						RemoveItem(i--);				
				}
	} // end : OnCollisionStay


	/* RemoveItem
	 *  Swipes clean the list element at index
	 */
	private void RemoveItem(int index) {
		Destroy(contentIds[index].obj);
		contentIds.RemoveAt(index);
	} // end : RemoveItem
} // end : ItemContainerController
