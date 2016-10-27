using UnityEngine;
using System.Collections;

public class PlayerController : PlayerAnimator {

    void Awake() {
		GameManager.Instance.SetPlayer(tag);
		/*
		 * Coroutine that throws constantly raycast to check:
			- The interactable objects
			- Stop the character if hits the wall
			- Stop the character from walking on ramps using a slope limit angle
		*/
		StartCoroutine("UpdateRaycast");    // limit raycasts calls for better performance
    }

    void Start() {
        InitialSetup();         // Character initia setup: Get all components that are needed from the character
    }

    void FixedUpdate() {
        UpdateMotor();          // Update all character movements
        UpdateAnimator();       // Update all character animations
    }

    void Update() {
        // Add the time since Update was last called to the timer.
        timer += Time.deltaTime;
    }

    void LateUpdate() {
        HandleInput();          // Handle input: Get all controls mouse & keyboard or controller
    }

    /* HANDLE ALL ACTIONS INPUTS */
    void HandleInput() {
        if (!lockPlayer) {
            ControllerInput();
            RunningInput();
            RollingInput();
            CrouchInput();
            JumpInput();
            AimInput();
            AttackInput();
        } else {
            input = Vector2.zero;
            speed = 0f;
            canSprint = false;
        }
    }

    /* MOVEMENT INPUT CONFIGURATION	*/
    void ControllerInput() {
        if (inputType == InputType.MouseKeyboard) {
            input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        } else if (inputType == InputType.Controller) {
            float deadzone = 0.25f;
            input = new Vector2(Input.GetAxis("LeftAnalogHorizontal"), Input.GetAxis("LeftAnalogVertical"));

            if (input.magnitude < deadzone)
                input = Vector2.zero;
            else
                input = input.normalized * ((input.magnitude - deadzone) / (1 - deadzone));
        }
    }

    /* RUNNING INPUT CONFIGURATION */
    void RunningInput() {
        if (Input.GetButtonDown("Run") && currentStamina > 0 && input.sqrMagnitude > 0.1f) {
            if (onGround && !strafing && !crouch)
                canSprint = true;
        } else if (Input.GetButtonUp("Run") || currentStamina <= 0 || input.sqrMagnitude < 0.1f || strafing)
            canSprint = false;

        if (canSprint) {
            currentStamina -= 0.5f;
            if (currentStamina < 0)
                currentStamina = 0;
        } else {
            currentStamina += 1f;
            if (currentStamina >= startingStamina)
                currentStamina = startingStamina;
        }
    }

    /* ROLLING INPUT CONFIGURATION */
    void RollingInput() {
        if (Input.GetButtonDown("Roll")) {
            Rolling();
        }
    }

    /* CROUCHING INPUT CONFIGURATION
		Change "pressToCrouch" in PlayerMotor to:
			- Crouch on button hit once.
			- Crouch while mantaining the button. */
    void CrouchInput() {
        if (pressToCrouch) {
            crouch = Input.GetButton("Crouch") && onGround && !actions;
        } else {
            crouch = !crouch;
        }
    }

    /* JUMPING INPUT CONFIGURATION */
    void JumpInput() {
        bool jumpConditions = !crouch && onGround && !actions;
        if (Input.GetButtonDown("Jump") && jumpConditions) {
            jump = true;
        }
    }

    /* AIM INPUT CONFIGURATION */
    void AimInput() {
        if ((Input.GetButtonDown("Aim") || Input.GetAxis("Aim2") <= -0.5) && !actions) {
            locomotionType = LocomotionType.OnlyStrafe;
            strafing = true;
        }
        if (Input.GetButtonUp("Aim")) {
            locomotionType = LocomotionType.FreeWithStrafe;
            strafing = false;
        }
    }

    /* ATTACK INPUT CONFIGURATION */
    void AttackInput() {
        if (strafing) {
            //Aiming: Shoot
            if (Input.GetButtonDown("Fire") && timer >= timeBetweenBullets) {
                Shooting();
            }
        } else {
            //Not aiming: Melee attack
            if (Input.GetButtonDown("Fire")) {
                MeleeAttacking();
            }
        }
    }

    /* UPDATE RAYCASTS: Handles a separate update for better performance */	
    public IEnumerator UpdateRaycast() {
        while (true) {
            yield return new WaitForEndOfFrame();

            CheckForwardAction();
            StopMove();
            SlopeLimit();
        }
    }

    /* ACTIONS: Check if there is anything interactable ahead */			
    void CheckForwardAction() {
        var hitObject = CheckActionObject();
        if (hitObject != null) {
            try {
                if (hitObject.CompareTag("ClimbUp"))
                    DoAction(hitObject, ref climbUp);
                else if (hitObject.CompareTag("StepUp"))
                    DoAction(hitObject, ref stepUp);
            } catch (UnityException e) {
                Debug.LogWarning(e.Message);
            }
        }
    }

    void DoAction(GameObject hitObject, ref bool action) {
        var triggerAction = hitObject.transform.GetComponent<TriggerAction>();
        if (!triggerAction) {
            //Debug.LogWarning("Missing TriggerAction Component on " + hitObject.transform.name + "Object");
            return;
        }
        if (Input.GetButton("A") && !actions || triggerAction.autoAction && !actions) {
            // turn the action bool true and call the animation
            action = true;
            // find the cursorObject height to match with the character animation
            matchTarget = triggerAction.target;
            // align the character rotation with the object rotation
            var rot = hitObject.transform.rotation;
            transform.rotation = rot;
        }
    }
}
