using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MapScreenButtonScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    GameObject textComponent;
    public GameObject mapScreen;

    // Start is called before the first frame update
    void Start()
    {
        textComponent = transform.GetChild(0).gameObject;
        textComponent.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        textComponent.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        textComponent.SetActive(false);
    }

    public void OnClick(string nameOfNextScene)
    {
        if (nameOfNextScene == "null")
        {
            Debug.Log("No scene name entered for button's OnClick event");
        }
        else
        {
            GameManagerScript.gameManager.fadeAndLoadScene(nameOfNextScene);
        }
    }

}
