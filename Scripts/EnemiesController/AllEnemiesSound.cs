using UnityEngine;
using UnityEngine.Audio;

public class AllEnemiesSound : MonoBehaviour {
    public AudioMixerGroup weaponGroup;                 // Reference to the enemy weapon group    
    public AudioMixerGroup voicesGroup;                 // Reference to the enemy voices group    

    public AudioClip enemyDetectSound;                  // Reference to the enemy detect sound    
    public AudioClip[] enemyAttackSound;                // Reference to the enemy attack sound   
    public AudioClip enemyHurtingSound;                 // Reference to the enemy hurting sound 
    public AudioClip enemyDieSound;                     // Reference to the enemy dying sound   

    private AudioSource enemyAudioSource;				// Reference to the audio source.

    // Use this for initialization
    void Start () {
        enemyAudioSource = GetComponent<AudioSource>();

        // Default sound for the enemy is detect sound
        enemyAudioSource.clip = enemyDetectSound;
        enemyAudioSource.outputAudioMixerGroup = voicesGroup;
    }

    // Function to call when the enemy detects the player
    public void enemiesDetectingSound() {
		// Change the output mixer to voices group
        enemyAudioSource.outputAudioMixerGroup = voicesGroup;

		// Change the clip to the enemy detect sound
        enemyAudioSource.clip = enemyDetectSound;
        enemyAudioSource.Play();
    }
    
    // Function to call when the enemy attacks the player
    public void enemiesAttackingSound(string attackType) {
		// Change the output mixer to voices group
        enemyAudioSource.outputAudioMixerGroup = weaponGroup;

		// Choose the attack type to play the correponding audio clip
		switch (attackType){
			case "Throw":
				enemyAudioSource.clip = enemyAttackSound[0];		// Enemy throw attack sound
				break;
			case "Attack":
				enemyAudioSource.clip = enemyAttackSound[1];		// Enemy normal attack sound
				break;
			case "Punch":
				enemyAudioSource.clip = enemyAttackSound[2];		// Enemy punch attack sound
				break;
			default:
				enemyAudioSource.clip = enemyAttackSound[1];		// For default enemy normal attack sound
				break;
		}
        enemyAudioSource.Play();
    }

    // Function to call when the player hurts the enemy
    public void enemiesHurtingSound() {
		// Change the output mixer to voices group
        enemyAudioSource.outputAudioMixerGroup = voicesGroup;

		// Change the clip to the enemy hurting sound
        enemyAudioSource.clip = enemyHurtingSound;
        enemyAudioSource.Play();
    }

    // Function to call when the enemy dies
    public void enemiesDyingSound() {
		// Change the output mixer to voices group
        enemyAudioSource.outputAudioMixerGroup = voicesGroup;

		// Change the clip to the enemy dying sound
        enemyAudioSource.clip = enemyDieSound;
        enemyAudioSource.Play();
    }
}
