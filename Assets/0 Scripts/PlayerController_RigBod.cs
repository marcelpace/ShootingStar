using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController_RigBod : MonoBehaviour {


    #region Initializing    
    // Horizontal move
    public float speedX = 10f;
    Vector2 move;
    private float moveInputX;
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

    public ParticleSystem jetParticles1;    
    public ParticleSystem jetParticles2;
    public GameObject jetPack;

    public float jetPackRotation = 30f;
    public float jetPackRotationSpeed = 1f;
    private float jetPackTargetRotation = 0f;


    
    private Animator animator;


    private SpriteRenderer spriteRenderer;






    // Debug
    public TextMeshPro debugText1;
    public TextMeshPro debugText2;
    public TextMeshPro debugText3;
    public TextMeshPro debugText4;


    void Awake() {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();   
        animator = GetComponent<Animator>(); 

    }


    void Start() {
        jumpFuel = jumpFuelMax;         
        //TouchSimulation.Enable();
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
        Rendering();

        debugText1.text = "jumpInitialPosition: " + jumpInitialPosition.ToString();
        debugText2.text = "jumpMaxHeight: " + jumpMaxHeight.ToString();
        debugText3.text = "screenWidth: " + Screen.width.ToString() + ", screenHeight: " + Screen.height.ToString();
    }
    #endregion




    #region Collision
    void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.CompareTag("Ground")) {
            isGrounded = true;

            //this will start a countdown to recharge the fuel
            isJumpFuelRecharging = true;
            //here we ensure the jump cool down will reset
            isJumpAirCoolingDown = false;
        }
    }


    void OnCollisionExit2D(Collision2D collision) {
        if (collision.gameObject.CompareTag("Ground")) {
            isGrounded = false; 
        }
    }

    #endregion




    #region Functions
    void Movement(){
        // Change the drag/intertia if it is on air other on ground
        rb.drag = isGrounded ? groundDrag : airDrag;

        // Initialize movement input
        moveInputX = 0;     

        // Horizontal Movement

        // Input detection for
        // controller:
        moveInputX = Input.GetAxisRaw("Horizontal"); 

        
        // Handle touchscreen input for movement
        if (Input.touchCount > 0) {
            for (int i = 0; i < Input.touchCount; i++) {
                Touch touch = Input.GetTouch(i);  // Get every touch

                if(touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled) {
                    Debug.Log("Touch has ended");
                    moveInputX = 0;
                    jumpCanAddForce = false;
                }

                // Convert touch position into percentage 
                float touchXPercentage = touch.position.x / Screen.width;

                // Check touch position and perform action accordingly
                if (touch.phase != TouchPhase.Ended && touch.phase != TouchPhase.Canceled) {

                    // if we touch 20% left side of the screen: the area of the "button"
                    if (touchXPercentage < 0.2f) {
                        // Move player left
                        moveInputX = -1;
                    }
                    // if we touch 20% right side of the screen: the area of the "button"
                    else if (touchXPercentage > 0.8f) {
                        // Move player right
                        moveInputX = 1; 
                    }
                }
            }
        }
    
        // Move
        move = new Vector2(moveInputX,0); 
        
        // Apply movement
        rb.velocity = new Vector2 (move.x * speedX, rb.velocity.y);

        // Jet Pack Rotation
        float angle = Mathf.LerpAngle(jetPack.transform.rotation.eulerAngles.z, jetPackTargetRotation, Time.deltaTime * jetPackRotationSpeed);
        jetPack.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
 
    }

    
    void Jump() {

        // Initialize jump input
        jumpCanAddForce = false;


        // CONTROLLER: Jump if pressing button
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
        // stop add force if button is unpressed
        if (Input.GetButtonUp("Jump")) {
            jumpCanAddForce = false;
        }


        // TOUCHSCREEN Jump if touching screen
        if (Input.touchCount > 0) {
            for (int i = 0; i < Input.touchCount; i++) {
                Touch touch = Input.GetTouch(0);  // Get the first touch

                if(touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled) {
                    jumpCanAddForce = false;
                }

                // Convert touch position into percentage 
                float touchYPercentage = touch.position.y / Screen.height;

                // Check touch position and perform action accordingly
                if (touch.phase != TouchPhase.Ended && touch.phase != TouchPhase.Canceled) {
                    
                    if (touchYPercentage < 0.3f) {
                        // Jump
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
                }
            }
        }


        // Jump max height
        if (transform.position.y - jumpInitialPosition > jumpMaxHeight) {
            transform.position = new Vector2(transform.position.x, jumpInitialPosition + jumpMaxHeight);
            rb.velocity = new Vector2(rb.velocity.x, 0);
        }

        // Jump Air cooldown [cosmetics]
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

        
        // ensure no jump if no fuel
        if (jumpFuel <= 0) {
            jumpCanAddForce = false;
        }
        
        
    }

    void Rendering() {
        // Sprite flip
        // Flip the sprite according to getAxisRaw
        if (moveInputX == -1) {
            spriteRenderer.flipX = true;

            jetPackTargetRotation = jetPackRotation;
        }
        else if (moveInputX == 0) {
            jetPackTargetRotation = 0;      
        }
        else if (moveInputX == 1) { 
            spriteRenderer.flipX = false;

            jetPackTargetRotation = -jetPackRotation;            
        }
            //print ("play");
        
        // code for the jetpacks, on hold
        if (jumpCanAddForce){
            jetParticles1.Play();
            jetParticles2.Play();
        }
        else {
            jetParticles1.Stop();
            jetParticles2.Stop();            
        }
    }



    #endregion
}
