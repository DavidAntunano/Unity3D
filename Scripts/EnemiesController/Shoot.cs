using UnityEngine;
using System.Collections;

public class Shoot : MonoBehaviour {

	public float moveSpeed = 7.0f;
	public float destroyTime = 5.0f;
	public int damage = 5;

	[HideInInspector]
	public Transform target;

	void Start() {
		Destroy(gameObject, destroyTime);
	}

	void Update() {
		transform.position += transform.forward * moveSpeed * Time.deltaTime;
	}

	void OnTriggerEnter(Collider other) {
		if (other.tag == "Player") {
			GameManager.Instance.SetPlayerHealthBy(-damage);
			if (gameObject.GetComponent<AudioSource>() != null) {
				gameObject.GetComponent<AudioSource>().Play();
			}
		}
        if (destroyTime != 10)
        {
            gameObject.SetActive(false);
			Destroy(gameObject);  
        }
	}
}