
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Android;


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
    [SerializeField] private float boxCastDistance = 0.2f;
    [SerializeField] private float jumpBuffer = 0.2f;

    [SerializeField] private Vector2 boxCastSize = new Vector2(0.15f, 0.15f);


    [SerializeField] private float gravity = 1;

    [SerializeField] private float coyoteTime = 1;
    [SerializeField] private float jumpGracePeriod = 1;
    [SerializeField] private float dashTime = 1;


    [SerializeField] private Rigidbody2D rigidBody2D;
    [SerializeField] private BoxCollider2D collisionBox;

    [SerializeField] private InputAction respawnControls;

    [SerializeField] private GameObject lastCheckpoint;
    [SerializeField] private AudioClip dashSound;
    [SerializeField] private LogicScript logicScript;
    [SerializeField] private AudioManagerScript audioManagerScript;

    private Vector2 pausedVelocity;

    private PlayerControls controls;


    private bool canDash = true;
    private bool hasDivedSinceLastOnGround = false;
    private bool shouldSlowVelocityAfterDash = false;
    private bool isOnIce = false;
    private bool levelIsComplete = false;
    public bool isPaused = false;

    private Vector2 movement;
    private float jumpAndDive;
    private float dash;

    private float timeLastOnGround = -99;
    private float timeLastOnLeftWall = -99;
    private float timeLastOnRightWall = -99;
    private float timeLastPressedJump = -99;
    private float timeLastDashed = -99;
    private float timeLastJumped = -99;

    private void Awake()
    {
        controls = new PlayerControls();

        controls.Player.Move.performed += ctx => movement = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += ctx => movement = Vector2.zero;

        controls.Player.JumpAndDive.started += ctx => jumpAndDive = ctx.ReadValue<float>();
        controls.Player.JumpAndDive.canceled += ctx => jumpAndDive = 0;

        controls.Player.Dash.started += ctx => dash = ctx.ReadValue<float>();

        controls.Player.Dash.canceled += ctx => dash = 0;
    }

    // Start is called before the first frame update
    void Start()
    {
        logicScript = GameObject.Find("Logic Manager").GetComponent<LogicScript>();
        audioManagerScript = GameObject.Find("AudioManager").GetComponent<AudioManagerScript>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!(levelIsComplete || isPaused))
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


    }

    private void HandleJumpingAndDiving()
    {
        if (jumpAndDive > 0)
        {
            timeLastPressedJump = Time.time;
        }
        if (timeLastPressedJump + jumpGracePeriod > Time.time && timeLastJumped + jumpBuffer < Time.time)
        {
            if (timeLastOnGround + coyoteTime > Time.time)
            {
                rigidBody2D.velocity = new Vector2(rigidBody2D.velocity.x, Mathf.Clamp(rigidBody2D.velocity.y, 0, 20) + jumpVelocity);
                timeLastPressedJump = -9;
                timeLastJumped = Time.time;
            }
            else if (timeLastOnLeftWall + coyoteTime > Time.time)
            {
                rigidBody2D.velocity = new Vector2(jumpVelocity, jumpVelocity);
                hasDivedSinceLastOnGround = false;
                timeLastPressedJump = -9;
                timeLastJumped = Time.time;
            }
            else if (timeLastOnRightWall + coyoteTime > Time.time)
            {
                rigidBody2D.velocity = new Vector2(-jumpVelocity, jumpVelocity);
                hasDivedSinceLastOnGround = false;
                timeLastPressedJump = -9;
                timeLastJumped = Time.time;
            }
  
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

        if (dash > 0 && canDash && !(IsOnGround() || IsOnWallLeftSide() || IsOnWallRightSide()))
        {
            rigidBody2D.velocity = movement.normalized * dashSpeed;
            dash = 0;
            timeLastDashed = Time.time;
            audioManagerScript.PlayEffect(dashSound);
            canDash = false;
            shouldSlowVelocityAfterDash = true;
            hasDivedSinceLastOnGround = false;
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
            if (!isOnIce)
            {
                rigidBody2D.velocity += new Vector2((movement.x * groundedAcceleration - rigidBody2D.velocity.x * groundedAccelerationDecrease) * Time.deltaTime, 0);
            } else
            {
                rigidBody2D.velocity += new Vector2((movement.x * groundedAcceleration) * Time.deltaTime, 0);
            }
            HandleFriction();
        }
        else
        {
            rigidBody2D.velocity += new Vector2(movement.x * aerialAcceleration * Time.deltaTime, 0);
            HandleDrag();
        }
        //ClampVelocity();
    }

    private void HandleFriction()
    { if (!isOnIce)
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
    }

    private void HandleDrag()
    {
        if (!hasDivedSinceLastOnGround)
        {
            rigidBody2D.velocity -= rigidBody2D.velocity.normalized * rigidBody2D.velocity.sqrMagnitude * aerialDrag * Time.deltaTime * 1f;
        }

    }



    private void HandleGravity()
    {

        rigidBody2D.velocity += Vector2.down * gravity * Time.deltaTime;

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (IsOnWallLeftSide() || IsOnWallRightSide() || IsOnCieling() || IsOnGround())
        {
            timeLastDashed = 0;
            timeLastJumped = 0;
        }
        

    }



    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!levelIsComplete)
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
            }
            if (IsOnWallLeftSide())
            {
                timeLastOnLeftWall = Time.time;
            }
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
        if (collision.gameObject.tag.Equals("Star"))
        {
            logicScript.CompleteLevel();
            levelIsComplete = true;
            rigidBody2D.velocity = Vector2.zero;
        }
        if (collision.gameObject.tag.Equals("Dodge Refresh"))
        {
            canDash = true;
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        isOnIce = collision.tag.Equals("Ice");
        if (collision.tag.Equals("No-Jump Surface")) {
            hasDivedSinceLastOnGround = false;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag.Equals("Ice"))
        {
            isOnIce = false;
        }


    }


    public bool IsOnGround()
    {

        LayerMask mask = LayerMask.GetMask("Jumpable Surface");
        RaycastHit2D hit = Physics2D.BoxCast(transform.position, boxCastSize * 0.9f, 0, Vector2.down, boxCastDistance, mask);
        if (hit.collider != null) {
            if (hit.collider.tag.Equals("No-Jump Surface"))
            {
                return false;
            }
        }
        return hit;
    }

    public bool IsOnWallRightSide()
    {
        LayerMask mask = LayerMask.GetMask("Jumpable Surface");
        RaycastHit2D hit = Physics2D.BoxCast(transform.position, boxCastSize * 0.9f, 0, Vector2.right, boxCastDistance, mask);
        if (hit.collider != null)
        {
            if (hit.collider.tag.Equals("No-Jump Surface"))
            {
                return false;
            }
        }
        return hit;
    }

    public bool IsOnWallLeftSide()
    {
        LayerMask mask = LayerMask.GetMask("Jumpable Surface");
        RaycastHit2D hit = Physics2D.BoxCast(transform.position, boxCastSize * 0.9f, 0, Vector2.left, boxCastDistance, mask);
        if (hit.collider != null)
        {
            if (hit.collider.tag.Equals("No-Jump Surface"))
            {
                return false;
            }
        }
        return hit;
    }

    public bool IsOnCieling()
    {
        LayerMask mask = LayerMask.GetMask("Jumpable Surface");
        RaycastHit2D hit = Physics2D.BoxCast(transform.position, boxCastSize * 0.9f, 0, Vector2.up, boxCastDistance, mask);
        if (hit.collider != null)
        {
            if (hit.collider.tag.Equals("No-Jump Surface"))
            {
                return false;
            }
        }
        return hit;
    }

    public void returnToCheckpoint()
    {
        rigidBody2D.velocity = Vector2.zero;
        transform.position = lastCheckpoint.transform.position + new Vector3(0, 0.45f, 0);
        hasDivedSinceLastOnGround = false;
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

    public void SetPause(bool value)
    {
        isPaused = value;
        if (value)
        {
            pausedVelocity = rigidBody2D.velocity;
            rigidBody2D.velocity = Vector2.zero;
        } else
        {
            rigidBody2D.velocity = pausedVelocity;
        }
    }
}
