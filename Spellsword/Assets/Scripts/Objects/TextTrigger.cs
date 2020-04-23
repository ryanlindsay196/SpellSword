using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextTrigger : MonoBehaviour
{
    SaveObelisk.ControllerTypes controllerType;
    [SerializeField]
    TextMeshProUGUI displayControllerText, displayKeyboardText, interactControllerText, interactKeyboardText;

    CharacterMovement characterMovement;

    string placeholderText;
    // Start is called before the first frame update
    void Start()
    {
        controllerType = SaveObelisk.ControllerTypes.controller;
    }

    // Update is called once per frame
    void Update()
    {
        if(GetComponent<SavableObject>().GetState() >= 1 && !characterMovement.tutorialTextActive)
        {
            //gameObject.SetActive(false);
            characterMovement.tutorialTextActive = false;
        }
        if (Input.GetKey(KeyCode.JoystickButton0) || Input.GetKey(KeyCode.JoystickButton1) || Input.GetKey(KeyCode.JoystickButton2) ||
            Input.GetKey(KeyCode.JoystickButton3) || Input.GetKey(KeyCode.JoystickButton4) || Input.GetKey(KeyCode.JoystickButton5) ||
            Input.GetKey(KeyCode.JoystickButton6) || Input.GetKey(KeyCode.JoystickButton7) || Input.GetKey(KeyCode.JoystickButton8) ||
            Input.GetKey(KeyCode.JoystickButton9) || Input.GetKey(KeyCode.JoystickButton10) || Input.GetKey(KeyCode.JoystickButton11) ||
            Input.GetKey(KeyCode.JoystickButton12) || Input.GetKey(KeyCode.JoystickButton13) || Input.GetKey(KeyCode.JoystickButton14) ||
            Input.GetKey(KeyCode.JoystickButton15) || Input.GetKey(KeyCode.JoystickButton16) || Input.GetKey(KeyCode.JoystickButton17) ||
            Input.GetKey(KeyCode.JoystickButton18) || Input.GetKey(KeyCode.JoystickButton19))
        {
            controllerType = SaveObelisk.ControllerTypes.controller;
            Debug.Log("Controller type: Controller");
            if (characterMovement != null && Input.GetButtonDown("Interact"))
            {
                Interact();
            }
        }
        else if (Input.anyKeyDown)
        {
            controllerType = SaveObelisk.ControllerTypes.keyboard;
            Debug.Log("Controller type: Keyboard");
            if (characterMovement != null && Input.GetButtonDown("Interact"))
            {
                Interact();
            }
        }

    }

    void Interact()
    {///determine whether to activate or deactivate text box

        Debug.Log("Text trigger interacted: " + controllerType.ToString());
        if (characterMovement.tutorialTextActive)
        {
            characterMovement.tutorialTextActive = false;
            displayControllerText.gameObject.SetActive(false);
            displayKeyboardText.gameObject.SetActive(false);
            if(controllerType == SaveObelisk.ControllerTypes.controller)
                interactControllerText.gameObject.SetActive(true);
            else if(controllerType == SaveObelisk.ControllerTypes.keyboard)
                interactKeyboardText.gameObject.SetActive(true);
            //gameObject.SetActive(false);
        }
        else
            ActivateText();
    }

    void ActivateText()
    {
        if (characterMovement != null)//Make character not move
        {
            characterMovement.tutorialTextActive = true;
            interactControllerText.gameObject.SetActive(false);
            interactKeyboardText.gameObject.SetActive(false);

            //Debug.Log("Text trigger interacted: " + controllerType.ToString());
            if (controllerType == SaveObelisk.ControllerTypes.controller)
            {
                displayControllerText.gameObject.SetActive(true);
            }
            else if(controllerType == SaveObelisk.ControllerTypes.keyboard)
            {
                displayKeyboardText.gameObject.SetActive(true);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.GetComponent<CharacterMovement>() != null)
        {
            Debug.Log("TextTrigger::OnTriggerEnter(Collider)");
            //DISPLAY TEXT
            if (controllerType == SaveObelisk.ControllerTypes.controller)
            {
                interactControllerText.gameObject.SetActive(true);
            }
            else if (controllerType == SaveObelisk.ControllerTypes.keyboard)
            {
                interactKeyboardText.gameObject.SetActive(true);
            }
            characterMovement = other.gameObject.GetComponent<CharacterMovement>();

        }
    }
    private void OnTriggerExit(Collider other)
    {
        interactControllerText.gameObject.SetActive(false);
        interactKeyboardText.gameObject.SetActive(false);
    }
}
