using UnityEngine;
using System.Collections;
using UnityStandardAssets.ImageEffects;
using DG.Tweening;
using System;

public class MenuAnimator : Singleton<MenuAnimator>
{

    protected MenuAnimator() { }

    public bool isAnimating = false;

    GameObject mCamera, sCamera, player;
    bool sCameraON = false;
    bool playerON = false;

    Sequence sequence;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        mCamera = GameObject.FindGameObjectWithTag("MainCamera");
        sCamera = GameObject.FindGameObjectWithTag("UICamera");
        if (player != null) playerON = true;
        if (sCamera != null) sCameraON = true;
    }

    void SetAnimation(bool isAnimation)
    {
        isAnimating = isAnimation;
        player.GetComponent<PlayerController>().lockPlayer = isAnimation;
        mCamera.GetComponent<ThirdPersonCamera>().enabled = !isAnimation;
        sCamera.GetComponent<Camera>().enabled = !isAnimation;
    }

    public void StartAnimation()
    {
        SetAnimation(true);
    }

    public void StopAnimation()
    {
        SetAnimation(false);
    }

    IEnumerator DoCameraTravel(float delay)
    {
        sCamera.SetActive(false);
        yield return new WaitForSeconds(delay);
        SwitchBlurEffects(false);
    }

    IEnumerator DoTimeWait(float delay)
    {
        yield return new WaitForSeconds(delay);
        SwitchBlurEffects(false);
    }

    public void SwitchBlurEffects(bool state)
    {
        mCamera.GetComponent<Blur>().enabled = state;
        mCamera.GetComponent<DepthOfField>().enabled = state;
    }


    public void DoNew()
    {
        mCamera.transform.DOMove(new Vector3(-2.18f, 2f, -4.21f), 1.5f).SetEase(Ease.Linear);
        mCamera.transform.DORotate(new Vector3(26.8601f, 18.54f, 3.2827f), 1.5f).SetEase(Ease.Linear);
        StartCoroutine("DoCameraTravel", 1.5f);

    }

    public void DoContinue()
    {
        mCamera.transform.DOMove(new Vector3(-2.18f, 2f, -4.21f), 1.5f).SetEase(Ease.Linear);
        mCamera.transform.DORotate(new Vector3(26.8601f, 18.5487f, 3.2827f), 1.5f).SetEase(Ease.Linear);
        StartCoroutine("DoCameraTravel", 1.5f);
    }

    public void DoOptions()
    {
        mCamera.transform.DOMove(new Vector3(-1.37f, 5.61f, -4.81f), 3f).SetEase(Ease.Linear);
        mCamera.transform.DORotate(new Vector3(15.29f, -73.2f, 0f), 3f).SetEase(Ease.Linear);
        sCamera.transform.DOMove(new Vector3(-1.37f, 5.61f, -4.81f), 3f).SetEase(Ease.Linear);
        sCamera.transform.DORotate(new Vector3(15.29f, -73.2f, 0f), 3f).SetEase(Ease.Linear);
    }

    public void DoCredits()
    {
        mCamera.transform.DOMove(new Vector3(-1.334f, 4.306f, -0.904f), 3f).SetEase(Ease.Linear);
        mCamera.transform.DORotate(new Vector3(9.716599f, -6.8861f, -9.8651f), 3f).SetEase(Ease.Linear);
        sCamera.transform.DOMove(new Vector3(-1.334f, 4.306f, -0.904f), 3f).SetEase(Ease.Linear);
        sCamera.transform.DORotate(new Vector3(9.716599f, -6.8861f, -9.8651f), 3f).SetEase(Ease.Linear);
        StartCoroutine("DoTimeWait", 3f);
    }

    public void DoBack()
    {
        SwitchBlurEffects(true);
        mCamera.transform.DOMove(new Vector3(-0.717f, 2.112f, -6.33f), 3f).SetEase(Ease.Linear);
        mCamera.transform.DORotate(new Vector3(19.5812f, -68.111f, 0f), 3f).SetEase(Ease.Linear);
        sCamera.transform.DOMove(new Vector3(-0.717f, 2.112f, -6.33f), 3f).SetEase(Ease.Linear);
        sCamera.transform.DORotate(new Vector3(19.5812f, -68.111f, 0f), 3f).SetEase(Ease.Linear);
    }
}
