using UnityEngine;
using System.Collections;

public class Horno : MonoBehaviour {

    public int firesDuration = 5;
    public int timeBetweenFires = 2;
    public GameObject[] fireList;

    private int[] lightedFires = new int[4];
    private int sequenceIndex = 1;

    private AudioSource[] fireSounds = new AudioSource[4];
    private ParticleSystem[] fireParticles = new ParticleSystem[4];
    private Collider[] fireColliders = new Collider[4];

    // Use this for initialization
    void Start () {
        FiresPreparation();
        StartCoroutine("Timer");
    }

    void FiresPreparation() {
        for (int i = 0; i < fireList.Length; i++) {
            fireSounds[i] = fireList[i].GetComponent<AudioSource>();
            fireParticles[i] = fireList[i].GetComponent<ParticleSystem>();
            fireColliders[i] = fireList[i].GetComponent<Collider>();
        }
    }

    IEnumerator Timer() {
        while (true) {
            SequenceChoose();
            StopAllFires();
            yield return new WaitForSeconds(timeBetweenFires);
            ActivateFires();
            yield return new WaitForSeconds(firesDuration);
        }
    }

    void SequenceChoose() {
        // 0 - Front left fire
        // 1 - Front right fire
        // 2 - Back right fire 
        // 3 - Back left fire
        switch (sequenceIndex) {
            case 1:
                // 01 light fire 
                // 23 extinct fire
                lightedFires[0] = 1;
                lightedFires[1] = 1;
                lightedFires[2] = 0;
                lightedFires[3] = 0;
                break;
            case 2:
                // 01 extinct fire
                // 23 light fire
                lightedFires[0] = 0;
                lightedFires[1] = 0;
                lightedFires[2] = 1;
                lightedFires[3] = 1;
                break;
            case 3:
                // 02 light fire
                // 13 extinct fire
                lightedFires[0] = 1;
                lightedFires[1] = 0;
                lightedFires[2] = 1;
                lightedFires[3] = 0;
                break;
            case 4:
                // 02 extinct fire
                // 12 light fire
                lightedFires[0] = 0;
                lightedFires[1] = 1;
                lightedFires[2] = 0;
                lightedFires[3] = 1;
                break;
        }

        if (sequenceIndex != 4) {
            sequenceIndex += 1;
        } else {
            sequenceIndex = 1;
        }
    }

    void StopAllFires() {
        // Stop all fires sounds, particlesystems and disable the colliders
        for (int i = 0; i < fireList.Length; i++) {
            fireColliders[i].enabled = false;
            fireSounds[i].Stop();
            fireParticles[i].Stop();
        }
    }

    void ActivateFires() {
        //Activate only the sequenced fires
        for (int i = 0; i < fireList.Length; i++) {
            if (lightedFires[i] == 1) {
                fireParticles[i].Play();
                fireSounds[i].Play();
                fireColliders[i].enabled = true;
            }
        }
    }
}


 
 
 
