using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterScript : InteractableObjectScript
{
    public ConversationData[] allConversations;
    public int indexOfInitialConversation;
    ConversationData loadedConversation;
    public CharacterData characterData;

    // Start is called before the first frame update
    void Start()
    {
        base.Setup();
        loadedConversation = allConversations[indexOfInitialConversation];
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
                FindObjectOfType<GameManagerScript>().conversationUI.GetComponent<ConversationScript>().openConversation(null, loadedConversation);
                break;
        }
    }
}
