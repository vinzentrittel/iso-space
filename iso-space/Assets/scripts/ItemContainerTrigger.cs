using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemContainerTrigger : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerStay(Collider other) {
		ItemContainerController parent =
			gameObject.GetComponentInParent<ItemContainerController>()
			as ItemContainerController;
		parent.OnTriggerStay(other);
	}
}
