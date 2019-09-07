using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class GameManagerScript : MonoBehaviour
{
    public struct PreviousConversationData
    {
        public string characterName;
        public int numberOfItemsAlreadyShown;
        public ActorData[] itemsAlreadyShown;
    }
    [HideInInspector] List<PreviousConversationData> previousConversationData;

    public static GameManagerScript gameManager;

    [SerializeField] private Image fadeOutImage_exclUI;
    [SerializeField] private Image fadeOutImage_inclUI;
    public GameObject conversationUI;
    public GameObject notebookUI;
    [HideInInspector] public SceneClass currentScene;

    public SceneClass[] allScenes;
    public event Action BeforeSceneUnload;
    public event Action AfterSceneLoad;

    private Color fadeOutImageColour;
    private float fadeDuration = 1f;
    private bool isCurrentlyFading;

    public NoteData DEBUG_TEST_NOTE;
    public ConversationData failedItemCombinationConversationData;

    void Awake()
    {
        if (gameManager == null)
        {
            DontDestroyOnLoad(this);
            gameManager = this;
        }
        else if (gameManager != this)
        {
            Destroy(gameObject);
        }
        
        ConversationUIOpen = false;

        fadeOutImageColour = fadeOutImage_exclUI.color;

        previousConversationData = new List<PreviousConversationData>();

        // TODO: LOAD DATA FROM FILE
    }

    private IEnumerator Start()
    {
        // FOR TESTING PURPOSES
        currentScene = allScenes[0];
        notebookUI.GetComponent<NotebookScript>().addNote(DEBUG_TEST_NOTE);

        yield return StartCoroutine(loadSceneAndSetActive("DactylStreet_1"));
        StartCoroutine(fade(0f));

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool ConversationUIOpen { get; set; }



    public void fadeOutBackground()
    {
        PlayerInputScript player = FindObjectOfType<PlayerInputScript>();
        player.setHighlightedInventoryItemToNull();
        player.setHighlightedWorldObjectToNull();
        fadeOutImage_exclUI.enabled = true;
        fadeOutImage_exclUI.color = new Color(fadeOutImageColour.r, fadeOutImageColour.g, fadeOutImageColour.b, 0.5f);
    }

    public void fadeInBackground()
    {
        PlayerInputScript player = FindObjectOfType<PlayerInputScript>();
        player.setHighlightedInventoryItemToNull();
        player.setHighlightedWorldObjectToNull();
        fadeOutImage_exclUI.color = new Color(fadeOutImageColour.r, fadeOutImageColour.g, fadeOutImageColour.b, 0);
        fadeOutImage_exclUI.enabled = false;
    }

    public void fadeAndLoadScene(string sceneName)
    {
        if (!isCurrentlyFading)
        {
            StartCoroutine(fadeAndSwitchScene(sceneName));
        }
    }

    private IEnumerator fade(float finalAlpha)
    {
        isCurrentlyFading = true;
        fadeOutImage_inclUI.enabled = true;

        float fadeSpeed = Mathf.Abs(fadeOutImage_inclUI.color.a - finalAlpha) / fadeDuration;

        while (!Mathf.Approximately(fadeOutImage_inclUI.color.a, finalAlpha))
        {
            fadeOutImage_inclUI.color = new Color(fadeOutImageColour.r, fadeOutImageColour.g, fadeOutImageColour.b, 
                Mathf.MoveTowards(fadeOutImage_inclUI.color.a, finalAlpha, fadeSpeed * Time.deltaTime));
            yield return null;
        }

        isCurrentlyFading = false;
        fadeOutImage_inclUI.enabled = false;
    }

    private IEnumerator fadeAndSwitchScene(string sceneName)
    {
        PlayerInputScript player = FindObjectOfType<PlayerInputScript>();
        player.setHighlightedInventoryItemToNull();
        player.setHighlightedWorldObjectToNull();

        yield return StartCoroutine(fade(1f));

        BeforeSceneUnload?.Invoke();

        yield return SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().buildIndex);

        yield return StartCoroutine(loadSceneAndSetActive(sceneName));

        AfterSceneLoad?.Invoke();

        yield return StartCoroutine(fade(0f));
    }

    private IEnumerator loadSceneAndSetActive(string sceneName)
    { 
        yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        Scene newlyLoadedScene = SceneManager.GetSceneAt(SceneManager.sceneCount -1);
        SceneManager.SetActiveScene(newlyLoadedScene);
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
        if (firstActorData.actorsThisCanBeCombinedWith[indexOfSecondActor].outcomeForThisActor == ActorData.ActorOutcomeAfterCombination.Deactivate)
            FindObjectOfType<InventoryScript>().removeItem(firstActorData);

        if (firstActorData.actorsThisCanBeCombinedWith[indexOfSecondActor].outcomeForOtherActor == ActorData.ActorOutcomeAfterCombination.Deactivate)
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
        if (firstActorData.actorsThisCanBeCombinedWith[indexOfSecondActor].outcomeForThisActor == ActorData.ActorOutcomeAfterCombination.Deactivate)
            FindObjectOfType<InventoryScript>().removeItem(firstActorData);
        if (firstActorData.actorsThisCanBeCombinedWith[indexOfSecondActor].outcomeForOtherActor == ActorData.ActorOutcomeAfterCombination.Deactivate)
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


            default:
                break;
        }

        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInputScript>().checkIfMouseHighlightingObject();
    }

    public bool separateActor(InteractableObjectScript worldObjectToSeparate) 
    {
        // splits previously combined world object into components

        ActorData objectToSeparateData = worldObjectToSeparate.data;

        if (!resolveSeparation(objectToSeparateData))
        {
            Debug.Log("separation failed");
            return false; // return false if separation is not possible
        }
        
        currentScene.deactivateWorldObject(worldObjectToSeparate.gameObject);

        return true;
    }

    public bool separateActor(ItemInteractionScript inventoryItemToSeparate)
    {
        // splits previously combined inventory item into components

        ActorData objectToSeparateData = inventoryItemToSeparate.dataOfItemInSlot;

        FindObjectOfType<InventoryScript>().removeItem(objectToSeparateData);

        if (!resolveSeparation(objectToSeparateData))
        {
            Debug.Log("separation failed");
            FindObjectOfType<InventoryScript>().addItem(objectToSeparateData);
            return false; // return false and readd removed item if separation is not possible
        }

        return true;
    }

    private bool resolveSeparation(ActorData actorToSeparate)
    {
        ActorData[] actorsToCreate = actorToSeparate.getComponents();
        // check if actor can be separated
        if (actorsToCreate.Length == 0)
            return false;

        for (int i = 0; i < actorsToCreate.Length; i++)
        {
            bool actorIsWorldObject = false;

            for (int j = 0; j < currentScene.inactiveWorldObjects.Count; j++)
            {
                if (actorsToCreate[i].actorName == currentScene.inactiveWorldObjects[j].GetComponent<InteractableObjectScript>().data.actorName)
                {
                    actorIsWorldObject = true;
                    currentScene.activateWorldObject(actorsToCreate[i]);
                    break;
                }
            }

            if (!actorIsWorldObject)
            {
                FindObjectOfType<InventoryScript>().addItem(actorsToCreate[i]);
            }
        }

        return true;
    }

    public PreviousConversationData getPreviousConversationData(CharacterData characterData)
    {
        foreach (PreviousConversationData conversationData in previousConversationData)
        {
            if (conversationData.characterName == characterData.characterName)
            {
                return conversationData;
            }
        }

        PreviousConversationData emptyData = new PreviousConversationData();
        emptyData.characterName = characterData.characterName;
        emptyData.numberOfItemsAlreadyShown = 0;
        emptyData.itemsAlreadyShown = new ActorData[16];

        return emptyData;
    }

    public void saveGame()
    {
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/savefile1.dat");

        SaveFileData saveData = new SaveFileData();
        // TODO: CREATE SAVE DATA CLASS
        // saveData.currentSceneNumber = currentScene.sceneID
        // etc.

        binaryFormatter.Serialize(file, saveData);
        file.Close();
    }

    public void loadGame()
    {
        if (File.Exists(Application.persistentDataPath + "/savefile1.dat"))
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/savefile1.dat", FileMode.Open);

            SaveFileData loadedData = (SaveFileData)binaryFormatter.Deserialize(file);
            file.Close();

            // TODO: CONVERT LOADED FILE DATA INTO GAME DATA
            // currentScene.sceneID = loadedData.currentSceneNumber;
            // etc.
        }
        else
        {
            // TODO: SET VALUES FOR EACH VARIABLE AS THEY WILL BE AT THE START OF THE GAME
            // chapterNumber = 0;
            // sceneNumber = 0;
            // etc.
        }
    }

}

[Serializable]
class SaveFileData
{
    public int currentChapterNumber;
    public int currentSceneNumber;
    public NoteData[] currentNotes;
    public ActorData[] inventory;
}
