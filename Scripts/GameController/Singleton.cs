using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour {

	private static T _instance;
	private static object _lock = new object();

	public static T Instance {
		get {
			if (applicationIsQuitting) {
				Debug.LogWarning("[Singleton] Cerrando APP. Instancia '" + typeof(T) + "' eliminada.");
				return null;
			}

			lock (_lock) {
				if (_instance == null) {
					_instance = (T)FindObjectOfType(typeof(T));

					if (FindObjectsOfType(typeof(T)).Length > 1) {
						Debug.LogError("[Singleton] ERROR FATAL - Reinicia Unity");
						return _instance;
					}

					if (_instance == null) {
						GameObject singleton = new GameObject();
						_instance = singleton.AddComponent<T>();
						singleton.name = "(singleton) " + typeof(T).ToString();
						DontDestroyOnLoad(singleton);
						Debug.Log("[Singleton] Necesaria instancia de " + typeof(T) +", '" + singleton + "' con DontDestroyOnLoad.");
					} else {
						Debug.Log("[Singleton] Usando instancia: " + _instance.gameObject.name);
					}
				}

				return _instance;
			}
		}
	}

	private static bool applicationIsQuitting = false;
	
	public void OnDestroy() {
		applicationIsQuitting = true;
	}
}