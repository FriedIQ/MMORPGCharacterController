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

    public bool aiming;
    public float aimingWeight;

    public bool lookInCameraDirection = true;
    Vector3 lookPosition;

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
    }

    void FixedUpdate()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        aiming = Input.GetMouseButton(0);

        if (!aiming)
        {
            Debug.Log("Not Aiming!");
            if (playerCamera != null)   // if there is a camera
            {
                cameraForward = Vector3.Scale(playerCamera.forward, new Vector3(1, 0, 1)).normalized;
                move = (vertical * cameraForward) + (horizontal * playerCamera.right);
            }
            else
            {
                move = (vertical * Vector3.forward) + (horizontal * Vector3.right);
            }
        }
        else
        {
            Debug.Log("Aiming!");
            move = Vector3.zero; // stop moving if aiming
            Vector3 dir = lookPosition - transform.position;
            dir.y = 0;

            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 20 * Time.deltaTime);
        }

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

        lookPosition = (lookInCameraDirection && playerCamera != null && Input.GetMouseButton(1)) ? transform.position + playerCamera.forward * 100 : transform.position + transform.forward * 100;
        move *= walkMultiplier;

        characterMove.Move(move, aiming, lookPosition);
    }
}
