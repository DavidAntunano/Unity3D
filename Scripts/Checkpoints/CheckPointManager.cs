using UnityEngine;

public class CheckPointManager : MonoBehaviour
{
    public CheckPoint[] arrayCheckPoints;
    GameManager gameManager;
    ScenesManager scenesManager;

    void Awake() {
        gameManager = GameManager.Instance;
        gameManager.SetCheckpointManager(tag);
        scenesManager = ScenesManager.Instance;
    }

    void Start() {
        if (arrayCheckPoints.Length < 2) {
            Debug.LogError("CHECKPOINT MANAGER ERROR - expected 2 checkpoint at least");
        } else {
            arrayCheckPoints[0].Type = CheckPoint.CheckpointType.SceneEntry;
            gameManager.SetPlayerSpawnPoint(arrayCheckPoints[0].transform, true);
            arrayCheckPoints[arrayCheckPoints.Length - 1].Type = CheckPoint.CheckpointType.SceneExit;

            if (gameManager.playerStatus.currentLevel == scenesManager.GetCurrentSceneNumber()) {
                for (int i = 0; i <= gameManager.playerStatus.currentCheckPoint; i++) {
                    arrayCheckPoints[i].done = true;
                    if (arrayCheckPoints[i].Type == CheckPoint.CheckpointType.SaveGame) {
                        gameManager.SetPlayerSpawnPoint(arrayCheckPoints[i].transform, true);
                    }
                }
            } else {
                for (int i = 0; i < arrayCheckPoints.Length; i++) {
                    if (arrayCheckPoints[i].Type == CheckPoint.CheckpointType.SaveGame) {
                        arrayCheckPoints[i].done = true;
                    };
                }
                gameManager.SetPlayerSpawnPoint(arrayCheckPoints[0].transform, true);
            }

            for (int i = 0; i < arrayCheckPoints.Length; i++) {
                if (arrayCheckPoints[i].Type == CheckPoint.CheckpointType.SceneEntry || arrayCheckPoints[i].Type == CheckPoint.CheckpointType.SceneExit) {
                    arrayCheckPoints[i].Color = CheckPoint.ColorMode.None;
                }
                arrayCheckPoints[i].ColorSetup();
            }
        }
    }
}