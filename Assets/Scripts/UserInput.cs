using System;
using System.Net.Sockets;
using UnityEngine;
using System.Collections;


public class UserInput : MonoBehaviour
{
    public bool walkByDefault = false;

    private CharacterMovement characterMove;
    private Transform playerCamera;
    private Vector3 cameraForward;              // stores the forward vector of the camera
    private Vector3 move;

    public bool sneaking;

    public bool crouching;

    public bool aiming;
    public float aimingWeight;

    public bool lookInCameraDirection = true;
    Vector3 lookPosition;

    Animator animator;

    // Aiming IK values
    public Transform spine;

    [SerializeField]
    public IKAiming ikAiming;

    [Serializable]
    public class IKAiming
    {
        public float aimingX = 0f;
        public float aimingY = 130f;         // horizontal
        public float aimingZ = -50f;        // vertical
        public float point = 30;
        public bool debugAim = false;
    }

    //public ParticleSystem particleSys;
    //public AudioSource audioSource;

    void Start()
    {
        if (Camera.main != null)
        {
            playerCamera = Camera.main.transform;
        }
        else
        {
            print("Unable to locate main camera!");
        }

        characterMove = GetComponent<CharacterMovement>();

        animator = GetComponent<Animator>();

        // audioSource = transform.GetComponentInChildren<AudioSource>();
    }

    void Update()
    {
        if (!ikAiming.debugAim)
        {
            aiming = Input.GetMouseButton(1);
        }
        sneaking = (Input.GetKeyDown(KeyCode.LeftControl)) ? !sneaking : sneaking;

        if(aiming)
        {
            if(Input.GetMouseButtonDown(0))
            {
                // Fire!
            }
        }
    }

    public void SetAnimationTrigger(string animationName, int layer)
    {
        if (!animator.GetCurrentAnimatorStateInfo(layer).IsName(animationName))
            animator.SetTrigger(animationName);
    }

    public void ResetAnimationTrigger(string animationName, int layer)
    {
        if (!animator.GetCurrentAnimatorStateInfo(layer).IsName(animationName))
            animator.ResetTrigger(animationName);
    }

    void LateUpdate()
    {
        //aimingWeight = Mathf.MoveTowards(aimingWeight, (aiming) ? 1.0f : 0.0f, Time.deltaTime * 5);

        //Vector3 normalState = new Vector3(0, 0, -2f);
        //Vector3 aimingState = new Vector3(0, 0, 0.5f);

        //Vector3 pos = Vector3.Lerp(normalState, aimingState, aimingWeight);

        //playerCamera.transform.localPosition = pos;

        if (aiming)
        {
            Vector3 eulierAngleOffset = Vector3.zero;
            eulierAngleOffset = new Vector3(ikAiming.aimingX, ikAiming.aimingY, ikAiming.aimingZ);

            Ray ray = new Ray(playerCamera.position, playerCamera.forward);
            Vector3 lookPosition = ray.GetPoint(ikAiming.point);

            spine.LookAt(lookPosition);
            spine.Rotate(eulierAngleOffset, Space.Self);
        }

        //ResetAnimationTrigger("Fire", 2);
    }

    void FixedUpdate()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        if (playerCamera != null)   // if there is a camera
        {
            // take the foward vector of the camera (from it's transform) and 
            // eliminate the y component then scale the camera forward
            // with the mask (1, 0, 1) to eliminate y and normalize it
            cameraForward = Vector3.Scale(playerCamera.forward, new Vector3(1, 0, 1)).normalized;

            // move input front/backwards = forward direction of the camera * user input amount (vertical)
            // move input left/right = right direction of the camera * user input amount (horizontal)
            move = (vertical * cameraForward) + (horizontal * playerCamera.right);
        }
        else
        {
            // if there is no camera, use the global forward (+z) and right (+x)
            move = (vertical * Vector3.forward) + (horizontal * Vector3.right);
        }

        //if (!sneaking)
        //{
        //    if (playerCamera != null)   // if there is a camera
        //    {
        //        // take the foward vector of the camera (from it's transform) and 
        //        // eliminate the y component then scale the camera forward
        //        // with the mask (1, 0, 1) to eliminate y and normalize it
        //        cameraForward = Vector3.Scale(playerCamera.forward, new Vector3(1, 0, 1)).normalized;

        //        // move input front/backwards = forward direction of the camera * user input amount (vertical)
        //        // move input left/right = right direction of the camera * user input amount (horizontal)
        //        move = (vertical * cameraForward) + (horizontal * playerCamera.right);
        //    }
        //    else
        //    {
        //        // if there is no camera, use the global forward (+z) and right (+x)
        //        move = (vertical * Vector3.forward) + (horizontal * Vector3.right);
        //    }
        //}
        //else
        //{
        //    move = Vector3.zero; // stop moving if aiming
        //    Vector3 dir = lookPosition - transform.position;
        //    dir.y = 0;

        //    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 20 * Time.deltaTime);

        //    animator.SetFloat("Vertical", vertical);
        //    animator.SetFloat("Horizontal", horizontal);
        //}

        if (move.magnitude > 1)
        {
            move.Normalize();
        }

        bool walkToggle = Input.GetKey(KeyCode.LeftShift) || aiming; // check for walking or aiming input

        // the walk multiplier determine if the character is running or walking
        float walkMultiplier = 1;

        if (walkByDefault)
        {
            walkMultiplier = walkToggle ? 1 : 0.5f;
        }
        else
        {
            walkMultiplier = walkToggle ? 0.5f : 1f;
        }

        if (lookInCameraDirection && playerCamera != null)
        {
            lookPosition = transform.position + playerCamera.forward * 100;
        }
        else
        {
            lookPosition = transform.position + transform.forward * 100;
        }
        
        move *= walkMultiplier;

        characterMove.Move(move, aiming, sneaking, lookPosition);
    }
}
