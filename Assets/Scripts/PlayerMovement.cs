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

    [Header("Gravity Feel")]
    [SerializeField] float fallGravityMultiplier;
    [SerializeField] float lowJumpGravityMultiplier;


    [Header("Dependencies")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private CapsuleCollider playerCollider;
    [SerializeField] private LayerMask floorLayer;



    bool isGrounded;
    bool wasGrounded;
    Vector3 desiredVelocity;
    Vector2 moveInput;
    bool sprintHeld;
    bool jumpPressed;
    bool jumpHeld;
    bool canJump;
    bool isJumping;
    float coyoteTime;
    RaycastHit groundHit;



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
        wasGrounded = isGrounded; // wasGrounded = Last frame's isGrounded.
        isGrounded = CheckGrounded(out groundHit); // isGrounded is now THIS frame's isGrounded.

        if (!wasGrounded && isGrounded) // Checks if the player touched the ground THIS
        {                               // frame but not the previous (did the player JUST land?)
            OnLanded();
        }
        else
        {
            CoyoteTimeCountdown();
        }

        ApplyBetterGravity();

        canJump = isGrounded || coyoteTime > 0f;

        if (jumpPressed && canJump)
        {
            Jump();
        }

        jumpPressed = false;

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
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        rb.AddForce(Vector3.up * jumpHeight, ForceMode.VelocityChange);

        coyoteTime = 0f;
        isJumping = true;
    }
    private void JumpCancel()
    {
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, rb.linearVelocity.y * jumpCutMultiplier, rb.linearVelocity.z);

        isJumping = false;
    }

    private void OnLanded()
    {
        ResetCoyoteTime();
        isJumping = false;

        if (rb.linearVelocity.y < 0f)
        {
            rb.linearVelocity = new Vector3(
                rb.linearVelocity.x,
                0f,
                rb.linearVelocity.z
            );
        }
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
        Vector3 vel = rb.linearVelocity;
        float damping = isGrounded ? groundFriction : airFriction;

        vel.x *= 1f - damping * Time.fixedDeltaTime;
        vel.z *= 1f - damping * Time.fixedDeltaTime;

        rb.linearVelocity = new Vector3(vel.x, rb.linearVelocity.y, vel.z);
    }

    private void ApplyBetterGravity()
    {
        if (rb.linearVelocity.y < 0f)
        {
            rb.AddForce(
                Vector3.up * Physics.gravity.y * (fallGravityMultiplier - 1f),
                ForceMode.Acceleration
            );
        }
        else if (rb.linearVelocity.y > 0f && !jumpHeld)
        {
            rb.AddForce(
                Vector3.up * Physics.gravity.y * (lowJumpGravityMultiplier - 1f),
                ForceMode.Acceleration
            );
        }
    }

    private bool CheckGrounded(out RaycastHit hit)
    {
        Vector3 center = playerCollider.bounds.center;
        float radius = playerCollider.radius * 0.95f;
        float height = playerCollider.height * 0.5f - radius;

        Vector3 bottom = center + Vector3.down * height;

        float checkDistance = 0.15f;

        return Physics.SphereCast(bottom, radius, Vector3.down, out hit, checkDistance, floorLayer, QueryTriggerInteraction.Ignore);
    }

    private void OnDrawGizmos()
    {
        Vector3 center = playerCollider.bounds.center;
        float radius = playerCollider.radius * 0.95f;
        float height = playerCollider.height * 0.5f - radius;

        Vector3 bottom = center + Vector3.down * height;

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(bottom, radius);
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
            moveInput = Vector2.zero;
            jumpPressed = false;
            jumpHeld = false;
            sprintHeld = false;

            rb.linearVelocity = Vector3.zero;
        }
    }
}
