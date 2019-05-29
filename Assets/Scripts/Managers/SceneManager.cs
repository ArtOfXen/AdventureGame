using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager : MonoBehaviour
{
    public int id;
    public string sceneName;
    public List<GameObject> activeWorldObjects;
    public List<GameObject> inactiveWorldObjects;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void deactivateWorldObject(GameObject worldObject)
    {
        for (int i = 0; i < activeWorldObjects.Count; i++)
        {
            if (activeWorldObjects[i] == worldObject)
            {
                
                worldObject.SetActive(false);
                inactiveWorldObjects.Add(worldObject);
                activeWorldObjects.RemoveAt(i);
                break;
            }
        }
    }

    public void activateWorldObject(ActorData worldObjectData)
    {
        for (int i = 0; i < inactiveWorldObjects.Count; i++)
        {
            if (inactiveWorldObjects[i].GetComponent<InteractableObjectScript>().data = worldObjectData)
            {
                activeWorldObjects.Add(inactiveWorldObjects[i]);
                inactiveWorldObjects[i].SetActive(true);
                inactiveWorldObjects.RemoveAt(i);
                break;
            }
        }
    }
}
