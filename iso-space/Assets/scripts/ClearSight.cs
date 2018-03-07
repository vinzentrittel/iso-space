using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearSight : MonoBehaviour {


	public Material TransparentMaterial = null;
	public float FadeInTimeout = 0.6f;
	public float FadeOutTimeout = 0.2f;
	public float TargetTransparency = 0.3f;

	public float DistanceToPlayer = 5.0f;
	public Transform playerTransform;
	public Transform cameraTransform;
	public Vector3 rayDirection;
	//public int layerMask;

	private void Awake(){
		DistanceToPlayer = (cameraTransform.position - playerTransform.position).magnitude;
		rayDirection = (cameraTransform.position - playerTransform.position).normalized;
		//layerMask = 
	}

	private void Update() {
		RaycastHit[] hits; // you can also use CapsuleCastAll() 
		// TODO: setup your layermask it improve performance and filter your hits. 
		// Only do mask if special needs arise
		Debug.DrawRay(playerTransform.position, rayDirection);
		hits = Physics.RaycastAll(playerTransform.position, rayDirection, DistanceToPlayer);
		foreach (RaycastHit hit in hits) {
			Renderer R = hit.collider.GetComponent<Renderer>();
			if (R == null) {
				continue;
			}
			// no renderer attached? go to next hit 
			// TODO: maybe implement here a check for GOs that should not be affected like the player
			AutoTransparent AT = R.GetComponent<AutoTransparent>();
			if (AT == null) // if no script is attached, attach one
			{
				AT = R.gameObject.AddComponent<AutoTransparent>();
				AT.TransparentMaterial = TransparentMaterial;
				AT.FadeInTimeout = FadeInTimeout;
				AT.FadeOutTimeout = FadeOutTimeout;
				AT.TargetTransparency = TargetTransparency;
			}
			AT.BeTransparent(); // get called every frame to reset the falloff
		}
	}
}
