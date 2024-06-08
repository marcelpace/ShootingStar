    using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class SceneManagerScript : MonoBehaviour {

    private Scene currentScene;

    private TextMeshProUGUI sceneTitle;

    private ScreenTransistor screenTransistor;
    private DeathCall deathCall;

  


    void Awake() {

        currentScene = SceneManager.GetActiveScene();

        GameObject textObject = GameObject.Find("SceneTitle");
        sceneTitle = textObject.GetComponent<TextMeshProUGUI>();
        sceneTitle.text = currentScene.name;

        //Debug.Log("Active Scene name is: " + currentScene.name + "    Active Scene index: " + currentScene.buildIndex);

        // Screen constrains
        //print("Screen width: " +Screen.width + ", Screen height: " +Screen.height);


        screenTransistor = GameObject.FindWithTag("TransitionWall").GetComponent<ScreenTransistor>();        
        deathCall = GameObject.FindWithTag("DeathGround").GetComponent<DeathCall>();

    }

       


    
    // Update is called once per frame
    void Update() {

        if(screenTransistor.transitToNextScreen) {
            SceneManager.LoadScene(currentScene.buildIndex+1);
            screenTransistor.transitToNextScreen = false;
        }

        if(deathCall.isPlayerDead) {
            SceneManager.LoadScene(currentScene.buildIndex);
            deathCall.isPlayerDead = false;
        }
    }
}
