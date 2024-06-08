using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ScreenTransistor : MonoBehaviour {

    [HideInInspector] public bool transitToNextScreen = false;

    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        //print(transitToNextScreen);
    }

    void OnTriggerEnter2D(Collider2D other) {
        
        if (other.CompareTag("Player")) {
            transitToNextScreen = true;
            print("TriggerCustomEvent");
        }

    }
}
