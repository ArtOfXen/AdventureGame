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
        if (other.gameObject.Equals(player))
        {
            PlayerInputScript playerScript = player.GetComponent<PlayerInputScript>();
            if (playerScript.isAnActionQueued())
            {
                if (playerScript.getQueuedActionObject().Equals(transform.parent.gameObject))
                {
                    playerScript.stopMovement();
                    // TODO make player look at parent object
                    InteractableObjectScript.InteractionType interaction = playerScript.getQueuedActionType();
                    playerScript.setQueuedAction(null, InteractableObjectScript.InteractionType.Examine); // reset queued action to stop this function running indefinitely

                    if (interaction == InteractableObjectScript.InteractionType.Combine)
                    {
                        if (!FindObjectOfType<GameManagerScript>().combineActors(playerScript.combiningItem, transform.parent.GetComponent<InteractableObjectScript>()))
                        {
                            player.GetComponent<PlayerInputScript>().enableExamineObjectText("I can't combine those...");
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
