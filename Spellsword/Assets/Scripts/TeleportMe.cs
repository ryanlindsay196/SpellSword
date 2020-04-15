using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportMe : MonoBehaviour
{
    public GameObject sendMyPlayerHere;

    void OnTriggerEnter(Collider collision)
    {
        //if(collision.gameObject.tag == "Player")
        if(collision.gameObject.GetComponent<CharacterMovement>() != null)
        {
            gameObject.GetComponent<AudioSource>().Play();
            collision.gameObject.transform.position = sendMyPlayerHere.transform.position;
            collision.gameObject.transform.rotation = sendMyPlayerHere.transform.rotation;
        }
    }
}
