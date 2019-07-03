using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryScript : MonoBehaviour
{
    public const int INVENTORY_SIZE = 16;

    public Image[] itemImages = new Image[INVENTORY_SIZE];
    public ActorData[] items = new ActorData[INVENTORY_SIZE];


    public void addItem(ActorData newItem)
    {
        // start at index 1 because index 0 will always be the notebook
        for (int i = 1; i < items.Length; i++)
        {
            if (items[i] == null)
            {
                items[i] = newItem;
                itemImages[i].sprite = newItem.inventoryImage;
                itemImages[i].enabled = true;
                return;
            }
        }
        Debug.Log("Inventory Full. InventoryScript ln27.");
    }

    public void removeItem(ActorData itemToRemove)
    {
        // start at index 1 because index 0 will always be the notebook
        for (int i = 1; i < items.Length; i++)
        {
            if (items[i] == itemToRemove)
            {
                items[i] = null;
                itemImages[i].sprite = null;
                itemImages[i].enabled = false;
                return;
            }
        }
        Debug.Log("Item to remove not found. InventoryScript ln43.");
    }
}
