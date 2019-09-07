using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ActorData : ScriptableObject // data for objects picked up by player
{
    public enum CombinationType
    {
        CreateNewPermanentInventoryItem,
        CreateNewPermanentWorldObject,
        CreateNewTemporaryInventoryItem,
        CreateNewTemporaryWorldObject,
    }

    public enum ActorOutcomeAfterCombination
    {
        Deactivate,
        Unchanged
    }

    [System.Serializable]
    public struct ActorCombinationStruct
    {
        public ActorData actorData;
        public CombinationType combinationType;
        public ActorOutcomeAfterCombination outcomeForThisActor;
        public ActorOutcomeAfterCombination outcomeForOtherActor;
        public ActorData newActorToCreate;
    }

    public string actorName;
    public string examineText;
    public ConversationData examineData;
    public InteractableObjectScript.InteractionType[] interactionsOtherThanExamine_worldObject;
    public Vector3 worldPosition;


    public Sprite inventoryImage;
    public InteractableObjectScript.InteractionType[] interactionsOtherThanExamine_inventoryItem;
    public ActorCombinationStruct[] actorsThisCanBeCombinedWith;
    private ActorData[] components;

    public void setComponents(ActorData[] actorsUsedToCreateThis)
    {
        components = actorsUsedToCreateThis;
    }

    public ActorData[] getComponents()
    {
        // when player uses seperate action, these items will be added to their inventory
        return components;
    }
}
