using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

public class MenuController : MonoBehaviour {

    public List<MenuButtonBase> menuButtonList = new List<MenuButtonBase>();
    public List<MenuSliderBase> menuSliderList = new List<MenuSliderBase>();
    public List<MenuCheckboxBase> menuCheckboxList = new List<MenuCheckboxBase>();
    public List<MenuDropdownBase> menuDropdownList = new List<MenuDropdownBase>();

    GameManager gameManager;
	ScenesManager scenesManager;
    MenuAnimator menuAnimator;

	void Awake() {
		gameManager = GameManager.Instance;
		scenesManager = ScenesManager.Instance;
        menuAnimator = MenuAnimator.Instance;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;

        menuButtonList = GetComponentsInChildren<MenuButtonBase>().ToList();
        menuSliderList = GetComponentsInChildren<MenuSliderBase>().ToList();
        menuCheckboxList = GetComponentsInChildren<MenuCheckboxBase>().ToList();
        menuDropdownList = GetComponentsInChildren<MenuDropdownBase>().ToList();

        menuButtonList.ForEach(m => {
			m.OnButtonClicked += OnButtonList;
		});
        menuSliderList.ForEach(m => {
            m.OnSliderDragged += OnSliderList;
        });
        menuCheckboxList.ForEach(m => {
            m.OnCheckboxClicked += OnCheckboxList;
        });
        menuDropdownList.ForEach(m => {
            m.OnDropdownClicked += OnDropdownList;
        });
    }

    private void OnDropdownList(MenuDropdownBase sender) {
        switch (sender.dropdownType) {
            case MenuDropdownBase.DropdownTypes.QualitySetting:
                gameManager.SetQuality(sender.GetComponent<Dropdown>().value);
                break;
            default:
                break;
        }
    }

    private void OnCheckboxList(MenuCheckboxBase sender) {
        switch (sender.checkboxType) {
            case MenuCheckboxBase.CheckboxTypes.Vibration:
                gameManager.preferences.vibration = sender.GetComponent<Toggle>().isOn;
                break;
            default:
                break;
        }
    }

    private void OnSliderList(MenuSliderBase sender) {
        switch (sender.sliderType) {
            case MenuSliderBase.SliderTypes.musicVolume:
                gameManager.preferences.musicVolume = sender.GetComponent<Slider>().value / 100;
                break;
            case MenuSliderBase.SliderTypes.effectVolume:
                gameManager.preferences.effectsVolume = sender.GetComponent<Slider>().value / 100;
                break;
            default:
                break;
        }
    }

    private void OnButtonList(MenuButtonBase sender) {
        switch (sender.buttonType) {
            case MenuButtonBase.ButtonTypes.NewGame:
				XMLManager.Instance.NewGame();
                menuAnimator.DoNew();
                scenesManager.LoadSceneByNumberDelay(2, 1.45f);
                break;
            case MenuButtonBase.ButtonTypes.Continue:
                menuAnimator.DoContinue();
                scenesManager.LoadSceneByNumberDelay(gameManager.playerStatus.currentLevel, 1.45f);
				break;
            case MenuButtonBase.ButtonTypes.Options:
                Slider musicVol = GameObject.Find("volumeSlider").GetComponent<Slider>();
                Slider effectsVol = GameObject.Find("effectSlider").GetComponent<Slider>();
                Toggle vibration = GameObject.Find("vibrationToggle").GetComponent<Toggle>();
                Dropdown quality = GameObject.Find("qualityDropdown").GetComponent<Dropdown>();
                musicVol.normalizedValue = gameManager.preferences.musicVolume;
                effectsVol.normalizedValue = gameManager.preferences.effectsVolume;
                vibration.isOn = gameManager.preferences.vibration;
                quality.value = gameManager.GetQuality();
                menuAnimator.DoOptions();
                break;
            case MenuButtonBase.ButtonTypes.Credits:
                menuAnimator.DoCredits();
                break;
            case MenuButtonBase.ButtonTypes.Exit:
                XMLManager.Instance.SaveData();
                Application.Quit();
                break;
            case MenuButtonBase.ButtonTypes.Back:
                XMLManager.Instance.SaveData();
                menuAnimator.DoBack();
                break;
            case MenuButtonBase.ButtonTypes.Resume:
                gameManager.continueButton = true;
                break;
            case MenuButtonBase.ButtonTypes.ToMainMenu:
                ScenesManager.Instance.LoadSceneByNumber(1);
                break;
            default:
                break;
        }
    }

    void OnDestroy() {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}