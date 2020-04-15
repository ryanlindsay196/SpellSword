using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spell : MonoBehaviour
{
    [SerializeField]
    protected float damage;
    [SerializeField]
    protected EnemyStats.DamageType spellType;

    [SerializeField]
    float manaCost;
    public float ManaCost
    {
        get { return manaCost; }
    }

    [SerializeField]
    float coolDownMaxTime;
    public float CoolDownMaxTime
    {
        get { return coolDownMaxTime; }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected virtual void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<CharacterMovement>() == null)
        {
            if (collision.gameObject.GetComponent<SpellInteractableObject>() != null)
            {
                collision.gameObject.GetComponent<SpellInteractableObject>().OnActivated(this);
            }
            if(collision.gameObject.GetComponent<EnemyStats>() != null)
            {
                collision.gameObject.GetComponent<EnemyStats>().DamageEnemy(damage, spellType);
            }
        }
    }
}
