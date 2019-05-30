using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemInteractionScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    bool mouseOver;
    bool menuOpen;

    public Text itemNameUI;
    public int itemSlotIndex;

    public Sprite examineButtonSprite;
    public Sprite useButtonSprite;
    public Sprite combineButtonSprite;
    public Sprite separateButtonSprite;

    public Button[] interactionButtons;
    private InteractableObjectScript.InteractionType[] interactions;

    [HideInInspector] public ActorData dataOfItemInSlot;

    InventoryScript inventory;

    // Start is called before the first frame update
    void Start()
    {
        inventory = FindObjectOfType<InventoryScript>();
        menuOpen = false;
        mouseOver = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (itemNameUI.enabled && dataOfItemInSlot != inventory.items[itemSlotIndex])
        {
            dataOfItemInSlot = inventory.items[itemSlotIndex];
            itemNameUI.text = dataOfItemInSlot.actorName;
        }
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (inventory.items[itemSlotIndex] != null)
        {
            dataOfItemInSlot = inventory.items[itemSlotIndex];
            itemNameUI.text = dataOfItemInSlot.actorName;
            itemNameUI.enabled = true;
            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInputScript>().setHighlightedInventoryItem(this);
        }
        mouseOver = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!menuOpen)
            itemNameUI.enabled = false;
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInputScript>().stopHighlightingInventoryItem(this);
        mouseOver = false;
    }

    public void openItemMenu()
    {
        InteractableObjectScript.InteractionType[] interactionsOtherThanExamine = inventory.items[itemSlotIndex].interactionsOtherThanExamine_inventoryItem;
        interactions = new InteractableObjectScript.InteractionType[interactionsOtherThanExamine.Length + 1];
        interactions[0] = InteractableObjectScript.InteractionType.Examine;

        for (int i = 0; i < interactionsOtherThanExamine.Length; i++)
        {
            interactions[i + 1] = interactionsOtherThanExamine[i];
        }

        for (int i = 0; i < interactions.Length; i++)
        {
            interactionButtons[i].GetComponent<Image>().enabled = true;

            switch (interactions[i])
            {
                case InteractableObjectScript.InteractionType.Examine:
                    interactionButtons[i].GetComponent<Image>().sprite = examineButtonSprite;
                    break;
                case InteractableObjectScript.InteractionType.Use:
                    interactionButtons[i].GetComponent<Image>().sprite = useButtonSprite;
                    break;
                case InteractableObjectScript.InteractionType.Combine:
                    interactionButtons[i].GetComponent<Image>().sprite = combineButtonSprite;
                    break;
                case InteractableObjectScript.InteractionType.Separate:
                    interactionButtons[i].GetComponent<Image>().sprite = separateButtonSprite;
                    break;
                default:
                    Debug.Log("Item in item slot " + itemSlotIndex + " has an incompatable interaction set");
                    break;
            }
        }

        menuOpen = true;
    }

    public void interactionButtonClicked(int buttonNumber)
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInputScript>().stopMovement();

        switch (interactions[buttonNumber])
        {
            case InteractableObjectScript.InteractionType.Examine:
                string examineText = inventory.items[itemSlotIndex].examineText;
                GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInputScript>().enableExamineObjectText(examineText);
                break;

            case InteractableObjectScript.InteractionType.Combine:
                GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInputScript>().enableCombineWithText(this);
                break;

            case InteractableObjectScript.InteractionType.Separate:
                FindObjectOfType<GameManagerScript>().GetComponent<GameManagerScript>().separateActor(this);
                break;

            default:
                Debug.Log("Interaction not coded");
                break;
        }

        closeItemMenu();
    }

    public void buttonHoveredOver(int buttonNumber)
    {
        itemNameUI.text = interactions[buttonNumber].ToString();
    }

    public void endButtonHoverOver()
    {
        itemNameUI.text = dataOfItemInSlot.actorName;
    }

    public void closeItemMenu()
    {
        for (int i = 0; i < interactions.Length; i++)
        {
            interactionButtons[i].GetComponent<Image>().enabled = false;
        }
        
        if (!mouseOver)
            itemNameUI.enabled = false;

        menuOpen = false;
    }
}
