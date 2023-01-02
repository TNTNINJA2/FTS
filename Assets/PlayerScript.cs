using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerScript : MonoBehaviour
{
    [SerializeField] private float groundedAcceleration = 1;
    [SerializeField] private float aerialAcceleration = 1;
    [SerializeField] private float groundedFriction = 1;
    [SerializeField] private float groundedPseudoDrag = 1;
    [SerializeField] private float aerialDrag = 1;
    [SerializeField] private float jumpVelocity = 1;
    [SerializeField] private float maxSpeed = 1;
    [SerializeField] private float maxFallSpeed = 1;
    [SerializeField] private float diveSpeed = 1;
    [SerializeField] private float dashSpeed = 1;
    [SerializeField] private float verticalDashSpeed = 1;
    [SerializeField] private float gravity = 1;


    [SerializeField] private Rigidbody2D rigidBody2D;
    [SerializeField] private BoxCollider2D collisionBox;

    [SerializeField] private InputAction respawnControls;

    [SerializeField] private GameObject lastCheckpoint;

    private PlayerControls controls;


    private bool canDash = true;
    private bool shouldClampHoriznontalSpeed = true;
    private bool shouldClampVerticalSpeed = true;

    private float horizontalMovement;
    private float jumpAndDive;
    private float dash;

    private void Awake()
    {
        controls = new PlayerControls();

        controls.Player.Move.performed += ctx => horizontalMovement = ctx.ReadValue<float>();
        controls.Player.Move.canceled += ctx => horizontalMovement = 0;

        controls.Player.JumpAndDive.performed += ctx => jumpAndDive = ctx.ReadValue<float>();
        controls.Player.JumpAndDive.canceled += ctx => jumpAndDive = 0;

        controls.Player.Dash.performed += ctx => dash = ctx.ReadValue<float>();
        controls.Player.Dash.canceled += ctx => dash = 0;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        HandleGravity();
        if (jumpAndDive < 0)
        {
            if (!IsOnGround())
            {
                rigidBody2D.velocity = new Vector2(rigidBody2D.velocity.x, rigidBody2D.velocity.y - diveSpeed);
            }
        }

        if (respawnControls.ReadValue<float>() == 1)
        {
            if (lastCheckpoint != null)
            {
                returnToCheckpoint();
            }
        }

        if (!canDash)
        {
            if (IsOnGround())
            {
                canDash = true;
                shouldClampHoriznontalSpeed = true;
                Debug.Log("Reset canDash to true");
            }
        }

        if (!IsOnGround())
        {
            if (canDash) {
                if (dash > 0) {
                    rigidBody2D.velocity = new Vector2(rigidBody2D.velocity.x + horizontalMovement * dashSpeed, verticalDashSpeed);
                    canDash = false;
                    shouldClampHoriznontalSpeed = false;
                 }
            }
        }



        HandleAcceleration();

    }

    private void HandleAcceleration()
    {
        if (IsOnGround())
        {
            rigidBody2D.velocity += new Vector2(horizontalMovement * groundedAcceleration * Time.deltaTime, 0);
        } else
        {
            rigidBody2D.velocity += new Vector2(horizontalMovement * aerialAcceleration * Time.deltaTime, 0);
        }
        HandleDrag();
        //ClampVelocity();
    }

    private void HandleDrag()
    {
        if (IsOnGround())
        {
            // If the force of the friction Will NOT cause the velocity vector to switch directions (velocity is not too small)
            if (rigidBody2D.velocity.magnitude > groundedFriction * Time.deltaTime)
            {
                rigidBody2D.velocity -= rigidBody2D.velocity.normalized * groundedFriction * Time.deltaTime;
            } else
            {
                rigidBody2D.velocity = Vector2.zero;
            }
        }
        else
        {
            rigidBody2D.velocity -= rigidBody2D.velocity.normalized * rigidBody2D.velocity.sqrMagnitude * aerialDrag * Time.deltaTime * 1f;
        }
    }

    private void ClampVelocity()
    {
        if (shouldClampHoriznontalSpeed)
        {
            rigidBody2D.velocity = new Vector2(Mathf.Clamp(rigidBody2D.velocity.x, -maxSpeed, maxSpeed), rigidBody2D.velocity.y);
        }
        if (shouldClampVerticalSpeed)
        {
            rigidBody2D.velocity = new Vector2(rigidBody2D.velocity.x, Mathf.Clamp(rigidBody2D.velocity.x, -maxFallSpeed, maxFallSpeed));
        }
    }

    private void HandleGravity()
    {
        if (!IsOnGround())
        {
            rigidBody2D.velocity += Vector2.down * gravity * Time.deltaTime;
        }
    }

    private void OnEnable()
    {

        respawnControls.Enable();
        controls.Player.Enable();
    }

    private void OnDisable()
    {

        respawnControls.Disable();
        controls.Player.Disable();
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (jumpAndDive > 0)
        {
            if (IsOnGround())
            {
                rigidBody2D.velocity = new Vector2(rigidBody2D.velocity.x, jumpVelocity);
            }
            if (IsOnWallRightSide())
            {
                rigidBody2D.velocity = new Vector2(-jumpVelocity, jumpVelocity);
            }
            if (IsOnWallLeftSide())
            {
                rigidBody2D.velocity = new Vector2(jumpVelocity, jumpVelocity);
            }

        }
        if (collision.collider.gameObject.tag.Equals("Checkpoint"))
        {
            lastCheckpoint = collision.collider.gameObject; 
        }
        if (collision.collider.gameObject.tag.Equals("Hazard"))
        {
            returnToCheckpoint();
        }
    }

    public bool IsOnGround()
    {
        LayerMask mask = LayerMask.GetMask("Jumpable Surface");
        return Physics2D.BoxCast(transform.position, collisionBox.size * 0.9f, 0, Vector2.down, 0.05f, mask);

    }

    public bool IsOnWallRightSide()
    {
        LayerMask mask = LayerMask.GetMask("Jumpable Surface");
        return Physics2D.BoxCast(transform.position, collisionBox.size * 0.9f, 0, Vector2.right, 0.05f, mask);
    }

    public bool IsOnWallLeftSide()
    {
        LayerMask mask = LayerMask.GetMask("Jumpable Surface");
        return Physics2D.BoxCast(transform.position, collisionBox.size * 0.9f, 0, Vector2.left, 0.05f, mask);
    }

    public void returnToCheckpoint()
    {
        rigidBody2D.velocity = Vector2.zero;
        transform.position = lastCheckpoint.transform.position + new Vector3(0, 0.45f, 0);
        
        

    }
}
