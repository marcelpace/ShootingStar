using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class SceneManagerScript : MonoBehaviour {

    private Scene currentScene;

    private TextMeshProUGUI sceneTitle;


    [HideInInspector] public GameObject player;
  



    void Awake() {

        currentScene = SceneManager.GetActiveScene();

        GameObject textObject = GameObject.Find("SceneTitle");
        sceneTitle = textObject.GetComponent<TextMeshProUGUI>();
        sceneTitle.text = currentScene.name;

        //Debug.Log("Active Scene name is: " + currentScene.name + "    Active Scene index: " + currentScene.buildIndex);

        // Screen constrains
        print("Screen width: " +Screen.width + ", Screen height: " +Screen.height);

        player = GameObject.FindWithTag("Player");
    }

       


    


    // Update is called once per frame
    void Update() {

        if(Input.GetKeyDown(KeyCode.N)) {
            print(currentScene.buildIndex);
            SceneManager.LoadScene(currentScene.buildIndex+1);
            
        }
    }
}
