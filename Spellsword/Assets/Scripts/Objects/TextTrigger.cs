using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextTrigger : MonoBehaviour
{
    SaveObelisk.ControllerTypes controllerType;
    [SerializeField]
    TextMeshProUGUI displayControllerText, displayKeyboardText;

    CharacterMovement characterMovement;

    string placeholderText;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(GetComponent<SavableObject>().GetState() >= 1 && !characterMovement.tutorialTextActive)
        {
            gameObject.SetActive(false);
            characterMovement.tutorialTextActive = false;
        }
        if (Input.anyKeyDown)
        {
            controllerType = SaveObelisk.ControllerTypes.keyboard;
            if (characterMovement != null && Input.GetButton("Interact"))
            {
                characterMovement.tutorialTextActive = false;
                gameObject.SetActive(false);
            }
        }
        else if (Input.GetKey(KeyCode.JoystickButton0) || Input.GetKey(KeyCode.JoystickButton1) || Input.GetKey(KeyCode.JoystickButton2) ||
            Input.GetKey(KeyCode.JoystickButton3) || Input.GetKey(KeyCode.JoystickButton4) || Input.GetKey(KeyCode.JoystickButton5) ||
            Input.GetKey(KeyCode.JoystickButton6) || Input.GetKey(KeyCode.JoystickButton7) || Input.GetKey(KeyCode.JoystickButton8) ||
            Input.GetKey(KeyCode.JoystickButton9) || Input.GetKey(KeyCode.JoystickButton10) || Input.GetKey(KeyCode.JoystickButton11) ||
            Input.GetKey(KeyCode.JoystickButton12) || Input.GetKey(KeyCode.JoystickButton13) || Input.GetKey(KeyCode.JoystickButton14) ||
            Input.GetKey(KeyCode.JoystickButton15) || Input.GetKey(KeyCode.JoystickButton16) || Input.GetKey(KeyCode.JoystickButton17) ||
            Input.GetKey(KeyCode.JoystickButton18) || Input.GetKey(KeyCode.JoystickButton19))
        {
            controllerType = SaveObelisk.ControllerTypes.controller;
            if (characterMovement != null && Input.GetButton("Interact"))
            {
                characterMovement.tutorialTextActive = false;
                gameObject.SetActive(false);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.GetComponent<CharacterMovement>() != null)
        {
            Debug.Log("TextTrigger::OnTriggerEnter(Collider)");
            GetComponent<SavableObject>().UpdateObjectState(1);
            //DISPLAY TEXT
            if (controllerType == SaveObelisk.ControllerTypes.controller)
            {
                displayControllerText.gameObject.SetActive(true);
            }
            else if (controllerType == SaveObelisk.ControllerTypes.keyboard)
            {
                displayKeyboardText.gameObject.SetActive(true);
            }

            //Make character not move
            characterMovement = other.gameObject.GetComponent<CharacterMovement>();
            characterMovement.tutorialTextActive = true;
        }
    }
}
