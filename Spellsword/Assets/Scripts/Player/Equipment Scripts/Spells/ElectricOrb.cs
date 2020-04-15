using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricOrb : Spell
{
    [SerializeField]
    float speed;

    [SerializeField]
    float baseStunTime;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Rigidbody>().velocity = Camera.main.transform.forward * speed;
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnTriggerEnter(Collider other)
    {
    }

    private void OnTriggerExit(Collider other)
    {
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.GetComponent<EnemyStats>() != null)
        {
            other.gameObject.GetComponent<EnemyStats>().DamageEnemy(damage, spellType);
        }
    }
}
