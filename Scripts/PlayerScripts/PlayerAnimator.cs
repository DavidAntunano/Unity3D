using UnityEngine;
using System.Collections;

public class PlayerAnimator : PlayerMotor {

	#region Variables
	// access the animator states (layers)
	[HideInInspector] public AnimatorStateInfo stateInfo;
    // match cursorObject to help animation to reach their cursorObject
    [HideInInspector] public Transform matchTarget;

    [HideInInspector] public float oldSpeed;
    public float speedTime {
        get {
            var _speed = animator.GetFloat("Speed");
            var acceleration = (_speed - oldSpeed) / Time.fixedDeltaTime;
            oldSpeed = _speed;
            return Mathf.Round(acceleration);
        }
    }
    #endregion

    public void UpdateAnimator(){
		timeBetweenShooting += Time.deltaTime;
		timeBetweeenAttacks += Time.fixedDeltaTime;

		stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        LocomotionAnimation();          // Movement animations
        RollForwardAnimation();         // Rolling animation
        JumpAnimation();                // Jumping animation
        ClimbUpAnimation();             // Climbing up animation
        StepUpAnimation();              // Stepping up animation
        QuickStopAnimation();           // Quick stop animation
        ExtraMoveSpeed();               // Extra move speed aplication
        LandHighAnimation();            // Landing high animation
        DieAnimation();                 // Dying animation

        // Animation updating
        animator.SetBool("Aiming", strafing);
        animator.SetBool("Crouch", crouch);
        animator.SetBool("OnGround", onGround);
		animator.SetFloat("GroundDistance", groundDistance);
	    animator.SetFloat("VerticalVelocity", verticalVelocity);
	}

    /* Movement animations */ 
	void LocomotionAnimation(){
		if (freeLocomotionConditions){
			// free directional movement
			animator.SetFloat("Direction", lockPlayer ? 0f : direction);
		}
		else{
			// strafe movement
			animator.SetFloat("Direction", lockPlayer ? 0f : direction, 0.15f, Time.fixedDeltaTime);
		}
			
		animator.SetFloat("Speed", !stopMove || lockPlayer ? speed : 0f, 0.2f, Time.fixedDeltaTime);
	}

    /* Rolling animations */
    void RollForwardAnimation() {
        animator.SetBool("RollForward", rolling);

        // Roll forward
        if (stateInfo.IsName("Action.RollForward")) {
            lockPlayer = true;
            theRigidBody.useGravity = false;

            // prevent the character to rolling up 
            if (verticalVelocity >= 1)
                theRigidBody.velocity = Vector3.ProjectOnPlane(theRigidBody.velocity, groundHit.normal);

            // reset the rigidbody a little ealier to the character fall while on air
            if (stateInfo.normalizedTime > 0.3f)
                theRigidBody.useGravity = true;

            if (!crouch && stateInfo.normalizedTime > 0.85f) {
                lockPlayer = false;
                rolling = false;
            } else if (crouch && stateInfo.normalizedTime > 0.75f) {
                lockPlayer = false;
                rolling = false;
            }
        }
    }

    // Control the direction of rolling when strafing
    public void Rolling() {
        bool conditions = (strafing || speed >= 0.25f) && !stopMove && onGround && !actions && !landHigh;

        if (conditions) {
            if (strafing) {
                // check the right direction for rolling if you are aiming
                freeRotation = Quaternion.LookRotation(targetDirection, Vector3.up);
                var newAngle = freeRotation.eulerAngles - transform.eulerAngles;
                Vector3 newNormalizeAngle = NormalizeAngle(newAngle);
                direction = newNormalizeAngle.y;
                transform.Rotate(0, direction, 0, Space.Self);
            }
            rolling = true;
        }
    }
    
    /* Jumping animation */
    void JumpAnimation() {
        
        animator.SetBool("Jump", jump);
        
        var newSpeed = (jumpForward * speed);

        isJumping = stateInfo.IsName("Action.Jump") || stateInfo.IsName("Action.JumpMove") || stateInfo.IsName("Airborne.FallingFromJump");
        animator.SetBool("IsJumping", isJumping);

        if (stateInfo.IsName("Action.Jump")) {
            // apply extra height to the jump
            if (stateInfo.normalizedTime < 0.85f) {
                theRigidBody.velocity = new Vector3(theRigidBody.velocity.x, jumpForce, theRigidBody.velocity.z);
                transform.position += transform.up * (jumpForce * Time.fixedDeltaTime);
            }
            // end jump animation
            if (stateInfo.normalizedTime >= 0.85f)
                jump = false;
            // apply extra speed forward
            if (stateInfo.normalizedTime >= 0.65f && jumpAirControl)
                transform.position += transform.forward * (newSpeed * Time.fixedDeltaTime);
            else if (stateInfo.normalizedTime >= 0.65f && !jumpAirControl)
                transform.position += transform.forward * Time.fixedDeltaTime;
        }

        if (stateInfo.IsName("Action.JumpMove")) {
            // apply extra height to the jump
            if (stateInfo.normalizedTime < 0.85f) {
                theRigidBody.velocity = new Vector3(theRigidBody.velocity.x, jumpForce, theRigidBody.velocity.z);
                transform.position += transform.up * (jumpForce * Time.fixedDeltaTime);
            }
            // end jump animation
            if (stateInfo.normalizedTime >= 0.55f)
                jump = false;
            // apply extra speed forward
            if (jumpAirControl)
                transform.position += transform.forward * (newSpeed * Time.fixedDeltaTime);
            else
                transform.position += transform.forward * Time.fixedDeltaTime;
        }

        // apply extra speed forward when falling
        if (stateInfo.IsName("Airborne.FallingFromJump") && jumpAirControl)
            transform.position += transform.forward * (newSpeed * Time.fixedDeltaTime);
        else if (stateInfo.IsName("Airborne.FallingFromJump") && !jumpAirControl)
            transform.position += transform.forward * Time.fixedDeltaTime;
    }

    /* Climbing up animation */
    void ClimbUpAnimation() {
        animator.SetBool("ClimbUp", climbUp);

        if (stateInfo.IsName("Action.ClimbUp")) {
            if (stateInfo.normalizedTime > 0.1f && stateInfo.normalizedTime < 0.3f) {
                theRigidBody.useGravity = false;
                gameObject.GetComponent<Collider>().isTrigger = true;
            }

            // we are using matchtarget to find the correct height of the object
            if (!animator.IsInTransition(0))
                MatchTarget(matchTarget.position, matchTarget.rotation,
                           AvatarTarget.LeftHand, new MatchTargetWeightMask
                           (new Vector3(0, 1, 1), 0), 0f, 0.2f);

            if (stateInfo.normalizedTime >= 0.85f) {
                gameObject.GetComponent<Collider>().isTrigger = false;
                theRigidBody.useGravity = true;
                climbUp = false;
            }
        }
    }

    /* Stepping up animation */
    void StepUpAnimation() {
        animator.SetBool("StepUp", stepUp);

        if (stateInfo.IsName("Action.StepUp")) {
            if (stateInfo.normalizedTime > 0.1f && stateInfo.normalizedTime < 0.3f) {
                gameObject.GetComponent<Collider>().isTrigger = true;
                theRigidBody.useGravity = false;
            }

            // we are using matchtarget to find the correct height of the object                
            if (!animator.IsInTransition(0))
                MatchTarget(matchTarget.position, matchTarget.rotation,
                            AvatarTarget.LeftHand, new MatchTargetWeightMask
                            (new Vector3(0, 1, 1), 0), 0f, 0.5f);

            if (stateInfo.normalizedTime > 0.9f) {
                gameObject.GetComponent<Collider>().isTrigger = false;
                theRigidBody.useGravity = true;
                stepUp = false;
            }
        }
    }

    /* Quick stop animation	*/
    void QuickStopAnimation() {
        animator.SetBool("QuickStop", quickStop);

        bool quickStopConditions = !actions && onGround;
        if (inputType == InputType.MouseKeyboard) {
            // make a quickStop when release the key while running
            if (speedTime <= -3f && quickStopConditions)
                quickStop = true;
        } else if (inputType == InputType.Controller) {
            // make a quickStop when release the analogue while running
            if (speedTime <= -6f && quickStopConditions)
                quickStop = true;
        }

        // disable quickStop
        if (quickStop && input.sqrMagnitude > 0.9f)
            quickStop = false;
        else if (stateInfo.IsName("Action.QuickStop")) {
            if (stateInfo.normalizedTime > 0.9f || input.sqrMagnitude >= 0.1f || stopMove)
                quickStop = false;
        }
    }

    /* Extra move speed: Apply extra speed for the the free directional movement or the strafe movement */
    void ExtraMoveSpeed() {
        if (stateInfo.IsName("Grounded.Strafing Movement") || stateInfo.IsName("Grounded.Strafing Crouch")) {
            var newSpeed_Y = (extraStrafeSpeed * speed);
            var newSpeed_X = (extraStrafeSpeed * direction);
            newSpeed_Y = Mathf.Clamp(newSpeed_Y, -extraStrafeSpeed, extraStrafeSpeed);
            newSpeed_X = Mathf.Clamp(newSpeed_X, -extraStrafeSpeed, extraStrafeSpeed);
            transform.position += transform.forward * (newSpeed_Y * Time.fixedDeltaTime);
            transform.position += transform.right * (newSpeed_X * Time.fixedDeltaTime);
        } else if (stateInfo.IsName("Grounded.Free Movement") || stateInfo.IsName("Grounded.Free Crouch")) {
            var newSpeed = (extraMoveSpeed * speed);
            transform.position += transform.forward * (newSpeed * Time.fixedDeltaTime);
        }
    }

    /* Hard landing animation */
    void LandHighAnimation() {
        animator.SetBool("LandHigh", landHigh);

        // if the character fall from a great height, landhigh animation
        if (!onGround && verticalVelocity <= landHighVel && groundDistance <= 0.5f)
            landHigh = true;

        if (landHigh && stateInfo.IsName("Airborne.LandHigh")) {
            quickStop = false;
            if (stateInfo.normalizedTime >= 0.1f && stateInfo.normalizedTime <= 0.2f) {
                // vibrate the controller 
            #if !UNITY_WEBPLAYER && !UNITY_ANDROID && !UNITY_IOS
                StartCoroutine(GamepadVibration(0.15f));
            #endif
            }

            if (stateInfo.normalizedTime > 0.9f) {
                landHigh = false;
            }
        }
    }

    /* Match target: 
        Call this method to help animations find the correct cursorObject.						
        don't forget to add the curve MatchStart and MatchEnd on the animation clip	*/	
    void MatchTarget(Vector3 matchPosition, Quaternion matchRotation, AvatarTarget target, MatchTargetWeightMask weightMask, float normalisedStartTime, float normalisedEndTime) {
        if (animator.isMatchingTarget)
            return;

        float normalizeTime = Mathf.Repeat(animator.GetCurrentAnimatorStateInfo(0).normalizedTime, 1f);
        if (normalizeTime > normalisedEndTime)
            return;
    }

    /* Normalize angle method */
    public Vector3 NormalizeAngle(this Vector3 eulerAngle) {
        var delta = eulerAngle;

        if (delta.x > 180) delta.x -= 360;
        else if (delta.x < -180) delta.x += 360;

        if (delta.y > 180) delta.y -= 360;
        else if (delta.y < -180) delta.y += 360;

        if (delta.z > 180) delta.z -= 360;
        else if (delta.z < -180) delta.z += 360;

        return new Vector3((int)delta.x, (int)delta.y, (int)delta.z);	//round values to angle;
    }

    /* Weapons animations */
    // No weapons in hand from ranged
    public void ChangeWeaponToHolster() {
        smokeParticles.Play();
        smokeParticles.Stop();

        holsteredRangedWeapon.SetActive(true);
        rangedWeaponInHand.SetActive(false);

        holsteredMeleeWeapon.SetActive(true);
        meleeWeaponInHand.SetActive(false);
    }
    // Ranged weapon in hand
    public void ChangeWeaponInAim(){
        smokeParticles.Play();
        //smokeParticles.Stop();

        holsteredRangedWeapon.SetActive (false);
		rangedWeaponInHand.SetActive (true); 

        holsteredMeleeWeapon.SetActive(true);
        meleeWeaponInHand.SetActive(false);
    }
    // Melee weapon in hand
    public void ChangeMeleeWeaponToIdle() {
        holsteredMeleeWeapon.SetActive(false);
        meleeWeaponInHand.SetActive(true);

        holsteredRangedWeapon.SetActive(true);
        rangedWeaponInHand.SetActive(false);
    }
    // No weapons in hand from melee weapon
    public void ChangeMeleeWeaponToHolster() {
        holsteredMeleeWeapon.SetActive(true);
        meleeWeaponInHand.SetActive(false);

        holsteredRangedWeapon.SetActive(true);
        rangedWeaponInHand.SetActive(false);
    }

    /* SHOOTING */
    public void Shooting() {
        // Reset the timer
        timer = 0f;

        animator.SetFloat("TimeBetweenShoots", timeBetweenBullets);
        animator.SetTrigger("Shoot");

        // Check if our raycast has hit anything
        RaycastHit shootHit;
		Ray shootRay = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));

        if (Physics.Raycast(shootRay, out shootHit, range)) {
            // something was hit
           	Vector3 direction = (shootHit.point - gunBarrelEnd.transform.position);

            gunBarrelEnd.transform.rotation = Quaternion.LookRotation(direction);

			AllEnemiesMovement aEM = shootHit.collider.GetComponent<AllEnemiesMovement>();
			if (aEM != null) {
				aEM.TakeDamage(damagePerShot);
			}
		}
		// Start our ShotEffect coroutine to turn our laser line on and off
		StartCoroutine(ShotEffect());
        gunParticles.Stop();
    }

    private IEnumerator ShotEffect() {
        // Play the shooting sound effect
        gunAudio.Play();

        // Stop the particles from playing if they were, then start the particles
        gunParticles.Stop();
        gunParticles.Play();

        //Wait for .07 seconds
        yield return shotDuration;
    }

    /* Melee Attacking animations */
    public void MeleeAttacking(){
		int randomAttack = Random.Range (1, 3);

		animator.SetFloat("HolsterWeaponTime", timeBetweeenAttacks);
		animator.SetInteger ("MeleeAttackNumber", randomAttack);
		animator.SetTrigger ("MeleeAttack");

		meleeAudio.Play();

		timeBetweeenAttacks = 0;
	}

    /* Dying */
    public void DieAnimation() {
		if (isDead) {
			lockPlayer = true;
			animator.SetTrigger("isDead");
			isDead = false;
		}
	}
}
