using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerObject : MonoBehaviour {

	/* Action
	 *  TriggerObject triggers the the execute function of
	 *  an Action implementation.
	 */
	public interface Action {
		void execute();
	}


	private string PLAYER_TAG = "Player";

	private Action action;
	private bool hasItem;
	private Inventory inventory;

	[Tooltip("ID to item, neccessary in inventory, to trigger this script.")]
	public int triggerItem = ItemDatabase.Item.INVALID_ID;


	void Start () {
		hasItem = false;

		action = GetComponentInParent<Action>() as Action;
		if(action == null)
			Debug.LogErrorFormat("Parent to {0} does not implement TriggerObject.Action", name);

		Collider collider = GetComponent<Collider>() as Collider;
		if(action == null)
			Debug.LogErrorFormat("{0} has no Collider component attached", name);
		else
			collider.isTrigger = true;

		inventory = GameObject.FindObjectOfType<Inventory>();
		if(inventory == null)
			Debug.LogErrorFormat("No inventory found in scene", name);
	}
	

	void OnTriggerEnter(Collider other) {
		if(other.CompareTag(PLAYER_TAG))
			hasItem = inventory.HasItem(triggerItem);
	}


	void OnTriggerStay(Collider other) {
		if(hasItem && other.CompareTag(PLAYER_TAG))
			action.execute();
	}


	void OnTriggerLeave(Collider other) {
		if(other.CompareTag(PLAYER_TAG))
			hasItem = false;
	}

}
