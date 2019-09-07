using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterScript : InteractableObjectScript
{
    public ConversationData[] allConversations;
    public int indexOfInitialConversation;
    ConversationData loadedConversation;
    public CharacterData characterData;

    [HideInInspector] public GameManagerScript.PreviousConversationData previousConversationData;

    // Start is called before the first frame update
    void Start()
    {
        base.Setup();
        loadedConversation = allConversations[indexOfInitialConversation];
        
        previousConversationData = GameManagerScript.gameManager.getPreviousConversationData(characterData);
    }

    // Update is called once per frame
    void Update()
    {
    }

    public override void doAction(InteractionType interaction)
    {
        switch (interaction)
        {
            case InteractionType.TalkTo:
                GameManagerScript.gameManager.conversationUI.GetComponent<ConversationScript>().openConversationWithCharacter(null, loadedConversation, this);
                break;
            default:
                base.doAction(interaction);
                break;
        }
    }
}
