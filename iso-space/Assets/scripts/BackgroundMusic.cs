using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundMusic : MonoBehaviour {

	public AudioSource normalMusic;
	public AudioSource panicMusic;

	bool isNormal = true;

	// Use this for initialization
	void Start () {
		normalMusic.Play ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
