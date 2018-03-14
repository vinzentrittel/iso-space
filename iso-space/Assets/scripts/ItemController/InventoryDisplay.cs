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

    public GameObject ItemIcon;
    public GameObject ItemSlot;
    

    void Start()
    {
        slots      = new List<GameObject>();
        slotsFree  = new List<int>();
        hasChanged = false;
        var slotPanel = transform.Find(DisplayPanelName).Find(SlotPanelName);

        inventory = transform.parent.GetComponent<Inventory>();
        items = inventory.dictItems;

        // populate inventory with slots
        for (int i = 0; i < SlotCount; i++)
        {
            var currentSlot = Instantiate(ItemSlot);
            currentSlot.transform.SetParent(slotPanel);
            slots.Add(currentSlot);

            slotsFree.Add(ItemDatabase.Item.INVALID_ID);
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

    public void RemoveItem()
    {
        hasChanged = true;
    }

    private bool NotHasItem(int id)
    {
        for (int i = 0; i < SlotCount; i++)
            if (slotsFree[i] == id)
                return false;
        return true;
    }

    private int NextSlot()
    {
        for (int i = 0; i < SlotCount; i++)
            if (slotsFree[i] == ItemDatabase.Item.INVALID_ID)
                return i;
        return SlotCount;
    }


    private void PopulateSlot(int slotId, string spriteName)
    {
        var item = InstantiateAt(ItemIcon, slots[slotId]);
        var icon = item.GetComponent<Image>();

        icon.sprite = ItemIconRegister.GetInstance().GetIcon(spriteName);
    }


    private GameObject InstantiateAt(GameObject src, GameObject trg)
    {
        var obj = Instantiate(src);
        // snap to parent
        obj.transform.SetParent(trg.transform);
        obj.transform.localPosition = Vector3.zero;

        return obj;
    }


    private void ThrowSlotRangeError()
    {
        var message = "Inventory hold more items than there are available slots in the display.";
        message += " Currently " + SlotCount.ToString() + " slots available.";

        throw new ArgumentOutOfRangeException(paramName: message);
    }
}
