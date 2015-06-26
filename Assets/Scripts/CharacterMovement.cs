using UnityEngine;
using System.Collections;

public class CharacterMovement : MonoBehaviour 
{
    public bool onGround = false;       // flag indicating if the character is on the ground

    float moveSpeedMultiplier = 1;
    float stationaryTurnSpeed = 180;    // if the character is not moving, how fast he will turn
    float movingTurnSpeed = 360;        // as above except when the character is moving

    Rigidbody rigidBody;
    Animator animator;

    Vector3 moveInput;                  // the move vector
    float turnAmount;                   // the calculated turn amountto pass to mechanim
    float forwardAmount;                // the calculated forward amount to pass to mechanim
    Vector3 velocity;                   // the 3d velocity of the character

    float jumpPower = 10;

    IComparer rayHitComparer;

    float autoTurnThreshold = 10f;
    float autoTurnSpeed = 20f;
    bool aiming;
    Vector3 currentLookPos;

	// Use this for initialization
	void Start () 
    {
        rigidBody = GetComponent<Rigidbody>();

        SetupAnimator();
	}

    public void Move(Vector3 moveInput, bool aiming, Vector3 lookPosition)
    {
        if (moveInput.magnitude > 1)     // make sure that the movement is normalized
        {
            moveInput.Normalize();
        }

        this.moveInput = moveInput;           // store the movement vector
        this.aiming = aiming;
        currentLookPos = lookPosition;

        velocity = rigidBody.velocity;  // store the current velocity

        ConvertMoveInput();

        if (!aiming)
        {
            TurnTowardsCameraForward();
            ApplyExtraTurnRotation();
        }

        CheckGrounded();

        UpdateAnimator();
    }

    /// <summary>
    /// Convert the world relative moveInput vector into a local relative
    /// turn amount and forward amount required to head in the desired direction
    /// </summary>
    void ConvertMoveInput()
    {
        // convert the move input (e.g left -> (-1, 0, 0) from world space  to the characters local space
        Vector3 localMove = transform.InverseTransformDirection(moveInput);
        // calculate the turn amount trigonometrically
        turnAmount = Mathf.Atan2(localMove.x, localMove.z);
        forwardAmount = localMove.z;
    }

    void ApplyExtraTurnRotation()
    {
        float turnSpeed = Mathf.Lerp(stationaryTurnSpeed, movingTurnSpeed, forwardAmount);
        transform.Rotate(0, turnAmount * turnSpeed * Time.deltaTime, 0);
    }

    // updates the movement of the character based on its current speed and the moveSpeedMultiplier
    void OnAnimatorMove()
    {
        // if the character is on the ground and it is not the first frame of play
        if(onGround && Time.deltaTime > 0)
        {
            // calculate the speed that the character should have. 
            // deltaPosition (position difference) - the difference in the position between this fram and the last.
            // speed = 
            Vector3 velocity = (animator.deltaPosition * moveSpeedMultiplier) / Time.deltaTime;
            velocity.y = rigidBody.velocity.y;  // store the characters verticle velocity (in order to no affect the jump speed)
            rigidBody.velocity = velocity;      // update the character's speed
        }
    }

    void CheckGrounded()
    {
        Ray ray = new Ray(transform.position + Vector3.up * 0.5f, -Vector3.up);

        RaycastHit[] hits = Physics.RaycastAll(ray, 0.5f);

        rayHitComparer = new RayHitComparer();
        System.Array.Sort(hits, rayHitComparer);

        if(velocity.y < (jumpPower * 0.5f))
        {
            onGround = false;
            rigidBody.useGravity = true;

            foreach (RaycastHit hit in hits)
            {
                if(!hit.collider.isTrigger)
                {
                    // stick to the surface - helps the chacter stick to the ground - particulairly when running down
                    if(velocity.y <= 0)
                    {
                        // change the rigidbody position to the hit point
                        rigidBody.position = Vector3.MoveTowards(rigidBody.position, hit.point, Time.deltaTime * 5);
                    }

                    onGround = true;    // set the ongroundvariable since we found our collider
                    rigidBody.useGravity = false;   // disable gravity since we use the above to stick the character to the ground

                    break; // ignore any other hits
                }
            }
        }
    }

    void SetupAnimator ()
    {
        animator = GetComponent<Animator>();

        // we look for a child animator component if present
        // this makes it easier to swap out of the character model as a child node
        foreach(Animator childAnimator in GetComponentsInChildren<Animator>())
        {
            if(childAnimator != animator)
            {
                animator = childAnimator;
                Destroy(childAnimator);
                break;  // stop looping if you find an animator
            }
        }
    }

    void UpdateAnimator()
    {
        animator.applyRootMotion = true;

        animator.SetFloat("Vertical", forwardAmount, 0.1f, Time.deltaTime);
        animator.SetFloat("Horizontal", turnAmount, 0.1f, Time.deltaTime);

       //animator.SetBool("Aim", aiming);
    }
	
	// Update is called once per frame
	void Update () {}

    void TurnTowardsCameraForward()
    {
        if(Mathf.Abs(forwardAmount) < 0.1f)
        {
            Vector3 lookDelta = transform.InverseTransformDirection(currentLookPos - transform.position);

            float lookAngle = Mathf.Atan2(lookDelta.x, lookDelta.z) * Mathf.Rad2Deg;

            if(Mathf.Abs(lookAngle) > autoTurnThreshold)
            {
                turnAmount += lookAngle * autoTurnSpeed * 0.001f;
            }
        }
    }
}
