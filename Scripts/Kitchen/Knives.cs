using UnityEngine;
using System.Collections;
using DG.Tweening;
using System;

[Serializable]
public class Knives : MonoBehaviour {

	public Vector3[] directions;
	public bool backToOrigin;
	public int loops;
	[Range(1.0f, 10.0f)]
	public float sectionTime = 1.0f;
	[Range(0.0f, 5.0f)]
	public float stopTime = 1.0f;

	Sequence sequence;
	Transform knife;
	Vector3 pivote, temp;

	void Start() {
		knife = GetComponent<Transform> ();
		pivote = knife.position;
		temp = pivote;
		if (loops == 0) {
            loops = -1;
        }
		Recorrido();
	}

	void Recorrido() {
		sequence = DOTween.Sequence();
		for (int i = 0; i < directions.Length; i++) {
			temp = temp + directions[i];
			sequence.AppendInterval(stopTime);
			sequence.Append(knife.transform.DOMove(temp, sectionTime).SetEase(Ease.Linear));
		}
		if (backToOrigin) {
			sequence.Append(knife.transform.DOMove(pivote, sectionTime).SetEase(Ease.Linear));
			sequence.SetLoops(loops, LoopType.Restart);
		}
	}
}
