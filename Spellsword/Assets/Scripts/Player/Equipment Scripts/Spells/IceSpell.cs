using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceSpell : Spell
{
    [SerializeField]
    float collisionActivatedMaxTime;
    [SerializeField]
    float particleSystemActivatedMaxTime;
    [SerializeField]
    float killSelfMaxTime;

    [SerializeField]
    AudioSource castAudioSource, hitAudioSource;
    //[SerializeField]
    //AudioClip castAudioClip, hitAudioClip;

    float spellActiveTimer;

    [SerializeField]
    GameObject particleSystem;

    //GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        //GetComponent<Rigidbody>().velocity = Camera.main.transform.forward;
        transform.rotation = Camera.main.transform.rotation;
        castAudioSource.Play();
        //transform.eulerAngles += new Vector3(0, 90, 0);
        //transform.eulerAngles = Camera.main.transform.up;
        //player = GameObject.FindObjectOfType<CharacterMovement>().gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Camera.main.transform.position;//player.transform.position;
        transform.rotation = Quaternion.Lerp(transform.rotation, Camera.main.transform.rotation, 0.3f);//player.transform.rotation;
        spellActiveTimer += Time.deltaTime;

        if (spellActiveTimer >= killSelfMaxTime && !hitAudioSource.isPlaying)
            Destroy(gameObject);
        else
        {
            if (spellActiveTimer >= collisionActivatedMaxTime && GetComponent<BoxCollider>().enabled)
            {
                GetComponent<BoxCollider>().enabled = false;
            }
            if (spellActiveTimer >= particleSystemActivatedMaxTime)
            {
                particleSystem.SetActive(false);
            }
        }
    }

    protected override void OnCollisionEnter(Collision collision)
    {
        base.OnCollisionEnter(collision);
        hitAudioSource.Play();
    }
}
