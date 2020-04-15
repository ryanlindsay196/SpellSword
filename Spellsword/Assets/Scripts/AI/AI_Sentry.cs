using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Sentry : MonoBehaviour
{
    public enum AIState
    {
        None,
        Idle,
        /*LookLeft,
        LookRight,*/
        Attack
    }

    //Set AI State to null, just precautionary
    public AIState myAIState = AIState.None;

    //Our resident time tracker, could have multiple applications, found in update
    private float SecondsInCurrentState;
    public float fireRate;

    //All Target GameObjects
    public GameObject playerToKill;
    private RaycastHit hit;
    //public Transform[] lookHere;
    public GameObject projectile;
    public GameObject spawn;

    //Our direction variables
   /* private Vector3 toPoint1;
    private Quaternion point1Rotation;
    private Vector3 toPoint2;
    private Quaternion point2Rotation;*/
    private Vector3 targetDir;
    private float tarDistance;

    //Distance variable
    private Vector3 tarPos;

    //Can we see him?
    private bool spottedHim = false;
    public bool enraged = false;
    private bool lostHim = false;

    //Audio Sounds
    public AudioSource numberOne;
    public AudioSource numberTwo;
    public AudioClip plantDeath;

    // Start is called before the first frame update
    void Start()
    {
        spottedHim = false;

        if (playerToKill == null)
        {
            playerToKill = FindObjectOfType<CharacterMovement>().gameObject;
        }

        tarPos = playerToKill.transform.position;

        //Set our default initial state
        SetAIState(AIState.Idle);

        /*toPoint1 = lookHere[0].position - gameObject.transform.position;
        toPoint1.y = gameObject.transform.forward.y;
        point1Rotation = Quaternion.Euler(toPoint1);
        toPoint2 = lookHere[1].position - gameObject.transform.position;
        toPoint2.y = gameObject.transform.forward.y;
        point2Rotation = Quaternion.Euler(toPoint2);*/
    }

    // Update is called once per frame
    void Update()
    {
        if (gameObject.GetComponent<EnemyStats>().health <= 0 && numberTwo.clip != plantDeath)
        {
            //gameObject.GetComponent<AudioSource>().loop = false;
            numberOne.enabled = false;
            numberTwo.clip = plantDeath;
            numberTwo.enabled = true;
            Debug.Log("No way");
        }

        tarPos = new Vector3(playerToKill.transform.position.x, playerToKill.transform.position.y - 1, playerToKill.transform.position.z);

        tarDistance = Vector3.Distance(gameObject.transform.position, tarPos);

        targetDir = tarPos - gameObject.transform.position;
        SecondsInCurrentState += Time.deltaTime;
        //spottedHim = false;

        Debug.DrawRay(spawn.transform.position, targetDir);

        if (Vector3.Distance(spawn.transform.position, tarPos) < 25.0f || enraged == true)
        {
            Physics.Raycast(gameObject.transform.position + 2 * transform.up, targetDir, out hit, 25.0f);

            if (hit.transform != null && hit.transform.tag == "Player" || hit.transform != null && hit.transform.tag == "targetFinder")
            {
                //Recognize that we have found the player
                spottedHim = true;
                enraged = true;
            }
            else
            {
                if (spottedHim == true)
                {
                    //Debug.Log("We can't see the player");
                    spottedHim = false;
                    StartCoroutine(WeLostHim());
                }
            }

            if (spottedHim == false && lostHim == true)
            {
                //Debug.Log("I should've stopped shooting");
                enraged = false;
                SetAIState(AIState.Idle);
                lostHim = false;
            }
        }

        switch (myAIState)
        {
            /*case AIState.LookLeft:
                gameObject.transform.forward = Vector3.RotateTowards(gameObject.transform.forward, toPoint1, 1.0f * Time.deltaTime, float.PositiveInfinity);
                if (SecondsInCurrentState >= 5)
                {
                    SetAIState(AIState.LookRight);
                }
                if(spottedHim == true)
                {
                    SetAIState(AIState.Attack);
                }
                break;
            case AIState.LookRight:
                gameObject.transform.forward = Vector3.RotateTowards(gameObject.transform.forward, toPoint2, 1.0f * Time.deltaTime, float.PositiveInfinity);
                if(SecondsInCurrentState >= 5)
                {
                    SetAIState(AIState.LookLeft);
                }
                if (spottedHim == true)
                {
                    SetAIState(AIState.Attack);
                }
                break;*/
            case AIState.Idle:
                if (enraged == true && spottedHim == true)
                {
                    SetAIState(AIState.Attack);
                }
                break;
            case AIState.Attack:
                gameObject.transform.forward = Vector3.RotateTowards(gameObject.transform.forward, targetDir, 1.3f * Time.deltaTime, float.PositiveInfinity);
                if(SecondsInCurrentState >= fireRate/2)
                {
                    gameObject.GetComponent<Animator>().SetBool("Attack", true);
                }
                if (SecondsInCurrentState >= fireRate)
                {
                    GameObject bullet = Instantiate(projectile, spawn.transform.position, Quaternion.identity) as GameObject;
                    bullet.GetComponent<Rigidbody>().AddForce(transform.forward * (tarDistance * 50));
                    SecondsInCurrentState = 0;
                    gameObject.GetComponent<AudioSource>().Play();
                    gameObject.GetComponent<Animator>().SetBool("Attack", false);
                    Destroy(bullet, 3.0f);
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
                /*case AIState.LookLeft:
                    break;
                case AIState.LookRight:
                    break;*/
                case AIState.Idle:
                    gameObject.GetComponent<Animator>().SetBool("Attack", false);
                    break;
                case AIState.Attack:
                    //gameObject.GetComponent<Animator>().SetBool("Attack", true);
                    break;
            }
        }
    }
    IEnumerator WeLostHim()
    {
        yield return new WaitForSeconds(2);
        lostHim = true;
    }
}
