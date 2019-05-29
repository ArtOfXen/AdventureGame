using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryScript : MonoBehaviour
{
    public const int inventorySize = 16;

    public Image[] itemImages = new Image[inventorySize];
    public ActorData[] items = new ActorData[inventorySize];


    public void addItem(ActorData newItem)
    {
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] == null)
            {
                items[i] = newItem;
                itemImages[i].sprite = newItem.inventoryImage;
                itemImages[i].enabled = true;
                return;
            }
        }
    }

    public void removeItem(ActorData itemToRemove)
    {
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] == itemToRemove)
            {
                items[i] = null;
                itemImages[i].sprite = null;
                itemImages[i].enabled = false;
                return;
            }
        }
    }
}
