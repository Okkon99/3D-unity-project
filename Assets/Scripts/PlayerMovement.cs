using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Division State")]
    public bool isActivePlayer = true;

    [Header("Player Settings")]
    [Header("")]
    [Header("Mouse/Aim/Look Settings")]

    // Mouse/Aim/Look
    [SerializeField] public float lookSensitivity;

    [Header("Movement Settings")]
    [SerializeField] private float groundSpeed;
    [SerializeField] private float airSpeed;
    [SerializeField] private float acceleration;
    [SerializeField] private float sprintSpeedPercent;

    [Header("Jump/Air control Settings")]
    [SerializeField] public float jumpHeight;
    [SerializeField] private float jumpCutMultiplier;
    [SerializeField] private float coyoteTimer;


    [Header("Friction Settings")]
    [SerializeField] private float groundFriction;
    [SerializeField] private float airFriction;


    [Header("Dependencies")]
    [SerializeField] private InputManager inputManager;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private CapsuleCollider playerCollider;
    [SerializeField] private LayerMask floorLayer;



    bool isGrounded;
    Vector3 desiredVelocity;
    Vector2 moveInput;
    bool jumpPressed;
    bool jumpHeld;
    bool canJump;
    bool isJumping;
    float coyoteTime;
    bool sprintHeld;

    RaycastHit groundHit;


    public GameObject dividedPrefab;
    public PlayerMovement activePlayer;
    PlayerMovement inactivePlayer;


    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (!isActivePlayer)
        {
            return;
        }

        // CACHE INPUTS
        var input = InputManager.instance.Input.Gameplay;

        moveInput = input.Move.ReadValue<Vector2>();
        jumpHeld = input.Jump.IsPressed();
        sprintHeld = input.Sprint.IsPressed();

        if (input.Jump.triggered)
        {
            jumpPressed = true;
        }
        // CACHE INPUTS END


        Vector2 lookDelta = input.Look.ReadValue<Vector2>();
        float mouseX = lookDelta.x * lookSensitivity;
        transform.Rotate(Vector3.up * mouseX);

        float mouseY = lookDelta.y * lookSensitivity;

    }

    private void FixedUpdate()
    {
        isGrounded = CheckGrounded(out groundHit);

        GetDesiredVelocity();

        ApplyAccelVector();

        ApplyFriction();

        //Jump
        if (isGrounded)
        {
            ResetCoyoteTime();
            isJumping = false;
        }
        else
        {
            CoyoteTimeCountdown();
        }

        canJump = isGrounded || coyoteTime > 0f;

        if (jumpPressed && canJump)
        {
            Jump();
        }

        if (!jumpHeld && isJumping && rb.linearVelocity.y > 0f)
        {
            JumpCancel();
        }
    }



    private void ResetCoyoteTime()
    {
        coyoteTime = coyoteTimer;
    }
    private void CoyoteTimeCountdown()
    {
        coyoteTime -= Time.fixedDeltaTime;
        coyoteTime = Mathf.Max(coyoteTime, 0f);
    }

    private void Jump()
    {
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpHeight, rb.linearVelocity.z);
        coyoteTime = 0f;
        isJumping = true;
    }
    private void JumpCancel()
    {
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, rb.linearVelocity.y * jumpCutMultiplier, rb.linearVelocity.z);

        isJumping = false;
    }

    private void GetDesiredVelocity()
    {
        Vector3 inputDirection = transform.forward * moveInput.y + transform.right * moveInput.x;

        if (inputDirection.sqrMagnitude > 1f)
        {
            inputDirection.Normalize();
        }

        float speed = isGrounded ? groundSpeed : airSpeed;

        if (isGrounded)
        {
            if (sprintHeld)
            {
                speed *= sprintSpeedPercent / 100f;
            }
            inputDirection = Vector3.ProjectOnPlane(inputDirection, groundHit.normal).normalized;
        }
        desiredVelocity = inputDirection * speed;
    }

    private void ApplyAccelVector()
    {
        Vector3 currentVelocity = rb.linearVelocity;
        Vector3 currentHorizontal = new Vector3(currentVelocity.x, 0f, currentVelocity.z);
        Vector3 velocityDelta = desiredVelocity - currentHorizontal;

        float maxAccel = acceleration * Time.fixedDeltaTime;
        Vector3 accelVector = Vector3.ClampMagnitude(velocityDelta, maxAccel);
        rb.linearVelocity += accelVector;
    }

    private void ApplyFriction()
    {
        rb.linearDamping = isGrounded ? groundFriction : airFriction;
    }

    private bool CheckGrounded(out RaycastHit hit)
    {
        Vector3 center = transform.position;
        float castDistance = 1.25f;

        return Physics.Raycast(center, Vector3.down, out hit, castDistance, floorLayer);
    }

    private void OnDrawGizmos()
    {
        Vector3 center = transform.position;
        float castDistance = 1.25f;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(center, center + Vector3.down * castDistance);
    }



    // Divide methods

    public void SetActive(bool active)
    {
        isActivePlayer = active;

        var cam = GetComponentInChildren<Camera>();
        if (cam != null)
            cam.enabled = active;

        var listener = GetComponentInChildren<AudioListener>();
        if (listener != null)
            listener.enabled = active;

        if (!active)
        {
            rb.linearVelocity = Vector3.zero;
        }
    }
}
