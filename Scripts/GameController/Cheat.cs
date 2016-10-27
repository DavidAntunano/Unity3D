using UnityEngine;
using System.Collections;

public class Cheat : MonoBehaviour {
    public GameObject[] doors;

    void OnTriggerEnter(Collider other) {
        for (int i = 0; i < doors.Length; i++)
        {
            doors[i].GetComponent<Door>().locked = false;
        }
    }
}
