using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AI_Weaver : MonoBehaviour
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
    private float SecondsInCurrentState;
    private float waitTime;

    //Our resident Fiend
    private GameObject Jeffery;
    private NavMeshAgent agent;
    public int enemyHealth;
    public float fireRate;

    //His Attack
    public GameObject spawn;
    public GameObject projectile;

    //All Target GameObjects
    private GameObject playerToKill;
    private RaycastHit hit;

    //All Position variables
    private Vector3 currPos;
    private Vector3 tarPos;
    private Vector3 lastPos;
    private Vector3 vectorToPosition;
    private Vector3 vectorToPlayer;
    private Vector3 raycastDir;

    //Enemy Range Values
    public float distanceToTarget;
    private float maxDistance = 20.0f;
    private float minDistance = 10.0f;
    public float sightlineMax = 50.0f;

    //Start the Encounter by setting this = true
    private bool spottedHim = false;
    private bool needToMove = false;
    private bool attackHim = false;

    void Start()
    {
        //Get Jeffery, get his position, get his target (the player)
        Jeffery = gameObject;
        if (playerToKill == null)
        {
            playerToKill = FindObjectOfType<CharacterMovement>().gameObject;
        }
        currPos = Jeffery.transform.position;
        tarPos = playerToKill.transform.position;

        //Get the agent information for the nav mesh integration
        agent = Jeffery.GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updatePosition = true;
        agent.autoBraking = true;

        //Set our default initial state
        SetAIState(AIState.Idle);
    }

    void Update()
    {
        SecondsInCurrentState += Time.deltaTime;

        currPos = Jeffery.transform.position;
        tarPos = playerToKill.transform.position;
        vectorToPosition = agent.destination - currPos;
        vectorToPlayer = tarPos - currPos;
        distanceToTarget = vectorToPlayer.magnitude;
        vectorToPosition.y = 0.001f;

        //Look towards where we are walking
        if (myAIState != AIState.Pursue)
        {
            Jeffery.transform.forward = Vector3.RotateTowards(Jeffery.transform.forward, vectorToPosition, 15f * Time.deltaTime, float.PositiveInfinity);
        }

        if (Vector3.Distance(currPos, tarPos) < sightlineMax)
        {
            sightlineMax = 1000f;
            raycastDir = tarPos - currPos;

            //Can the enemy reasonably see the player?
            Physics.Raycast(currPos, raycastDir, out hit, sightlineMax);
            if (hit.transform != null && hit.transform.tag == "Player" || hit.transform != null && hit.transform.tag == "targetFinder")
            {
                //Help us see the sightline when spotted = true, ***REMOVE LATER***
                Debug.DrawRay(currPos, raycastDir);

                spottedHim = true;
                lastPos = tarPos;
                needToMove = false;
            }
            else
            {
                needToMove = true;
                spottedHim = false;
            }
        }

        //How do we transition between states? What do we do inside of each state?
        switch (myAIState)
        {
            case AIState.Idle:
                agent.destination = currPos;

                if (spottedHim)
                {
                    SetAIState(AIState.Pursue);
                }
                break;

            case AIState.Pursue:
                Jeffery.transform.forward = Vector3.RotateTowards(Jeffery.transform.forward, playerToKill.transform.position - Jeffery.transform.position, 6.28f * Time.deltaTime, float.PositiveInfinity);
                //transform.LookAt(playerToKill.transform);
                //Set the agent to go to the player

                if (distanceToTarget > maxDistance)
                {
                    agent.destination = tarPos;
                }
                if (distanceToTarget > minDistance && distanceToTarget < maxDistance)
                {
                    agent.destination = currPos;
                    if (needToMove == false)
                    {
                        if (SecondsInCurrentState >= fireRate)
                        {
                            StartCoroutine("Attack");
                            if (attackHim == true)
                            {
                                GameObject bullet = Instantiate(projectile, spawn.transform.position, Quaternion.LookRotation(Jeffery.transform.forward, Jeffery.transform.up)) as GameObject;
                                projectile.GetComponent<Weaver_Fireball>().speed = 1.0f;
                                SecondsInCurrentState = 0;
                                attackHim = false;
                            }
                        }
                    }
                    else
                    {
                        agent.destination = Vector3.MoveTowards(currPos, lastPos, float.PositiveInfinity);
                    }
                }
                if (distanceToTarget < minDistance && needToMove == false)
                {
                    agent.destination += -vectorToPlayer;
                    if (SecondsInCurrentState >= (fireRate / 1.25))
                    {
                        StartCoroutine("Attack");
                        if (attackHim == true)
                        {
                            GameObject bullet = Instantiate(projectile, spawn.transform.position, Quaternion.LookRotation(Jeffery.transform.forward, Jeffery.transform.up)) as GameObject;
                            projectile.GetComponent<Weaver_Fireball>().speed = 0.5f;
                            SecondsInCurrentState = 0;
                            attackHim = false;
                        }
                    }
                }
                break;
        }
    }

    //Affect leaving and entering states, and things to do when that happens
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
                    gameObject.GetComponent<Animator>().SetBool("Idle", true);
                    gameObject.GetComponent<Animator>().SetBool("Attack", false);
                    gameObject.GetComponent<Animator>().SetBool("Walk", false);
                    break;
                case AIState.Pursue:
                    gameObject.GetComponent<Animator>().SetBool("Idle", false);
                    gameObject.GetComponent<Animator>().SetBool("Attack", false);
                    gameObject.GetComponent<Animator>().SetBool("Walk", true);
                    break;
            }
        }
    }

    IEnumerator Attack()
    {
        gameObject.GetComponent<Animator>().SetBool("Attack", true);
        yield return new WaitForSeconds(0.25f);
        attackHim = true;
        gameObject.GetComponent<Animator>().SetBool("Attack", false);
    }
}
