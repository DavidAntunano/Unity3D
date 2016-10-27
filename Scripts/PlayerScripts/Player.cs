using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System;


public class Player : MonoBehaviour{

    #region Global properties
    [HideInInspector] public ThirdPersonCamera myCamera;
    [HideInInspector] public Transform cameraTransform;
    [HideInInspector] public Animator animator;
    [HideInInspector] public CapsuleCollider capsuleCollider;
    [HideInInspector] public float colliderRadius;
    [HideInInspector] public float colliderHeight;
    [HideInInspector] public Vector3 colliderCenter;
    [HideInInspector] public Rigidbody theRigidBody;
    #endregion

    #region Player properties
    public bool isDead = false;
    public float startingHealth = 100;
    public float startingStamina = 500;
    public float currentHealth { get; set; }
    public float currentStamina { get; set; }

    /* Movement variables */
    [HideInInspector] public float direction;
    [HideInInspector] public float speed;
    [HideInInspector] public float verticalVelocity;
    #endregion

    #region Weapon properties
    public GameObject holsteredRangedWeapon;
	public GameObject holsteredMeleeWeapon;
	public GameObject rangedWeaponInHand;
	public GameObject meleeWeaponInHand;
	public GameObject gunBarrelEnd;
    [HideInInspector] public WaitForSeconds shotDuration = new WaitForSeconds(0.07f);	// WaitForSeconds object used by our ShotEffect coroutine, determines time gun line will remain visible

    public int damagePerShot = 10;                                      // The damage inflicted by each bullet 
    public float timeBetweenBullets = 1.0f;                             // The time between each shot
    [HideInInspector] public float timer;                               // A timer to determine when to fire.
    public float range = 100f;					                        // The distance the gun can fire.

    [HideInInspector] public ParticleSystem gunParticles;				// Reference to the particle system.
    [HideInInspector] public ParticleSystem smokeParticles;             // Reference to the smoke particle system
    [HideInInspector] public AudioSource gunAudio;						// Reference to the audio source.

    [HideInInspector] public AudioSource meleeAudio;
    #endregion

    #region Controller properties
    [HideInInspector]
	public enum InputType{
		MouseKeyboard,
		Controller
	};

	[HideInInspector] public InputType inputType = InputType.MouseKeyboard;
    #endregion

    // Physics material to use while walking and in air
    [HideInInspector] public PhysicMaterial frictionPhysics, defaultPhysics;

    // Variable to check inputs used in "PlayerController", "PlayerMotor" and "PlayerAnimator"
    [HideInInspector] public Vector2 input;

	/* Property to stop all player animations*/
	[HideInInspector] public bool lockPlayer;

	public void InitialSetup(){
        // Cursor invisible
        Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;

		// Camera Setup
		myCamera = Camera.main.GetComponent<ThirdPersonCamera>();
		cameraTransform = Camera.main.transform;

		// Animator Setup
		animator = GetComponent<Animator>();

		// Rigidbody Setup
		theRigidBody = GetComponent<Rigidbody>();

		// Collider Setup
		capsuleCollider = GetComponent<CapsuleCollider>();
		colliderCenter = capsuleCollider.center;
		colliderRadius = capsuleCollider.radius;
		colliderHeight = capsuleCollider.height;

        // prevents the collider from slipping on ramps
        frictionPhysics = new PhysicMaterial();
        frictionPhysics.name = "frictionPhysics";
        frictionPhysics.staticFriction = 0.6f;
        frictionPhysics.dynamicFriction = 0.6f;

        // default physics 
        defaultPhysics = new PhysicMaterial();
        defaultPhysics.name = "defaultPhysics";
        defaultPhysics.staticFriction = 0f;
        defaultPhysics.dynamicFriction = 0f;

		// Set Up the references for shooting elements
		gunParticles = rangedWeaponInHand.GetComponent<ParticleSystem>();
        smokeParticles = gunBarrelEnd.GetComponent<ParticleSystem>();
        gunAudio = rangedWeaponInHand.GetComponent<AudioSource> ();

		// Set Up the references for melee components
		meleeAudio = meleeWeaponInHand.GetComponent<AudioSource>();
    }

	/* GAMEPAD VIBRATION */
	#if !UNITY_WEBPLAYER && !UNITY_ANDROID && !UNITY_IOS
	public IEnumerator GamepadVibration(float vibTime){
		if (inputType == InputType.Controller){
			XInputDotNetPure.GamePad.SetVibration(0, 1, 1);
			yield return new WaitForSeconds(vibTime);
			XInputDotNetPure.GamePad.SetVibration(0, 0, 0);
		}
	}
	#endif
}
