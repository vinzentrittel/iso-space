using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundMusic : MonoBehaviour {

	public AudioClip music_Normal;
	public AudioClip music_Panic;
	AudioSource backgroundMusic;
	// public AudioSource normalMusic;
	// public AudioSource panicMusic;

	bool isNormal = true;

	// Use this for initialization
	void Start () {
		backgroundMusic = GetComponent<AudioSource> ();
		// backgroundMusic.clip = music_Normal;
	}
	
	// Update is called once per frame
	void Update () {

		// Debugging function
		if (Input.GetKeyDown (KeyCode.Alpha1)) {
			isNormal = !isNormal;
			ChangeMusic ();
			backgroundMusic.Play ();
		}
	}

	void ChangeMusic(){
		if (isNormal == false)
			backgroundMusic.clip = music_Panic;
		else
			backgroundMusic.clip = music_Normal;
	}
}
