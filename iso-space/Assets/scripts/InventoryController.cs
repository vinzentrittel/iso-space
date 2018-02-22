using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Item = ItemDatabase.Item;

public class InventoryController : MonoBehaviour {

	private const string PLAYER_NAME = "Player";

	private Dictionary<int, TupleItem> dictItems;
	private GameObject player;

	[Tooltip("Number of instances allowed in the inventory")]
	public int stackSize = 5;

	struct TupleItem {
		public int	count;
		public Item data;
		public GameObject gObject;
	}


	void Start () {
		dictItems = new Dictionary<int, TupleItem>();
		player = GameObject.Find("Player");
	}


	void Update() {
		if(Input.GetKeyDown(KeyCode.Q))
			DumpItem(1);

		//Debug.Log(dictItems[1].count);
	}
	

	public bool AddToInventory(Item dataItem, GameObject objItem) {		
		// if an equal item is in this inventory
		if(dictItems.ContainsKey(dataItem.ID)) {
			if(!AddToStack(dataItem.ID)) {
				// TODO: show in UI
				Debug.LogFormat("You cannot store more than {0} {1}s in this inventory", stackSize, dataItem.Name);
				return false;
			}
			if(objItem != dictItems[dataItem.ID].gObject)
				Destroy(objItem);
			// if no other instancec of item is found
		} else {
			AddNewStack(dataItem, objItem);
			Hide(objItem);
		}

		return true;
	}


	public void DumpItem(int id) {
		GameObject objItem;
		bool success;

		objItem = RemoveFromInventory(id, out success);
		if(!success) return;
		
		objItem.transform.position = player.transform.position;
		Hide(objItem, true);
	}


	public GameObject RemoveFromInventory(int id, out bool success) {

		GameObject item = null;
		int count = 0;
		success = false;

		// find item
		if(dictItems.ContainsKey(id)) {
			// store item
			item = dictItems[id].gObject;
			// remove one item
			RemoveFromStack(id);
			if((count = dictItems[id].count) <= 0) {
				// remove stored data, if no item remains
				Debug.LogFormat("{0} remain.", dictItems[id].count);
				dictItems.Remove(id);
			}
			// item successfully popped
			success = true;
		}
		// return popped item, if any
		return (count > 0) ? Instantiate(item) : item;
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


	private void Hide(GameObject obj, bool unhide = false) {
		obj.SetActive(unhide);
		obj.hideFlags = unhide ? HideFlags.None : HideFlags.HideInInspector;
	}


	private void AddNewStack(Item dataItem, GameObject objItem) {
		TupleItem newTuple = new TupleItem();
		newTuple.count = 1;
		newTuple.data = dataItem;
		newTuple.gObject = objItem;

		dictItems.Add(dataItem.ID, newTuple);
	}
}
