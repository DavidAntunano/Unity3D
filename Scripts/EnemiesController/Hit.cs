using UnityEngine;

public class Hit : MonoBehaviour {
    
    public int damage = 5;

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            GameManager.Instance.SetPlayerHealthBy(-damage);
        }
    }
}
