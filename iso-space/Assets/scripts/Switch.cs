using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch : MonoBehaviour {

	bool isInRange = false;
	Collider entrance;

	// Use this for initialization
	void Start () {
		entrance = gameObject.GetComponentInParent<BoxCollider> ();
		entrance.isTrigger = false;
		Debug.Log (entrance.gameObject.name);
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.E)) {
			if (isInRange) {
				entrance.isTrigger = !entrance.isTrigger;
				Debug.Log ("Door Open: " + entrance.isTrigger);
			}
		}
	}

	// Trigger when player is in range of switch
	void OnTriggerEnter(Collider other){
		if (other.CompareTag ("Player")) {
			Debug.Log ("Player is in range");
			isInRange = true;
		}
	}

	// Trigger when player exit range
	void OnTriggerExit(Collider other){
		if (other.CompareTag ("Player")) {
			Debug.Log ("Player is not in range");
			isInRange = false;
		}
	}
}
