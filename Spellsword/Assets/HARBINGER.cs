using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class HARBINGER : MonoBehaviour
{
    //What states does our enemy need to have?
    public enum AIState
    {
        None,
        Idle,
        Immune,
        Damage
    }

    //Set AI State to null, just precautionary
    public AIState myAIState = AIState.None;

    //Our resident time tracker, could have multiple applications, found in update
    private float SecondsInCurrentState;
    private float waitTime;

    //Our resident Fiend
    private GameObject Jeffery;
    public float fireRate;

    //His Attack
    public GameObject spawn;
    public GameObject skull;
    private int phaseNum = 0;
    public GameObject portal1;
    public GameObject portal2;
    public GameObject portal3;
    public GameObject protectBubble;

    //All Target GameObjects
    private GameObject playerToKill;
    private RaycastHit hit;

    //All Position variables
    private Vector3 currPos;
    private Vector3 tarPos;
    private Vector3 lastPos;
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

        //Set our default initial state
        SetAIState(AIState.Immune);
    }

    void Update()
    {
        SecondsInCurrentState += Time.deltaTime;

        currPos = Jeffery.transform.position;
        tarPos = playerToKill.transform.position;
        vectorToPlayer = tarPos - currPos;
        vectorToPlayer.y = vectorToPlayer.y - 4;
        distanceToTarget = vectorToPlayer.magnitude;

        //Look towards the player


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
                break;
            case AIState.Immune:
                Jeffery.transform.forward = Vector3.RotateTowards(Jeffery.transform.forward, vectorToPlayer, 6.28f * Time.deltaTime, float.PositiveInfinity);

                if (SecondsInCurrentState >= fireRate)
                {
                    StartCoroutine("Attack");
                    if (SecondsInCurrentState >= fireRate + 1)
                    {
                        GameObject bullet = Instantiate(skull, spawn.transform.position, Quaternion.LookRotation(Jeffery.transform.forward, Jeffery.transform.up)) as GameObject;
                        SecondsInCurrentState = 0;
                    }
                }

                if(phaseNum == 1 && portal1.GetComponentInChildren<BossSpawner>().maxEnemies == 0)
                {
                    SetAIState(AIState.Damage);
                }

                if (phaseNum == 2 && portal1.GetComponentInChildren<BossSpawner>().maxEnemies == 0 && portal2.GetComponentInChildren<BossSpawner>().maxEnemies == 0)
                {
                    SetAIState(AIState.Damage);
                }

                if (phaseNum >= 3 && portal1.GetComponentInChildren<BossSpawner>().maxEnemies == 0 && portal2.GetComponentInChildren<BossSpawner>().maxEnemies == 0 && portal3.GetComponentInChildren<BossSpawner>().maxEnemies == 0)
                {
                    SetAIState(AIState.Damage);
                }

                break;
            case AIState.Damage:
                if(SecondsInCurrentState > 10)
                {
                    SetAIState(AIState.Immune);
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
                    break;
                case AIState.Immune:
                    gameObject.GetComponent<BoxCollider>().enabled = false;
                    protectBubble.SetActive(true);
                    gameObject.GetComponent<Animator>().SetBool("Stun", false);
                    phaseNum++;
                    if (phaseNum == 1)
                    {
                        portal1.GetComponentInChildren<BossSpawner>().maxEnemies = 3;
                        portal1.SetActive(true);
                    }
                    if (phaseNum == 2)
                    {
                        portal1.GetComponentInChildren<BossSpawner>().maxEnemies = 2;
                        portal2.GetComponentInChildren<BossSpawner>().maxEnemies = 2;
                        portal1.SetActive(true);
                        portal2.SetActive(true);
                    }
                    if (phaseNum >= 3)
                    {
                        portal1.GetComponentInChildren<BossSpawner>().maxEnemies = 1;
                        portal2.GetComponentInChildren<BossSpawner>().maxEnemies = 1;
                        portal3.GetComponentInChildren<BossSpawner>().maxEnemies = 3;
                        portal1.SetActive(true);
                        portal2.SetActive(true);
                        portal3.SetActive(true);
                    }
                    break;
                case AIState.Damage:
                    gameObject.GetComponent<BoxCollider>().enabled = true;
                    protectBubble.SetActive(false);
                    gameObject.GetComponent<Animator>().SetBool("Stun", true);
                    break;
            }
        }
    }

    IEnumerator Attack()
    {
        gameObject.GetComponent<Animator>().SetBool("Cast", true);
        yield return new WaitForSeconds(1.5f);
        gameObject.GetComponent<Animator>().SetBool("Cast", false);
    }
}
