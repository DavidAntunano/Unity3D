using UnityEngine;
using System.Collections;

public class MagicAttack : MonoBehaviour {

	public float destroyTime = 5.0f;
	public int damage = 10;

	void Start() {
		Destroy(gameObject, destroyTime);
	}

	void OnTriggerEnter(Collider other) {
		if (other.tag == "Player") {
			GameManager.Instance.SetPlayerHealthBy(-damage);
			if (gameObject.GetComponent<AudioSource>() != null) {
				gameObject.GetComponent<AudioSource>().Play();
			}
			gameObject.GetComponent<SphereCollider>().enabled = false;
		}
	}
}