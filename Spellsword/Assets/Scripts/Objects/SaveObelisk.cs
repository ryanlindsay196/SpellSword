using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SaveObelisk : MonoBehaviour
{
    [SerializeField]
    ParticleSystem[] playerInRangeParticles, savingParticles;
    [SerializeField]
    Transform respawnTransform;
    [SerializeField]
    TextMeshProUGUI saveText;

    public enum ControllerTypes { keyboard, controller };
    ControllerTypes controllerType;

    bool playerInRange;
    public bool PlayerInRange
    { 
        get { return playerInRange; }
        set { playerInRange = value; saveText.gameObject.SetActive(value); ActivateInRangeParticles(playerInRange); }
    }

    public Transform RespawnTransform
    {
        get { return respawnTransform; }
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(Input.inputString);
        if(Input.anyKeyDown)
        {
            controllerType = ControllerTypes.keyboard;
        }
        else if(Input.GetKey(KeyCode.JoystickButton0) || Input.GetKey(KeyCode.JoystickButton1) || Input.GetKey(KeyCode.JoystickButton2) ||
            Input.GetKey(KeyCode.JoystickButton3) || Input.GetKey(KeyCode.JoystickButton4) || Input.GetKey(KeyCode.JoystickButton5) ||
            Input.GetKey(KeyCode.JoystickButton6) || Input.GetKey(KeyCode.JoystickButton7) || Input.GetKey(KeyCode.JoystickButton8) ||
            Input.GetKey(KeyCode.JoystickButton9) || Input.GetKey(KeyCode.JoystickButton10) || Input.GetKey(KeyCode.JoystickButton11) ||
            Input.GetKey(KeyCode.JoystickButton12) || Input.GetKey(KeyCode.JoystickButton13) || Input.GetKey(KeyCode.JoystickButton14) ||
            Input.GetKey(KeyCode.JoystickButton15) || Input.GetKey(KeyCode.JoystickButton16) || Input.GetKey(KeyCode.JoystickButton17) ||
            Input.GetKey(KeyCode.JoystickButton18) || Input.GetKey(KeyCode.JoystickButton19))
        {
            controllerType = ControllerTypes.controller;
        }
        if(controllerType == ControllerTypes.controller)
        {
            saveText.text = "Press X to Save";
        }
        else if(controllerType == ControllerTypes.keyboard)
        {
            saveText.text = "Press Q to Save";
        }
    }

    void ActivateInRangeParticles(bool turnParticlesOn)
    {
        for(int i = 0; i < playerInRangeParticles.Length; i++)
        {
            playerInRangeParticles[i].gameObject.SetActive(turnParticlesOn);
        }
    }

    public void ActivateSaveParticles()
    {
        for(int i = 0; i < savingParticles.Length; i++)
        {
            savingParticles[i].gameObject.SetActive(true);
        }
        Invoke("DeactivateSaveParticles", 3);
    }

    void DeactivateSaveParticles()
    {
        for(int i = 0; i < savingParticles.Length; i++)
        {
            savingParticles[i].gameObject.SetActive(false);
        }
        ActivateInRangeParticles(false);
    }

}
