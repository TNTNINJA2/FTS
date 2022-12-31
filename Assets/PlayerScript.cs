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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        rigidBody2D.velocity = new Vector2(movementControls.ReadValue<float>() * moveSpeed, rigidBody2D.velocity.y);

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
        }
    }
}
