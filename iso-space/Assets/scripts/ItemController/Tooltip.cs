using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour {

    private const string TooltipName = "Tooltip";

    private ItemDatabase.Item item;
    private string content;
    private GameObject tooltip;


    void Start()
    {
        tooltip = GameObject.Find(TooltipName);
    }

    void Update()
    {
        if(tooltip.activeSelf)
        {
            tooltip.transform.position = Input.mousePosition;
        }
    }

    public void Activate(int itemId)
    {
        if (itemId == ItemDatabase.Item.INVALID_ID)
            return;

        item = ItemDatabase.FetchItemById(itemId);
        ConstructDataString();
        tooltip.SetActive(true);
    }


    public void Deactivate()
    {
        tooltip.SetActive(false);
    }


    public void ConstructDataString()
    {
        content = "<color=#000000><b>" + item.Name + "</b></color>\n" 
            + "<color=#000020>" + item.Description + "</color>";
        tooltip.GetComponentInChildren<Text>().text = content;
    }
}
