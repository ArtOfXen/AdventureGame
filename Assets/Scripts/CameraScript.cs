using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    GameObject player;
    Vector3 offset;
    private float cameraOrbitDistance = 200f;
    [HideInInspector] public float currentRotation; // this is used to change which direction the player moves when they input movement

    // Start is called before the first frame update
    void Start()
    {
        //player = GameObject.FindGameObjectWithTag("Player");
        //offset = new Vector3(50f, cameraOrbitDistance, 50f);
        //transform.position = player.transform.position + offset;
        //transform.LookAt(player.transform, new Vector3(0, 1f, 0));
        //transform.LookAt(Vector3.zero, new Vector3(0, 1f, 0));
        //currentRotation = transform.rotation.y;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        //transform.position = new Vector3(
        //    cameraOrbitDistance * Mathf.Cos(currentRotation) + player.transform.position.x,
        //    cameraOrbitDistance,
        //    cameraOrbitDistance * Mathf.Sin(currentRotation) + player.transform.position.z);
        //transform.LookAt(player.transform, new Vector3(0, 1f, 0));
        //transform.LookAt(Vector3.zero, new Vector3(0, 1f, 0));
    }

    public void rotateCamera(float deltaTime)
    {
        //currentRotation += deltaTime * 2.5f;
    }
}
