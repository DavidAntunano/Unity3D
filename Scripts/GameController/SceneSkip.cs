using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class SceneSkip : MonoBehaviour {

    void OnAwake() {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;    
    }

	void Start () {
        gameObject.GetComponent<AudioSource>().PlayDelayed(2f);
        StartCoroutine("NextLevel", 5f);
	}

    IEnumerator NextLevel(float time)
    {
        yield return new WaitForSeconds(time);
        SceneManager.LoadScene(1);
    }

    void OnDestroy() {
        Cursor.visible = true;
    }

}
