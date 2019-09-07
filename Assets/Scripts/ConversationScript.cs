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
    private CharacterScript npcCharacterScript;

    [SerializeField] private GameObject inventoryGameObject;
    [SerializeField] private GameObject notebookGameObject;
    [SerializeField] private GameObject infoTextGameObject;
    //public Button previousButton;
    public Button nextButton;
    public Image leftConversationalistSprite;
    public Image rightConversationalistSprite;
    public Text leftConversationalistName;
    public Text rightConversationalistName;
    public Text conversationText;

    private Image mainCharacterSprite;
    private string mainCharacterName;

    bool areDecisionButtonsActive;
    public Button[] decisionButtons;
    int[] dialogueLineIDsForDecisionButtons;

    private ConversationData.ItemReaction itemBeingShown;
    private bool inventoryAvailableDuringConversation;

    private void Start()
    {
        areDecisionButtonsActive = false;
        dialogueLineIDsForDecisionButtons = new int[4];
        mainCharacterSprite = leftConversationalistSprite;
        mainCharacterName = leftConversationalistName.text;
    }

    public void openConversationWithCharacter(ActorData itemCombinedWithCharacter, ConversationData newConversation, CharacterScript characterScriptForNPC)
    {
        int indexOfItemCombined = findIndexOfReactionItem(itemCombinedWithCharacter);

        inventoryAvailableDuringConversation = true;

        currentConversation = newConversation;

        npcCharacterScript = characterScriptForNPC;

        rightConversationalistName.text = characterScriptForNPC.characterData.characterName;

        gameObject.SetActive(true);
        nextButton.gameObject.SetActive(true);
        nextButton.GetComponentInChildren<Text>().text = "Next";

        areDecisionButtonsActive = false;

        GameManagerScript.gameManager.fadeOutBackground();
        GameManagerScript.gameManager.ConversationUIOpen = true;

        // hide inventory when conversation starts
        inventoryGameObject.GetComponent<InventoryScript>().hideInventory();

        if (indexOfItemCombined < 0)
            changeDialogueExcerpt(0);
        else
            changeDialogueExcerpt(currentConversation.itemReactions[indexOfItemCombined].idOfDialogueExcerpt);
    }
    
    public void examineActor(ActorData examinedActor, bool actorIsWorldObject)
    {
        currentConversation = examinedActor.examineData;
        int dialogueID;
        if (actorIsWorldObject)
            dialogueID = 0;
        else
            dialogueID = 1;

        if (currentConversation.dialogue[dialogueID].rightCharacter != null)
        {
            rightConversationalistName.gameObject.SetActive(true);
            rightConversationalistName.text = currentConversation.dialogue[dialogueID].rightCharacter.characterName;
        }
        else
        {
            rightConversationalistName.gameObject.SetActive(false);
        }

        inventoryAvailableDuringConversation = false;

        gameObject.SetActive(true);
        nextButton.gameObject.SetActive(true);
        nextButton.GetComponentInChildren<Text>().text = "Next";

        areDecisionButtonsActive = false;

        GameManagerScript.gameManager.fadeOutBackground();
        GameManagerScript.gameManager.ConversationUIOpen = true;

        // hide inventory when conversation starts
        inventoryGameObject.GetComponent<InventoryScript>().hideInventory();

        changeDialogueExcerpt(dialogueID);
    }
    
    public void showConversation(ConversationData newConversation)
    {
        currentConversation = newConversation;

        if (currentConversation.dialogue[0].rightCharacter != null)
        {
            rightConversationalistName.gameObject.SetActive(true);
            rightConversationalistName.text = currentConversation.dialogue[0].rightCharacter.characterName;
        }
        else
        {
            rightConversationalistName.gameObject.SetActive(false);
        }

        inventoryAvailableDuringConversation = false;

        gameObject.SetActive(true);
        nextButton.gameObject.SetActive(true);
        nextButton.GetComponentInChildren<Text>().text = "Next";

        areDecisionButtonsActive = false;

        GameManagerScript.gameManager.fadeOutBackground();
        GameManagerScript.gameManager.ConversationUIOpen = true;

        // hide inventory when conversation starts
        inventoryGameObject.GetComponent<InventoryScript>().hideInventory();

        changeDialogueExcerpt(0);
    }

    void changeDialogueExcerpt(int indexOfDialogueExcerpt)
    {
        currentDialogueExcerpt = currentConversation.dialogue[indexOfDialogueExcerpt];

        // if wrong excerpt was found, look for it manually
        if (currentDialogueExcerpt.dialogueID != indexOfDialogueExcerpt)
        {
            Debug.Log("Wrong dialogue excerpt found");
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
        if (currentDialogueExcerpt.rightCharacter == null)
        {
            rightConversationalistSprite.gameObject.SetActive(false);
            rightConversationalistName.gameObject.SetActive(false);
        }
        else
        {
            rightConversationalistSprite.gameObject.SetActive(true);
            rightConversationalistName.gameObject.SetActive(true);
            rightConversationalistSprite.sprite = currentDialogueExcerpt.rightCharacter.sprite;
        }

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
            // add items and notes to players inventory if needed
            string infoText = "";
            if (currentDialogueExcerpt.itemToGiveToPlayer != null)
            {
                inventoryGameObject.GetComponent<InventoryScript>().addItem(currentDialogueExcerpt.itemToGiveToPlayer);
                infoText += (currentDialogueExcerpt.itemToGiveToPlayer.actorName + " added to inventory.\n");
            }
            if (currentDialogueExcerpt.noteToGiveToPlayer != null)
            {
                notebookGameObject.GetComponent<NotebookScript>().addNote(currentDialogueExcerpt.noteToGiveToPlayer);
                infoText += "New note added to notebook.\n";
            }

            if (infoText != "")
            {
                infoTextGameObject.GetComponent<InfoTextScript>().updateInfoText(infoText);
            }

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
            { // if no dialogue after this one, change next button to exit button
                nextButton.gameObject.SetActive(true);
                nextButton.GetComponentInChildren<Text>().text = "Exit";
                if (inventoryAvailableDuringConversation)
                    inventoryGameObject.GetComponent<InventoryScript>().unhideInventory();
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
        if (currentDialogueLineIndex < currentDialogueExcerpt.dialogueLines.Length - 1)
        {
            changeDialogueLine(currentDialogueLineIndex + 1);
        }
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

    public void decisionButtonPressed(int indexOfButton)
    {
        changeDialogueExcerpt(dialogueLineIDsForDecisionButtons[indexOfButton]);
        disableDecisionButtons();
    }

    public void showInventoryItem(ActorData itemShown)
    {
        disableDecisionButtons();

        // returns -1 if not found
        int indexOfItemShown = findIndexOfReactionItem(itemShown);
        
        if (indexOfItemShown > -1)
        {
            itemBeingShown = currentConversation.itemReactions[indexOfItemShown];
            if (itemBeingShown.interactionType == ConversationData.ItemInteractionType.give)
                FindObjectOfType<InventoryScript>().removeItem(itemShown);
            Debug.Log("Change Dialogue Excerpt due to relevant item being shown");
            
            bool itemPreviouslyShown = false;
            for (int i = 0; i < npcCharacterScript.previousConversationData.numberOfItemsAlreadyShown; i++)
            {
                if (npcCharacterScript.previousConversationData.itemsAlreadyShown[i].actorName == itemShown.actorName)
                {
                    itemPreviouslyShown = true;
                    changeDialogueExcerpt(itemBeingShown.idOfDialogueAfterShown);
                    // if new dialogue is longer than one line, hide the inventory
                    if (currentDialogueExcerpt.dialogueLines.Length > 1)
                    {
                        inventoryGameObject.GetComponent<InventoryScript>().hideInventory();
                    }
                    break;
                }
            }

            if (!itemPreviouslyShown)
            {
                npcCharacterScript.previousConversationData.itemsAlreadyShown[npcCharacterScript.previousConversationData.numberOfItemsAlreadyShown] = itemShown;
                npcCharacterScript.previousConversationData.numberOfItemsAlreadyShown++;
                changeDialogueExcerpt(itemBeingShown.idOfDialogueExcerpt);
                if (currentDialogueExcerpt.dialogueLines.Length > 1)
                {
                    inventoryGameObject.GetComponent<InventoryScript>().hideInventory();
                }
            }
        }
        else
        {
            leftConversationalistSprite.color = new Color(255, 255, 255);
            rightConversationalistSprite.color = new Color(135, 135, 135);
            leftConversationalistSprite.transform.localScale = new Vector3(1.2f, 1.2f, 1f);
            rightConversationalistSprite.transform.localScale = new Vector3(0.8f, 0.8f, 1f);

            nextButton.gameObject.SetActive(true);
            if (inventoryAvailableDuringConversation)
                inventoryGameObject.GetComponent<InventoryScript>().unhideInventory();

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
        GameManagerScript.gameManager.fadeInBackground();
        GameManagerScript.gameManager.ConversationUIOpen = false;
        inventoryGameObject.GetComponent<InventoryScript>().unhideInventory();
        npcCharacterScript = null;
    }
}
