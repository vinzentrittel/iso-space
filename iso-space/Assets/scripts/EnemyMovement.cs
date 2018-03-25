using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour {



	Transform player;
	NavMeshAgent nav;

	// Use this for initialization
	void Awake () {
		player = GameObject.FindGameObjectWithTag ("Player").transform;
		nav = GetComponent<NavMeshAgent> ();
	}
	
	// Update is called once per frame
	void Update () {
		nav.SetDestination (player.position);
	}

	void OnTriggerEnter(Collider other){
		if (other.CompareTag("Player"))
			Debug.Log ("Trigger Collider: Close to player");
	}
}
