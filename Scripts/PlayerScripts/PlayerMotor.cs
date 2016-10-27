using UnityEngine;
using System.Collections;

public class PlayerMotor : Player {

	public enum LocomotionType{
		FreeWithStrafe,
		OnlyStrafe,
		OnlyFree
	}
	public LocomotionType locomotionType = LocomotionType.FreeWithStrafe;

	#region PLAYER MOVEMENT VARIABLES
	[HideInInspector] public bool canSprint;
	[HideInInspector] public bool stopMove;
    [HideInInspector] public float stopMoveDistance = 0.5f;
    [HideInInspector] public float extraMoveSpeed = 0f;
    [Tooltip("Max angle to walk")]
    [HideInInspector] public float slopeLimit = 45f;

    [HideInInspector] public float rotationSpeed = 8f;			// Rotation Speed on free directional movement

	[HideInInspector] public bool pressToCrouch = false;			// True: mantain button to crouch, false: hit the botton once to crouch
	[HideInInspector] public bool crouch;

	[HideInInspector] public bool jump;
	[HideInInspector] public bool jumpAirControl = true; 		// True to control the character while jumping
    [HideInInspector] public float jumpForward = 4f;
    public float jumpForce = 3f;

    [HideInInspector] public bool strafing;
    [HideInInspector] public float extraStrafeSpeed = 0f;

	[HideInInspector] public bool onGround;
    [HideInInspector] public float groundDistance;

    [HideInInspector] public bool isJumping;
	[HideInInspector] public bool landHigh;
    [HideInInspector] public float landHighVel = -5f;


    [Tooltip("Choose the layers the your character will stop moving when hit with the BLUE Raycast")]
    [HideInInspector] public LayerMask stopMoveLayer;
    public LayerMask groundLayer;
    [HideInInspector] public RaycastHit groundHit;
    [HideInInspector] public float groundCheckDistance = 0.5f;
    [HideInInspector] public float extraGravity = 4f;
    [Tooltip("ADJUST IN PLAY MODE - Offset height limit for sters - GREY Raycast in front of the legs")]
    [HideInInspector] public float stepOffsetEnd = 0.36f;
    [Tooltip("ADJUST IN PLAY MODE - Offset height origin for sters, make sure to keep slight above the floor - GREY Raycast in front of the legs")]
    [HideInInspector] public float stepOffsetStart = 0.05f;
    [Tooltip("ADJUST IN PLAY MODE - Offset forward to detect sters - GREY Raycast in front of the legs")]
    [HideInInspector] public float stepOffsetFwd = 0.05f;

    #endregion

    #region CAMERA VARIABLES

    // Variables to find the correct direction to move
    [HideInInspector] public bool keepDirection;
	[HideInInspector] public Quaternion freeRotation;
	[HideInInspector] public Vector3 cameraForward;
	[HideInInspector] public Vector3 cameraRight;
	[HideInInspector] public Vector2 oldInput;

	#endregion

	#region PLAYER ANIMATOR VARIABLES
	[HideInInspector] public bool rolling;
	[HideInInspector] public bool quickStop;
    [HideInInspector] public bool climbUp;
    [HideInInspector] public bool stepUp;

	[HideInInspector] public bool shooting;
	[HideInInspector] public float timeBetweenShooting;			// A timer to determine when to fire.
	

	[HideInInspector] public bool meleeAttacking;
	[HideInInspector] public float timeBetweeenAttacks = 0.5f;
	[HideInInspector] public int meleeAttackNumber;

    // to check actions
    [HideInInspector]
	public bool actions{
		get{
            //return jumpOver || stepUp || climbUp || rolling || usingLadder || quickStop || quickTurn180 || jump;
			return rolling || climbUp || quickStop || jump || shooting || meleeAttacking;
		}
	}
	#endregion

	/* METHOD THAT HANDLES ALL PLAYER MOVEMENTS */
	public void UpdateMotor(){
		CheckGround();
		ControlHeight();
		ControlLocomotion();
	}

	/* CHANGE THE COLLIDER ATTRIBUTES WHEN DOING ACTIONS*/
	void ControlHeight(){
		if (crouch || rolling){
			capsuleCollider.center = colliderCenter / 1.4f;
			capsuleCollider.height = colliderHeight / 1.4f;
		}
		else{
			capsuleCollider.center = colliderCenter;
			capsuleCollider.height = colliderHeight;
		}
	}

	/* CONTROL PLAYER FREE AND STRAFE MOVEMENT */
	void ControlLocomotion(){
		if (freeLocomotionConditions){
			// Free directional movement
			// set speed to both vertical and horizontal inputs
			speed = Mathf.Abs(input.x) + Mathf.Abs(input.y);
			speed = Mathf.Clamp (speed, 0, 1);

			// add 0.5f on sprint to change the animation on animator
			if (canSprint)
				speed += 0.5f;
			
			if (stopMove)
				speed = 0f;

			if (!actions || quickStop)
				FreeRotationMovement();
		}
		else{
			//Strafe movement
			speed = input.y;
			direction = input.x;

			// To face the player in the direction to the camera, we get the forward vector of the camera
			Vector3 forward = myCamera.transform.TransformDirection (Vector3.forward);

			float finalTurnSmoothing = 15.0f;

			// We calculate the rotation which our player transform is going to look at
			Quaternion targetRotation = Quaternion.LookRotation (forward, Vector3.up);
			targetRotation = Quaternion.Euler (new Vector3 (0f, targetRotation.eulerAngles.y, 0f));

			// We change the player transform rotation
			transform.rotation = Quaternion.Slerp (transform.rotation, targetRotation, finalTurnSmoothing * Time.deltaTime);
		}
	}


	public bool freeLocomotionConditions{
		get{
			if (locomotionType.Equals (LocomotionType.OnlyStrafe))
				strafing = true;
			return !strafing && !landHigh && !locomotionType.Equals(LocomotionType.OnlyStrafe) || locomotionType.Equals(LocomotionType.OnlyFree);
		}
	}

	void FreeRotationMovement(){
		if (input != Vector2.zero && !lockPlayer && targetDirection.magnitude > 0.1f){
			freeRotation = Quaternion.LookRotation (targetDirection, Vector3.up);
			Vector3 velocity = Quaternion.Inverse (transform.rotation) * targetDirection.normalized;
			direction = Mathf.Atan2 (velocity.x, velocity.z) * 180.0f / 3.1415f;

			// Apply free directional rotation while not turning180 animations
			if (!isJumping || (isJumping && jumpAirControl)){
				Vector3 lookDirection = targetDirection.normalized;
				freeRotation = Quaternion.LookRotation (lookDirection, transform.up);
				Vector3 euler = new Vector3 (0, freeRotation.eulerAngles.y, 0);
				transform.rotation = Quaternion.Slerp (transform.rotation, Quaternion.Euler (euler), rotationSpeed * Time.deltaTime);
			}

			if (!keepDirection)
				oldInput = input;

			if (Vector2.Distance (oldInput, input) > 0.9f && keepDirection)
				keepDirection = false;
		}
	}

	public Vector3 targetDirection {
		get {
			Vector3 refDir = Vector3.zero;
			cameraForward = keepDirection ? cameraForward : cameraTransform.TransformDirection (Vector3.forward);
			cameraForward.y = 0;

			// get the right-facing direction of the camera
			cameraRight = keepDirection ? cameraRight : cameraTransform.TransformDirection (Vector3.right);

			// determine the direction the player will face base on input and the camera's right and forward directions
			refDir = input.x * cameraRight + input.y * cameraForward;

			return refDir;
		}
	}

    /* STOP MOVE: Stop the character if hits a wall and apply slope limit to ramps	*/
    public void StopMove() {
        if (input.sqrMagnitude < 0.1 || !onGround) return;

        RaycastHit hitinfo;
        Ray ray = new Ray(transform.position + new Vector3(0, colliderHeight / 3, 0), transform.forward);

        if (Physics.Raycast(ray, out hitinfo, stopMoveDistance, stopMoveLayer)) {
            var hitAngle = Vector3.Angle(Vector3.up, hitinfo.normal);

            if (hitinfo.distance <= stopMoveDistance && hitAngle > 85)
                stopMove = true;
            else if (hitAngle >= slopeLimit + 1f && hitAngle <= 85)
                stopMove = true;
        } else
            stopMove = false;
    }

    /* SLOPE LIMIT:	Stop the character from walking on ramps using a slope limit angle */
    public void SlopeLimit() {
        if (input.sqrMagnitude < 0.1 || !onGround) return;

        RaycastHit hitinfo;
        Ray ray = new Ray(transform.position + new Vector3(0, colliderHeight / 3.5f, 0), transform.forward);

        if (Physics.Raycast(ray, out hitinfo, 1f, stopMoveLayer)) {
            var hitAngle = Vector3.Angle(Vector3.up, hitinfo.normal);
            if (hitAngle >= slopeLimit + 1f && hitAngle <= 85)
                stopMove = true;
        } else
            stopMove = false;
    }

    /* ACTIONS: Raycast to check if there is anything interactable ahead */ 
    public GameObject CheckActionObject() {
        bool checkConditions = onGround && !landHigh && !actions;
        GameObject _object = null;

        if (checkConditions) {
            RaycastHit hitInfoAction;
            Vector3 yOffSet = new Vector3(0f, -0.5f, 0f);
            Vector3 fwd = transform.TransformDirection(Vector3.forward);

            if (Physics.Raycast(transform.position - yOffSet, fwd, out hitInfoAction, 0.45f)) {
                _object = hitInfoAction.transform.gameObject;
            }
        }
        return _object;
    }

    /* GROUND CHECKER: Check if the character is grounded or not */
    void CheckGround() {
        CheckGroundDistance();

        // change the physics material to very slip when not grounded
        capsuleCollider.material = (onGround) ? frictionPhysics : defaultPhysics;
        // we don't want to stick the character grounded if one of these bools is true
        bool groundStickConditions = !stepUp && !climbUp;

        if (groundStickConditions) {
            var onStep = StepOffset();

            if (groundDistance <= 0.05f) {
               
                onGround = true;
                // keeps the character grounded and prevents bounceness on ramps
                if (!onStep) theRigidBody.velocity = Vector3.ProjectOnPlane(theRigidBody.velocity, groundHit.normal);
            } else {
                if (groundDistance >= groundCheckDistance) {
                    onGround = false;
                    // check vertical velocity
                    verticalVelocity = theRigidBody.velocity.y;
                    // apply extra gravity when falling
                    if (!onStep && !rolling)
                        transform.position -= Vector3.up * (extraGravity * Time.deltaTime);
                } else if (!onStep && !rolling && !jump)
                    transform.position -= Vector3.up * (extraGravity * Time.deltaTime);
            }
        }
    }

	/* GROUND DISTANCE: Get the distance between the middle of the character to the ground */
	void CheckGroundDistance() {
		if (capsuleCollider != null) {
			// radius of the SphereCast
			float radius = capsuleCollider.radius * 0.9f;
			var dist = Mathf.Infinity;
			// position of the SphereCast origin starting at the base of the capsule
			Vector3 pos = transform.position + Vector3.up * (capsuleCollider.radius);

			// ray for RayCast
			Ray ray1 = new Ray(transform.position + new Vector3(0, colliderHeight / 2, 0), Vector3.down);
			// ray for SphereCast
			Ray ray2 = new Ray(pos, -Vector3.up);
			
			if (Physics.Raycast(ray1, out groundHit, Mathf.Infinity, groundLayer))
				dist = transform.position.y - groundHit.point.y;

			if (Physics.SphereCast(ray2, radius, out groundHit, Mathf.Infinity, groundLayer)) {
				// check if sphereCast distance is small than the ray cast distance
				if (dist > (groundHit.distance - capsuleCollider.radius * 0.1f))
					dist = (groundHit.distance - capsuleCollider.radius * 0.1f);
			}
			groundDistance = dist;
		}
	}

    /* STEP OFFSET LIMIT: Check the height of the object ahead, control by stepOffSet */
    bool StepOffset() {
        if (input.sqrMagnitude < 0.1 || !onGround) return false;
        var hit = new RaycastHit();
        Ray rayStep = new Ray((transform.position + new Vector3(0, stepOffsetEnd, 0) + transform.forward * ((capsuleCollider).radius + stepOffsetFwd)), Vector3.down);
        if (Physics.Raycast(rayStep, out hit, stepOffsetEnd - stepOffsetStart, groundLayer))
            if (!stopMove && hit.point.y >= (transform.position.y) && hit.point.y <= (transform.position.y + stepOffsetEnd)) {
                var heightPoint = new Vector3(transform.position.x, hit.point.y + 0.1f, transform.position.z);
                transform.position = Vector3.Slerp(transform.position, heightPoint, (speed * 3.5f) * Time.deltaTime);
                return true;
            }
        return false;
    }
}
