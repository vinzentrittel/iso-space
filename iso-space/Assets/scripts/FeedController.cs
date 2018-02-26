using System.Collections;
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
public class FeedController : MonoBehaviour {

	private List<SlotTuple> slots;
	private Queue<InfoTuple> lastItems;
	private bool hasChanged;
	private double lastUpdate;
	private float alphaMenu;

	[Tooltip("Drop a reference to the label, you want the feed displayed on.")]
	public GameObject feedLabel;
	[Tooltip("Set how long the feed will be displayed")]
	public float fadeAwayTime = 1.0f;

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
		LayoutGroup layout;
		SlotTuple data;

		alphaMenu = 0.0f;
		lastUpdate = Time.realtimeSinceStartup;
		hasChanged = false;
		slots = new List<SlotTuple>();
		lastItems = new Queue<InfoTuple>();

		// get object, image, and text references to all slots
		foreach(Transform child in feedLabel.transform) {
			data = BakeSlot(child.gameObject);
			slots.Add(data);
		}

		// hide all UI elements for now (there's nothing to see yet)
		HideAll(alphaMenu);

		// check for attached grid layout
		layout = feedLabel.GetComponent<LayoutGroup>() as LayoutGroup;
		if(layout == null)
			Debug.LogErrorFormat(
				"No LayoutGroup component attached to '{0}'.",
				feedLabel
			);
	} // end : Start
	

	void Update () {
		int reverseIndex;

		// check for preceding status updates
		if(hasChanged) {
			// print most recent statuses to available slots
			// newest first, hence reverseIndex
			reverseIndex = lastItems.Count - 1;
			foreach(InfoTuple info in lastItems) {
				slots[reverseIndex].img.sprite = info.sprite;
				slots[reverseIndex].info.text = info.descr;
				reverseIndex--;
			}

			// display non-empty feed
			//Unhide();

			alphaMenu = 1.0f;
			lastUpdate = Time.realtimeSinceStartup;
			hasChanged = false;
		}

		// auto hide menu
		if(alphaMenu > 0.0f) {
			alphaMenu = smoothInterp(Time.realtimeSinceStartup - lastUpdate, 0.0f, fadeAwayTime);
			Hide(alphaMenu);
		}
	} // end : Update


	/* AddItem()
	 *  saves relevant item information.
	 *  If number of updates exeeds the number of slots,
	 *  the oldest message will be deleted
	 */
	public void AddItem(int id) {
		// get relevant information to structure
		InfoTuple info = BakeInfo(id);

		lastItems.Enqueue(info);

		// remove oldest message
		if(lastItems.Count > slots.Count)
			lastItems.Dequeue();

		hasChanged = true;
	} // end : AddItem


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
	private InfoTuple BakeInfo(int id) {
		InfoTuple info;
		ItemDatabase.Item item;

		// fetch data
		item = ItemDatabase.FetchItemById(id);

		info.descr = item.Name + " added to inventory.";
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


	private float smoothInterp(double t, float start = 0.0f, float end = 1.0f) {
		if(t < start) return 1.0f;
		else if(t > end) return 0.0f;

		// [0,pi] -> [1,-1] => [start,end] -> [0,1]
		float normT = (float) t * Mathf.PI / (end - start);

		// damp
		float result = Mathf.Cos(normT);

		// target interval [0,1]
		return result * 0.5f + 0.5f;
	}
} // end : FeedController
