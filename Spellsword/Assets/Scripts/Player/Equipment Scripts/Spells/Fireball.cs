using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : Spell
{
    [SerializeField]
    float speed;

    [SerializeField]
    GameObject fireballImpact, fireball;
    bool hasHitObject;

    [SerializeField]
    AudioClip impactSoundClip;
    // Start is called before the first frame update
    void Start()
    {
        //GetComponent<Rigidbody>().velocity = Transform.forward() FindObjectOfType<CharacterMovement>().gameObject.transform.forward * speed;
        GetComponent<Rigidbody>().velocity = Camera.main.transform.forward * speed;
    }

    // Update is called once per frame
    void Update()
    {
        //if (hasHitObject)
        //{
        //    transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(), 0.3f);
        //}
        //if(transform.localScale.magnitude <= 0.1f)
        //{
        //    Destroy(gameObject);
        //}
    }

    protected override void OnCollisionEnter(Collision collision)
    {
        base.OnCollisionEnter(collision);
        if (collision.gameObject.GetComponent<CharacterMovement>() == null)
        {
            if (fireballImpact.activeInHierarchy == false)
            {
                GetComponent<AudioSource>().clip = impactSoundClip;
                GetComponent<AudioSource>().Play();
            }
            Debug.Log("Fireball::OnCollisionEnter(Collision)::" + collision.gameObject.name);
            //hasHitObject = true;
            Invoke("DestroySelf", 2.2f);
            fireballImpact.SetActive(true);
            fireball.SetActive(false);
        }
        //Destroy(gameObject);
    }

    void DestroySelf()
    {
        Destroy(gameObject);
    }
}
