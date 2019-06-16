using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConversationScript : MonoBehaviour
{
    private enum CauseOfDialogueChange
    { next, previous }

    public ConversationData currentConversation;
    ConversationData.DialogueExcerpt currentDialogueExcerpt;
    int currentDialogueLineIndex;

    public Button previousButton;
    public Button nextButton;
    public Image leftConversationalistSprite;
    public Image rightConversationalistSprite;
    public Text conversationText;

    bool areDecisionButtonsActive;
    public Button[] decisionButtons;
    int[] dialogueLineIDsForDecisionButtons;

    private bool onFirstLineOfConversation;

    private void Start()
    {
        areDecisionButtonsActive = false;
        onFirstLineOfConversation = true;
        dialogueLineIDsForDecisionButtons = new int[4];
    }

    public void openConversation(ActorData itemCombinedWithCharacter, ConversationData newConversation)
    {
        int indexOfRelevantItem = findDialogueIndexforItem(itemCombinedWithCharacter);

        currentConversation = newConversation;

        gameObject.SetActive(true);
        nextButton.gameObject.SetActive(true);
        previousButton.gameObject.SetActive(true);
        nextButton.GetComponentInChildren<Text>().text = "Next";
        previousButton.GetComponentInChildren<Text>().text = "Exit";

        areDecisionButtonsActive = false;
        onFirstLineOfConversation = true;

        GameManagerScript gmScript = FindObjectOfType<GameManagerScript>();
        gmScript.fadeIn_Conversation();
        gmScript.ConversationUIOpen = true;

        if (indexOfRelevantItem < 0)
            changeDialogueExcerpt(0, CauseOfDialogueChange.next);
        else
            changeDialogueExcerpt(indexOfRelevantItem, CauseOfDialogueChange.next);
    }

    void changeDialogueExcerpt(int indexOfDialogueExcerpt, CauseOfDialogueChange causeOfExcerptChange)
    {
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

        if (causeOfExcerptChange == CauseOfDialogueChange.next)
            changeDialogueLine(0, causeOfExcerptChange); // set to first line of excerpt

        else if (causeOfExcerptChange == CauseOfDialogueChange.previous)
            changeDialogueLine(currentDialogueExcerpt.dialogueLines.Length - 1, causeOfExcerptChange); // set to last line of excerpt
    }

    private void changeDialogueLine(int indexOfDialogueLine, CauseOfDialogueChange causeOfLineChange)
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
                }
                else
                    nextButton.gameObject.SetActive(false);

                if (currentDialogueExcerpt.previousDialogueID < 0)
                {
                    // disable previousButton, because in this case, both next and previous buttons would show 'Exit'
                    previousButton.gameObject.SetActive(false);
                }
            }
        }

        else
        { // set next button to be normal if not end of excerpt
            nextButton.GetComponentInChildren<Text>().text = "Next";
            nextButton.gameObject.SetActive(true);
        }

        // only need to change the previous button if the previous button is pressed.
        if (causeOfLineChange == CauseOfDialogueChange.previous)
        {
            previousButton.gameObject.SetActive(true);
            if (currentDialogueExcerpt.previousDialogueID < 0 && currentDialogueLineIndex <= 0)
            {
                onFirstLineOfConversation = true;
                previousButton.GetComponentInChildren<Text>().text = "Exit";
            }
            else
                previousButton.GetComponentInChildren<Text>().text = "Previous";
        }
    }

    public void nextButtonPressed()
    {
        if (onFirstLineOfConversation)
        {
            previousButton.GetComponentInChildren<Text>().text = "Previous";
            onFirstLineOfConversation = false;
        }

        if (currentDialogueLineIndex < currentDialogueExcerpt.dialogueLines.Length - 1)
            changeDialogueLine(currentDialogueLineIndex + 1, CauseOfDialogueChange.next);
        else
        {
            if (currentDialogueExcerpt.nextDialogueID < 0)
            {
                exitConversation();
            }
            else
                changeDialogueExcerpt(currentDialogueExcerpt.nextDialogueID, CauseOfDialogueChange.next);
        }
    }

    public void previousButtonPressed()
    {
        if (areDecisionButtonsActive)
        {
            disableDecisionButtons();
        }

        if (currentDialogueLineIndex > 0)
        {
            changeDialogueLine(currentDialogueLineIndex - 1, CauseOfDialogueChange.previous);
        }
        else
        {
            if (currentDialogueExcerpt.previousDialogueID < 0)
            {
                exitConversation();
            }
            else
                changeDialogueExcerpt(currentDialogueExcerpt.previousDialogueID, CauseOfDialogueChange.previous);
        }
    }

    public void decisionButtonPressed(int indexOfButton)
    {
        changeDialogueExcerpt(dialogueLineIDsForDecisionButtons[indexOfButton], CauseOfDialogueChange.next);
        disableDecisionButtons();
    }

    public void showInventoryItem(ActorData itemShown)
    {
        // returns -1 if not found
        int dialogueIndexForItem = findDialogueIndexforItem(itemShown);
        if (dialogueIndexForItem > -1)
        {
            disableDecisionButtons();
            if (currentConversation.itemReactions[dialogueIndexForItem].interactionType == ConversationData.ItemInteractionType.give)
                FindObjectOfType<InventoryScript>().removeItem(itemShown);
            changeDialogueExcerpt(dialogueIndexForItem, CauseOfDialogueChange.next);
        }
        else
        {
            Debug.Log("item not found");
            // TODO: SOME KIND OF FEEDBACK ABOUT NOT SHOWING THEM THIS ITEM
            // If character.gender == 'm'
            //     Player says “I don’t want to talk to him about that.”
            // Else Player says “I don’t want to talk to her about that.”
        }
    }

    private int findDialogueIndexforItem(ActorData item)
    {
        if (item != null)
        {
            for (int i = 0; i < currentConversation.itemReactions.Length; i++)
            {
                if (currentConversation.itemReactions[i].item.Equals(item))
                {
                    return currentConversation.itemReactions[i].idOfDialogueExcerpt;
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
        gmScript.fadeIn_Conversation();
        gmScript.ConversationUIOpen = false;
    }
}
