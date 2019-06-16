using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ConversationData : ScriptableObject
{

    public enum ItemInteractionType { show, give} // 'show' keeps item, while 'give' removes it from inventory

    [System.Serializable]
    public struct Decision // an option that appears on the buttons below the conversation text
    {
        public int idOfNextDialogueExcerpt; // excerpt to go to if this option is chosen
        public string text; // text on button
        public bool unlocked; // can lock options until certain events happen
    }

    [System.Serializable]
    public struct ItemReaction // what happens when the player gives/shows an item to the character
    {
        public ActorData item;
        public ItemInteractionType interactionType;
        public int idOfDialogueExcerpt; // excerpt to go to if this item is used
        public bool unlocked;// can lock item until certain events happen
    }

    [System.Serializable]
    public struct DialogueLine
    {
        public string text; // the line that is being said
        public bool leftCharacterIsSpeaking; // is the left or right character speaking this line
    }

    [System.Serializable]
    public struct DialogueExcerpt // a block of dialogue lines. Usually starts with the conversation beginning or a decision outcome, and ends with the conversation end or a decision input. Also changes if the 2 characters who are speaking change.
    {
        public int dialogueID; // if of this dialogue
        public int nextDialogueID; // dialogue to go to after this is over. Negative if none.
        public int previousDialogueID; // last dialogue. Negative if none.
        public DialogueLine[] dialogueLines; // the lines of this conversation
        public CharacterData leftCharacter;
        public CharacterData rightCharacter;
        public bool decisionCanBeMade;
        public Decision[] decisions;
        public bool endOfConversation; // is this the last dialogue excerpt in the conversation
        // Effect effect // what happens after this excerpt ends? i.e. does the player unlock a new clue or item
    }

    public int conversationID;
    public ItemReaction[] itemReactions; // all item reactions for this conversation
    public DialogueExcerpt[] dialogue; // all dialogue in this conversation
}
