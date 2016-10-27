using UnityEngine;

public class CheckPoint : MonoBehaviour {

	public enum CheckpointType { SaveGame, Message , DeathTrigger, SceneEntry, SceneExit };
    public CheckpointType Type;

    public enum ColorMode { None, Off, Switch };
	public ColorMode Color;
    public string remitent = "UNKNOWN";
    public string message = "Mensaje";

	public float time = 3.0f;
    [Tooltip("Disables checkpoint itself, autosetting")]
    public bool done = false;
    [Tooltip("SceneExit loads levelToLoad. Next level if 0")]
    public int levelToLoad = 0;

    HUDController hudController;
	XMLManager savegame;
	GameManager gameManager;

	public GameObject rojo, verde, neutro;

    void Awake() {
    }

	void Start() {
		savegame = XMLManager.Instance;
		gameManager = GameManager.Instance;
        hudController = gameManager.hudMobile;
    }

    public void ColorSetup() {
        switch (Color)
        {
            case ColorMode.None:
                rojo.SetActive(false);
                verde.SetActive(false);
                neutro.SetActive(false);
                break;
            case ColorMode.Switch:
                rojo.SetActive(!done);
                verde.SetActive(done);
                neutro.SetActive(false);
                break;
            case ColorMode.Off:
                rojo.SetActive(false);
                verde.SetActive(false);
                neutro.SetActive(true);
                break;
            default: break;
        }
    }

	void OnTriggerEnter(Collider other) {
		if (other.tag == "Player" && !done) {
			if (Color == ColorMode.Switch) {
				rojo.SetActive(false);
				verde.SetActive(true);
			}
            switch (Type) {
				case CheckpointType.SaveGame:
					done = hudController.ShowMessageWithTime("System", "Checkpoint", time);
                    if (done) {
                        gameManager.SetPlayerSpawnPoint(this.transform, false);
                        gameManager.playerStatus.currentCheckPoint++;
                        savegame.SaveData();
                    } 
					break;
				case CheckpointType.Message:
                    done = hudController.ShowMessageWithTime(remitent, message, time);
					break;
				case CheckpointType.DeathTrigger:
                    hudController.ShowMessageWithTime("System", "Eso va a doler", time);
                    GameObject.Find("MainCamera").GetComponent<ThirdPersonCamera>().enabled = false;
                    gameManager.SetPlayerHealthBy(-100);
					break;
                case CheckpointType.SceneExit:
                    if (levelToLoad == 0) {
                        gameManager.NextLevel();
                    }
                    else gameManager.LoadLevel(levelToLoad);
                    break;
                default: break;
			}
		}
	}

}