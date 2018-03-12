using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour, TriggerObject.Action {

	private Collider entrance;

	// Use this for initialization
	void Start () {
		entrance = gameObject.GetComponentInParent<BoxCollider> ();
		entrance.isTrigger = false;
	}

	public void execute() {
		Open();
	}

	private void Open() {
		entrance.isTrigger = true;
	}
}
