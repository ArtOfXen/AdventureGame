using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpObjectScript : MonoBehaviour
{
    [HideInInspector] public ActorData data;

    private InventoryScript inventory;

    private void Start()
    {
        inventory = FindObjectOfType<InventoryScript>();
        data = GetComponent<InteractableObjectScript>().data;
    }

    public void addItemToInventory()
    {
        inventory.addItem(data);
        Destroy(gameObject);
    }
}
