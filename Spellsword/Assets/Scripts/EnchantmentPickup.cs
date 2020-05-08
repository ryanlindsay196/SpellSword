using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnchantmentPickup : MonoBehaviour
{
    [SerializeField]
    int indexOfEnchantmentToUnlock;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<CharacterMovement>() != null)
        {
            EquipmentManager equipmentManager = FindObjectOfType<EquipmentManager>();
            //foreach (GameObject spellPrefab in book.SpellPrefabs)
            SwordBehavior sword = null;
            for (int i = 0; i < equipmentManager.Equipment.Count; i++)
            {
                if (equipmentManager.Equipment[i].GetComponent<SwordBehavior>() != null)
                {
                    sword = equipmentManager.Equipment[i].GetComponent<SwordBehavior>();
                    break;
                }
            }
            if (sword != null)
            {
                sword.UnlockEnchantment(indexOfEnchantmentToUnlock, false);
                GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerSoundManager>().SpellPickup();
                Destroy(gameObject);
            }
            else
            {
                throw new System.Exception("PagePickup::OnCollisionEnter(Collision)::Book is null");
            }
        }
    }

    /*
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<CharacterMovement>() != null)
        {
            EquipmentManager equipmentManager = FindObjectOfType<EquipmentManager>();
            //foreach (GameObject spellPrefab in book.SpellPrefabs)
            SwordBehavior sword = null;
            for (int i = 0; i < equipmentManager.Equipment.Count; i++)
            {
                if (equipmentManager.Equipment[i].GetComponent<SwordBehavior>() != null)
                {
                    sword = equipmentManager.Equipment[i].GetComponent<SwordBehavior>();
                    break;
                }
            }
            if (sword != null)
            {
                sword.UnlockEnchantment(indexOfEnchantmentToUnlock, false);
                GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerSoundManager>().SpellPickup();
                Destroy(gameObject);
            }
            else
            {
                throw new System.Exception("PagePickup::OnCollisionEnter(Collision)::Book is null");
            }
        }
    }
    */
}
