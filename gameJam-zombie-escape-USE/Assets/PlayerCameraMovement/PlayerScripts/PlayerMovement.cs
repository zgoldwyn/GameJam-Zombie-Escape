using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementTutorial : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;
    public float groundDrag;
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump;

    [HideInInspector] public float walkSpeed;
    [HideInInspector] public float sprintSpeed;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    bool grounded;

    public Transform orientation;

    float horizontalInput;
    float verticalInput;
    Vector3 moveDirection;
    Rigidbody rb;

    [SerializeField] private float rcooldown = 5f; // Serialized cooldown field
    private float currentCooldown;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        readyToJump = true;
        currentCooldown = rcooldown;
    }

    private void Update()
    {
        MyInput();
        SpeedControl();

        // Handle drag based on grounded state
        rb.drag = grounded ? groundDrag : 0;
        if (Input.GetKeyDown(KeyCode.R) && currentCooldown >= rcooldown)
        {
            currentCooldown = 0; // Reset current cooldown
            StartCoroutine(DebugRise());
        }

        // Update cooldown timer
        if (currentCooldown < rcooldown)
        {
            currentCooldown += Time.deltaTime; // Increment cooldown
        }
    }
    private IEnumerator DebugRise()
    {
        float riseAmount = 2f; // Amount to rise
        float duration = 0.5f; // Duration of the rise
        float elapsedTime = 0f;

        // Apply upward force
        rb.AddForce(Vector3.up * riseAmount, ForceMode.Impulse);
        
        // Wait for a short duration before allowing gravity to take over
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            yield return null; // Wait until the next frame
        }
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        // Jumping logic
        if (Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    private void MovePlayer()
    {
        // Calculate movement direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        // Apply force for movement based on grounded state
        if (grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        else
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        // Limit velocity if needed
        if (flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

    private void Jump()
    {
        // Reset y velocity and apply jump force
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        readyToJump = true;
    }

    private void OnCollisionEnter(Collision other)
    {
        // Check if the player is grounded by colliding with the ground layer
        if (((1 << other.gameObject.layer) & whatIsGround) != 0)
        {
            grounded = true;
        }
    }

    private void OnCollisionExit(Collision other)
    {
        // Check if the player is no longer in contact with the ground layer
        if (((1 << other.gameObject.layer) & whatIsGround) != 0)
        {
            grounded = false;
        }
    }
}
