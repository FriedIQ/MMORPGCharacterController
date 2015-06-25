using UnityEngine;
using UnityEditor;

public class FreeLookCamera : Pivot
{
    [SerializeField]
    private float moveSpeed = 5.0f;
    [SerializeField]
    private float turnSpeed = 1.5f;
    [SerializeField]
    private float turnSmoothing = 1.0f;
    [SerializeField]
    private float tiltMax = 75.0f;
    [SerializeField]
    private float tiltMin = 45.0f;
    [SerializeField]
    private bool lockCursor = false;

    private float lookAngle;
    private float tiltAngle;

    private const float LookDistance = 100f;

    private float smoothX = 0f;
    private float smoothY = 0f;
    private float smoothXVelocity = 0f;
    private float smoothYVelocity = 0f;

    protected override void Awake()
    {
        base.Awake();

        Cursor.lockState = (lockCursor) ? CursorLockMode.Locked : CursorLockMode.None;

        cam = GetComponentInChildren<Camera>().transform;
        pivot = cam.parent;
    }


    protected override void Update()
    {
        base.Update();

        HandleRotation();

        if (lockCursor && Input.GetMouseButtonUp(0))
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    void OnDisable()
    {
        Cursor.lockState = CursorLockMode.None;
    }

    protected override void Follow(float deltaTime)
    {
        transform.position = Vector3.Lerp(transform.position, target.position, deltaTime * moveSpeed);
    }

    void HandleRotation()
    {
        float x = Input.GetAxis("Mouse X");
        float y = Input.GetAxis("Mouse Y");

        if (turnSmoothing > 0)
        {
            smoothX = Mathf.SmoothDamp(smoothX, x, ref smoothXVelocity, turnSmoothing);
            smoothY = Mathf.SmoothDamp(smoothY, y, ref smoothYVelocity, turnSmoothing);
        }
        else
        {
            smoothX = x;
            smoothY = y;
        }

        lookAngle += smoothX * turnSpeed;
        transform.rotation = Quaternion.Euler(0f, lookAngle, 0f);

        tiltAngle -= smoothY * turnSpeed;
        tiltAngle = Mathf.Clamp(tiltAngle, -tiltMin, tiltMax);

        pivot.localRotation = Quaternion.Euler(tiltAngle, 0f, 0f);
    }
}
