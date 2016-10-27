using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager> {

	protected GameManager() { }

	public PlayerStatus playerStatus = new PlayerStatus();
	public Preferences preferences = new Preferences();

    public GameObject player, hud, checkpointManager, soundManager, cameraAnimator;
    public HUDController hudMobile;
    public CheckPointManager checkpoints;
    public SoundManager sounds;
    public CameraAnimator cutscene;
    [HideInInspector]
    public bool continueButton = false;

    public Transform playerSpawn;

	GameObject camara;
    XMLManager loadGame;
 
	bool playerIsAlive = true;
	bool pausedGame = false;
	
	void Awake() {
        loadGame = XMLManager.Instance;
		loadGame.LoadData();
    }

	void Update() {
		PauseGame();
	}

	public void PauseGame() {
		if (Input.GetKeyDown(KeyCode.Escape) && !pausedGame) {
			switch (ScenesManager.Instance.GetCurrentSceneNumber()) {
				case 1: //main menu, do nothing
					break;
				case 2: //intro, skip
					NextLevel();
					break;
				default: //game levels, pause
					hudMobile.ShowPauseMessage();
					pausedGame = true; 
                    DOTween.To(value => Time.timeScale = value, 1.0f, 0.0f, 1.0f).SetEase(Ease.Linear).SetUpdate(true);
					cutscene.SwitchBlurEffects(pausedGame);
                    sounds.PitchBackgroundMusic(pausedGame);
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.Confined;
                    break;
			}			
		} else if ((Input.GetKeyDown(KeyCode.Escape) && pausedGame) || continueButton) {
            hudMobile.HidePauseMessage();
            pausedGame = false;
			continueButton = false;
			cutscene.SwitchBlurEffects(pausedGame);
			DOTween.To(value => Time.timeScale = value, 0.0f, 1.0f, 1.0f).SetEase(Ease.Linear).SetUpdate(true);
            sounds.PitchBackgroundMusic(pausedGame);
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            
        }
	}

	public void SetQuality(int quality) {
		switch (quality) {
			case 2:
				QualitySettings.SetQualityLevel(0, true);
                preferences.qualityLevel = 0;
                break;
			case 1:
				QualitySettings.SetQualityLevel(2, true);
                preferences.qualityLevel = 2;
                break;
			case 0:
				QualitySettings.SetQualityLevel(5, true);
                preferences.qualityLevel = 5;
                break;
			default:
				break;
		}
	}

    public int GetQuality() {
        switch (preferences.qualityLevel) {
            case 5:
                return 0;
            case 2:
                return 1;
            case 0:
                return 2;
            default: return 0;
        }
    }

	//Allow player script to set the reference each level load
    public void SetPlayer(string playerName) {
        player = GameObject.FindGameObjectWithTag(playerName);
        if ((player != null) && (playerSpawn != null)) {
            player.transform.position = playerSpawn.transform.position;
            player.transform.rotation = playerSpawn.transform.rotation;
        }
	}

	//Allow HUDController to set its reference on each level load
    public void SetHUD(string hudName) {
        hud = GameObject.FindGameObjectWithTag(hudName);
        if (hud != null) {
            hudMobile = hud.GetComponent<HUDController>();
        }
    }

	//Allow Checkpoint Manager to set its reference on each level load
	public void SetCheckpointManager(string checkpointManagerName) {
        checkpointManager = GameObject.FindGameObjectWithTag(checkpointManagerName);
        if (checkpointManager != null) {
            checkpoints = checkpointManager.GetComponent<CheckPointManager>();
        }
    }

	//Allow Sound Manager to set its reference on each level load
	public void SetSoundManager(string soundManagerName) {
        soundManager = GameObject.FindGameObjectWithTag(soundManagerName);
        if (soundManager != null) {
            sounds = soundManager.GetComponent<SoundManager>();
        }
    }

    //Allow Camera Animator to set its reference on each level load
    public void SetCameraAnimator(string cameraAnimatorName) {
        cameraAnimator = GameObject.FindGameObjectWithTag(cameraAnimatorName);
        if (cameraAnimator != null){
            cutscene = cameraAnimator.GetComponent<CameraAnimator>();
        }
    }

    public void SetPlayerSpawnPoint(Transform spawn, bool set) {
		playerSpawn = spawn;
		if (set) SetPlayer(player.tag);
	}

    public void SetPlayerHealthBy(int health) {
        playerStatus.playerHealth += health;
        if (playerStatus.playerHealth > 100)
        {
            playerStatus.playerHealth = 100;
        }
        else if (playerStatus.playerHealth <= 0 && playerIsAlive) {
			playerIsAlive = false;
            player.GetComponent<PlayerController>().isDead = true;
            player.GetComponent<PlayerAnimator>().lockPlayer = true;
            StartCoroutine("RespawnPlayer", 5f);
        }
    }

    public void SetGoldBy(int gold) {
        playerStatus.gold += gold;
    }

    public void NextLevel() {
        int nextScene = ScenesManager.Instance.GetCurrentSceneNumber() + 1;
		Time.timeScale = 1.0f; 
        if (SceneManager.sceneCountInBuildSettings >= nextScene) {
			playerStatus.currentLevel++;
            playerStatus.currentCheckPoint = 0;
            loadGame.SaveData();
            ScenesManager.Instance.LoadNextScene();
        }
    }

    public void LoadLevel(int levelToLoad) {
        Time.timeScale = 1.0f;
        if (SceneManager.sceneCountInBuildSettings >= levelToLoad)
        {
            playerStatus.currentLevel = levelToLoad;
            playerStatus.currentCheckPoint = 0;
            loadGame.SaveData();
            ScenesManager.Instance.LoadSceneByNumber(levelToLoad);
        }
    }


	IEnumerator RespawnPlayer (float delay) {
		yield return new WaitForSeconds(delay);
		playerStatus.playerHealth = 100;
		player.GetComponent<PlayerController>().isDead = false;
		player.GetComponent<PlayerAnimator>().lockPlayer = false;
		GameObject.Find("MainCamera").GetComponent<ThirdPersonCamera>().enabled = true;
		playerIsAlive = true;
        SetPlayer(player.tag);
		loadGame.SaveData();
	}
}

[System.Serializable]
public class PlayerStatus {
	public int playerHealth;
	public int currentLevel;
	public int currentCheckPoint;
    public int gold;
}

[System.Serializable]
public class Preferences {
	public float effectsVolume;
	public float musicVolume;
	public bool vibration;
	public int qualityLevel;
}