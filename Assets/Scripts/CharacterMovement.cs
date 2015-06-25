using UnityEngine;
using System.Collections;

public class CharacterMovement : MonoBehaviour 
{
    public bool onGround = false;

    float moveSpeedMultiplier = 1;
    float stationaryTurnSpeed = 180;
    float movingTurnSpeed = 360;

    Rigidbody rigidBody;
    Animator animator;

    Vector3 moveInput;
    float turnAmount;
    float forwardAmount;
    Vector3 velocity;

    float jumpPower = 10;

    IComparer rayHitComparer;

	// Use this for initialization
	void Start () 
    {
        rigidBody = GetComponent<Rigidbody>();

        SetupAnimator();
	}

    public void Move(Vector3 move)
    {
        if (move.magnitude > 1)
        {
            move.Normalize();
        }

        moveInput = move;
        velocity = rigidBody.velocity;

        ConvertMoveInput();
        ApplyExtraTurnRotation();
        CheckGrounded();
        UpdateAnimator();
    }

    void ConvertMoveInput()
    {
        Vector3 localMove = transform.InverseTransformDirection(moveInput);
        turnAmount = Mathf.Atan2(localMove.x, localMove.z);
        forwardAmount = localMove.z;
    }

    void ApplyExtraTurnRotation()
    {
        float turnSpeed = Mathf.Lerp(stationaryTurnSpeed, movingTurnSpeed, forwardAmount);
        transform.Rotate(0, turnAmount * turnSpeed * Time.deltaTime, 0);
    }


    void OnAnimatorMove()
    {
        if(onGround && Time.deltaTime > 0)
        {
            Vector3 velocity = (animator.deltaPosition * moveSpeedMultiplier) / Time.deltaTime;
            velocity.y = rigidBody.velocity.y;
            rigidBody.velocity = velocity;
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
                    if(velocity.y <= 0)
                    {
                        rigidBody.position = Vector3.MoveTowards(rigidBody.position, hit.point, Time.deltaTime * 5);
                    }

                    onGround = true;
                    rigidBody.useGravity = false;

                    break;
                }
            }
        }
    }

    void SetupAnimator ()
    {
        animator = GetComponent<Animator>();

        foreach(Animator childAnimator in GetComponentsInChildren<Animator>())
        {
            if(childAnimator != animator)
            {
                animator = childAnimator;
                Destroy(childAnimator);
                break;
            }
        }
    }

    void UpdateAnimator()
    {
        animator.applyRootMotion = true;
        animator.SetFloat("Vertical", forwardAmount, 0.1f, Time.deltaTime);
        animator.SetFloat("Horizontal", turnAmount, 0.1f, Time.deltaTime);
    }
	
	// Update is called once per frame
	void Update () 
    {
	
	}
}
