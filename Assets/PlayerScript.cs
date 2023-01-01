using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerScript : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 1;
    [SerializeField]
    private float jumpVelocity = 1;
    [SerializeField]
    private float maxSpeed = 1;
    [SerializeField]
    private float diveSpeed = 1;
    [SerializeField]
    private float dashSpeed = 1;
    [SerializeField]
    private float verticallDashSpeed = 1;

    public Rigidbody2D rigidBody2D;

    [SerializeField]
    private Collider2D bottomEdgeCollider;
    [SerializeField]
    private Collider2D rightEdgeCollider;
    [SerializeField]
    private Collider2D leftEdgeCollider;
    [SerializeField]
    private Collider2D topEdgeCollider;

    [SerializeField]
    private InputAction movementControls;
    [SerializeField]
    private InputAction jumpAndDiveControls;
    [SerializeField]
    private InputAction dashControls;
    [SerializeField]
    private InputAction respawnControls;

    [SerializeField]
    private GameObject lastCheckpoint;

    private PlayerControls controls;


    private bool canDash = true;
    private bool shouldClampHoriznontalSpeed = true;

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
                    rigidBody2D.velocity = new Vector2(rigidBody2D.velocity.x + horizontalMovement * dashSpeed, verticallDashSpeed);
                    canDash = false;
                    shouldClampHoriznontalSpeed = false;
                 }
            }
        }

      
       
        rigidBody2D.velocity = new Vector2(rigidBody2D.velocity.x + horizontalMovement * moveSpeed * Time.deltaTime, rigidBody2D.velocity.y);
        if (shouldClampHoriznontalSpeed)
        {
            rigidBody2D.velocity = new Vector2(Mathf.Clamp(rigidBody2D.velocity.x, -maxSpeed, maxSpeed), rigidBody2D.velocity.y);
        }
    }

    private void OnEnable()
    {
        movementControls.Enable();
        jumpAndDiveControls.Enable();
        dashControls.Enable();
        respawnControls.Enable();
        controls.Player.Enable();
    }

    private void OnDisable()
    {
        movementControls.Disable();
        jumpAndDiveControls.Disable();
        dashControls.Disable();
        respawnControls.Disable();
        controls.Player.Disable();
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (jumpAndDive > 0)
        {
            if (collision.collider.IsTouching(bottomEdgeCollider))
            {
                rigidBody2D.velocity = new Vector2(rigidBody2D.velocity.x, jumpVelocity);
            }
            if (collision.collider.IsTouching(rightEdgeCollider))
            {
                rigidBody2D.velocity = new Vector2(-jumpVelocity, jumpVelocity);
            }
            if (collision.collider.IsTouching(leftEdgeCollider))
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
        Component[] colliders = Component.FindObjectsOfType<Collider2D>();
        
        foreach (Component collider in colliders)
        {if (!bottomEdgeCollider.Equals(collider))
            {
                if (bottomEdgeCollider.IsTouching(collider.GetComponent<Collider2D>()))
                {
                    
                    return true;
                }
            }
        }
      
        return false;
    }

    public void returnToCheckpoint()
    {
        rigidBody2D.velocity = Vector2.zero;
        transform.position = lastCheckpoint.transform.position + new Vector3(0, 0.45f, 0);
        Debug.Log("trying to respawn");
    }
}
