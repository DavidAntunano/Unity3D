using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour {

	public Transform pivot;
    public string tagToOpen = "Player";
    public bool locked;
    [Tooltip("Door closes behind you")]
    public bool autoClose = true;
    [Tooltip("Works one time, locks you inside. Overrides Locked and AutoClose")]
    public bool itsATrap = false;
	[Range(1f, 10f)]
	public float speed = 2f;

	private bool canOpen, stop, isOpen, isInTransition;
	private Vector3 currentAngle;
	private float forwardDotVelocity;
	private bool invertAngle;
    private int multiDoor = 1;

	//metodos públicos para el gameplay (bloqueo puertas)
	public void EnableOpen() {
		locked = false;
	}

	public void DisableOpen() {
		if (isOpen) {
			StartCoroutine(_Close());
		}
		locked = true;
	}


	void Start() {
		if (!pivot) this.enabled = false;
        if (pivot.name == "pivot_b") {
            multiDoor = -1;
        }
	}

	//Corutina apertura de puertas
	IEnumerator _Open() {
		isInTransition = true;
		while (currentAngle.x != (invertAngle ? -90f : 90f)) {
			yield return new WaitForEndOfFrame();
			if (invertAngle) {
				currentAngle.x -= (speed * 10f) * Time.deltaTime;
				currentAngle.x = Mathf.Clamp(currentAngle.x, -90f, 0);
			} else {
				currentAngle.x += (speed * 10f) * Time.deltaTime;
				currentAngle.x = Mathf.Clamp(currentAngle.x, 0, 90f);
			}
			pivot.localEulerAngles = -currentAngle * multiDoor;
		}
		isInTransition = false;
		isOpen = true;
	}

	//Corutina cierre de puertas
	IEnumerator _Close() {
		yield return new WaitForSeconds(1f);
		isInTransition = true;
		while (currentAngle.x != 0) {
			yield return new WaitForEndOfFrame();
			if (stop)
				break;
			if (invertAngle) {
				currentAngle.x += (speed * 50f) * Time.deltaTime;
				currentAngle.x = Mathf.Clamp(currentAngle.x, -90f, 0);
			} else {
				currentAngle.x -= (speed * 50f) * Time.deltaTime;
				currentAngle.x = Mathf.Clamp(currentAngle.x, 0, 90f);
			}
			pivot.localEulerAngles = -currentAngle * multiDoor;
		}
		if (!stop) {
			isInTransition = false;
		}
		stop = false;
		isOpen = false;
	}

	//mejor que trigger enter
	void OnTriggerStay(Collider collider) {
		if (!isOpen && tagToOpen.Equals(collider.tag) && !locked) {
			forwardDotVelocity = Mathf.Abs(Vector3.Angle(transform.forward, collider.transform.position - transform.position));
			if (forwardDotVelocity < 60.0f) {
				if (!isInTransition || (currentAngle.x > -30f && currentAngle.x < 30f))
					invertAngle = false;
				canOpen = true;
			} else if (forwardDotVelocity >= 60.0f && forwardDotVelocity < 120f) {
				canOpen = false;
			} else {
				if (!isInTransition || (currentAngle.x > -30f && currentAngle.x < 30f))
					invertAngle = true;
				canOpen = true;
			}

			if (canOpen && !isOpen) {
				StartCoroutine(_Open());
			}
		} else if (isInTransition && isOpen && tagToOpen.Equals(collider.tag)) {
			//stop = true;
			//isOpen = false;
		}
	}

	void OnTriggerExit(Collider collider) {
		if (isOpen && tagToOpen.Equals(collider.tag) && autoClose) {
			StartCoroutine(_Close());
		}
        if (itsATrap && tagToOpen.Equals(collider.tag))
        {
            autoClose = true;
            locked = true;
            StartCoroutine(_Close());
        }
    }
}
