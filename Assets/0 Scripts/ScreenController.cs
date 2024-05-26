using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// handle screen size, constrains, and detecting if the player leaves it, so the SceneManagerSript can load a new scene 
public class ScreenController : MonoBehaviour {

    [HideInInspector] public Camera cam;

    [HideInInspector] public float camHeight;
    [HideInInspector] public float camWidth;

    [HideInInspector] private GameObject player;
    private BoxCollider2D playerBc2d;
    [HideInInspector] public float screenWidth;
    [HideInInspector] public float screenHeigth;
    [HideInInspector] public float halfCamWidth;
    [HideInInspector] public float halfCamHeight;


    // Start is called before the first frame update
    void Start() {
        // Camera constrains
        //Here we calculate camera width
        cam = Camera.main;
        camHeight = 2f * cam.orthographicSize;
        camWidth = camHeight * cam.aspect;
        halfCamWidth = camWidth * 0.5f;
        halfCamHeight = camHeight * 0.5f;


        /*
        screenHeigth = Screen.height;
        screenWidth = Screen.width;
        */


        player = GameObject.FindWithTag("Player");
        playerBc2d = player.GetComponent<BoxCollider2D>();
    


    }

    // Update is called once per frame
    void Update() {
        // transition to other screen if player leaves playarea
        // that means, if half of player is out of the screen area, he left the area
        //if ()

        print("Cam Width: " +camWidth + ", Cam Height: " +camHeight + ", pX: " +player.transform.position.x);


        //clamp player x position so he does not leaves play area
    }
}
