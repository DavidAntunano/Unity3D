using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Light))]
public class brokenLight : MonoBehaviour {

    Light light;
    SphereCollider collider;

    [Range(0.5f, 20.0f)]
	public float ciclo = 2.0f;
    
	public float pause = 0.2f;
	public bool lightOn = true;

	public bool explodesWhenNear = true;
	[Range(2f, 20.0f)]
	public float distanceToExplode = 5.0f;

    float minLight, maxLight;
    float targetIntensity;
	float randomTime;
    

    void Awake() {
		light = GetComponent<Light>();
		if (explodesWhenNear) {
			collider = gameObject.AddComponent<SphereCollider>();
			collider.radius = distanceToExplode;
			collider.isTrigger = true;
		}
		light.intensity = 0f;
		targetIntensity = maxLight;
	}

	void Update() {
		if (lightOn) {
			light.intensity = Mathf.Lerp(light.intensity, targetIntensity, ciclo * Time.deltaTime);
			CheckTargetIntensity();
		} else
			light.intensity = Mathf.Lerp(light.intensity, 0f, 2.0f * Time.deltaTime);
	}

	void CheckTargetIntensity() {
		if (Mathf.Abs(targetIntensity - light.intensity) < 0.2) {
			StartCoroutine("WaitAndChange");
		}
	}

	void OnTriggerEnter (Collider other) {
		if (other.tag == "Player") {
			lightOn = false;
            //TO-DO: algo mas explosivo que solo apagar bombilla...
		}
	}

	IEnumerator WaitAndChange() {
		minLight = Random.Range(0f, 1f);
		maxLight = Random.Range(3f, 8f);
		pause = Random.Range(0.01f, 0.5f);
		yield return new WaitForSeconds(pause);
		if (targetIntensity == maxLight)
			targetIntensity = minLight;
		else
			targetIntensity = maxLight;
	}

	
}