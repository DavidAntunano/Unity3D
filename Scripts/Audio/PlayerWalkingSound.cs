using UnityEngine;
using System.Collections;

public class PlayerWalkingSound : MonoBehaviour {


    public AudioClip[] FootSteps;

    public AudioSource AudioSource;


    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {

        WalkingSound();

    }

    void WalkingSound() {

        if (Input.GetKeyDown(KeyCode.W)) {
       
            // sonidos de los pasos an andar 


        }

        if (Input.GetKeyDown(KeyCode.Space)) {
            AudioSource.clip = FootSteps[4]; // jump

            AudioSource.Play();
        }

    
    }


}
