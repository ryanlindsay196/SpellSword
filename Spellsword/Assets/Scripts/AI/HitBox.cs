using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBox : MonoBehaviour
{
    [SerializeField]
    float damage;
    [SerializeField]
    bool destroyOnHit;
    [SerializeField]
    float hitCoolDown;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.GetComponent<PlayerStats>() != null)
        {
            collision.gameObject.GetComponent<PlayerStats>().DamagePlayer(damage);
            if(destroyOnHit)
            {
                Destroy(gameObject);
            }
        }
    }
    /*public void OnCollisionStay(Collision collision)
    {
        if (this.gameObject.tag == "Ghoul")
        {
            if (collision.gameObject.GetComponent<PlayerStats>() != null)
            {
                collision.gameObject.GetComponent<PlayerStats>().DamagePlayer(damage);
                if (destroyOnHit)
                {
                    Destroy(gameObject);
                }
            }
        }
    }*/
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.GetComponent<PlayerStats>() != null)
        {
            collision.gameObject.GetComponent<PlayerStats>().DamagePlayer(damage);
            if (destroyOnHit)
            {
                Destroy(gameObject);
            }
        }
    }
}
