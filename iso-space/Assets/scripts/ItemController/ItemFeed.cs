﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/* FeedController
 *  Displays changes on the inventory.
 *  
 *  Requirements:
 *  - gameObject must be child to inventory with InventoryController
 *  - feedLable must have some LayoutGroup component attached
 *  - feedLabel must follow a structures, as follows :
 *    feedLabel : Label
 *      - slotname : Label
 *        - imagename : Image
 *        - textname : Text
 *      - slotname : label
 *        - ...
 *      - ...
 */
public class ItemFeed : MonoBehaviour {

	private List<SlotTuple> slots;
	private Queue<InfoTuple> lastItems;
	private Queue<InfoTuple> pendingItems;
	private Outline slotSelect;
	private double latestUpdate;
	private float alphaMenu;
	private float alphaSlot;

	[Tooltip("Drop a reference to the label, you want the feed displayed on.")]
	public GameObject feedPanel;
	[Tooltip("Set how long the feed will be displayed.")]
	public float fadeAwayFeed = 1.0f;
	[Tooltip("Set how long the 'slot updated'-highlight will be displayed.")]
	public float fadeAwaySlot = 1.0f;
	[Tooltip("Set color of the slot highlight.")]
	public Color selectionColor;


	struct InfoTuple {
		public Sprite sprite;
		public string descr;
	}
	struct SlotTuple {
		public GameObject obj;
		public Image img;
		public Text info;
	}


	void Start () {
		alphaMenu = 0.0f;
		alphaSlot = 0.0f;
		latestUpdate = Time.realtimeSinceStartup;
		slots = new List<SlotTuple>();
		lastItems = new Queue<InfoTuple>();
		pendingItems = new Queue<InfoTuple>();

		// get object, image, and text references to all slots
		foreach(Transform child in feedPanel.transform) {
			SlotTuple data = BakeSlot(child.gameObject);
			slots.Add(data);
		}

		// reference highlight
		selectionColor.a = 0.0f;
		slotSelect = slots[0].obj.GetComponent<Outline>();
		slotSelect.effectColor = selectionColor;

		// hide all UI elements for now (there's nothing to see yet)
		HideAll(alphaMenu);

		if(feedPanel.GetComponent<LayoutGroup>() == null)
			Debug.LogErrorFormat(
				"No LayoutGroup component attached to '{0}'.",
				feedPanel
			);
	} // end : Start
	

	void Update () {
		// check for preceding status updates
		if(pendingItems.Count != 0 && alphaSlot == 0.0f) {
			AddPending();
			UpdateDisplay();
		}

		// auto hide menu
		if(alphaMenu > 0.0f) {
			// make it more transparent, step by step
			alphaMenu = smoothInterp((float) (Time.realtimeSinceStartup - latestUpdate), 0.0f, fadeAwayFeed);
			Hide(alphaMenu);
		}

		// higlight slot marker
		if(alphaSlot > 0.0f) {
			// make it more transparent step by step
			alphaSlot = smoothInterp((float) (Time.realtimeSinceStartup - latestUpdate), 0.0f, fadeAwaySlot);
			selectionColor.a = alphaSlot;
			slotSelect.effectColor = selectionColor;
		}
	} // end : Update


	private void UpdateDisplay() {
		
		int reverseIndex = lastItems.Count - 1;
		foreach(InfoTuple info in lastItems) {
			slots[reverseIndex].img.sprite = info.sprite;
			slots[reverseIndex].info.text = info.descr;
			reverseIndex--;
		}

		// start feed animations
		alphaMenu = alphaSlot = 1.0f;
		latestUpdate = Time.realtimeSinceStartup;
	} // end : UpdateDisplay


	/* AddItem()
	 *  Triggers feed to display the new item
	 */
	public void AddItem(int id) {
		AddUpdate(id, " was added to inventory");
	} // end : AddItem


	/* RemoveItem()
	 *  Triggers feed to display item removal
	 */
	public void RemoveItem(int id) {
		AddUpdate(id, " was removed from the inventory");
	} // end : RemoveItem


	/* FullStack()
	 *  Triggers feed to display a full item stack
	 */
	public void FullStack(int id) {
		AddUpdate(id, " could not be stored.\nThere is no more space for this item");
	} // end : FullStack


	/* MissingItem()
	 *  Triggers feed to display an item is not present in the inventory
	 */
	public void MissingItem(int id) {
		AddUpdate(id, " is needed. Sorry!\nThat is not in your inventory.");
	}


	/* AddUpdate()
	 *  Adds a new item to pending, to be displayed in the feed
	 */
	private void AddUpdate(int id, string eventDescr) {
		// get relevant information to structure
		InfoTuple info = BakeInfo(id, eventDescr);

		// set event pending for displaying
		pendingItems.Enqueue(info);
	} // end : AddUpdate


	/* AddPending()
	 *  sends oldest pending update to be displayed in the feed
	 */
	private void AddPending() {
		InfoTuple info = pendingItems.Dequeue();
		lastItems.Enqueue(info);

		// check for feed overflow
		if(lastItems.Count > slots.Count)
			lastItems.Dequeue();
	} // end : AddPending


	/* BakeSlot()
	 *  stores references to Image and Text components for the
	 *  requested slot in a structure
	 */
	private SlotTuple BakeSlot(GameObject obj) {
		SlotTuple data;
		Transform node;

		node = obj.transform;
		data.obj = obj;
		data.img = node.GetChild(0).GetComponent<Image>() as Image;
		data.info = node.GetChild(1).GetComponent<Text>() as Text;

		return data;
	} // end : BakeSlot


	/* BakeInfo()
	 *  stores image and text output information for item id in
	 *  a structure
	 */
	private static InfoTuple BakeInfo(int id, string eventDescr = " was added to inventory.") {
		InfoTuple info;
		ItemDatabase.Item item;

		// fetch data
		item = ItemDatabase.FetchItemById(id);

		info.descr = item.Name + eventDescr;
		info.sprite = ItemIconRegister.GetInstance().GetIcon(item.Handle);

		return info;
	} // end : BakeInfo


	/* Hide()
	 *  Applys passed alpha value to this gameObject and all active
	 *  slots. A valid slot has stored a message at least once.
	 */
	private void Hide(float alpha = 0) {
		
		gameObject.GetComponentInChildren<CanvasRenderer>().SetAlpha(alpha);

		for(int i = 0; i < lastItems.Count; i++)
			SetSlotAlpha(i, alpha);
	} // end : Hide
	private void Unhide() { Hide(1); } // helper for Hide()


	/* HideAll()
	 *  Applys alpha value to to this gameObject and all slots
	 */
	private void HideAll(float alpha = 0) {
		gameObject.GetComponentInChildren<CanvasRenderer>().SetAlpha(alpha);

		for(int i = 0; i < slots.Count; i++)
			SetSlotAlpha(i, alpha);
	} // end : HideAll


	/* SetSlotAlpha()
	 */
	private void SetSlotAlpha(int index, float alpha) {
		foreach(CanvasRenderer renderer in slots[index].obj.GetComponentsInChildren<CanvasRenderer>())
			renderer.SetAlpha(alpha);
	} // end : SetSlotAlpha


	/* smoothInterp()
	 *  smoothens out an interpolation from domain [start,end] to interval [0,1] with
	 *  time parameter t. The interpolation will be smoothed out by the cosine in
	 *  the domain [0,pi] and projected on [0,1], instead of [-1,1].
	 *  Hence note, that t = start produces 1 and t = end produce 0 as a result.
	 */
	private static float smoothInterp(float t, float start = 0.0f, float end = 1.0f) {
		float normalizedParam;
		float result;

		if(t < start) return 1.0f;
		else if(t > end) return 0.0f;

		// [0,pi] -> [1,-1] => [start,end] -> [0,1]
		normalizedParam = (float) t * Mathf.PI / (end - start);

		// damp
		result = Mathf.Cos(normalizedParam);

		// target interval [0,1]
		return result * 0.5f + 0.5f;
	} // end : smoothInterp
} // end : FeedController
