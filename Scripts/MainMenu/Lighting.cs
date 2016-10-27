using UnityEngine;
using System.Collections;
using DG.Tweening;

public class Lighting : MonoBehaviour {

    public Transform luz;
	void Start () {
        if (!luz) this.enabled = false;
        transform.DORotate(new Vector3(41.38f, -36f, 14.7f), 16f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
    }
}
