using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterMovement : MonoBehaviour
{
    [SerializeField]
    PlayerSoundManager playerSoundManager;
    #region player stats
    [SerializeField]
    float walkSpeed;
    [SerializeField]
    float runSpeed;
    [SerializeField]
    float turnSpeed;
    [SerializeField]
    float jumpForce;

    [SerializeField]
    float rotationSensitivityOnController;
    #endregion

    public bool tutorialTextActive;

    #region control adjustments
    [SerializeField][Tooltip("Minimum mouse movement speed to rotate the player.")]
    Vector2 rotationDeadZone;
    #endregion

    [SerializeField][Tooltip("Layer mask for the jump raycast")]
    LayerMask playerJumpLayerMask;

    [SerializeField]
    RectTransform pauseUI;

    #region components
    Rigidbody rigidBody;
    Camera mainCamera;
    [SerializeField]
    GameObject cameraHolder;
    public GameObject CameraHolder
    {
        get { return cameraHolder; }
    }
    #endregion

    #region Other scripts
    [SerializeField]
    RadialMenu radialMenu, spellRadialMenu, swordEnchantmentRadialMenu;
    #endregion

    [SerializeField]
    float autoAimSpeed, autoAimDuration;
    Transform autoAimTarget;

    // Start is called before the first frame update
    void Start()
    {
        //playerSoundManager = FindObjectOfType<PlayerSoundManager>();
        Cursor.lockState = CursorLockMode.Locked;
        rigidBody = GetComponent<Rigidbody>();

        mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if(autoAimTarget != null)
            Debug.Log("Aim assist target: " + autoAimTarget.name);
        float verticalAngle = mainCamera.transform.eulerAngles.x;
        float horizontalAngle = mainCamera.transform.eulerAngles.y;

        if(Input.GetButtonDown("Pause"))
        {
            Debug.Log("Trying to pause game");
            Time.timeScale = 0;
            pauseUI.gameObject.SetActive(true);
        }
        if (Time.timeScale == 0)
            return;
        if (!tutorialTextActive)
        {
            #region move player
            #region direction vectors (based on player rotation)
            Vector3 direction = new Vector3(Mathf.Cos(Mathf.Deg2Rad * verticalAngle) * Mathf.Sin(Mathf.Deg2Rad * horizontalAngle), Mathf.Sin(Mathf.Deg2Rad * verticalAngle), Mathf.Cos(Mathf.Deg2Rad * verticalAngle) * Mathf.Cos(Mathf.Deg2Rad * horizontalAngle));

            //Right vector
            Vector3 right = new Vector3(Mathf.Sin((Mathf.Deg2Rad * horizontalAngle) - 3.14f / 2.0f), 0, Mathf.Cos((Mathf.Deg2Rad * horizontalAngle) - 3.14f / 2.0f));

            //Up vector : perpendicular to both direction and right
            Vector3 up = Vector3.Cross(right, direction);
            #endregion

            Vector3 desiredDirection = (direction * Input.GetAxis("Vertical")) + (right * -Input.GetAxis("Horizontal"));
            #region Make the normalized velocity not affected by vertical velocity
            float tempFallSpeed = rigidBody.velocity.y;
            rigidBody.velocity = new Vector3(rigidBody.velocity.x, 0, rigidBody.velocity.z);
            rigidBody.velocity = desiredDirection.normalized * walkSpeed;
            rigidBody.velocity = new Vector3(rigidBody.velocity.x, tempFallSpeed, rigidBody.velocity.z);
            #endregion
            if ((new Vector3(rigidBody.velocity.x, 0, rigidBody.velocity.z)).magnitude > 0.2f && Mathf.Abs(rigidBody.velocity.y) <= 0.35f)
            {
                playerSoundManager.PlaySound(3, 1);
            }
            else if(new Vector3(rigidBody.velocity.x, 0, rigidBody.velocity.z).magnitude < 0.2f)
            {//if not walking or walking very slow
                playerSoundManager.StopSound(3, 1);//stop the player walking sound loop
            }
            GetComponent<Rigidbody>().AddForce(Vector3.down * 50);
            //check distance to ground
            float distanceToGround;
            RaycastHit hitInfo;
            //Physics.Raycast(transform.position, -Vector3.up, out hitInfo);
            Physics.Raycast(transform.position, -Vector3.up, out hitInfo, 50, playerJumpLayerMask);
            distanceToGround = hitInfo.distance;
            if (Input.GetButtonDown("Jump"))
            {
                if (distanceToGround <= 5.9f && distanceToGround != 0)//if close enough to ground, allow player to jump
                {
                    rigidBody.velocity = new Vector3(rigidBody.velocity.x, jumpForce, rigidBody.velocity.z);
                    playerSoundManager.StopSound(3, 1);//Stops the player walking sound loop
                }
                Debug.Log("CharacterMovement::Update()::distanceToGround = " + distanceToGround);
            }


            if (desiredDirection.magnitude == 0 && rigidBody.velocity.y == 0)
                GetComponent<Rigidbody>().velocity = new Vector3();
            #endregion
        }
        else
        {
            playerSoundManager.StopSound(3, 1);
            GetComponent<Rigidbody>().velocity = new Vector3();
            transform.position -= GetComponent<Rigidbody>().velocity;
        }
        if (!radialMenu.IsFannedOut && !tutorialTextActive)
            RotateView();
        else
            //radialMenu.UpdateSelectionPosition(new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")));
            radialMenu.UpdateSelectionPosition(new Vector2(RotationDirection().y, -RotationDirection().x));

        if (Input.GetButton("RadialMenu"))
        {
            radialMenu.IsFannedOut = true;
        }
        else if(radialMenu.IsFannedOut)
        {
            radialMenu.IsFannedOut = false;
            if (GetComponent<EquipmentManager>() != null && GetComponent<EquipmentManager>().GetCurrentEquipment.GetComponent<BookBehavior>() == null &&
                //spellRadialMenu.CurrentSpellIndex != -1)
                radialMenu.CurrentSpellIndex != -1 &&
                spellRadialMenu == radialMenu.SubRadialMenus[radialMenu.CurrentSpellIndex].menu)
            {//If currently holding the sword, and the spell radial menu is selected, switch to the book
                GetComponent<EquipmentManager>().SwitchEquipment();
            }
            else if (GetComponent<EquipmentManager>() != null && GetComponent<EquipmentManager>().GetCurrentEquipment.GetComponent<SwordBehavior>() == null &&
               radialMenu.CurrentSpellIndex != -1 &&
               swordEnchantmentRadialMenu == radialMenu.SubRadialMenus[radialMenu.CurrentSpellIndex].menu)
            {//If currently holding the book, and the sword enchantment radial menu is selected, switch to the sword
                GetComponent<EquipmentManager>().SwitchEquipment();
            }
        }
        //If switched to book and new spell selected, switch to new spell based off the spell radial menu's spell index
        if (GetComponent<EquipmentManager>().GetCurrentEquipment.GetComponent<BookBehavior>() != null && spellRadialMenu.CurrentSpellIndex != -1 &&
            spellRadialMenu.CurrentSpellIndex != GetComponent<EquipmentManager>().GetCurrentEquipment.GetComponent<BookBehavior>().CurrentPageIndex)
        {
            GetComponent<EquipmentManager>().GetCurrentEquipment.GetComponent<BookBehavior>().SwitchSpell(spellRadialMenu.CurrentSpellIndex);
            Invoke("PlayDelayedPageTurnSound", 0.6f);
        }

    }

    void PlayDelayedPageTurnSound()
    {
        playerSoundManager.PlaySound(7);
    }

    internal void SetAutoAimTarget(Transform in_AutoAimTarget)
    {
        autoAimTarget = in_AutoAimTarget;
        Invoke("ResetAutoAimTarget", autoAimDuration);
    }

    void ResetAutoAimTarget()
    {
        Debug.Log("CharacterMovement::ResetAutoAimTarget()");
        autoAimTarget = null;
    }

    void RotateView()
    {
        Vector2 desiredRotation = RotationDirection();

        float maxRotationSpeed = 1.1f;
        if (desiredRotation.magnitude > maxRotationSpeed)
            desiredRotation = desiredRotation.normalized * maxRotationSpeed;

        //cameraHolder.transform.Rotate(new Vector3(desiredRotation.x, desiredRotation.y, 0) * turnSpeed);
        cameraHolder.transform.localEulerAngles += new Vector3(desiredRotation.x, desiredRotation.y, 0) * turnSpeed;
        #region limit camera angle
        //limit how low/high the camera can look
        if (cameraHolder.transform.eulerAngles.x < 90)
            cameraHolder.transform.eulerAngles = new Vector3(Mathf.Clamp(cameraHolder.transform.eulerAngles.x, -40, 40), cameraHolder.transform.eulerAngles.y, cameraHolder.transform.eulerAngles.z);
        else if (cameraHolder.transform.eulerAngles.x > 320)
            cameraHolder.transform.eulerAngles = new Vector3(Mathf.Clamp(cameraHolder.transform.eulerAngles.x, 335, 390), cameraHolder.transform.eulerAngles.y, cameraHolder.transform.eulerAngles.z);
        //make the camera not tilt
        cameraHolder.transform.eulerAngles = new Vector3(cameraHolder.transform.eulerAngles.x, cameraHolder.transform.eulerAngles.y, 0);
        #endregion

        //TEST CODE RenderSettings.ambientLight = new Color(UnityEngine.Random.Range(0, 1), 1, 1);
        if (autoAimTarget != null)
        {
            Debug.Log("AutoAim Target: " + autoAimTarget.gameObject.name);
            //GetComponent<EquipmentManager>().PlayerAimAssist.RemoveNullTargets();
            Quaternion tempCameraHolderRotation = cameraHolder.transform.rotation;
            cameraHolder.transform.LookAt(autoAimTarget.GetComponent<Targetable>().TargetTransform);

            cameraHolder.transform.rotation = Quaternion.Lerp(tempCameraHolderRotation, cameraHolder.transform.rotation, autoAimSpeed);
        }
    }

    public Vector2 RotationDirection()
    {
        //return new Vector2(-Input.GetAxis("Mouse Y") - (Input.GetAxis("Look Y") * rotationSensitivityOnController), Input.GetAxis("Mouse X") + Input.GetAxis("Look X") * rotationSensitivityOnController);
        return new Vector2(-Input.GetAxis("Mouse Y") - (Input.GetAxis("Look Y") * rotationSensitivityOnController), Input.GetAxis("Mouse X") + Input.GetAxis("Look X") * rotationSensitivityOnController);
    }
}
