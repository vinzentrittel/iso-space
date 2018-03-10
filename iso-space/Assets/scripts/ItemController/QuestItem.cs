using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestItem : MonoBehaviour {

	public string displayText = "pick up";

	// Use this for initialization
	void Start () {
		Transform parent = transform.parent;
		transform.localPosition = new Vector3(
			0,
			2 * parent.localScale.y,
			0);

		TextMesh text = GetComponent<TextMesh>() as TextMesh;
		text.alignment = TextAlignment.Center;
		text.anchor = TextAnchor.LowerCenter;
		text.fontSize = 100;
		text.text = displayText;
	}
	
	// Update is called once per frame
	void Update () {
		transform.eulerAngles = new Vector3(0, 45, 0);
	}
}
