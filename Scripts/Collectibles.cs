using UnityEngine;
using DG.Tweening;
using System.Collections;

public class Collectibles : MonoBehaviour
{
	GameManager gameManager;
	Sequence sequence1, sequence2;
	AudioSource audioSource;
	SphereCollider sphereCollider;
	public AudioClip collectibleGet, collectibleLose;
	float autoPivot;
	
	public enum CollectibleType { Health, Gold };
    public CollectibleType Type;
	public int gold = 10;
	public int health = 25;
	[Range(5, 50)]
	public int lifeTime = 20;

	void Start() {
        gameManager = GameManager.Instance;
		audioSource = GetComponent<AudioSource>();
		StartCoroutine("DestroyCollectible", lifeTime);
		autoPivot = transform.position.y;
		sequence1 = DOTween.Sequence();
		sequence1.Append(transform.DOLocalMoveY(autoPivot + 0.4f, 3f).SetEase(Ease.Linear));
		sequence1.SetLoops(-1, LoopType.Yoyo);
		sequence2 = DOTween.Sequence();
		sequence2.Append(transform.DOLocalRotate(new Vector3(-90f, 180f, 0f), 3f).SetEase(Ease.Linear));
		sequence2.SetLoops(-1, LoopType.Incremental);
	}

    void OnTriggerEnter(Collider other) {
        if (other.tag == "Player") {
			switch (Type) {
                case CollectibleType.Health:
					gameManager.SetPlayerHealthBy(health);
                    break;
                case CollectibleType.Gold:
					gameManager.SetGoldBy(gold);
                    break;
                default: break;
            }
			audioSource.PlayOneShot(collectibleGet, 1F);
			Destroy(gameObject);
		}
	}

	IEnumerator DestroyCollectible(int lifeTime) {
		yield return new WaitForSeconds(lifeTime);
		audioSource.PlayOneShot(collectibleLose, 1F);
		Destroy(gameObject);
	}
}