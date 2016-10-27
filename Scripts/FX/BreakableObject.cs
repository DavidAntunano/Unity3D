using UnityEngine;
using System.Collections;


public class BreakableObject:MonoBehaviour{
	public Transform fragments;
	public float waitForRemoveCollider = 1.0f;
	public float waitForRemoveRigid = 10.0f;
	public float waitForDestroy = 2.0f;
	public float explosiveForce = 750.0f;
	public enum FragmentsDirectionConstrainType { None, X, Y, Z };
	public FragmentsDirectionConstrainType Constrain;

	Transform fragmentd;
	bool broken;
	bool gravity = true;
	float xDir = 0.1f;
	float yDir = 0.1f;
	float zDir = 0.1f; 
    public bool destroyNow;

	void Start() {
		switch (Constrain) {
			case FragmentsDirectionConstrainType.X:
				xDir = 1.0f;
				break;
			case FragmentsDirectionConstrainType.Y:
				yDir = 1.0f;
				break;
			case FragmentsDirectionConstrainType.Z:
				zDir = 1.0f;
				gravity = false;
				break;
			default:
                xDir = 1.0f;
                yDir = 1.0f;
                zDir = 1.0f;
                break;
		}
	}

    void Update() {
        if (destroyNow) {
            triggerBreak();
        }
    }

	public void OnCollisionEnter(Collision collision) {
        triggerBreak();
	}
	
	public void triggerBreak() {
	    Destroy(transform.FindChild("object").gameObject);
	    Destroy(transform.GetComponent<Collider>());
	    Destroy(transform.GetComponent<Rigidbody>());
	    StartCoroutine(breakObject());
	}
	
	public IEnumerator breakObject() {
		
	    if (!broken) {
	    
	    	if(this.GetComponent<AudioSource>() != null){
				GetComponent<AudioSource>().pitch = Time.timeScale;
				GetComponent<AudioSource>().Play();
			}
	    	
	    	broken = true;
	        fragmentd = (Transform)Instantiate(fragments, transform.position, transform.rotation); // adds fragments to stage (!memo:consider adding as disabled on start for improved performance > mem)
	        fragmentd.localScale = transform.localScale; // set size of fragments

	        Transform frags = fragmentd.FindChild("fragments");
	        foreach(Transform child in frags) {
				child.GetComponent<Rigidbody>().useGravity = gravity;
				child.GetComponent<Rigidbody>().AddForce(xDir*Random.Range(-explosiveForce, explosiveForce), yDir*Random.Range(-explosiveForce, explosiveForce), zDir*Random.Range(-explosiveForce, explosiveForce));
	            child.GetComponent<Rigidbody>().AddTorque(Random.Range(-explosiveForce, explosiveForce), Random.Range(-explosiveForce, explosiveForce), Random.Range(-explosiveForce, explosiveForce));
	        }
	        StartCoroutine(removeColliders());
	        StartCoroutine(removeRigids());
	        if (waitForDestroy > 0) { // destroys fragments after "waitForDestroy" delay
	            foreach(Transform child in transform) {
	   					child.gameObject.SetActive(false);
				}				
	            yield return new WaitForSeconds(waitForDestroy);
	            GameObject.Destroy(fragmentd.gameObject); 
	            GameObject.Destroy(transform.gameObject);
	        }else if (waitForDestroy <=0){ // destroys gameobject
	        	foreach(Transform child in transform) {
	   					child.gameObject.SetActive(false);
				}
	        	yield return new WaitForSeconds(1.0f);
	            GameObject.Destroy(transform.gameObject);
	        }	
	    }
	}
	
	public IEnumerator removeRigids() {// removes rigidbodies from fragments after "waitForRemoveRigid" delay
	    if (waitForRemoveRigid > 0 && waitForRemoveRigid != waitForDestroy) {
	        yield return new WaitForSeconds(waitForRemoveRigid);
	        foreach(Transform child in fragmentd.FindChild("fragments")) {
	            child.GetComponent<Rigidbody>().isKinematic = true;
	        }
	    }
	}
	
	public IEnumerator removeColliders() {// removes colliders from fragments "waitForRemoveCollider" delay
	    if (waitForRemoveCollider > 0){
	        yield return new WaitForSeconds(waitForRemoveCollider);
	        foreach(Transform child in fragmentd.FindChild("fragments")) {
	            child.GetComponent<Collider>().enabled = false;
	        }
	    }
	}
}