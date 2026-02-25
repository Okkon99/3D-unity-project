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
    [SerializeField] private float coyoteTimer;


    [Header("Friction Settings")]
    [SerializeField] private float groundFriction;
    [SerializeField] private float airFriction;

    [Header("Gravity Feel")]
    [SerializeField] float fallGravityMultiplier;

    [Header("Camera State")]
    [SerializeField] public float storedPitch;

    [Header("Backpack settings")]
    [SerializeField] public float deployLaunchVelocity = 15f;


    [Header("Dependencies")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] public CapsuleCollider playerCollider;
    [SerializeField] private LayerMask floorLayer;
    [SerializeField] private InputManager inputManager;
    [SerializeField] public Transform cameraAnchor;
    [SerializeField] public PlayerBackpack backpack;
    [SerializeField] private PlayerGrab playerGrab;
    [SerializeField] private DeployAudioPlayer deployAudio;
    [SerializeField] private float velocity; //debug line

    bool isGrounded;
    bool wasGrounded;
    public Vector3 inputDirection;
    Vector3 desiredVelocity;
    Vector2 moveInput;
    bool sprintHeld;
    bool jumpPressed;
    bool canJump;
    float coyoteTime;
    RaycastHit groundHit;
    public AugmentBase equippedAugment;

    //debug stuff
    Vector3 startPos;
    Vector3 tempInputDir;


    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        startPos = transform.position;
    }

    void Update()
    {
        velocity = rb.linearVelocity.magnitude; //debug line
        if (!isActivePlayer)
        {
            return;
        }

        // CACHE INPUTS
        var input = InputManager.instance.Input.Gameplay;

        moveInput = input.Move.ReadValue<Vector2>();
        sprintHeld = input.Sprint.IsPressed();

        if (input.Jump.triggered)
        {
            jumpPressed = true;
        }

        if (input.Augment.triggered)
        {
            if (playerGrab.heldBody != null && !backpack.IsOccupied)
            {
                if (playerGrab.heldBody.TryGetComponent<IsBackpackable>(out var item))
                {
                    playerGrab.Release();
                    backpack.TryInsert(item);
                }
            }
            else if (equippedAugment != null)
            {
                equippedAugment.Toggle();
            }
        }

        Vector2 lookDelta = input.Look.ReadValue<Vector2>();
        
        // CACHE INPUTS END

        // Mouse:
        float mouseX = lookDelta.x * lookSensitivity;
        float mouseY = lookDelta.y * lookSensitivity;

        transform.Rotate(Vector3.up * mouseX);

        if (input.Deploy.triggered)
        {
            if (backpack.IsOccupied)
            {
                backpack.Deploy(transform.forward * (deployLaunchVelocity / 4f) + (transform.up * deployLaunchVelocity));
                
                if (deployAudio != null)
                {
                    deployAudio.PlayDeploySequence();
                }
            }
        }
    }

    private void FixedUpdate()
    {
        //Jump
        wasGrounded = isGrounded;                   // wasGrounded = Last frame's isGrounded.
        isGrounded = CheckGrounded(out groundHit);  // isGrounded is now THIS frame's isGrounded.
        if (!wasGrounded && isGrounded)             // Checks if the player touched the ground THIS
        {                                           // frame but not the previous (did the player JUST land?)
            OnLanded();
        }
        else
        {
            CoyoteTimeCountdown();
        }

        GetDesiredVelocity();

        ApplyAccelVector();

        //ApplyFriction();




        //ApplyBetterGravity(); obsolete potentially, staying as a reminder just in case i do want it back.

        canJump = isGrounded || coyoteTime > 0f;

        if (jumpPressed && canJump)
        {
            Jump();
        }
        jumpPressed = false;
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
    }

    private void OnLanded()
    {
        ResetCoyoteTime();

        if (rb.linearVelocity.y < 0f)
        {
            rb.linearVelocity = new Vector3(
                rb.linearVelocity.x,
                0f,
                rb.linearVelocity.z
            );
        }

        transform.position = new Vector3(transform.position.x, groundHit.point.y+transform.localScale.y, transform.position.z);
    }

    private void GetDesiredVelocity()
    {
        inputDirection = transform.forward * moveInput.y + transform.right * moveInput.x;

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
        tempInputDir = inputDirection;
        inputDirection = RemoveWallPush(inputDirection);
        desiredVelocity = inputDirection * speed;
    }

    private void ApplyAccelVector()
    {
        Vector3 velocity = rb.linearVelocity;

        if (isGrounded)
        {
            Vector3 velocityDelta = desiredVelocity - velocity;

            float maxAccel = acceleration * Time.fixedDeltaTime;
            Vector3 accelVector = Vector3.ClampMagnitude(velocityDelta, maxAccel);

            rb.linearVelocity += accelVector;
        }
        else if (isActivePlayer && (velocity.magnitude < 10f))
        {
            Vector3 lateralVelocity = new Vector3(velocity.x, 0f, velocity.z);
            Vector3 desiredLateral = new Vector3(desiredVelocity.x, 0f, desiredVelocity.z);

            Vector3 airDelta = desiredLateral - lateralVelocity;

            float airControl = acceleration * 0.5f;
            float maxAirAccel = airControl * Time.fixedDeltaTime;

            Vector3 airAccel = Vector3.ClampMagnitude(airDelta, maxAirAccel);

            rb.linearVelocity += new Vector3(airAccel.x, 0f, airAccel.z);
        }
    }

    private Vector3 RemoveWallPush(Vector3 direction)
    {
        float checkDistance = 0.6f;

        if (Physics.SphereCast(
            playerCollider.bounds.center,
            playerCollider.radius * 0.9f,
            direction,
            out RaycastHit hit,
            checkDistance,
            floorLayer,
            QueryTriggerInteraction.Ignore))
        {
            // Remove component pushing into the wall
            direction = Vector3.ProjectOnPlane(direction, hit.normal);
        }

        return direction;
    }




    private bool CheckGrounded(out RaycastHit hit)
    {
        Vector3 center = playerCollider.bounds.center;
        float radius = playerCollider.radius * 0.95f;
        float height = playerCollider.height * 0.5f - playerCollider.radius;

        Vector3 bottom = center + Vector3.down * height;

        float checkDistance = 0.15f;

        return Physics.SphereCast(bottom, radius, Vector3.down, out hit, checkDistance, floorLayer, QueryTriggerInteraction.Ignore);
    }

    private void OnDrawGizmos()
    {
        Vector3 center = playerCollider.bounds.center;
        float radius = playerCollider.radius * 0.95f;
        float height = playerCollider.height * 0.5f - playerCollider.radius;

        Vector3 bottom = center + Vector3.down * height;

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(bottom, radius);


        ////


        Vector3 velocity = rb.linearVelocity;

        if (isGrounded)
        {
            Vector3 velocityDelta = desiredVelocity - velocity;

            float maxAccel = acceleration * Time.fixedDeltaTime;
            Vector3 accelVector = Vector3.ClampMagnitude(velocityDelta, maxAccel);
            Gizmos.DrawLine(transform.position, rb.linearVelocity + transform.position);
        }
        else
        {
            Vector3 lateralVelocity = new Vector3(velocity.x, 0f, velocity.z);
            Vector3 desiredLateral = new Vector3(desiredVelocity.x, 0f, desiredVelocity.z);

            Vector3 airDelta = desiredLateral - lateralVelocity;

            float airControl = acceleration * 0.5f;
            float maxAirAccel = airControl * Time.fixedDeltaTime;

            Vector3 airAccel = Vector3.ClampMagnitude(airDelta, maxAirAccel);

            Gizmos.DrawLine(transform.position, rb.linearVelocity + transform.position);
        }

    }

    public void SetActive(bool active)
    {
        isActivePlayer = active;

        if (!active)
        {
            moveInput = Vector2.zero;
            jumpPressed = false;
            sprintHeld = false;
        }

        if (active && backpack != null)
        {
            backpack.RefreshPreview();
        }
    }


    public void EquipAugment(AugmentBase augment)
    {
        equippedAugment = augment;
        equippedAugment.OnEquipped(this);
    }

    public void UnequipAugment()
    {
        if (equippedAugment != null)
            equippedAugment.OnUnequipped();

        equippedAugment = null;
    }
}
