using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ScenesManager : Singleton<ScenesManager> {

	protected ScenesManager() { }

	GameManager gameManager;

	void Awake() {
		gameManager = GameManager.Instance;
	}

    public int GetCurrentSceneNumber() {
        return SceneManager.GetActiveScene().buildIndex;
    }

    public void LoadNextScene() {
        LoadSceneByNumber(GetCurrentSceneNumber() + 1);
    }

	public void LoadSceneByNumber (int escena) {
		if (SceneManager.sceneCountInBuildSettings >= escena) {
			gameManager.playerStatus.currentLevel = escena;
			SceneManager.LoadScene(escena);
		} else {
			Debug.LogWarning("Escena " + escena + " no parece existir");
		}
	}

	public void LoadSceneByNumberDelay(int escena, float espera) {
		object[] parms = new object[2] { escena, espera };
		StartCoroutine("DoWaitAndLoad", parms);
	}

    public void LoadSceneByName (string escena) {
        if (!SceneManager.GetSceneByName(escena).isLoaded) {
            SceneManager.LoadScene(escena, LoadSceneMode.Additive);
        } else {
            Debug.LogWarning("Escena " + escena + " no parece existir o ya esta cargada");
        }
    }

    public void UnloadSceneByName(string escena) {
        if (SceneManager.GetSceneByName(escena).isLoaded) {
            StartCoroutine("UnloadScene", escena);
        } else {
            Debug.LogWarning("Escena " + escena + " no parece existir o no esta cargada");
        }
    }

    private IEnumerator DoWaitAndLoad(object[] parms) {
		yield return new WaitForSeconds((float)parms[1]);
		LoadSceneByNumber((int)parms[0]);
	}

    private IEnumerator UnloadScene(string escena)
    {
        yield return new WaitForSeconds(1.46f);
        SceneManager.UnloadScene(escena);
    }


}
