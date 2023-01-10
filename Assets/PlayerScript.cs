using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerScript : MonoBehaviour
{
    [SerializeField] private float groundedAcceleration = 1;
    [SerializeField] private float groundedAccelerationDecrease = 1;
    [SerializeField] private float aerialAcceleration = 1;
    [SerializeField] private float groundedFriction = 1;
    [SerializeField] private float aerialDrag = 1;

    [SerializeField] private float jumpVelocity = 1;
    [SerializeField] private float diveSpeed = 1;
    [SerializeField] private float dashSpeed = 1;
    [SerializeField] private float afterDashVelocity = 1;
    [SerializeField] private float diveBounceSpeed = 3;


    [SerializeField] private float gravity = 1;

    [SerializeField] private float coyoteTime = 1;
    [SerializeField] private float jumpGracePeriod = 1;
    [SerializeField] private float dashTime = 1;


    [SerializeField] private Rigidbody2D rigidBody2D;
    [SerializeField] private BoxCollider2D collisionBox;

    [SerializeField] private InputAction respawnControls;

    [SerializeField] private GameObject lastCheckpoint;
    [SerializeField] private AudioSource dashSound;

    private PlayerControls controls;


    private bool canDash = true;
    private bool hasDivedSinceLastOnGround = false;
    private bool shouldSlowVelocityAfterDash = false;

    private Vector2 movement;
    private float jumpAndDive;
    private float dash;

    private float timeLastOnGround = -99;
    private float timeLastOnLeftWall = -99;
    private float timeLastOnRightWall = -99;
    private float timeLastPressedJump = -99;
    private float timeLastDashed = -99;

    private void Awake()
    {
        controls = new PlayerControls();

        controls.Player.Move.performed += ctx => movement = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += ctx => movement = Vector2.zero;

        controls.Player.JumpAndDive.started += ctx =>
        {
                jumpAndDive = ctx.ReadValue<float>();
        };
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
        HandleJumpingAndDiving();
        HandleDashing();
        // If the player hasn't dashed recently, apply gravity and acceleration
        if (timeLastDashed + dashTime < Time.time)
        {
            HandleAcceleration();
            HandleGravity();
        }


        if (respawnControls.ReadValue<float>() == 1)
        {
            if (lastCheckpoint != null)
            {
                returnToCheckpoint();
            }
        }


    }

    private void HandleJumpingAndDiving()
    {
        if (jumpAndDive > 0)
        {
            timeLastPressedJump = Time.time;
        }
        if (timeLastPressedJump + jumpGracePeriod > Time.time)
        {
            if (timeLastOnGround + coyoteTime > Time.time)
            {
                rigidBody2D.velocity = new Vector2(rigidBody2D.velocity.x, Mathf.Clamp(rigidBody2D.velocity.y, 0, 20) + jumpVelocity);
                timeLastPressedJump = 0;
            }
            else if (timeLastOnLeftWall + coyoteTime > Time.time)
            {
                rigidBody2D.velocity = new Vector2(jumpVelocity, jumpVelocity);
                timeLastPressedJump = 0;
            }
            else if (timeLastOnRightWall + coyoteTime > Time.time)
            {
                rigidBody2D.velocity = new Vector2(-jumpVelocity, jumpVelocity);
                timeLastPressedJump = 0;
            }
            timeLastOnGround = 0;
            timeLastOnLeftWall = 0;
            timeLastOnRightWall = 0;
        } else if (jumpAndDive < 0)
        {
            if (!IsOnGround())
            {
                rigidBody2D.velocity = new Vector2(rigidBody2D.velocity.x / 10, -diveSpeed);
                hasDivedSinceLastOnGround = true;
            }
        }
        jumpAndDive = 0;
    }

    private void HandleDashing()
    {

        if (!canDash)
        {
            if (IsOnGround() || IsOnWallLeftSide() || IsOnWallRightSide())
            {
                canDash = true;

            }
        }

        if (!(IsOnGround() || IsOnWallLeftSide() || IsOnWallRightSide()))
        {
            if (canDash)
            {
                if (dash > 0)
                {
                    rigidBody2D.velocity = movement.normalized * dashSpeed;
                    timeLastDashed = Time.time;
                    dashSound.Play();
                    canDash = false;
                    shouldSlowVelocityAfterDash = true;

                }
            }
        }

        if (timeLastDashed + dashTime < Time.time && shouldSlowVelocityAfterDash)
        {
            rigidBody2D.velocity = rigidBody2D.velocity.normalized * afterDashVelocity;
            shouldSlowVelocityAfterDash = false;
        }
    }

    private void HandleAcceleration()
    {
        if (IsOnGround())
        {
            rigidBody2D.velocity += new Vector2((movement.x * groundedAcceleration - rigidBody2D.velocity.x * groundedAccelerationDecrease) * Time.deltaTime, 0);
        } else
        {
            rigidBody2D.velocity += new Vector2(movement.x * aerialAcceleration * Time.deltaTime, 0);
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
            }
            else
            {
                rigidBody2D.velocity = Vector2.zero;
            }
        }
        else
        {
            if (!hasDivedSinceLastOnGround)
            {
                rigidBody2D.velocity -= rigidBody2D.velocity.normalized * rigidBody2D.velocity.sqrMagnitude * aerialDrag * Time.deltaTime * 1f;
            }
        }
    }



    private void HandleGravity()
    {
        if (!IsOnGround())
        {
            rigidBody2D.velocity += Vector2.down * gravity * Time.deltaTime;
        }
    }



    private void OnCollisionStay2D(Collision2D collision)
    {

        if (IsOnGround())
        {
            if (hasDivedSinceLastOnGround)
            {
                rigidBody2D.velocity = new Vector2(rigidBody2D.velocity.x, diveBounceSpeed);
                hasDivedSinceLastOnGround = false;
            }
            timeLastOnGround = Time.time;
        }
        if (IsOnWallRightSide())
        {
            timeLastOnRightWall = Time.time;
            hasDivedSinceLastOnGround = false;

        }
        if (IsOnWallLeftSide())
        {
            timeLastOnLeftWall = Time.time;
            hasDivedSinceLastOnGround = false;

        }


    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Equals("Checkpoint"))
        {
            lastCheckpoint = collision.gameObject;
        }
        if (collision.gameObject.tag.Equals("Hazard"))
        {
            returnToCheckpoint();
        }
    }


    public bool IsOnGround()
    {
        LayerMask mask = LayerMask.GetMask("Jumpable Surface");
        return Physics2D.BoxCast(transform.position, new Vector2(0.14f, 0.14f) * 0.9f, 0, Vector2.down, 0.05f, mask);

    }

    public bool IsOnWallRightSide()
    {
        LayerMask mask = LayerMask.GetMask("Jumpable Surface");
        return Physics2D.BoxCast(transform.position, new Vector2(0.14f, 0.14f) * 0.9f, 0, Vector2.right, 0.05f, mask);
    }

    public bool IsOnWallLeftSide()
    {
        LayerMask mask = LayerMask.GetMask("Jumpable Surface");
        return Physics2D.BoxCast(transform.position, new Vector2(0.14f, 0.14f) * 0.9f, 0, Vector2.left, 0.05f, mask);
    }

    public bool IsOnCieling()
    {
        LayerMask mask = LayerMask.GetMask("Jumpable Surface");
        return Physics2D.BoxCast(transform.position, new Vector2(0.14f, 0.14f) * 0.9f, 0, Vector2.up, 0.05f, mask);
    }

    public void returnToCheckpoint()
    {
        rigidBody2D.velocity = Vector2.zero;
        transform.position = lastCheckpoint.transform.position + new Vector3(0, 0.45f, 0);
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
}
