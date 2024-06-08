    using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathCall : MonoBehaviour {

    [HideInInspector] public bool isPlayerDead = false;


    void OnTriggerEnter2D(Collider2D other) {
    
    if (other.CompareTag("Player")) {
        isPlayerDead = true;
    }

    }
}
