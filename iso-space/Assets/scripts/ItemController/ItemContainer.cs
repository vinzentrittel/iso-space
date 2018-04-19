using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* ItemContainerController
 *  Attach this script to every container for collectible items.
 *  The item must have a Rigidbody attached
 */
public class ItemContainer : MonoBehaviour {

	private const string PLAYER_NAME = "Player";
	private List<Item> contentIds;
	private Inventory inventory;

	[Tooltip("Key binding to collect contained items")]
	public /*const*/ KeyCode actionKey = KeyCode.E;


	/* Item
	 *  Data structure, to manage the children items
	 */
	struct Item { public int id; public GameObject obj; }


	void Start () {
		contentIds = new List<Item>();
		inventory  = FindObjectOfType(typeof(Inventory)) as Inventory;

		// check for items in this container
		foreach(Transform child in transform)
			foreach(StoreItem item in child.GetComponents<StoreItem>())
				AddItem(item);
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


	/* AddItem()
	 */
	private void AddItem(StoreItem storeItem) {
		Item itemTuple = new Item();
		itemTuple.id = storeItem.itemID;
		itemTuple.obj = storeItem.gameObject;

		contentIds.Add(itemTuple);
	} // end : AddItem


	/* RemoveItem
	 *  Swipes clean the list element at index
	 */
	private void RemoveItem(int index) {
		Destroy(contentIds[index].obj);
		contentIds.RemoveAt(index);
	} // end : RemoveItem
} // end : ItemContainerController
