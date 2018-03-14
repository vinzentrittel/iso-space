using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {


    private string DisplayName = "Display";
    private InventoryDisplay inventory;

    public Color selectionColor = new Color(0, 210, 255);

    public int SlotId { set; private get; }

    void Start()
    {
        inventory = FindObjectOfType(typeof(InventoryDisplay)) as InventoryDisplay;
        SetAlpha(0);

        if (!inventory)
            Debug.LogError("Inventory Display script could not be found in scene.");
    }


    public void OnPointerEnter(PointerEventData eventData) {
        SetAlpha(255);
        inventory.SelectedSlot = SlotId;
    }

    public void OnPointerExit (PointerEventData eventData)
    {
        SetAlpha(0);
        inventory.SelectedSlot = InventoryDisplay.InvalidSlot;
    }

    private void SetAlpha(int alpha)
    {
        selectionColor.a = alpha;
        var slotSelect = GetComponent<Outline>();
        slotSelect.effectColor = selectionColor;
    }
}
