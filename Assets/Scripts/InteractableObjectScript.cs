using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class InteractableObjectScript : MonoBehaviour
{
    public enum InteractionType
    {
        Examine,
        Use,
        PickUp,
        TalkTo,
        GoTo,
        Combine,
        Separate,
    }

    public ActorData data;

    [HideInInspector] public InteractionType[] interactions;

    [HideInInspector] public int numberOfInteractions;
    private GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        Setup();
    }

    public void Setup()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        //numberOfInteractions = interactions.Length;
        numberOfInteractions = data.interactionsOtherThanExamine_worldObject.Length + 1;

        interactions = new InteractionType[numberOfInteractions];
        interactions[0] = InteractionType.Examine;

        for (int i = 0; i < data.interactionsOtherThanExamine_worldObject.Length; i++)
        {
            interactions[i + 1] = data.interactionsOtherThanExamine_worldObject[i];
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnMouseEnter()
    {
        // show object name as floating text
        player.GetComponent<PlayerInputScript>().setHighlightedWorldObject(this);
    }

    private void OnMouseExit()
    {
        // remove floating text created above
        
        player.GetComponent<PlayerInputScript>().stopHighlightingWorldObject(this);
    }

    public virtual void doAction(InteractionType interaction)
    {
        switch (interaction)
        {
            case InteractionType.Examine:
                player.GetComponent<PlayerInputScript>().enableExamineObjectText(data.examineText);
                Debug.Log("Object Examined");
                break;

            case InteractionType.Use:
                Debug.Log("Object Used");
                break;

            case InteractionType.PickUp:
                Debug.Log("Object Picked Up");
                PlayerInputScript playerScript = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInputScript>();
                playerScript.stopHighlightingWorldObject(this);
                GetComponent<PickUpObjectScript>().addItemToInventory();
                break;

            case InteractionType.GoTo:
                GetComponent<GoToNewAreaTriggerScript>().goToNewArea();
                break;

            case InteractionType.Separate:
                FindObjectOfType<GameManagerScript>().separateActor(this);
                break;
        }
    }
}
