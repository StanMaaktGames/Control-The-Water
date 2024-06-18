using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    private float moveSpeed;
    public float walkSpeed;
    public float sprintSpeed;

    public float groundDrag;

    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump;

    [Header("Crouching")]
    public float crouchSpeed;
    public float crouchYScale;
    private float startYScale;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    bool grounded;

    [Header("Slope Handling")]
    public float maxSlopeAngle;
    private RaycastHit slopeHit;
    private bool exitingSlope;

    [Header("keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode crouchKey = KeyCode.LeftControl;

    [field: SerializeField, Header("HUD")]

    [Header("Stamina")]
    public Slider staminaSlider;
    public float maxStamina = 100f;
    float stamina;
    public float staminaRechargeSpeed = 1f;
    public float staminaRechargeRequiredHydration = 25f;
    public float staminaSprintCost = 1f;

    [Header("Health & Hydration")]
    public Slider healthSlider;
    public Slider hydrationSlider;
    public float maxHealth = 100f;
    public float maxHydration = 100f;
    public float thirstSpeed = 1f;
    public float lionDamage = 40f;
    float health;
    float hydration;

    [Header("Interacting")]
    public float interactRange = 4f;
    public LayerMask interactLayers;

    [Header("Other")]

    public Transform orientation;
    public GameObject gameManager;
    public GameObject playerCam;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    Rigidbody rb;

    public MovementState state;
    public enum MovementState
    {
        walking,
        sprinting,
        crouching,
        air
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        readyToJump = true;

        health = maxHealth;
        hydration = maxHydration;
        stamina = maxStamina;

        startYScale = transform.localScale.y;
    }

    void Update()
    {
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);

        MyInput();
        speedContol();
        StateHandler();

        if(grounded)
            rb.drag = groundDrag;
        else
            rb.drag = 0;

        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("interact");
            RaycastHit hit;
            if (Physics.Raycast(transform.position, playerCam.transform.forward, out hit, interactRange, interactLayers))
            {
                Debug.DrawRay(transform.position, playerCam.transform.forward * interactRange, Color.yellow, 2, false);
                Debug.Log(gameObject);
                Debug.Log(hit.collider.gameObject.GetComponent<Food>());
                if (hit.collider.gameObject.GetComponent<Food>() == null)
                {
                    Debug.Log("ERRORERRORERRORERROR");
                }
                hit.collider.gameObject.GetComponent<Food>().Interact(gameObject);
            }
            else
            {
                Debug.DrawRay(transform.position, playerCam.transform.forward * interactRange, Color.red, 2, false);
            }
        }

        if (!Input.GetKey(sprintKey) && hydration > staminaRechargeRequiredHydration)
        {
            stamina += Time.deltaTime * staminaRechargeSpeed;
            if (stamina > maxStamina)
            {
                stamina = maxStamina;
            }
        }

        hydration -= Time.deltaTime * thirstSpeed;

        staminaSlider.value = stamina;
        healthSlider.value = health; 
        hydrationSlider.value = hydration;     
    }

    void FixedUpdate()
    {
        MovePlayer();
    }

    void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        //jump?
        if(Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }

        // crouch
        if(Input.GetKeyDown(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
            rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        }

        // no crouch
        if(Input.GetKeyUp(crouchKey))
        {
          transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
        }
    }

    void StateHandler()
    {
        // mode - crouching
        if(Input.GetKeyDown(crouchKey))
        {
            state = MovementState.crouching;
            moveSpeed = crouchSpeed;
        }

        // mode - sprinting
        else if(grounded && Input.GetKey(sprintKey))
        {
            if (stamina > 0)
            {
                state = MovementState.sprinting;
                moveSpeed = sprintSpeed;
                stamina -= Time.deltaTime * staminaSprintCost;
            }
            else
            {
                state = MovementState.walking;
                moveSpeed = walkSpeed;
            }
        }

        // mode - walking
        else if(grounded)
        {
            state = MovementState.walking;
            moveSpeed = walkSpeed;
        }

        // mode - air
        else
        {
            state = MovementState.air;
        }
    }

    void MovePlayer()
    {
        //calcutate movement moveDirection
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        //on ground
        if(grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);

        //on slope
        else if(OnSlope() && !exitingSlope)
        {
            rb.AddForce(GetSlopeMoveDirection() * moveSpeed * 10f, ForceMode.Force);
            if(rb.velocity.y > 0)
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);
        }

        //in air
        else if(!grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);

        rb.useGravity = !OnSlope();
    }

    void speedContol()
    {
        // limit speed on slope
        if(OnSlope() && !exitingSlope)
        {
            if(rb.velocity.magnitude > moveSpeed)
                rb.velocity = rb.velocity.normalized * moveSpeed;
        }

        // limit speed on ground or in air
        else
        {
            Vector3 flatVel =  new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            if(flatVel.magnitude > moveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * moveSpeed;
                rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
            }
        }
    }

    void Jump()
    {
        exitingSlope = true;

        // y velocity is 0
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    void ResetJump()
    {
        readyToJump = true;

        exitingSlope = false;
    }

    bool OnSlope()
    {
        if(Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }

        return false;
    }

    private Vector3 GetSlopeMoveDirection()
    {
       return Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized;
    }

    public void UpdateHealthAndHydration(float healthUpdate, float hydrationUpdate)
    {
        health += healthUpdate;
        hydration += hydrationUpdate;

        if (health > maxHealth)
        {
            health = maxHealth;
        }
        else if (health <= 0)
        {
            gameManager.GetComponent<GameManager>().Respawn();
        }
        if (hydration > maxHydration)
        {
            hydration = maxHydration;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log(other);
        if (other.CompareTag("Enemy Weapon"))
        {
            UpdateHealthAndHydration(lionDamage, 0);
        }
    }
}
