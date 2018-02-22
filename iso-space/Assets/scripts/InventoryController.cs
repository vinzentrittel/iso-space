using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using Item = ItemDatabase.Item;

public class InventoryController : MonoBehaviour {

	private const string PREFAB_PATH = "Assets/Prefabs/";
	private const string PLAYER_NAME = "Player";

	private Dictionary<int, TupleItem> dictItems;
	private GameObject player;

	[Tooltip("Number of instances allowed in the inventory")]
	public int stackSize = 5;

	struct TupleItem {
		public int	count;
		public Item data;
	}


	void Start () {
		dictItems = new Dictionary<int, TupleItem>();
		player = GameObject.Find(PLAYER_NAME);
	}


	void Update() {
		if(Input.GetKeyDown(KeyCode.Q))
			DumpItem(1);

		//Debug.Log(dictItems[1].count);
	}
	

	public bool AddToInventory(Item item) {		
		// if an equal item is in this inventory
		if(dictItems.ContainsKey(item.ID)) {
			if(!AddToStack(item.ID)) {
				// TODO: show in UI
				Debug.LogFormat("You cannot store more than {0} {1}s in this inventory", stackSize, item.Name);
				return false;
			}
		} else {
			AddNewStack(item);
		}

		return true;
	}


	public void DumpItem(int id) {
		GameObject objItem;
		Item dataItem;

		if(RemoveFromInventory(id, out dataItem)) {
			try {
				// TODO: register all prefabs and take them from an array
				// instead of loading at runtime
				objItem = (GameObject) AssetDatabase.LoadAssetAtPath(
					PREFAB_PATH + dataItem.Prefab + ".prefab", typeof(GameObject));
				// copy the prefab to player's location
				objItem = Instantiate(objItem);
				objItem.transform.position = player.transform.position;

			} catch (System.ArgumentException ex) {
				Debug.LogErrorFormat("Asset for {0} from ({1}) could not be loaded",
					dataItem.Handle,
					PREFAB_PATH + dataItem.Prefab + ".prefab");
			}
		}
	}


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
			return true;
		} else
			// no more items of id
			return false;
	}


	private void RemoveFromStack(int id) {
		TupleItem temp = dictItems[id];
		--temp.count;
		dictItems[id] = temp;
	}


	private bool AddToStack(int id) {
		TupleItem temp = dictItems[id];
		if(temp.count >= stackSize)
			return false;
		
		temp.count++;
		dictItems[id] = temp; 
		return true;
	}


	private void AddNewStack(Item item) {
		TupleItem newTuple = new TupleItem();
		newTuple.count = 1;
		newTuple.data = item;

		dictItems.Add(item.ID, newTuple);
	}
}
