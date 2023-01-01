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


    private bool canDash = true;
    private bool shouldClampHoriznontalSpeed = true;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (jumpAndDiveControls.ReadValue<float>() == -1)
        {
            if (!IsOnGround())
            {
                rigidBody2D.velocity = new Vector2(rigidBody2D.velocity.x, rigidBody2D.velocity.y - diveSpeed);
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
                if (dashControls.ReadValue<float>() != 0) {
                    rigidBody2D.velocity = new Vector2(rigidBody2D.velocity.x + dashControls.ReadValue<float>() * dashSpeed, verticallDashSpeed);
                    canDash = false;
                    shouldClampHoriznontalSpeed = false;
                 }
            }
        }

       
        rigidBody2D.velocity = new Vector2(rigidBody2D.velocity.x + movementControls.ReadValue<float>() * moveSpeed * Time.deltaTime, rigidBody2D.velocity.y);
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
    }

    private void OnDisable()
    {
        movementControls.Disable();
        jumpAndDiveControls.Disable();
        dashControls.Disable();
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (jumpAndDiveControls.ReadValue<float>() == 1)
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
    }

    public bool IsOnGround()
    {
        GameObject[] groundObjects = GameObject.FindGameObjectsWithTag("Ground");
        foreach (GameObject groundObject in groundObjects)
        {
            if (bottomEdgeCollider.IsTouching(groundObject.GetComponent<Collider2D>())) {
                return true;
            }
        }
        return false;
    }
}
