using UnityEngine;
using System.Collections;

public class Microwave_damage : MonoBehaviour {

    // If the player enters the fire
	void OnTriggerEnter(Collider col) {
        if (col.tag == "Player") {
            GameManager.Instance.SetPlayerHealthBy(-25);
        }
    }
}
