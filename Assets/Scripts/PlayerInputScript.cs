﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class PlayerInputScript : MonoBehaviour
{
    struct QueuedAction
    {
        public bool anActionIsQueued;
        public GameObject objectToActUpon;
        public InteractableObjectScript.InteractionType interactionType;
    }
    QueuedAction queuedAction; // when the player does an action on an object that they are far away from, it is stored here.

    private InteractableObjectScript highlightedWorldObject;
    private InteractableObjectScript selectedWorldObject;
    private ItemInteractionScript highlightedInventoryItem;
    private ItemInteractionScript selectedInventoryItem;
    private LeaveSceneScript highlightedSceneExit;

    bool aSelectionMenuIsOpen;
    [HideInInspector] public bool notebookIsOpen;

    bool combiningInProgress;
    [HideInInspector] public ItemInteractionScript combiningItem;

    GameObject UI_nameOfHighlightedObject;
    //public GameObject UI_playerThoughtsText;

    bool showExamineObjectText;
    float examineStartTime;
    const float examineDurationTime = 4f;

    private Vector3 currentSpeed;
    private Vector3 destinationPosition;
    float speedModifier = 6f;
    private NavMeshAgent navMeshAgent;
    private const float navMeshSampleDistance = 4f;
    

    [HideInInspector] public bool mouseOverUI;

    // Start is called before the first frame update
    void Start()
    {
        currentSpeed = Vector3.zero;
        UI_nameOfHighlightedObject = GameObject.FindGameObjectWithTag("HoveredObjectName_UIText");
        destinationPosition = gameObject.transform.position;
        navMeshAgent = GetComponent<NavMeshAgent>();
        setQueuedAction(null, InteractableObjectScript.InteractionType.Examine);
        mouseOverUI = false;
        showExamineObjectText = false;
        combiningInProgress = false;
        notebookIsOpen = false;
    }

    // Update is called once per frame
    void Update()
    {
        Camera mainCamera = Camera.main;

        // examine object UI
        
        if (showExamineObjectText)
        {
            if (Time.time - examineStartTime >= examineDurationTime)
            {
                showExamineObjectText = false;
                //UI_playerThoughtsText.GetComponent<Text>().enabled = false;
            }
        }

        //if (Input.GetMouseButtonDown(0))
        //{
        //    RaycastHit hit;
        //    Debug.Log("raycast sent");
        //    if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 5000))
        //    {
        //        navMeshAgent.SetDestination(hit.point);
        //        //navMeshAgent.destination = hit.point;
        //        Debug.Log("hit registered at " + hit.point + ". Agent moving to " + navMeshAgent.destination);
        //    }
        //}

        // get player inputs

        if (Input.GetButtonDown("RightMouseClick"))
        {
            if (aSelectionMenuIsOpen)
                closeCurrentSelectionMenu();

            if (combiningInProgress)
                combiningInProgress = false;

            checkIfMouseHighlightingObject();
        }

        if (Input.GetButtonUp("LeftMouseClick"))
        {
            // if world object is highlighted, that means the mouse is over it. Therefore, the player has clicked on it
            if (highlightedWorldObject != null)
            {
                worldObjectClickedOn(highlightedWorldObject);
            }

            // if inventory item is highlighted, that means the mouse is over it. Therefore, the player has clicked on it
            else if (highlightedInventoryItem != null)
            {
                // don't show item in conversation if item is the notebook, we need to open/close the notebook instead
                if (GameManagerScript.gameManager.ConversationUIOpen && highlightedInventoryItem.itemSlotIndex != 0)
                {
                    GameManagerScript.gameManager.conversationUI.GetComponent<ConversationScript>().showInventoryItem(highlightedInventoryItem.dataOfItemInSlot);
                }
                else
                {
                    if (highlightedInventoryItem == selectedInventoryItem)
                    {
                        closeCurrentSelectionMenu();
                    }
                    else
                    {
                        inventoryItemClickedOn(highlightedInventoryItem);
                    }
                }
            }

            // player has clicked on a scene exit
            else if (highlightedSceneExit != null)
            {
                closeCurrentSelectionMenu();
                setQueuedAction(highlightedSceneExit.gameObject, InteractableObjectScript.InteractionType.GoTo);

            }

            // if menu is open and player clicks outside of menu area, close menu.
            else if (aSelectionMenuIsOpen && !notebookIsOpen)
            {
                closeCurrentSelectionMenu();
            }

            // set movement destination to mouse click location
            else if (!mouseOverUI)
            {
                //destinationPosition = getMousePositionInWorld(); // TODO: cast mouse ray and only set destination position if ray hits ground
                
                setQueuedAction(null, InteractableObjectScript.InteractionType.Examine);
            }
        }

        // update highlighted object UI
        if (highlightedWorldObject != selectedWorldObject || combiningInProgress || highlightedSceneExit != null)
        {
            UI_nameOfHighlightedObject.transform.position = new Vector3(Input.mousePosition.x, Input.mousePosition.y + 20, Input.mousePosition.z);
        }
    }

    private void FixedUpdate()
    {
        //transform.position = Vector3.MoveTowards(transform.position, destinationPosition, speedModifier);
    }

    private void OnCollisionEnter(Collision collision)
    {
        stopMovement(); // TODO need to change this. at the moment if the player enters collisions area, they stop, but then they can click toward collision object again
                            // and they will start moving towards it again
    }

    public void worldObjectClickedOn(InteractableObjectScript worldObject)
    {
        if (combiningInProgress)
        {
            setQueuedAction(worldObject.gameObject, InteractableObjectScript.InteractionType.Combine);
            combiningInProgress = false;
            setHighlightedWorldObject(worldObject);
        }

        else
        {
            closeCurrentSelectionMenu();
            selectedWorldObject = worldObject;
            GameObject.FindGameObjectWithTag("SelectedObjectMenu_UIText").GetComponent<SelectedObjectMenuScript>().objectClicked(selectedWorldObject.GetComponent<InteractableObjectScript>(), new Vector3(Input.mousePosition.x, Input.mousePosition.y + 20, Input.mousePosition.z));
            aSelectionMenuIsOpen = true;
            if (worldObject == highlightedWorldObject) stopHighlightingWorldObject(highlightedWorldObject);
        }
    }

    public void inventoryItemClickedOn(ItemInteractionScript inventoryItem)
    {
        if (combiningInProgress && inventoryItem != combiningItem)
        {
            // check if the item player is attempting to combine with is the notebook
            if (inventoryItem.itemSlotIndex == 0)
                GameManagerScript.gameManager.GetComponent<ConversationScript>().showConversation(GameManagerScript.gameManager.failedItemCombinationConversationData);
                //enableExamineObjectText("I can't combine those...");

            // try to combine items
            else if (!GameManagerScript.gameManager.combineActors(combiningItem, inventoryItem))
                GameManagerScript.gameManager.GetComponent<ConversationScript>().showConversation(GameManagerScript.gameManager.failedItemCombinationConversationData);
                //enableExamineObjectText("I can't combine those...");
            
            else
            {
                combiningInProgress = false;
                checkIfMouseHighlightingObject();
            }
        }

        else
        {
            closeCurrentSelectionMenu();
            selectedInventoryItem = inventoryItem;
            selectedInventoryItem.openItemMenu();

            // the notebook has no selection menu, so it does not need to be set as 'open' if the notebook was the item clicked on.
            //if (inventoryItem.itemSlotIndex != 0)
            //{
                aSelectionMenuIsOpen = true;
            //}
        }
    }

    public void groundClickedOn(BaseEventData data)
    {
        PointerEventData pointerData = (PointerEventData)data;
        NavMeshHit hit;

        if (NavMesh.SamplePosition(pointerData.pointerCurrentRaycast.worldPosition, out hit, navMeshSampleDistance, NavMesh.AllAreas))
        {
            destinationPosition = hit.position;
        }
        else
        { // if a hit isn't found, move to nearest position
            destinationPosition = pointerData.pointerCurrentRaycast.worldPosition;
        }
        navMeshAgent.SetDestination(destinationPosition);
        navMeshAgent.isStopped = false;
        //navMeshAgent.destination = destinationPosition;
    }

    private void closeCurrentSelectionMenu()
    {
        if (aSelectionMenuIsOpen)
        {
            if (selectedWorldObject != null)
            {
                InteractableObjectScript unselectedWorldObject = selectedWorldObject;
                selectedWorldObject = null;
                GameObject.FindGameObjectWithTag("SelectedObjectMenu_UIText").GetComponent<SelectedObjectMenuScript>().closeMenu();

                // check if mouse is still over object
                RaycastHit mouseRayHit;
                Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(mouseRay, out mouseRayHit))
                {
                    InteractableObjectScript hitObjectInteractionScript = mouseRayHit.collider.GetComponent<InteractableObjectScript>();
                    if (hitObjectInteractionScript == unselectedWorldObject)
                    {
                        setHighlightedWorldObject(hitObjectInteractionScript);
                    }
                }
            }

            if (selectedInventoryItem != null)
            {
                selectedInventoryItem.closeItemMenu();
                selectedInventoryItem = null;
            }

            aSelectionMenuIsOpen = false;
        }
    }

    public void closeNotebook()
    {
        // don't fade-in to game if conversation is ongoing
        if (!GameManagerScript.gameManager.ConversationUIOpen)
        {
            GameManagerScript.gameManager.fadeInBackground();
        }
        closeCurrentSelectionMenu(); // this function closes the notebook
    }
        

    public void checkIfMouseHighlightingObject()
    {
        // resets mouse ray and sets highlighted object to null if an object is deactivated while mouse is hovering over it
        stopHighlightingWorldObject(highlightedWorldObject);
        RaycastHit mouseRayHit;
        Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(mouseRay, out mouseRayHit))
        {
            InteractableObjectScript hitObjectInteractionScript = mouseRayHit.collider.GetComponent<InteractableObjectScript>();
            setHighlightedWorldObject(hitObjectInteractionScript);
        }
    }

    public void stopMovement()
    {
        destinationPosition = transform.position; // player won't move because their destination is exactly where they are.
        navMeshAgent.SetDestination(destinationPosition);
        navMeshAgent.isStopped = true;
    }

    private Vector3 getMousePositionInWorld()
    {
        RaycastHit mouseRayHit;
        Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(mouseRay, out mouseRayHit)) //check if the ray hit something
        {
            Vector3 mousePositionInWorld = mouseRayHit.point;
            return new Vector3(mousePositionInWorld.x, transform.position.y, mousePositionInWorld.z);
        }
        else
            return destinationPosition; // if no hit found, don't change destination
    }

    public void stopHighlightingWorldObject(InteractableObjectScript worldObject)
    {
        if (highlightedWorldObject == worldObject)
        {
            highlightedWorldObject = null;
            if (!combiningInProgress)
                UI_nameOfHighlightedObject.GetComponent<Text>().enabled = false;
            else
                UI_nameOfHighlightedObject.GetComponent<Text>().text = "Combine with ...";

        }
    }

    public void stopHighlightingInventoryItem(ItemInteractionScript item)
    {
        if (highlightedInventoryItem == item)
        {
            highlightedInventoryItem = null;
        }
    }

    public void setHighlightedWorldObject(InteractableObjectScript worldObject)
    {
        // don't highlight if object is already selected
        if (worldObject == selectedWorldObject) return;

        // if the mouse is over a UI element, they shouldn't be able to highlight objects beneath UI
        if (mouseOverUI) return; 

        // clear current highlighted object
        if (highlightedWorldObject != null) stopHighlightingWorldObject(highlightedWorldObject);

        // set this object as highlighted object
        if (worldObject != null)
        {
            highlightedWorldObject = worldObject;
            UI_nameOfHighlightedObject.GetComponent<Text>().enabled = true;
            if (combiningInProgress)
            {
                UI_nameOfHighlightedObject.GetComponent<Text>().text = "Combine with " + highlightedWorldObject.data.actorName;
            }
            else
            {
                UI_nameOfHighlightedObject.GetComponent<Text>().text = highlightedWorldObject.data.actorName;
            }
            UI_nameOfHighlightedObject.transform.position = new Vector3(Input.mousePosition.x, Input.mousePosition.y + 20, Input.mousePosition.z);
        }
    }

    public void setHighlightedWorldObjectToSceneExit(LeaveSceneScript sceneExit)
    {
        // if the mouse is over a UI element, they shouldn't be able to highlight objects beneath UI
        if (mouseOverUI) return;
        setHighlightedWorldObjectToNull();
        highlightedSceneExit = sceneExit;
        UI_nameOfHighlightedObject.GetComponent<Text>().enabled = true;
        if (sceneExit.adjacentSceneName == "map_screen")
        {
            UI_nameOfHighlightedObject.GetComponent<Text>().text = "Leave";
        }
        else
        {
            UI_nameOfHighlightedObject.GetComponent<Text>().text = "Go to " + sceneExit.adjacentSceneName;
        }
        UI_nameOfHighlightedObject.transform.position = new Vector3(Input.mousePosition.x, Input.mousePosition.y + 20, Input.mousePosition.z);
    }

    public void stopHighlighingSceneExit()
    {
        UI_nameOfHighlightedObject.GetComponent<Text>().enabled = false;
        highlightedSceneExit = null;
    }

    public void setHighlightedInventoryItem(ItemInteractionScript item)
    {
        // don't highlight if item is already selected
        if (item == selectedInventoryItem) return;
        
        // clear current highlighted item
        if (highlightedInventoryItem != null) stopHighlightingInventoryItem(highlightedInventoryItem);

        // set this object as highlighted item
        if (item != null)
        {
            highlightedInventoryItem = item;
        }
    }

    public void setHighlightedWorldObjectToNull()
    {
        if (highlightedWorldObject != null)
            stopHighlightingWorldObject(highlightedWorldObject);
    }

    public void setHighlightedInventoryItemToNull()
    {
        if (highlightedInventoryItem != null)
            highlightedInventoryItem.endItemHoverOver();
    }

    public void setQueuedAction(GameObject newObjectToActUpon, InteractableObjectScript.InteractionType newInteraction)
    {
        queuedAction.objectToActUpon = newObjectToActUpon;
        if (newObjectToActUpon == null) queuedAction.anActionIsQueued = false;
        else
        {
            queuedAction.anActionIsQueued = true;
            // start moving towards object
            destinationPosition = new Vector3(queuedAction.objectToActUpon.transform.position.x, transform.position.y, queuedAction.objectToActUpon.transform.position.z);
            navMeshAgent.SetDestination(destinationPosition);
            navMeshAgent.isStopped = false;
        }
        queuedAction.interactionType = newInteraction;
    }

    public GameObject getQueuedActionObject()
    {
        if (queuedAction.anActionIsQueued)
            return queuedAction.objectToActUpon;
        else return null;
    }

    public bool isAnActionQueued()
    {
        return queuedAction.anActionIsQueued;
    }

    public InteractableObjectScript.InteractionType getQueuedActionType()
    {
        return queuedAction.interactionType;
    }

    //public void enableExamineObjectText(string examineText)
    //{
    //    Debug.Log(examineText);
    //    UI_playerThoughtsText.GetComponent<Text>().enabled = true;
    //    UI_playerThoughtsText.GetComponent<Text>().text = examineText;
    //    showExamineObjectText = true;
    //    examineStartTime = Time.time;
    //}

    public void enableCombineWithText(ItemInteractionScript _combiningItem)
    {
        combiningInProgress = true;
        combiningItem = _combiningItem;
        UI_nameOfHighlightedObject.GetComponent<Text>().enabled = true;
        UI_nameOfHighlightedObject.GetComponent<Text>().text = "Combine with ...";
    }
}
