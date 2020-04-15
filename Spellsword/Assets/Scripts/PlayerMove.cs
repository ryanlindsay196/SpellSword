using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{

    public enum CharacterStates
    {
        NONE,
        IDLE,
        IDLEJUMP,
        FIDGET,
        WALK,
        RUN,
        RUNNINGJUMP,
        CASTING
    };

    CharacterStates myPlayerState = CharacterStates.NONE;

    private CharacterController charController;

    private Animator anim;

    private bool isJumping = false;
        private bool  isMoving, runFidget, isRunning, isGrounded, isCasting;

    float vertInput;
    float horizInput;
    public float idleWaitTime;
    public float secondsInState;

    [SerializeField] private string horizontalInputName;
    [SerializeField] private string verticalInputName;

    [SerializeField] private float walkSpeed, runSpeed;
    [SerializeField] private float runBuildUpSpeed;
    [SerializeField] private KeyCode runKey;

    private float movementSpeed;

    [SerializeField] private float slopeForce;
    [SerializeField] private float slopeForceRayLength;

    [SerializeField] private AnimationCurve jumpFalloff;
    [SerializeField] private float jumpMultiplier;
    [SerializeField] private KeyCode jumpKey;

    [SerializeField] private KeyCode castKey;

    private void Awake()
    {
        charController = GetComponent<CharacterController>();
    }

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();

        SetPlayerState(CharacterStates.IDLE);
    }

    // Update is called once per frame
    void Update()
    {
        //Gather Input (horizInput, vertInput, isJumping, isRunning)
        CheckInputData();

        //Change State based on Input (and current state)
        //Move character, etc. based on State
        switch (myPlayerState)
        {
            case CharacterStates.IDLE:

                if(isMoving)
                {
                    if (!isRunning)
                    {
                        SetPlayerState(CharacterStates.WALK); //If isMoving is True and isRunning is false, the State will change to WALK
                    }
                    else if (isRunning)
                    {
                        SetPlayerState(CharacterStates.RUN); //If isMoving is True and isRunning is True, the State will change to RUN.
                    }
                    else if(runFidget)
                    {
                        SetPlayerState(CharacterStates.FIDGET);
                    }
                }
                else
                {
                    if (isJumping)
                    {
                        SetPlayerState(CharacterStates.IDLEJUMP);
                    }

                }

                if (isCasting)
                {
                    SetPlayerState(CharacterStates.CASTING); //If is Casting is True, the State will change to CASTING
                }
                break;

            case CharacterStates.WALK:
                if(!isMoving)
                {
                    SetPlayerState(CharacterStates.IDLE); //If isMoving is False, the State will change to IDLE
                }
                else if(isRunning)
                {
                    SetPlayerState(CharacterStates.RUN); //If isMoving is True and isRunning is True, the State will change to RUN.
                }
                break;

            case CharacterStates.RUN:
                if(!isMoving)
                {
                    SetPlayerState(CharacterStates.IDLE); //If isMoving is False then the State will change to IDLE
                }
                else if(!isRunning)
                {
                    SetPlayerState(CharacterStates.WALK); //If isMoving is True and isRunning is False, the State will change to WALK
                }
                break;

            case CharacterStates.CASTING:               
                if(secondsInState > 1.0f)
                {
                    SetPlayerState(CharacterStates.IDLE);
                }
                break;

            case CharacterStates.IDLEJUMP:
                //Put while loop/if statement here
                break;
        }
                
        PlayerMovement();

        secondsInState += Time.deltaTime;
    }

    void CheckInputData()
    {
        vertInput = Input.GetAxis(verticalInputName); //if the verticalInputName is pressed, vertInput becomes True.
        horizInput = Input.GetAxis(horizontalInputName); //if the horizontalInputName is pressed, horizInput becomes True.
        isMoving = vertInput != 0 || horizInput != 0;//if moving vertically or horizontally, set isWalking to true
        isRunning = Input.GetKey(runKey); //if the run key is pressed, then isRunning will be made True.
        isJumping = Input.GetKeyDown(jumpKey); // if the jump key is pressed and if isJumping is false, then isJumping will be true.
        isCasting = Input.GetKeyDown(castKey); // if the cast key is pressed and if isCassting is false, then isCasting will be true.
        runFidget = CheckStateTime(); //after X seconds in IDLE, runFidget will become true.
        
    }

    void SetPlayerState(CharacterStates newState)
    {
        if(newState != myPlayerState)
        {
            switch(myPlayerState)
            {
                case CharacterStates.IDLE:
                    break;
                case CharacterStates.FIDGET:
                    anim.SetBool("runFidget", false);
                    //Debug.Log("Running Fidget");
                    break;
                case CharacterStates.WALK:
                    anim.SetBool("isMoving", false);
                    //Debug.Log(isWalking);
                    break;
                case CharacterStates.RUN:
                    anim.SetBool("isMoving", false);
                    anim.SetBool("isRunning", false);
                    //Debug.Log(isRunning);
                    break;
                case CharacterStates.IDLEJUMP:
                    anim.SetBool("isJumping", false);
                    break;
                case CharacterStates.CASTING:
                    anim.SetBool("isCasting", false);
                    break;
            }

            myPlayerState = newState;
            secondsInState = 0.0f;

            switch (myPlayerState)
            {
                case CharacterStates.IDLE:
                    break;
                case CharacterStates.FIDGET:
                    Debug.Log("Running Fidget");
                    anim.SetBool("runFidget", true); //This is likely wrong.
                    break;
                case CharacterStates.WALK:
                    //Debug.Log(isWalking);
                    anim.SetBool("isMoving", true);
                    break;
                case CharacterStates.RUN:
                    anim.SetBool("isMoving", true);
                    anim.SetBool("isRunning", true);
                    break;
                case CharacterStates.IDLEJUMP:
                    anim.SetBool("isJumping", true);
                    anim.SetBool("isMoving", false);
                    StartCoroutine(JumpEvent());
                    break;
                case CharacterStates.CASTING:
                    anim.SetBool("isCasting", true);
                    break;
            }
        }
    }

    private void PlayerMovement()
    {
        //Debug.Log("Vert: " + vertInput + ", Horiz: " + horizInput);

        Vector3 fowardMovement = transform.forward * vertInput;
        Vector3 rightMovement = transform.right * horizInput;

        charController.SimpleMove(Vector3.ClampMagnitude(fowardMovement + rightMovement, 1.0f) * movementSpeed);

        if((vertInput != 0 || horizInput !=0) && OnSlope())
        {
            charController.Move(Vector3.down * charController.height / 2 * slopeForce * Time.deltaTime);
        }

        SetMovementSpeed();

        //JumpInput();
    }

    /*private bool CheckRunPress()
    {
        if (Input.GetKey(runKey))
        {
            isRunning = true;
            movementSpeed = Mathf.Lerp(movementSpeed, runSpeed, Time.deltaTime * runBuildUpSpeed);
        }
        else
        {
            isRunning = false;
            movementSpeed = Mathf.Lerp(movementSpeed, walkSpeed, Time.deltaTime * runBuildUpSpeed);
        }
        return isRunning;
    }
    */

    private void SetMovementSpeed() //if the run key is pressed the movement speed is increased
    {      
        if (isRunning)
        {
            movementSpeed = Mathf.Lerp(movementSpeed, runSpeed, Time.deltaTime * runBuildUpSpeed);
        }
        else
        {
            movementSpeed = Mathf.Lerp(movementSpeed, walkSpeed, Time.deltaTime * runBuildUpSpeed);
        }
    }

    private bool OnSlope()
    {
        if(isJumping)
        {
            return false;
        }

        RaycastHit hit;

        if(Physics.Raycast(transform.position, Vector3.down, out hit, charController.height/2 * slopeForceRayLength))
            if(hit.normal != Vector3.up)
            {
                return true;
            }
        return false;
    }

    private void JumpInput()
    {
        if(Input.GetKeyDown(KeyCode.Space))// && !isJumping)
        {
            Debug.Log("isJumping Value is " + isJumping.ToString());
            if (!isJumping)
            {
                isJumping = true;
                Debug.Log("Started jump");
                StartCoroutine(JumpEvent());
            }            
        }
    }

    private IEnumerator JumpEvent()
    {
        charController.slopeLimit = 90.0f;

        float timeInAir = 0.0f;

        do
        {
            float jumpForce = jumpFalloff.Evaluate(timeInAir);
            charController.Move(Vector3.up * jumpForce * jumpMultiplier * Time.deltaTime);
            timeInAir += Time.deltaTime;

            yield return null;
        }
        while (!charController.isGrounded && charController.collisionFlags != CollisionFlags.Above);
        Debug.Log("Jump has ended");
        charController.slopeLimit = 45.0f;
        isJumping = false;
    }

    public string GetStateName()
    {
        Debug.Log(myPlayerState.ToString());
        return myPlayerState.ToString();
    }

    private bool CheckStateTime()
    {
        if (secondsInState >= idleWaitTime)
        {
            runFidget = true;
        }
        else
        {
            runFidget = false;
        }
        return runFidget;
    }

    void CastSpell()
    {
        Debug.Log("MAGIC!!!!!");
    }
}
