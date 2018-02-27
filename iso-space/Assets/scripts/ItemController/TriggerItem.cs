using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerItem : MonoBehaviour {
	
	void OnTriggerStay(Collider other) {
		MonoBehaviour parent = null;

		if(parent == null)
			parent = GetComponentInParent<MoveItem>() as MonoBehaviour;
		if(parent == null)
			parent = GetComponentInParent<ItemContainer>() as MonoBehaviour;

		if(parent != null)
			if(parent is MoveItem) {
				(parent as MoveItem).OnTriggerStay(other);
				return;
			} else if(parent is ItemContainer) {
				(parent as ItemContainer).OnTriggerStay(other);
				return;
			}

		Debug.LogErrorFormat("'{0}' is no child to a movable item, or an item container.");
	} // end : OnTriggerStay
} // end : ItemTrigger
