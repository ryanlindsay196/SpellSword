using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportToBoss : MonoBehaviour
{
    public GameObject sendMyPlayerHere;
    public GameObject HARBINGER;
    void OnTriggerEnter(Collider collision)
    {
        //if(collision.gameObject.tag == "Player")
        if (collision.gameObject.GetComponent<CharacterMovement>() != null)
        {
            foreach(GameObject enemy in GameObject.FindGameObjectsWithTag("Ghoul"))
            {
                Destroy(enemy);
                HARBINGER.GetComponent<HARBINGER>().phaseNum = 1;
            }            
            gameObject.GetComponent<AudioSource>().Play();
            collision.gameObject.transform.position = sendMyPlayerHere.transform.position;
        }
    }
}
