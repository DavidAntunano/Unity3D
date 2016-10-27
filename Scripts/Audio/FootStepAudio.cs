using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;

public class FootStepAudio : MonoBehaviour {

	public PlayerController playerController;

	public float defaultPitchMin = 0.75f;					// Minimum pitch applied to the 'random.range' for variation in the footstep sound.
	public float defaultPitchMax = 1.0f;					// Same as above but for the maximum value in the 'random.range'.

	public float defaultVolumeMin = 0.8f;					
	public float defaultVolumeMax = 1.0f;

	public AudioSource carpetFootstep;
	public AudioSource grassFootstep;
	public AudioSource metalFootstep;
	public AudioSource rockFootstep;
	public AudioSource sandFootstep;
	public AudioSource woodFootstep;

    private string floorType;

    void FootstepAudioEvent() {
        GetFloorType();
        PlayAudio(); 
    }

    void GetFloorType() {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit)) {
            floorType = hit.collider.tag;
        }
    }

    void PlayAudio(){
		if (playerController.onGround && playerController.speed > 0){
			switch (floorType){
				case "CarpetFloor":
					WalkOnAudioSource (carpetFootstep);
					break;
				case "GrassFloor":
					WalkOnAudioSource(grassFootstep);
					break;
				case "MetalFloor":
					WalkOnAudioSource(metalFootstep);
					break;
				case "RockFloor":
					WalkOnAudioSource(rockFootstep);
					break;
				case "SandFloor":
					WalkOnAudioSource(sandFootstep);
					break;
				case "WoodFloor":
					WalkOnAudioSource(woodFootstep);
					break;
				default:
					WalkOnAudioSource(carpetFootstep);
					break;
			}
		}
	}

	void WalkOnAudioSource (AudioSource audio) {
		audio.volume = Random.Range(defaultVolumeMin, defaultVolumeMax);
		audio.pitch = Random.Range(defaultPitchMin, defaultPitchMax);
		if (audio.isPlaying) audio.Stop();
		audio.Play();
	}
}
