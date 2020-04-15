using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStateTracker : MonoBehaviour
{
    public Text stateText;
    public PlayerMove targetPlayer;
    string currentState;

    // Start is called before the first frame update
    void Start()
    {
        stateText = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        stateText.text = "PlayerState: " + PrintStateName().ToString();
    }

    string PrintStateName()
    {
        currentState = targetPlayer.GetStateName();
        /*if(targetPlayer.myPlayerState == PlayerMove.CharacterStates.NONE)
        {
            currentState = "None";
        */
        return currentState;
    }
}