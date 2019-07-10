using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConversationScript : MonoBehaviour
{
    //private enum CauseOfDialogueChange
    //{ next, previous }

    [HideInInspector] public ConversationData currentConversation;
    ConversationData.DialogueExcerpt currentDialogueExcerpt;
    int currentDialogueLineIndex;

    [SerializeField] private GameObject inventoryGameObject;
    //public Button previousButton;
    public Button nextButton;
    public Image leftConversationalistSprite;
    public Image rightConversationalistSprite;
    public Text conversationText;

    bool areDecisionButtonsActive;
    public Button[] decisionButtons;
    int[] dialogueLineIDsForDecisionButtons;

    //private bool onFirstLineOfConversation;
    private bool onIrrelevantItemLine;
    private bool itemWasJustShown;
    private ConversationData.ItemReaction itemBeingShown;


    private void Start()
    {
        areDecisionButtonsActive = false;
        //onFirstLineOfConversation = true;
        onIrrelevantItemLine = false;
        itemWasJustShown = false;
        dialogueLineIDsForDecisionButtons = new int[4];
    }

    

    public void openConversation(ActorData itemCombinedWithCharacter, ConversationData newConversation)
    {
        int indexOfItemCombined = findIndexOfReactionItem(itemCombinedWithCharacter);

        currentConversation = newConversation;

        gameObject.SetActive(true);
        nextButton.gameObject.SetActive(true);
        //previousButton.gameObject.SetActive(true);
        nextButton.GetComponentInChildren<Text>().text = "Next";
        //previousButton.GetComponentInChildren<Text>().text = "Exit";

        areDecisionButtonsActive = false;
        //onFirstLineOfConversation = true;
        itemWasJustShown = false;

        GameManagerScript gmScript = FindObjectOfType<GameManagerScript>();
        gmScript.fadeOutBackground();
        gmScript.ConversationUIOpen = true;

        // hide inventory when conversation starts
        inventoryGameObject.GetComponent<InventoryScript>().hideInventory();

        if (indexOfItemCombined < 0)
            changeDialogueExcerpt(0);
        else
            changeDialogueExcerpt(currentConversation.itemReactions[indexOfItemCombined].idOfDialogueExcerpt);
    }

    void changeDialogueExcerpt(int indexOfDialogueExcerpt)
    {
        if (itemWasJustShown)
        {
            if (itemBeingShown.changeDialogueAfterShown)
            {
                currentConversation.itemReactions[findIndexOfReactionItem(itemBeingShown.item)].idOfDialogueExcerpt = itemBeingShown.idOfNewDialogueAfterShown;
                //itemBeingShown.idOfDialogueExcerpt = itemBeingShown.idOfNewDialogueAfterShown;
                itemWasJustShown = false;
            }
        }
        currentDialogueExcerpt = currentConversation.dialogue[indexOfDialogueExcerpt];

        // if wrong excerpt was found, look for it manually
        if (currentDialogueExcerpt.dialogueID != indexOfDialogueExcerpt)
        {
            for (int i = 0; i < currentConversation.dialogue.Length; i++)
            {
                if (currentConversation.dialogue[i].dialogueID == indexOfDialogueExcerpt)
                {
                    currentDialogueExcerpt = currentConversation.dialogue[i];
                    break;
                }
            }
        }

        leftConversationalistSprite.sprite = currentDialogueExcerpt.leftCharacter.sprite;
        rightConversationalistSprite.sprite = currentDialogueExcerpt.rightCharacter.sprite;

        changeDialogueLine(0); // show first line of new excerpt

        //else if (causeOfExcerptChange == CauseOfDialogueChange.previous)
        //    changeDialogueLine(currentDialogueExcerpt.dialogueLines.Length - 1, causeOfExcerptChange); // set to last line of excerpt
    }

    private void changeDialogueLine(int indexOfDialogueLine)
    {
        ConversationData.DialogueLine newLine = currentDialogueExcerpt.dialogueLines[indexOfDialogueLine];
        currentDialogueLineIndex = indexOfDialogueLine;
        conversationText.text = newLine.text;

        Image speakingCharacter; Image nonSpeakingCharacter;

        if (newLine.leftCharacterIsSpeaking) {
            speakingCharacter = leftConversationalistSprite;
            nonSpeakingCharacter = rightConversationalistSprite;
        }
        else {
            speakingCharacter = rightConversationalistSprite;
            nonSpeakingCharacter = leftConversationalistSprite;
        }

        speakingCharacter.color = new Color(255, 255, 255);
        nonSpeakingCharacter.color = new Color(135, 135, 135);
        speakingCharacter.transform.localScale = new Vector3(1.2f, 1.2f, 1f);
        nonSpeakingCharacter.transform.localScale = new Vector3(0.8f, 0.8f, 1f);

        // check if last line of this dialogue excerpt
        if (currentDialogueLineIndex >= currentDialogueExcerpt.dialogueLines.Length - 1)
        {
            // TODO: do currentDialogue.effect;

            // check if decision needs to be made
            if (currentDialogueExcerpt.decisionCanBeMade)
            {
                int buttonIndex = 0;
                areDecisionButtonsActive = true;
                for (int i = 0; i < currentDialogueExcerpt.decisions.Length; i++)
                {
                    if (currentDialogueExcerpt.decisions[i].unlocked)
                    {
                        decisionButtons[buttonIndex].gameObject.SetActive(true);
                        decisionButtons[buttonIndex].GetComponentInChildren<Text>().text = currentDialogueExcerpt.decisions[i].text;
                        dialogueLineIDsForDecisionButtons[buttonIndex] = currentDialogueExcerpt.decisions[i].idOfNextDialogueExcerpt;

                        buttonIndex++;
                    }
                }
                areDecisionButtonsActive = true;
                nextButton.gameObject.SetActive(false);
            }

            // check if there is a dialogue excerpt after this one
            else if (currentDialogueExcerpt.nextDialogueID < 0)
            { // if no dialogue after this one, change next button
                if (currentDialogueExcerpt.endOfConversation)
                {
                    nextButton.gameObject.SetActive(true);
                    nextButton.GetComponentInChildren<Text>().text = "Exit";
                    inventoryGameObject.GetComponent<InventoryScript>().unhideInventory();
                }
                else
                    nextButton.gameObject.SetActive(false);
            }
        }

        else
        { // set next button to be normal if not end of excerpt
            nextButton.GetComponentInChildren<Text>().text = "Next";
            nextButton.gameObject.SetActive(true);
        }
    }

    public void nextButtonPressed()
    {
        if (onIrrelevantItemLine)
        {
            onIrrelevantItemLine = false;
            changeDialogueLine(currentDialogueLineIndex);
        }
        else
        {
            if (currentDialogueLineIndex < currentDialogueExcerpt.dialogueLines.Length - 1)
                changeDialogueLine(currentDialogueLineIndex + 1);
            else
            {
                if (currentDialogueExcerpt.nextDialogueID < 0)
                {
                    exitConversation();
                }
                else
                {
                    changeDialogueExcerpt(currentDialogueExcerpt.nextDialogueID);
                }
            }
        }
    }

    public void decisionButtonPressed(int indexOfButton)
    {
        changeDialogueExcerpt(dialogueLineIDsForDecisionButtons[indexOfButton]);
        disableDecisionButtons();
    }

    public void showInventoryItem(ActorData itemShown)
    {
        disableDecisionButtons();
        inventoryGameObject.GetComponent<InventoryScript>().hideInventory();

        // returns -1 if not found
        int indexOfItemShown = findIndexOfReactionItem(itemShown);
        
        if (indexOfItemShown > -1)
        {
            itemBeingShown = currentConversation.itemReactions[indexOfItemShown];
            itemWasJustShown = true;
            if (itemBeingShown.interactionType == ConversationData.ItemInteractionType.give)
                FindObjectOfType<InventoryScript>().removeItem(itemShown);
            Debug.Log("Change Dialogue Excerpt due to relevant item being shown");
            changeDialogueExcerpt(itemBeingShown.idOfDialogueExcerpt);
        }
        else
        {
            onIrrelevantItemLine = true;
            leftConversationalistSprite.color = new Color(255, 255, 255);
            rightConversationalistSprite.color = new Color(135, 135, 135);
            leftConversationalistSprite.transform.localScale = new Vector3(1.2f, 1.2f, 1f);
            rightConversationalistSprite.transform.localScale = new Vector3(0.8f, 0.8f, 1f);

            nextButton.gameObject.SetActive(true);

            if (currentDialogueExcerpt.rightCharacter.gender == 'f')
                conversationText.text = "I don't want to talk to her about that.";
            else if (currentDialogueExcerpt.rightCharacter.gender == 'm')
                conversationText.text = "I don't want to talk to him about that.";
            else
                conversationText.text = "I don't want to talk to them about that.";
        }
    }

    private int findIndexOfReactionItem(ActorData item)
    {
        if (item != null)
        {
            for (int i = 0; i < currentConversation.itemReactions.Length; i++)
            {
                if (currentConversation.itemReactions[i].item.Equals(item))
                {
                    return i;
                }
            }
        }

        return -1;
    }

    private void disableDecisionButtons()
    {
        areDecisionButtonsActive = false;
        for (int i = 0; i < decisionButtons.Length; i++)
            decisionButtons[i].gameObject.SetActive(false);
    }

    private void exitConversation()
    {
        gameObject.SetActive(false);
        GameManagerScript gmScript = FindObjectOfType<GameManagerScript>();
        gmScript.fadeInBackground();
        gmScript.ConversationUIOpen = false;
    }
}
