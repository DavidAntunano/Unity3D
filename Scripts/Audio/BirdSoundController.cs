using UnityEngine;
using System.Collections;

public class BirdSoundController : MonoBehaviour {


    public AudioSource audioSource;
    public float MinRangeToPlay;
    public float MaxRangeToPlay;
    float randomTime;
    public int pickedSound;
    public int rayMax; // The max amount of Sounds there are

    public AudioClip[] ricochet; // The array controlling the sounds
    void Awake() {
      
    }


	// Update is called once per frame
	void Update () {
	
	}

    IEnumerator Start() {

       
        while (true) {
            pickedSound = Random.Range(0, rayMax); // Grab a random sound out of the max
            randomTime = GetRandomNumber();
            audioSource.clip = ricochet[pickedSound];
           // yield return new WaitForSeconds(1.2f);
            audioSource.Play();
            yield return new WaitForSeconds(randomTime);
           // Debug.LogWarning(randomTime);
        }
    }

    float GetRandomNumber() {
        return Random.Range(MinRangeToPlay, MaxRangeToPlay);
    }


}
