using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;

// HUDCONTROLLER
//  HUD related class

public class HUDController : Singleton<HUDController> {

	protected HUDController() { }

	public enum HUDStatus {Hidden, Info, Full, Pause};	//HUD States 
	public HUDStatus hudStatus = HUDStatus.Info;
	GameObject mobilePhone;                             //HUD COntainer
    public Slider health, stamina;	 					//HUD elements
	public Text mensaje, remite, healthText, goldText;
	public Button continueButton, exitButton;

	GameManager gameManager;
	public GameObject player;

    //HUD CONTROLLER SETUP
	void Awake () {
		gameManager = GameManager.Instance;
        gameManager.SetHUD(tag);
        player = GameObject.FindGameObjectWithTag("Player");
		mobilePhone = GameObject.FindGameObjectWithTag("mobileHUD");
	}

	void LateUpdate() {
		SetLifeBar();
		SetStaminaBar();
		SetGoldNumber();
	}
	
	//Animates HUD position by Status (Hidden, Info, Full, Pause)
	void AnimateHUDStatusChange() {
		switch (hudStatus) {
			case HUDStatus.Hidden:
				mobilePhone.transform.DOLocalMove(new Vector3(6.48f, -14.25f, 8.64f), 0.25f).SetEase(Ease.Linear);
				mobilePhone.transform.DOLocalRotate(new Vector3(0.0f, 189.7f, 0.0f), 0.30f).SetEase(Ease.Linear);
				break;
			case HUDStatus.Info:
				remite.enabled = false;
				mobilePhone.transform.DOLocalMove(new Vector3(6.48f, -12.34f, 8.64f), 0.25f).SetEase(Ease.Linear);
				mobilePhone.transform.DOLocalRotate(new Vector3(0.0f, 189.7f, 0.0f), 0.30f).SetEase(Ease.Linear);
				break;
			case HUDStatus.Full:
				remite.enabled = true;
				continueButton.enabled = false;
				exitButton.enabled = false;
				mobilePhone.transform.DOLocalMove(new Vector3(5.638f, -9.29f, 8.64f), 0.25f).SetEase(Ease.Linear);
				mobilePhone.transform.DOLocalRotate(new Vector3(0.0f, 200.096f, 0.0f), 0.30f).SetEase(Ease.Linear);
				break;
			case HUDStatus.Pause:
				remite.enabled = false;
				continueButton.enabled = true;
				exitButton.enabled = true;
				mobilePhone.transform.DOLocalMove(new Vector3(-1.365f, -3.8f, 7.58f), 0.45f).SetEase(Ease.Linear);
				mobilePhone.transform.DOLocalRotate(new Vector3(-16.76f, 159.2f, -2.88f), 0.50f).SetEase(Ease.Linear);
				break;
			default: break;
		}
	}

	IEnumerator WaitBeforeInfoStatus(float time) {
		yield return new WaitForSeconds(time);
		hudStatus = HUDStatus.Info;
		AnimateHUDStatusChange();
	}

	public bool ShowMessageWithTime (string remitent, string message, float time) {
        if (hudStatus == HUDStatus.Info)
        {
            remite.text = remitent;
            mensaje.text = message;
            hudStatus = HUDStatus.Full;
            AnimateHUDStatusChange();
            StartCoroutine("WaitBeforeInfoStatus", time);
            return true;
        }
        else return false;
	}

	public void ShowMessage(string remitent, string message) {
        remite.text = remitent;
		mensaje.text = message;
		hudStatus = HUDStatus.Full;
		AnimateHUDStatusChange();
	}

    public void HideHUD()
    {
        hudStatus = HUDStatus.Hidden;
        AnimateHUDStatusChange();
    }

    public void ShowPauseMessage() {
		remite.text = "PLAYER";
        mensaje.text = "|| PAUSE";
        hudStatus = HUDStatus.Pause;
        AnimateHUDStatusChange();
    }

    public void HidePauseMessage() {
        mensaje.text = "|> PLAY";
        hudStatus = HUDStatus.Info;
        AnimateHUDStatusChange();
    }

    //PRIVATE METHODS
	void SetLifeBar() {
		if (gameManager.playerStatus.playerHealth >= 0) {
			health.value = gameManager.playerStatus.playerHealth;
			healthText.text = gameManager.playerStatus.playerHealth.ToString() + "%";
		}
	}

	void SetStaminaBar() {
		stamina.value = player.GetComponent<Player>().currentStamina;
	}

	void SetGoldNumber() {
		goldText.text = " " + gameManager.playerStatus.gold.ToString();
	}
}
