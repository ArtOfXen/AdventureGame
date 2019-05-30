using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectedObjectMenuScript : MonoBehaviour
{
    //private ItemInteractionScript currentlyOpenItemMenuScript;

    public Button[] interactionButtons;
    private InteractableObjectScript.InteractionType[] currentPossibleInteractions;
    [HideInInspector] public InteractableObjectScript currentObjectScript;
    private int numberOfInteractionsForCurrentObject;
    private string nameOfCurrentObject;

    public Sprite examineButtonSprite;
    public Sprite useButtonSprite;
    public Sprite pickUpButtonSprite;
    public Sprite talkToButtonSprite;
    public Sprite goToButtonSprite;
    public Sprite combineButtonSprite;
    public Sprite separateButtonSprite;

    private GameObject player;

    private float gapBetweenButtons = 40f;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        numberOfInteractionsForCurrentObject = 0;
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void closeMenu()
    {
        GetComponent<Text>().enabled = false;

        for (int i = 0; i < numberOfInteractionsForCurrentObject; i++)
        {
            interactionButtons[i].gameObject.SetActive(false);
        }

        numberOfInteractionsForCurrentObject = 0;

    }

    public void objectClicked(InteractableObjectScript objectScript, Vector3 position)
    {
        // close menu if already open
        closeMenu();
        
        currentObjectScript = objectScript;
        GetComponent<Text>().enabled = true;
        nameOfCurrentObject = objectScript.data.actorName;
        GetComponent<Text>().text = nameOfCurrentObject;
        transform.position = position;
        currentPossibleInteractions = objectScript.interactions;
        numberOfInteractionsForCurrentObject = objectScript.numberOfInteractions;

        for (int i = 0; i < objectScript.numberOfInteractions; i++)
        {
            interactionButtons[i].gameObject.SetActive(true);

            switch (objectScript.interactions[i])
            {
                case InteractableObjectScript.InteractionType.Examine:
                    interactionButtons[i].GetComponent<Image>().sprite = examineButtonSprite; break;

                case InteractableObjectScript.InteractionType.GoTo:
                    interactionButtons[i].GetComponent<Image>().sprite = goToButtonSprite; break;

                case InteractableObjectScript.InteractionType.PickUp:
                    interactionButtons[i].GetComponent<Image>().sprite = pickUpButtonSprite; break;

                case InteractableObjectScript.InteractionType.TalkTo:
                    interactionButtons[i].GetComponent<Image>().sprite = talkToButtonSprite; break;

                case InteractableObjectScript.InteractionType.Use:
                    interactionButtons[i].GetComponent<Image>().sprite = useButtonSprite; break;

                case InteractableObjectScript.InteractionType.Separate:
                    interactionButtons[i].GetComponent<Image>().sprite = separateButtonSprite; break;
            }

            if (objectScript.numberOfInteractions == 2)
            {
                interactionButtons[i].transform.position = new Vector2(position.x - (gapBetweenButtons/2) + (i * gapBetweenButtons), position.y + 30);
            }

            else if (objectScript.numberOfInteractions == 3)
            {
                interactionButtons[i].transform.position = new Vector2(position.x - gapBetweenButtons + (i * gapBetweenButtons), position.y + 30);
            }
        }
    }

    public void buttonOnClick(int buttonNumber)
    {
        player.GetComponent<PlayerInputScript>().setQueuedAction(currentObjectScript.gameObject, currentPossibleInteractions[buttonNumber]);
        closeMenu();
    }

    public void buttonHoveredOver(int buttonNumber)
    {
        GetComponent<Text>().text = currentPossibleInteractions[buttonNumber].ToString();
    }

    public void endButtonHover()
    {
        GetComponent<Text>().text = nameOfCurrentObject;
    }
}
