using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController_RigBod : MonoBehaviour {


    #region Initializing
    // Horizontal move
    public float speedX = 10f;
    Vector2 move;
    Rigidbody2D rb;

    // JUMP
    private bool jumpCanAddForce = false;

    // Decelerations
    [SerializeField] private float groundDrag = 33;
    [SerializeField] private float airDrag = 0;

    public float jumpFuel;
    [SerializeField] private float jumpFuelMax = 2f;
    public float jumpForce = 0.5f;
    [SerializeField] private bool isGrounded;
    // Jump clamping
    [SerializeField] private float maxDownSpeed = 4f;
    [SerializeField] private float maxUpSpeed = 5f;
   
    //Jump Max Height
    private float jumpInitialPosition;
    [SerializeField] private float jumpMaxHeight = 2;



    // Jump Air is just a cosmetic to simulate a faulty jet pack
    [SerializeField] private float jumpAirCoolDown = 0.5f;
    [SerializeField] private float jumpAirCurrentCoolDown;
    [SerializeField] private bool isJumpAirCoolingDown = false;


    // Jump fuel cool down is the time to recharge the fuel, so the player cannot jump right after touching ground. 
    [SerializeField] private float jumpFuelCurrentCoolDown;
    [SerializeField] private float jumpFuelCoolDown = 0.5f;
    [SerializeField] private bool isJumpFuelRecharging = false;
    [SerializeField] public float jumpFuelRechargeSpeed = 2;

    
    private Animator animator;


    private SpriteRenderer spriteRenderer;



    void Awake() {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();   
        animator = GetComponent<Animator>(); 
    }


    void Start() {
        jumpFuel = jumpFuelMax;
        
    }
    #endregion


    #region Updates
    void FixedUpdate() {    
        // JUMP
        if (jumpCanAddForce) {
            rb.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);
        }
    }


    void Update() {
        Movement();
        Jump();
    }
    #endregion


    #region Collision
    void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.tag == "Ground") {
            isGrounded = true;

            //this will start a countdown to recharge the fuel
            isJumpFuelRecharging = true;
            //here we ensure the jump cool down will reset
            isJumpAirCoolingDown = false;
        }
    }


    void OnCollisionExit2D(Collision2D collision) {
        if (collision.gameObject.tag == "Ground") {
            isGrounded = false; 
        }
    }

    #endregion


    #region Functions
    void Movement(){
        // Change the drag/intertia if it is on air other on ground
        rb.drag = isGrounded ? groundDrag : airDrag;

        // Horizontal Movement
        float moveInputX = Input.GetAxisRaw("Horizontal");
        move = new Vector2(moveInputX,0); 
        
        // Apply movement
        rb.velocity = new Vector2 (move.x * speedX, rb.velocity.y);

        // Sprite flip
        // Flip the sprite according to getAxisRaw
        if (moveInputX == -1) {
            spriteRenderer.flipX = true;
        }
        if (moveInputX == 1) {
            spriteRenderer.flipX = false;
        }
    }


    void Jump() {

    // Jump if pressing button
        if (Input.GetButton("Jump") ) { 
            
            if (jumpFuel > 0 && !isJumpFuelRecharging) {
                // we set the jump initial position on the first frame of the jump, where isGrounded is still true;
                if (isGrounded) {
                    jumpInitialPosition = transform.position.y;
                }
                //print ("initial jump position: " + initialJumpPosition);
                
                // will jump if jump is not in cool down.
                if (!isJumpAirCoolingDown) { 
                    // jump will reach only a certain height based on it's initial position
                    if (transform.position.y < jumpMaxHeight + jumpInitialPosition)  {
                        jumpCanAddForce = true;
                    }
                    // if the player keeps pressing the button but when it reaches max height it will shut down for a few seconds
                    else if (transform.position.y >= jumpMaxHeight + jumpInitialPosition) {
                        jumpCanAddForce = false;
                        isJumpAirCoolingDown = true;
                        //print ("max Height reached");
                    }
                }
            
                // Reduce the Fuel each frame
                jumpFuel -= Time.deltaTime;
            }
        }

        // Jump max height
        if (transform.position.y - jumpInitialPosition > jumpMaxHeight) {
            transform.position = new Vector2(transform.position.x, jumpInitialPosition + jumpMaxHeight);
            rb.velocity = new Vector2(rb.velocity.x, 0);
        }

        // Jump Air Cool Down
        if (isJumpAirCoolingDown) {
            jumpAirCurrentCoolDown -= Time.deltaTime;
            if (jumpAirCurrentCoolDown <= 0) {
                isJumpAirCoolingDown = false;
                jumpAirCurrentCoolDown = jumpAirCoolDown;
            }
        }

        // Jump Fuel recharge
        if (isJumpFuelRecharging) {
            if (jumpFuel < jumpFuelMax) {
                jumpFuel += jumpFuelRechargeSpeed * Time.deltaTime;
            }
            else if (jumpFuel >= jumpFuelMax && isGrounded) {
                jumpFuel = jumpFuelMax;
                isJumpFuelRecharging = false;
                animator.Play("PlayerFuelBlink");
            }
        }
        
        // Jump treshold (Clamp):
        var velocity = rb.velocity;
        velocity.y = Mathf.Clamp(rb.velocity.y, -maxDownSpeed, maxUpSpeed);
        rb.velocity = velocity;
    }
    #endregion
}
