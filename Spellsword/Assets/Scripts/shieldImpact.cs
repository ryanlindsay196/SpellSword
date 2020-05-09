using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shieldImpact : MonoBehaviour
{

    public AudioClip shieldImpactClip;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "sword")
        {
            gameObject.GetComponent<AudioSource>().clip = shieldImpactClip;
            gameObject.GetComponent<AudioSource>().Play();
        }
    }
}
