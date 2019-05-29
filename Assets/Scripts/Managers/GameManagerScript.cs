using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerScript : MonoBehaviour
{

    [HideInInspector] public SceneManager currentScene;

    public SceneManager[] allScenes;

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this);

        // FOR TESTING PURPOSES
        currentScene = allScenes[0];
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool combineActors(ItemInteractionScript inventorySlotOfFirstItem, InteractableObjectScript worldObjectCombinedWith)
    {
        bool actorsNeedSwitching = false;
        // check if these actors can be combined
        int indexOfSecondActor = getIndexOfSecondActor(inventorySlotOfFirstItem.dataOfItemInSlot, worldObjectCombinedWith.data);
        if (indexOfSecondActor == -1)
        {
            // switch first actor and second actor and check again
            indexOfSecondActor = getIndexOfSecondActor(worldObjectCombinedWith.data, inventorySlotOfFirstItem.dataOfItemInSlot);
            if (indexOfSecondActor == -1)
                return false;
            else
            { // switch the actors around
                actorsNeedSwitching = true;
            }
        }

        ActorData firstActorData;
        ActorData secondActorData;

        if (actorsNeedSwitching)
        {
            firstActorData = worldObjectCombinedWith.data;
            secondActorData = inventorySlotOfFirstItem.dataOfItemInSlot;
        }
        else
        {
            firstActorData = inventorySlotOfFirstItem.dataOfItemInSlot;
            secondActorData = worldObjectCombinedWith.data;
        }

        // remove actors if necessary
        if (firstActorData.actorsThisCanBeCombinedWith[indexOfSecondActor].outcomeForThisActor == ActorData.ActorOutcome.Deactivate)
            FindObjectOfType<InventoryScript>().removeItem(firstActorData);

        if (firstActorData.actorsThisCanBeCombinedWith[indexOfSecondActor].outcomeForOtherActor == ActorData.ActorOutcome.Deactivate)
            currentScene.deactivateWorldObject(worldObjectCombinedWith.gameObject);

        resolveComination(firstActorData, secondActorData, indexOfSecondActor);

        return true;
    }

    public bool combineActors(ItemInteractionScript inventorySlotOfFirstItem, ItemInteractionScript inventoryItemCombinedWith)
    {
        bool actorsNeedSwitching = false;
        // check if first actor has second actor in list of possible combinations
        int indexOfSecondActor = getIndexOfSecondActor(inventorySlotOfFirstItem.dataOfItemInSlot, inventoryItemCombinedWith.dataOfItemInSlot);
        if (indexOfSecondActor == -1)
        {
            // switch first actor and second actor and check again
            indexOfSecondActor = getIndexOfSecondActor(inventoryItemCombinedWith.dataOfItemInSlot, inventorySlotOfFirstItem.dataOfItemInSlot);
            if (indexOfSecondActor == -1)
                return false;
            else
            { // switch the actors around
                actorsNeedSwitching = true;
            }
        }

        ActorData firstActorData;
        ActorData secondActorData;

        if (actorsNeedSwitching)
        {
            firstActorData =  inventoryItemCombinedWith.dataOfItemInSlot;
            secondActorData = inventorySlotOfFirstItem.dataOfItemInSlot;
        }
        else
        {
            firstActorData = inventorySlotOfFirstItem.dataOfItemInSlot;
            secondActorData = inventoryItemCombinedWith.dataOfItemInSlot;
        }

        // remove actors if necessary
        if (firstActorData.actorsThisCanBeCombinedWith[indexOfSecondActor].outcomeForThisActor == ActorData.ActorOutcome.Deactivate)
            FindObjectOfType<InventoryScript>().removeItem(firstActorData);
        if (firstActorData.actorsThisCanBeCombinedWith[indexOfSecondActor].outcomeForOtherActor == ActorData.ActorOutcome.Deactivate)
            FindObjectOfType<InventoryScript>().removeItem(secondActorData);

        resolveComination(firstActorData, secondActorData, indexOfSecondActor);

        return true;
    }

    public int getIndexOfSecondActor(ActorData firstActor, ActorData secondActor)
    {
        bool canBeCombined = false;
        int index = 0;

        for (index = 0; index < firstActor.actorsThisCanBeCombinedWith.Length; index++)
        {
            if (firstActor.actorsThisCanBeCombinedWith[index].actorData == secondActor)
            {
                canBeCombined = true;
                break;
            }
        }

        if (!canBeCombined)
            return -1;
        else
            return index;
    }

    private void resolveComination(ActorData firstActorData, ActorData secondActorData, int indexOfSecondActor)
    {
        switch (firstActorData.actorsThisCanBeCombinedWith[indexOfSecondActor].combinationType)
        {
            case ActorData.CombinationType.CreateNewPermanentInventoryItem:
                FindObjectOfType<InventoryScript>().addItem(firstActorData.actorsThisCanBeCombinedWith[indexOfSecondActor].newActorToCreate);
                break;

            case ActorData.CombinationType.CreateNewPermanentWorldObject:
                currentScene.activateWorldObject(firstActorData.actorsThisCanBeCombinedWith[indexOfSecondActor].newActorToCreate);
                break;

            case ActorData.CombinationType.CreateNewTemporaryInventoryItem:
                FindObjectOfType<InventoryScript>().addItem(firstActorData.actorsThisCanBeCombinedWith[indexOfSecondActor].newActorToCreate);

                // add components so combination can be reversed
                ActorData[] componentsOfNewItem = { firstActorData, secondActorData };
                firstActorData.actorsThisCanBeCombinedWith[indexOfSecondActor].newActorToCreate.setComponents(componentsOfNewItem);
                break;

            case ActorData.CombinationType.CreateNewTemporaryWorldObject:
                currentScene.activateWorldObject(firstActorData.actorsThisCanBeCombinedWith[indexOfSecondActor].newActorToCreate);

                // add components so combination can be reversed
                ActorData[] componentsOfNewObject = { firstActorData, secondActorData };
                firstActorData.actorsThisCanBeCombinedWith[indexOfSecondActor].newActorToCreate.setComponents(componentsOfNewObject);

                break;

            case ActorData.CombinationType.StartConversation:
                break;

            default:
                break;
        }

        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInputScript>().checkIfMouseHighlightingObject();
    }

    public bool seperateActor() // splits previously combined actor into components
    {

        return true;
    }

}
