using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitSomething : MonoBehaviour
{
    private GameObject playerToKill;
    public int damageValue;

    void Start()
    {
        if (playerToKill == null)
        {
            playerToKill = FindObjectOfType<CharacterMovement>().gameObject;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            if(damageValue == 0)
            {
                playerToKill.GetComponent<PlayerStats>().CurrentHealth -= 3;
            }
            playerToKill.GetComponent<PlayerStats>().CurrentHealth -= damageValue;
            if (gameObject.tag != "Flying Skull")
            {
                playerToKill.GetComponent<PlayerSoundManager>().BulletHit();
            }

            if(gameObject.tag == "Flying Skull")
            {
                gameObject.GetComponent<SphereCollider>().enabled = false;
                foreach (Transform child in transform)
                {
                    if (child.name != "Particle Effect")
                    {
                        Destroy(child.gameObject);
                    }
                    else
                    {
                        child.GetComponent<SkullBlast>().fireballImpact.SetActive(true);
                        child.GetComponent<AudioSource>().Play();
                    }
                }
                StartCoroutine(DelayForASecond());
            }
            else
            {
                Destroy(gameObject);
            }
        }
        else if (collision.transform.tag != "Flying Skull")
        {
            if (gameObject.tag == "Flying Skull")
            {
                gameObject.GetComponent<SphereCollider>().enabled = false;
                foreach (Transform child in transform)
                {
                    if (child.name != "Particle Effect")
                    {
                        Destroy(child.gameObject);
                    }
                    else
                    {
                        child.GetComponent<SkullBlast>().fireballImpact.SetActive(true);
                        child.GetComponent<AudioSource>().Play();
                    }
                }
                StartCoroutine(DelayForASecond());
            }
        }
    }

    IEnumerator DelayForASecond()
    {
        yield return new WaitForSeconds(1);
        Destroy(gameObject);
    }
}
