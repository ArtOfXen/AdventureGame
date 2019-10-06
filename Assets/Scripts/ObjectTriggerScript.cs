using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectTriggerScript : MonoBehaviour
{
    InteractableObjectScript objectScript;
    GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        objectScript = GetComponentInParent<InteractableObjectScript>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay(Collider other)
    {
        Debug.Log("object collision");
        player = GameObject.FindGameObjectWithTag("Player");

        if (other.gameObject.Equals(player))
        {
            Debug.Log("Player collision");
            PlayerInputScript playerScript = player.GetComponent<PlayerInputScript>();
            if (playerScript.isAnActionQueued())
            {
                Debug.Log("Action queued");
                GameObject queuedObject = playerScript.getQueuedActionObject();
                if (queuedObject.Equals(transform.parent.gameObject))
                {
                    playerScript.stopMovement();
                    // TODO: make player look at parent object
                    InteractableObjectScript.InteractionType interaction = playerScript.getQueuedActionType();
                    playerScript.setQueuedAction(null, InteractableObjectScript.InteractionType.Examine); // reset queued action to stop this function running indefinitely
                    
                    if (interaction == InteractableObjectScript.InteractionType.Combine)
                    {
                        if (!GameManagerScript.gameManager.combineActors(playerScript.combiningItem, transform.parent.GetComponent<InteractableObjectScript>()))
                        {
                            GameManagerScript.gameManager.conversationUI.GetComponent<ConversationScript>().showConversation(GameManagerScript.gameManager.failedItemCombinationConversationData);
                            //player.GetComponent<PlayerInputScript>().enableExamineObjectText("I can't combine those...");
                        }
                    }
                    else if (interaction == InteractableObjectScript.InteractionType.GoTo)
                    {
                        LeaveSceneScript leaveSceneScript = queuedObject.GetComponent<LeaveSceneScript>();
                        if (leaveSceneScript.adjacentSceneName == "map_screen")
                        {
                            GameManagerScript.gameManager.openMapScreen();
                        }
                        else
                        {
                            GameManagerScript.gameManager.fadeAndLoadScene(leaveSceneScript.adjacentSceneName);
                        }
                    }
                    else
                    {
                        objectScript.doAction(interaction);
                    }
                }
            }
        }
    }
}
