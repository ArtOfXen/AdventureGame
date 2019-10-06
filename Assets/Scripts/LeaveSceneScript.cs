using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaveSceneScript : MonoBehaviour
{

    public string adjacentSceneName;
    private GameObject player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }
    private void OnMouseEnter()
    {
        // show object name as floating text
        player.GetComponent<PlayerInputScript>().setHighlightedWorldObjectToSceneExit(this);
    }

    private void OnMouseExit()
    {
        // remove floating text created above

        player.GetComponent<PlayerInputScript>().stopHighlighingSceneExit();
    }
}
