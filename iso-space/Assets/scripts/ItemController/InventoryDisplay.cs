using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryDisplay : MonoBehaviour {

    private const int SlotCount = 20;
    private const string DisplayPanelName = "Display Panel";
    private const string SlotPanelName    = "Slot Panel";

    private Inventory inventory;
    private Dictionary<int, Inventory.TupleItem> items;
    private List<GameObject> slots;
    private List<int> slotsFree;

    private bool hasChanged;

    public GameObject itemIcon;
    public GameObject itemCountText;
    public GameObject itemSlot;
    

    void Start()
    {
        slots      = new List<GameObject>();
        slotsFree  = new List<int>();
        hasChanged = false;
        var slotPanel = transform.Find(DisplayPanelName).Find(SlotPanelName);

        inventory = transform.parent.GetComponent<Inventory>();
        UpdateItems();

        // populate inventory with slots
        for (int i = 0; i < SlotCount; i++)
        {
            var currentSlot = Instantiate(itemSlot);
            currentSlot.transform.SetParent(slotPanel);
            slots.Add(currentSlot);

            slotsFree.Add(ItemDatabase.Item.INVALID_ID);
        }
    }


    void Update()
    {
        if (hasChanged)
        {
            for (var i = 0; i < SlotCount; i++)
            {
                if (slotsFree[i] != ItemDatabase.Item.INVALID_ID)
                {
                    UpdateItemCount(i);
                }
            }

            hasChanged = false;
        }
    }


    public void AddItem(int id)
    {
        hasChanged = true;

        if (NotHasItem(id))
        {
            var slot = NextSlot();
            if (slot < SlotCount)
            {
                PopulateSlot(slot, ItemDatabase.FetchItemById(id).Handle);
                // block this slot space
                slotsFree[slot] = id;
            }
            else
                ThrowSlotRangeError();
        }
    }

    public void RemoveItem(int id)
    {
        hasChanged = true;

        UpdateItems();
        if (NotHasItem(id)) { ClearSlotWith(id); }
    }

    private bool NotHasItem(int id)
    {
        int index = NextSlot(id);

        if (index < SlotCount && items.ContainsKey(id) && items[id].count > 0)
            return false;
        else
            return true;
    }

    private int NextSlot(int itemId = ItemDatabase.Item.INVALID_ID)
    {
        for (int i = 0; i < SlotCount; i++)
            if (slotsFree[i] == itemId)
                return i;
        return SlotCount;
    }


    private void PopulateSlot(int slotId, string spriteName)
    {
        var item = InstantiateAt(itemIcon, slots[slotId]);
        var icon = item.GetComponent<Image>();

        icon.sprite = ItemIconRegister.GetInstance().GetIcon(spriteName);
    }

    private void ClearSlotWith(int itemId)
    {
        int slotId = NextSlot(itemId);
        if (slotId == SlotCount) { return; }

        while(slots[slotId].transform.childCount > 0)
        {
            Transform child = slots[slotId].transform.GetChild(0);
            child.SetParent(null);
            Destroy(child.gameObject);
        }

        DisplayElements();

        slotsFree[slotId] = ItemDatabase.Item.INVALID_ID;
    }


    private void UpdateItems()
    {
        this.items = inventory.dictItems;
    }


    private void UpdateItemCount(int slotId)
    {
        var itemId    = slotsFree[slotId];
        var itemCount = items[itemId].count;

        if (itemCount > 1)
        {
            GameObject textBox;
            var textComp = slots[slotId].GetComponentInChildren<Text>();
            if (textComp == null)
                textBox = InstantiateAt(itemCountText, slots[slotId], itemCountText.transform.localPosition);
            else
                textBox = textComp.gameObject;

            var textMesh = textBox.GetComponent<Text>();
            textMesh.text = itemCount.ToString();
        }
        else if (itemCount == 1)
        {
            var textMesh = slots[slotId].GetComponentInChildren<Text>();
            if (textMesh) { textMesh.text = ""; }
        }
    }


    private GameObject InstantiateAt(GameObject src, GameObject trg, Vector3 localPosition = default(Vector3))
    {
        var obj = Instantiate(src);
        // snap to parent
        obj.transform.SetParent(trg.transform);
        obj.transform.localPosition = localPosition;

        return obj;
    }


    private void ThrowSlotRangeError()
    {
        var message = "Inventory hold more items than there are available slots in the display.";
        message += " Currently " + SlotCount.ToString() + " slots available.";

        throw new ArgumentOutOfRangeException(paramName: message);
    }

    private void DisplayElements()
    {
        for(int i = 0; i < SlotCount; i++)
            foreach(Transform child in slots[i].transform)
            {
                Debug.LogFormat("{0}: {1}", slots[i].name, child.gameObject.name);
            }
    }
}
