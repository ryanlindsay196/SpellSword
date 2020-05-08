using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WhenHarbingerDie : MonoBehaviour
{
    GameObject teleportThisGuy;
    //public GameObject portalTurnOff;
    //public GameObject portalLocation;
    public GameObject weaver;
    public bool WeaverDead = false;
    //public List<GameObject> turnOn;
    //public List<GameObject> turnOff;
    public int creditSceneIndex;

    /*
    void Awake()
    {
        foreach (GameObject tunnel in turnOn)
        {
            tunnel.SetActive(false);
        }
    }
    */
    // Start is called before the first frame update
    void Start()
    {
        teleportThisGuy = FindObjectOfType<CharacterMovement>().gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (weaver.GetComponent<EnemyStats>().health > 0)
        {
            WeaverDead = false;
        }
        else
        {
            WeaverDead = true;
        }

        if(weaver == null || WeaverDead == true)
        {
            /*
            foreach (GameObject enemy in portalTurnOff.GetComponentInChildren<SpawnMore>().enemies)
            {
                enemy.SetActive(false);
            }
            portalTurnOff.SetActive(false);
            foreach (GameObject floor in turnOff)
            {
                floor.SetActive(false);
            }
            foreach (GameObject tunnel in turnOn)
            {
                tunnel.SetActive(true);
            }
            teleportThisGuy.transform.position = portalLocation.transform.position;
            Destroy(this.gameObject);
            */
            SceneManager.LoadScene(creditSceneIndex);
        }
    }
}
