using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AI_Brawler_NoPatrol : MonoBehaviour
{
    //What states does our enemy need to have?
    public enum AIState
    {
        None,
        //Patrol,
        Look,
        Pursue,
        Attack,
        Confused
    }

    //Set AI State to null, just precautionary
    public AIState myAIState = AIState.None;

    //Our resident time tracker, could have multiple applications, found in update
    public float SecondsInCurrentState;
    private float lostThisFool;
    private float waitTime;

    //Audio Sounds
    public AudioSource numberOne;
    public AudioSource numberTwo;
    public AudioClip ghoulDeath;
    public AudioClip ghoulWalk;
    public AudioClip ghoulRun;

    //Our resident Fiend
    private GameObject Jeffery;
    private NavMeshAgent agent;

    //All Target GameObjects
    private GameObject playerToKill;
    public bool spottedHim = false;
    public List<Transform> patrolRoute;
    private RaycastHit hit;
    
    //All Position variables
    private Vector3 currPos;
    private Vector3 tarPos;
    private Vector3 lastPos;
    private Vector3 targetPosition;
    private Vector3 raycastDir;
    private Vector3 targetDir;
    private int destPoint;
    private int routeNum;
    private int oldestRoute;
    public float attackRange;

    public float updateThis;

    /*
    void Awake()
    {
        patrolRoute.Clear();
        foreach (GameObject patrolpoint in GameObject.FindGameObjectsWithTag("PatrolPoint"))
        {
            patrolRoute.Add(patrolpoint.transform);
        }
    }
    */
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
        agent.updateRotation = true;
        agent.updatePosition = true;
        agent.autoBraking = true;

        //Set Initial Values
        destPoint = 0;
        routeNum = 0;
        oldestRoute = 0;

        //Set our default initial state
        SetAIState(AIState.Look);
    }

    //Patrolling Function
    void GotoNextPoint()
    {
        // Returns if no points have been set up
        if (patrolRoute.Count == 0)
            return;

        // Set the agent to go to the currently selected destination.
        agent.destination = patrolRoute[destPoint].position;

        // Choose the next point in the array as the destination,
        // cycling to the start if necessary.
        destPoint = (destPoint + 1) % patrolRoute.Count;   
    }

    void Update()
    {
        updateThis = agent.remainingDistance;

        if (gameObject.GetComponent<EnemyStats>().health <= 0 && numberTwo.clip != ghoulDeath)
        {
            //gameObject.GetComponent<AudioSource>().loop = false;
            numberOne.enabled = false;
            numberTwo.clip = ghoulDeath;
            numberTwo.enabled = true;
        }

        Debug.DrawRay(currPos, transform.forward); //REMOVE this later
        SecondsInCurrentState += Time.deltaTime;

        currPos = Jeffery.transform.position;
        tarPos = playerToKill.transform.position;
        targetPosition = agent.destination - currPos;
        targetPosition.y = 0.01f;
        targetDir = tarPos - gameObject.transform.position;

        //Look towards where we are walking
        //Jeffery.transform.forward = Vector3.RotateTowards(Jeffery.transform.forward, targetPosition, 6.28f * Time.deltaTime, float.PositiveInfinity);

        //Is the player in range of the enemy? Set spotted to false, and check if we can see
        if (Vector3.Distance(gameObject.transform.position, tarPos) < 50.0f)
        {
            Physics.Raycast(gameObject.transform.position + 2 * transform.up, targetDir, out hit, 50.0f);

            if (hit.transform != null && hit.transform.tag == "Player" || hit.transform != null && hit.transform.tag == "targetFinder")
            {
                //Recognize that we have found the player
                spottedHim = true;
            }
            else
            {
                if (spottedHim == true)
                {
                    spottedHim = false;
                }
                lostThisFool += Time.deltaTime;
            }

            if (spottedHim == false && lostThisFool >= 2.0f)
            {
                SetAIState(AIState.Look);
                lostThisFool = 0.0f;
            }
        }

        //How do we transition between states? What do we do inside of each state?
        switch (myAIState)
        {
            /*case AIState.Patrol:
                // Choose the next destination point when the agent gets
                // close to the current one.
                waitTime = Random.Range(1.0f, 3.0f);

                if (!agent.pathPending && agent.remainingDistance < 2.0f)
                {
                    SetAIState(AIState.Look);
                }

                if (spottedHim)
                {
                    SetAIState(AIState.Pursue);
                }

                break;*/
            case AIState.Look:
                /*if(SecondsInCurrentState > waitTime)
                {
                    GotoNextPoint();
                    SetAIState(AIState.Patrol);
                }*/

                if (spottedHim)
                {
                    SetAIState(AIState.Pursue);
                }

                break;
            case AIState.Pursue:
                if (spottedHim && agent.remainingDistance > attackRange)
                {
                    //Set the agent to go to the player
                    agent.destination = tarPos;

                    //transform.LookAt(playerToKill.transform);
                    Jeffery.transform.forward = Vector3.RotateTowards(Jeffery.transform.forward, targetPosition, 6.28f * Time.deltaTime, float.PositiveInfinity);

                    //Remember where we last saw the player, different from the player's
                    //current position.
                    lastPos = tarPos;
                }
                else if (spottedHim && agent.remainingDistance < attackRange)
                {
                    //transform.LookAt(playerToKill.transform);
                    Jeffery.transform.forward = Vector3.RotateTowards(Jeffery.transform.forward, targetPosition, 6.28f * Time.deltaTime, float.PositiveInfinity);

                    //Set the agent to go to the player
                    agent.destination = tarPos;

                    SetAIState(AIState.Attack);
                }
                else
                {
                    agent.destination = lastPos;
                    if ((!agent.pathPending && agent.remainingDistance < 1.0f) &&
                    !spottedHim)
                    {
                        SetAIState(AIState.Confused);
                    }
                }

                break;
            case AIState.Attack:
                //gameObject.GetComponent<HitBox>().enabled = true;
                agent.destination = currPos;
                if (SecondsInCurrentState >= 0.4f)
                {
                    SetAIState(AIState.Pursue);
                }
                break;
            case AIState.Confused:
                if(spottedHim)
                {
                    SetAIState(AIState.Pursue);
                }
                else if(SecondsInCurrentState > 1.5f)
                {
                    SetAIState(AIState.Look);

                    /*//Increase the number of our next patrol point
                    routeNum++;
                    int indexSpot = 0;

                    //Create a new patrol point based off of Player's last known location
                    GameObject newPatrolPoint = new GameObject("KnownPlayerPos" + routeNum);
                    newPatrolPoint.transform.position = lastPos;

                    //Check newPatrolPoint against the rest of the patrol points and find the closest
                    Transform tMin = null;
                    float minDist = Mathf.Infinity;
                    foreach (Transform t in patrolRoute)
                    {
                        float dist = Vector3.Distance(t.position, newPatrolPoint.transform.position);
                        if (dist < minDist)
                        {
                            tMin = t;
                            minDist = dist;
                        }
                    }
                    indexSpot = patrolRoute.IndexOf(tMin);
                    indexSpot++;

                    //if closest point is further than 2 meters from the last point, add it to the patrolRoute, otherwise, ignore it
                    if (minDist > 2)
                    {
                        patrolRoute.Insert(indexSpot, newPatrolPoint.transform);
                    }
                    else
                    {
                        Destroy(newPatrolPoint);
                        routeNum--;
                    }

                    //Limit Patrol Size
                    if(patrolRoute.Count > 6)
                    {
                        oldestRoute++;
                        for (int i = 0; i < patrolRoute.Count; i++)
                        {
                            if(patrolRoute[i] == GameObject.Find("KnownPlayerPos" + oldestRoute).transform)
                            {
                                patrolRoute.Remove(GameObject.Find("KnownPlayerPos" + oldestRoute).transform);
                                Destroy(GameObject.Find("KnownPlayerPos" + oldestRoute));
                            }
                        }
                    }*/
                }
                break;
        }
    }

    //Affect leaving and entering states, and things to do when that happens
    void SetAIState(AIState newState)
    {
        if(newState != myAIState)
        {
            //On Leave State

            gameObject.GetComponent<NavMeshAgent>().speed = 1.5f;
            gameObject.GetComponent<NavMeshAgent>().acceleration = 4.0f;
            gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            //gameObject.GetComponent<HitBox>().enabled = false;
            myAIState = newState;
            SecondsInCurrentState = 0.0f;

            //On Enter State
            switch (myAIState)
            {
                /*case AIState.Patrol:
                    gameObject.GetComponent<HitBox>().enabled = false;
                    gameObject.GetComponent<Animator>().SetBool("Look", false);
                    gameObject.GetComponent<Animator>().SetBool("Patrol", true);
                    gameObject.GetComponent<Animator>().SetBool("Confused", false);
                    gameObject.GetComponent<AudioSource>().clip = ghoulWalk;
                    gameObject.GetComponent<AudioSource>().Play();
                    break;*/
                case AIState.Look:
                    //gameObject.GetComponent<HitBox>().enabled = false;
                    gameObject.GetComponent<Animator>().SetBool("Look", true);
                    gameObject.GetComponent<Animator>().SetBool("Patrol", false);
                    //gameObject.GetComponent<AudioSource>().Pause();
                    break;
                case AIState.Pursue:
                    agent.destination = tarPos;
                    Jeffery.transform.forward = Vector3.RotateTowards(Jeffery.transform.forward, targetPosition, 9.51f * Time.deltaTime, float.PositiveInfinity);
                    gameObject.GetComponent<NavMeshAgent>().speed = 10.0f;
                    gameObject.GetComponent<NavMeshAgent>().acceleration = 10.0f;
                    gameObject.GetComponent<Animator>().SetBool("Pursue", true);
                    gameObject.GetComponent<Animator>().SetBool("Patrol", false);
                    gameObject.GetComponent<Animator>().SetBool("Look", false);
                    //gameObject.GetComponent<AudioSource>().clip = ghoulRun;
                    //gameObject.GetComponent<AudioSource>().Play();
                    //gameObject.GetComponent<HitBox>().enabled = false;
                    break;
                case AIState.Attack:
                    Debug.Log("Hey Batter Batter");
                    /*gameObject.GetComponent<Animator>().SetBool("Pursue", false);
                    gameObject.GetComponent<Animator>().SetBool("Patrol", false);
                    gameObject.GetComponent<Animator>().SetBool("Look", false);*/
                    //gameObject.GetComponent<HitBox>().enabled = true;
                    break;
                case AIState.Confused:
                    //gameObject.GetComponent<HitBox>().enabled = false;
                    gameObject.GetComponent<Animator>().SetBool("Pursue", false);
                    gameObject.GetComponent<Animator>().SetBool("Patrol", false);
                    gameObject.GetComponent<Animator>().SetBool("Look", false);
                    gameObject.GetComponent<Animator>().SetBool("Confused", true);
                    //gameObject.GetComponent<AudioSource>().Pause();
                    agent.destination = currPos;
                    break;
            }
        }
    }
}
