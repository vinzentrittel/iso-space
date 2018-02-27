using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveItem : MonoBehaviour {

	[Tooltip("Key binding to pick up item")]
	public KeyCode actionKey = KeyCode.E;

	private bool isMoving, isDumpable, shouldMove;
	private GameObject player;
	private Transform parent;


	void Start () {
		isMoving = false;
		isDumpable = false;
		shouldMove = false;

		parent = gameObject.transform.parent;
	} // end : start
	

	void Update () {
		// check for moving item
		if(isMoving) {
			// update item position to players
			Pickup();

			// check if action key was released at least once
			// before dumping item [bug fix]
			if(isDumpable) {
				if(Input.GetKeyDown(actionKey))
					Letdown();
			} else
				// check for action key release
				if(Input.GetKeyUp(actionKey)) {
					isDumpable = true;
				}
		}
	} // end : Update

	public void OnTriggerStay(Collider other) {
		if(other.CompareTag("Player")
			&& Input.GetKeyDown(actionKey)) {
			if(!isMoving) {
				// prepare for moving in update()
				isMoving = true;
				player = other.gameObject;
			}
		}
	}


	/* Pickup()
	 *  sets the item into the local transform of the player with a
	 *  little offset
	 */
	private void Pickup() {
		Vector3 position;

		// make relative to player
		gameObject.transform.parent = player.transform;

		// set offset
		position = player.transform.forward * 0.7f + new Vector3(0, 1, 0);

		// apply player position
		gameObject.transform.position = player.transform.position + position;
		gameObject.transform.forward = player.transform.forward;
	} // end : Pickup


	/* Letdown()
	 *  assigns the item back to it's original transform
	 */
	private void Letdown() {
		isMoving = false;
		isDumpable = false;

		gameObject.transform.parent = parent;
	} // end : Letdown
} // end : MoveItem
