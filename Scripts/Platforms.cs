using UnityEngine;
using System.Collections;
using DG.Tweening;
using System;

[Serializable]
public class Platforms : MonoBehaviour {

	public GameObject platform;
	public Vector3[] directions;
	public bool backToOrigin;
    public bool waitInLast;
	public int loops;
	[Range(3.0f, 30.0f)]
	public float sectionTime = 5.0f;
	[Range(0.0f, 5.0f)]
	public float stopTime = 1.0f;
	public bool waitForPlayer;
	[Range(0, 60)]
	public int timeToFall;

	Sequence sequence;
	GameObject player;
	Vector3 pivote, temp;

	IEnumerator TimeToFall() {
        yield return new WaitForSeconds(timeToFall - 3);
        platform.transform.DOShakePosition(3.0f, 0.2f, 10, 0.0f, false);
	    yield return new WaitForSeconds(timeToFall);
		sequence.Kill();
        platform.GetComponent<Rigidbody>().isKinematic = false;
        platform.GetComponent<BoxCollider>().enabled = false;
        player.transform.parent = GameObject.Find("Personaje").transform;
    }

	void Start() {
		player = GameObject.FindGameObjectWithTag("Player");
		pivote = platform.transform.position;
		temp = pivote;
		if (loops == 0) {
            loops = -1;
        }
		if (!waitForPlayer) {
			Recorrido();
		}
		
	}

	void Recorrido() {
		sequence = DOTween.Sequence();
		for (int i = 0; i < directions.Length; i++) {
			temp = temp + directions[i];
			sequence.AppendInterval(stopTime);
			sequence.Append(platform.transform.DOMove(temp, sectionTime).SetEase(Ease.Linear));
		}
        if (waitInLast) {
            sequence.AppendInterval(stopTime);
        }
		if (backToOrigin) {
			sequence.Append(platform.transform.DOMove(pivote, sectionTime).SetEase(Ease.Linear));
			sequence.SetLoops(loops, LoopType.Restart);
		} else {
			sequence.SetLoops(loops, LoopType.Yoyo);
		}
	}

	void OnCollisionEnter(Collision col) {
		if (col.gameObject.tag == "Player") {
			player.transform.parent = platform.transform;
            if (timeToFall > 0)
            {
                StartCoroutine("TimeToFall");
            }
        }
		if (waitForPlayer && (col.gameObject.tag == "Player")) {
			Recorrido();
		}
	}

	void OnCollisionExit(Collision col) {
		if (col.gameObject.tag == player.gameObject.tag) {
			player.transform.parent = null;
		}
	}
}
