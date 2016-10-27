using UnityEngine;

//CUSTOM COLLECTIBLE MANAGER
//Autodetects any Collectible item in game with tag = "Collectible"
//Spawns one random collectible at desired location
//Can override random spawn for Health (customizable)

public class CollectibleManager : Singleton<CollectibleManager> {

	protected CollectibleManager() { }

	[Range(0, 100)]
	public int collectibleProbability = 30;
    public bool forcesHealthkitWhenLowLife = true;
    public string healthCollectibleName = "collect_health";
    [Range(5, 50)]
    public int lifeToSpawnHealthkit = 25;

	GameObject[] collectibles;
	GameObject healthPrioritizer;


	void Awake() {
		if (collectibles == null)
			collectibles = GameObject.FindGameObjectsWithTag("Collectible");
		    foreach (GameObject collectible in collectibles) {
			    if (collectible.name == healthCollectibleName) {
				    healthPrioritizer = collectible;
			}
		}
        if (healthPrioritizer == null) forcesHealthkitWhenLowLife = false;
    }

    //public function, instantiates a random collectible at target location
    //if playerHealth is low, the collectible is a health kit
	public void spawnCollectible (Transform targetLocation) {
		if (Random.Range(0, 100) >= 70) {
			if (forcesHealthkitWhenLowLife && GameManager.Instance.playerStatus.playerHealth <= lifeToSpawnHealthkit) {
				Instantiate(healthPrioritizer, targetLocation.position, targetLocation.rotation);
			} else {
				Instantiate(collectibles[Random.Range(0, collectibles.Length)], targetLocation.position, targetLocation.rotation);
			}
		}
		
	} 
}
