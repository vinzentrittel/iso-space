using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour, TriggerObject.Action {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	// Detecting when something pass the door
	void OnTriggerEnter(Collider other){
		if (other.CompareTag("Player")){
			Debug.Log ("Player Entered Gate");
		}
	}

	public void execute() {
		Debug.Log("Should open the door");
	}
}
