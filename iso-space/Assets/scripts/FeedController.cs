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
	private Outline slotSelect;
	private bool hasChanged;
	private double lastUpdate;
	private float alphaMenu;
	private float alphaSlot;

	[Tooltip("Drop a reference to the label, you want the feed displayed on.")]
	public GameObject feedLabel;
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

		// reference highlight
		selectionColor.a = 0.0f;
		slotSelect = slots[0].obj.GetComponent<Outline>();
		slotSelect.effectColor = selectionColor;

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

			lastUpdate = Time.realtimeSinceStartup;
			alphaMenu = alphaSlot = 1.0f;
			hasChanged = false;
		}

		// auto hide menu
		if(alphaMenu > 0.0f) {
			alphaMenu = smoothInterp((float) (Time.realtimeSinceStartup - lastUpdate), 0.0f, fadeAwayFeed);
			Hide(alphaMenu);
		}

		// higlight slot marker
		if(alphaSlot > 0.0f) {
			alphaSlot = smoothInterp((float) (Time.realtimeSinceStartup - lastUpdate), 0.0f, fadeAwaySlot);
			selectionColor.a = alphaSlot;
			slotSelect.effectColor = selectionColor;
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
	private static InfoTuple BakeInfo(int id, string eventDescr = " added to inventory.") {
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
