using UnityEngine;

public abstract class Saver : MonoBehaviour
{

    public string uniqueIdentifier;
    public SaveData saveData;

    protected string key;

    private void Awake()
    {
        key = setKey();
    }

    private void OnEnable()
    {
        // 'subscribes' this class to the BeforeSceneUnload and AfterSceneLoad functions
        // which means that when those functions are called, the save and load functions of
        // this class are also called
        GameManagerScript.gameManager.BeforeSceneUnload += save;
        GameManagerScript.gameManager.AfterSceneLoad += load;
    }

    private void OnDisable()
    {
        // 'unsubscribes' this class from the BeforeSceneUnload and AfterSceneLoad functions
        GameManagerScript.gameManager.BeforeSceneUnload -= save;
        GameManagerScript.gameManager.AfterSceneLoad -= load;
    }

    protected abstract string setKey();

    protected abstract void save();

    protected abstract void load();
}