using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using Item = ItemDatabase.Item;

/* InventoryController
 *  Add this to any of the existing gameObjects.
 *  You must then enter the game object's name in
 *  in the ItemController's INVENTORY_NAME.
 */
public class Inventory : MonoBehaviour {

	//private const string PREFAB_PATH = ItemPrefabRegister.PREFAB_PATH;
	private const string PLAYER_NAME = "Player";

    public Dictionary<int, TupleItem> dictItems { get; private set; }
	private GameObject player;

	[Tooltip("Number of instances allowed in the inventory")]
	public int stackSize = 5;

	public struct TupleItem {
		public int	count;
		public Item data;
	}


	void Start () {
		dictItems = new Dictionary<int, TupleItem>();
		player = GameObject.Find(PLAYER_NAME);
	} // end : Start
    	

	/* AddToInventory()
	 *  Adds one element of item inside the inventory.
	 *  If the item already exists, stack them up to stackSize
	 *  (see above) instances.
	 *  
	 *  returns false, if no more items id fit on the stack
	 *          true,  in all other cases
	 */
	public bool AddToInventory(Item item) {		
		if(dictItems.ContainsKey(item.ID)) {
			// try to add on top of stack
			if(!AddToStack(item.ID)) {
				BroadcastMessage("FullStack", item.ID, SendMessageOptions.DontRequireReceiver);
				return false;
			}
		} else {
			AddNewStack(item);
		}

		// Notify children
		BroadcastMessage("AddItem", item.ID, SendMessageOptions.DontRequireReceiver);

		// item was stacked
		return true;
	} // end : Add to Inventory


	/* DumpItem()
	 *  Removes the fetched item from the inventory or stack
	 *  and places a copy of the assigned prefab on the
	 *  player position.
	 *  If no item with id is left, nothing happens.
	 */
	public void DumpItem(int id) {
		GameObject objItem;
		Item dataItem;

		if(RemoveFromInventory(id, out dataItem)) {
			// copy the prefab to player's location
			objItem = ItemPrefabRegister.GetInstance().GetClone(dataItem.Prefab);
			objItem.transform.position = player.transform.position;
		}
	} // end : DumpItem


	/* RemoveFromInventory()
	 *  Removes the fetched item from the inventory or stack
	 *  If the last item is removed from stack, the dictionary
	 *  entry is dumped.
	 * 
	 *  returns false, if no items id are left in inventory
	 */
	public bool RemoveFromInventory(int id) {
		Item dummy;
		return RemoveFromInventory(id, out dummy);
	} // end : RemoveFromInventory


	/* RemoveFromInventory()
	 *  Removes the fetched item from the inventory or stack and
	 *  returns its data via the 'item' reference.
	 *  If the last item is removed from stack, the dictionary
	 *  entry is dumped.
	 * 
	 *  returns false, if no items id are left in inventory
	 */
	public bool RemoveFromInventory(int id, out Item item) {
		
		// find item
		item = new Item();
		if(dictItems.ContainsKey(id)) {
			// remove one item
			item = dictItems[id].data;
			RemoveFromStack(id);
			// remove stored data, if no item remains
			if(dictItems[id].count <= 0)
				dictItems.Remove(id);
			
			// item successfully popped
			// notify children
			BroadcastMessage("RemoveItem", id, SendMessageOptions.DontRequireReceiver);
			return true;
		} else
			// no more items of id
			return false;
	} // end : RemoveFromInventory


	/* HasItem()
	 *  returns true, if at least one instance of item is in your inventory
	 */
	public bool HasItem(int id) {
		if(dictItems.ContainsKey(id))
			return true;

		// item not in inventory, notify children
		BroadcastMessage("MissingItem", id, SendMessageOptions.DontRequireReceiver);
		return false;
	}

	/********** Helper Functions *********/
	//

	private void RemoveFromStack(int id) {
		TupleItem temp = dictItems[id];
		--temp.count;
		dictItems[id] = temp;
	} // end : RemoveFromStack


	/* AddToStack()
	 *  Increments the count for entry with id.
	 *  The id must exist in dictItem.
	 *  throws KeyNotFoundException
	 */
	private bool AddToStack(int id) {
		TupleItem temp = dictItems[id];
		if(temp.count >= stackSize)
			return false;
		
		temp.count++;
		dictItems[id] = temp; 
		return true;
	} // end : AddToStack


	/* AddNewStack()
	 *  Adds a new stack for id in dictionary.
	 *  The id must not exist, yet.
	 *  throws System.ArgumentException
	 */
	private void AddNewStack(Item item) {
		TupleItem newTuple = new TupleItem();
		newTuple.count = 1;
		newTuple.data = item;

		dictItems.Add(item.ID, newTuple);
	} // end : AddNewStack
} // end : InventoryController
