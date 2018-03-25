using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour {

	public Transform[] targets;
	public float distanceToFollow = 0.5f;

	Transform player;
	NavMeshAgent nav;
	private int destPoint = 0;
	private bool patroling = true; //checks if enemy is folowing player or patroling

	// Use this for initialization
	void Awake () {
		player = GameObject.FindGameObjectWithTag ("Player").transform;
		nav = GetComponent<NavMeshAgent> ();
	}

	void Start () {
		nav.autoBraking = false;
		GoToNextPoint ();
	}
	
	// Update is called once per frame
	void Update () {
		// Choose the next destination point when the agent gets
		// close to the current one.
		if (!nav.pathPending && (nav.remainingDistance < distanceToFollow) && patroling)
			GoToNextPoint();

		//follow player when not in patroling state
		if (!nav.pathPending && !patroling ) {
			nav.SetDestination (player.position);
		}

	
	}

	void OnTriggerEnter(Collider other){
		//follow player when he is in range
		if (other.CompareTag ("Player")) {
			Debug.Log ("Close to player, following player");
			patroling = false;
			nav.SetDestination (player.position);
		}
	}


	void OnTriggerExit(Collider other) {
		if (other.CompareTag ("Player")) {
			Debug.Log ("Player out of range, resuming patrol");
			patroling = true;
			GoToNextPoint ();
		}
	}


	/*Switches targets for the enemy*/
	void GoToNextPoint () {
		if (targets.Length == 0)
			return;
		nav.destination = targets[destPoint].position;
		//automatically cycles to new destpoint after last one is reached
		//TODO: This will break when you run out of int's.
		destPoint = (destPoint + 1) % targets.Length; 
	}
}
