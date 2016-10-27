using UnityEngine;
using System.Collections;

public class SirenMove : MonoBehaviour {

    public float jumpSpeed = 50.0f;
    public float rotateSpeed = 225.0f;
    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void FixedUpdate () {

        transform.position += (transform.forward) * Time.deltaTime * jumpSpeed;
        transform.Rotate(Vector3.right, rotateSpeed * Time.deltaTime);
    }
}
