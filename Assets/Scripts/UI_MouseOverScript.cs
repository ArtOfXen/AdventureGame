using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_MouseOverScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    // this script is attached to the UI canvas, and stops the mouse raycast from hitting objects beneath the UI

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // TODO: exception for when player isn't found OR change the mouseOverUI bool to be in gameManager
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInputScript>().mouseOverUI = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInputScript>().mouseOverUI = false;
    }
}
