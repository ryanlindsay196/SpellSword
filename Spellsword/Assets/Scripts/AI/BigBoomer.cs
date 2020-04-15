using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigBoomer : MonoBehaviour
{
    //What states does our enemy need to have?
    public enum AIState
    {
        None,
        Idle,
        Pursue
    }

    //Set AI State to null, just precautionary
    public AIState myAIState = AIState.None;

    //Our resident time tracker, could have multiple applications, found in update
    public float SecondsInCurrentState;

    //All Target GameObjects
    public static GameObject playerToKill;
    private RaycastHit hit;

    //All Position variables
    private Vector3 currPos;
    private Vector3 tarPos;
    public float speed;
    private Vector3 vectorToPlayer;
    private Vector3 raycastDir;

    //Enemy Range Values
    private float distanceToTarget;
    public float sightlineMax = 50.0f;

    //Start the Encounter by setting this = true
    private bool spottedHim = false;

    // Start is called before the first frame update
    void Start()
    {
        if (playerToKill == null)
        {
            playerToKill = GameObject.FindGameObjectWithTag("Player");
        }
        currPos = gameObject.transform.position;
        tarPos = playerToKill.transform.position;

        //Set our default initial state
        SetAIState(AIState.Idle);
    }

    // Update is called once per frame
    void Update()
    {
        SecondsInCurrentState += Time.deltaTime;

        //Updated Targeting information
        currPos = gameObject.transform.position;
        tarPos = playerToKill.transform.position;
        vectorToPlayer = tarPos - currPos;
        distanceToTarget = vectorToPlayer.magnitude;

        //Look forward
        gameObject.transform.forward = Vector3.RotateTowards(gameObject.transform.forward, vectorToPlayer, 15f * Time.deltaTime, float.PositiveInfinity);

        //Kill this thing
        if (gameObject.GetComponent<EnemyStats>().health <= 0)
        {
            foreach (Transform child in transform)
            {
                gameObject.GetComponent<SphereCollider>().enabled = false;
                Destroy(child.gameObject);
            }
        }

        if (Vector3.Distance(currPos, tarPos) < sightlineMax)
        {
            sightlineMax = 1000f;
            raycastDir = tarPos - currPos;

            //Can the enemy reasonably see the player?
            Physics.Raycast(currPos, raycastDir, out hit, sightlineMax);
            if (hit.transform != null && hit.transform.tag == "Player")
            {
                //Help us see the sightline when spotted = true, ***REMOVE LATER***
                Debug.DrawRay(currPos, raycastDir);

                spottedHim = true;
            }
            else
            {
                spottedHim = false;
            }
        }

        switch (myAIState)
        {
            case AIState.Idle:
                if (spottedHim)
                {
                    SetAIState(AIState.Pursue);
                }
                break;

            case AIState.Pursue:
                if(spottedHim == true)
                {
                    //gameObject.transform.position = Vector3.MoveTowards(currPos, tarPos, speed);
                    gameObject.transform.forward = Vector3.RotateTowards(gameObject.transform.forward, vectorToPlayer, 15f * Time.deltaTime, float.PositiveInfinity);
                    gameObject.GetComponent<Rigidbody>().AddForce(raycastDir * speed);
                }
                break;
        }
    }

    void SetAIState(AIState newState)
    {
        if (newState != myAIState)
        {
            //On Leave State

            myAIState = newState;
            SecondsInCurrentState = 0.0f;

            //On Enter State
            switch (myAIState)
            {
                case AIState.Idle:
                    break;
                case AIState.Pursue:
                    GetComponentInChildren<Animation>().Play();
                    break;
            }
        }
    }
}
