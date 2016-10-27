using UnityEngine;
using System.Collections;
using UnityStandardAssets.ImageEffects;
using DG.Tweening;

public class IntroAnimator : Singleton<IntroAnimator> {

    protected IntroAnimator() { }

	ScenesManager scenesManager;
    public GameObject mCamera, fx, breaker;
    public GameObject dolarRain1, dolarRain2, dolarRain3;
    Sequence sequence, sequence2;

    void Awake () {
		scenesManager = ScenesManager.Instance;
        mCamera = GameObject.FindGameObjectWithTag("MainCamera");
        fx = GameObject.FindGameObjectWithTag("Storm");
        breaker = GameObject.FindGameObjectWithTag("Breaker");
        dolarRain1 = GameObject.Find("DolarRain1");
        dolarRain2 = GameObject.Find("DolarRain2");
        dolarRain3 = GameObject.Find("DolarRain3");
    }

    void Start () {
        //StartAnimation();
        StartCoroutine("BreakGlass", 15.4f);
		StartCoroutine("MoveCamera1", 17.1f);
        StartCoroutine("MoveCamera2", 17.50f);
        StartCoroutine("MoveCamera3", 18.50f);
        StartCoroutine("NextLevel", 20.0f);
        sequence = DOTween.Sequence();
        sequence.Append(mCamera.transform.DOMove(new Vector3(112.2f, 56.6f, -108.5f), 15.1f).SetEase(Ease.Linear).SetUpdate(true));
        sequence.Insert(4, mCamera.transform.DORotate(new Vector3(0f, 0f, 0f), 8.5f).SetEase(Ease.Linear).SetUpdate(true));
        sequence.Append(mCamera.transform.DOMove(new Vector3(112.2f, 56.6f, -97.9f), 2f).SetEase(Ease.Linear).SetUpdate(true));
        


        //StopAnimation();
    }

    void SetAnimation(bool isAnimation)
    {
        mCamera.GetComponent<ThirdPersonCamera>().enabled = !isAnimation;
    }

    public void StartAnimation()
    {
        SetAnimation(true);
    }

    public void StopAnimation()
    {
        SetAnimation(false);
    }

    IEnumerator BreakGlass(float delay)
    {
        yield return new WaitForSeconds(delay);
        //fx.SetActive(false);
        dolarRain1.GetComponent<ParticleSystem>().Play();
        breaker.GetComponent<Rigidbody>().AddForce(0f, 0f, 500f);
        DOTween.To(value => Time.timeScale = value, 1.0f, 0.2f, 0.5f).SetEase(Ease.Linear).SetUpdate(true);
	}

	IEnumerator MoveCamera1(float delay) {
		yield return new WaitForSeconds(delay);
		mCamera.transform.position = new Vector3(110.64f, 56.6f, -98.5f);
        mCamera.transform.rotation = Quaternion.Euler(new Vector3(0f, -90f, 0f));
        mCamera.GetComponent<Camera>().fieldOfView = 40;
        mCamera.transform.DOMove(new Vector3(110.64f, 56.6f, -96.83f), 2f).SetEase(Ease.Linear).SetUpdate(true);
        dolarRain2.GetComponent<ParticleSystem>().Play();
    }

    IEnumerator MoveCamera2(float delay)
    {
        yield return new WaitForSeconds(delay);
        mCamera.transform.position = new Vector3(110.62f, 56.15f, -95.92f);
        mCamera.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
        mCamera.GetComponent<Camera>().fieldOfView = 30;
        mCamera.transform.DOMove(new Vector3(110.62f, 56.15f, -90.14f), 5f).SetEase(Ease.Linear).SetUpdate(true);
        dolarRain3.GetComponent<ParticleSystem>().Play();

    }

    IEnumerator MoveCamera3(float delay)
    {
        yield return new WaitForSeconds(delay);
        mCamera.transform.position = new Vector3(112.89f, 56.15f, -90.14f);
        mCamera.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
        mCamera.GetComponent<Camera>().fieldOfView = 15;
        sequence2.Append(mCamera.transform.DOMove(new Vector3(112.89f, 56.85f, -74.81f), 8f).SetEase(Ease.Linear).SetUpdate(true));
        sequence2.Insert(0, DOTween.To(value => mCamera.GetComponent<Camera>().fieldOfView = value, 15f, 60f, 8f).SetEase(Ease.Linear).SetUpdate(true));
    }

    IEnumerator NextLevel(float delay) {
		yield return new WaitForSeconds(delay);
		Time.timeScale = 1f;
        scenesManager.LoadNextScene();

	}

	void SwitchBlurEffects(bool state) {
		mCamera.GetComponent<Blur>().enabled = state;
		mCamera.GetComponent<DepthOfField>().enabled = state;
	}
}
