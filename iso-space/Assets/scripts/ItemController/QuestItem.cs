using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestItem : MonoBehaviour {

	public string displayText = "pick up";
	public TextMesh hoverText;

	void Start () {
		hoverText.transform.localPosition = new Vector3(
			0,
			2 * transform.localScale.y,
			0);

		hoverText.alignment = TextAlignment.Center;
		hoverText.anchor = TextAnchor.LowerCenter;
		hoverText.fontSize = 100;
		hoverText.text = displayText;
	}

	void Update () {
		hoverText.transform.eulerAngles = new Vector3(0, 45, 0);
	}

	public void HideText() {
		Renderer renderer = hoverText.gameObject.GetComponent<Renderer>() as Renderer;
		renderer.enabled = false;
	}
}
