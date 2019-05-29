using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GoToNewAreaTriggerScript : MonoBehaviour
{
    public int idOfNewArea;
    public Vector2 playerNewPosition;

    private Vector3 V3_playerNewPosition;

    // Start is called before the first frame update
    void Start()
    {
        V3_playerNewPosition = new Vector3(playerNewPosition.x, GameObject.FindGameObjectWithTag("Player").transform.position.y, playerNewPosition.y);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void goToNewArea()
    {
        Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, (idOfNewArea * 5000f) + 800f);

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        player.transform.position = V3_playerNewPosition;
        // WHEN PLAYED CLICKS ON GOTO OBJECT,
        // DO SCREEN FADE
        // MOVE PLAYER TO NEW POSITION
        // FADE CAMERA IN
    }
}
