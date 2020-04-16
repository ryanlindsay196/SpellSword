using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weaver_Fireball : MonoBehaviour
{
    private GameObject player;
    private GameObject jeffrey;

    public float speed;

    public GameObject fireballImpact;
    public GameObject fireball;

    private bool hasHitObject = false;
    private float timeInState = 0;

    public AudioClip impactSoundClip;
    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<CharacterMovement>().gameObject;
        jeffrey = FindObjectOfType<AI_Weaver>().gameObject;
        gameObject.GetComponent<Rigidbody>().velocity = new Vector3(player.transform.position.x - jeffrey.transform.position.x, player.transform.position.y - 2, player.transform.position.z - jeffrey.transform.position.z) * speed;

        fireballImpact.SetActive(false);
        fireball.SetActive(true);
    }


    // Update is called once per frame
    void Update()
    {
        timeInState += Time.deltaTime;
        if (!hasHitObject & timeInState >= 5.0f)
        {
            Invoke("DestroySelf", 2.2f);
            fireballImpact.SetActive(true);
            fireball.SetActive(false);
        }
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag != "Player")
        {
            if (fireballImpact.activeInHierarchy == false)
            {
                GetComponent<AudioSource>().clip = impactSoundClip;
                GetComponent<AudioSource>().Play();
            }
            hasHitObject = true;
            Invoke("DestroySelf", 2.2f);
            fireballImpact.SetActive(true);
            fireball.SetActive(false);
        }
        else if(collision.gameObject.tag == "Player")
        {
            player.GetComponent<PlayerStats>().CurrentHealth -= 10;
            if (fireballImpact.activeInHierarchy == false)
            {
                GetComponent<AudioSource>().clip = impactSoundClip;
                GetComponent<AudioSource>().Play();
            }
            hasHitObject = true;
            fireballImpact.SetActive(true);
            fireball.SetActive(false);
            Invoke("DestroySelf", 2.2f);
        }
        else
        {
            Invoke("DestroySelf", 2.2f);
        }
    }

    void DestroySelf()
    {
        Destroy(gameObject);
    }
}