using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PagePickup : MonoBehaviour
{
    [SerializeField]
    float pageBobSpeed;
    [SerializeField]
    float pageBobHeight;
    float bobAngle;
    public AudioSource audioSource;
    public AudioClip pagePickup;

    [SerializeField]
    GameObject unlockableSpellPrefab;
    // Start is called before the first frame update
    void Start()
    {
        audioSource.clip = pagePickup;
    }

    // Update is called once per frame
    void Update()
    {
        bobAngle += Time.deltaTime * pageBobSpeed;
        transform.position += new Vector3(0, Mathf.Sin(bobAngle), 0) * pageBobHeight;
    }
    /*
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<CharacterMovement>() != null)
        {
            EquipmentManager equipmentManager = FindObjectOfType<EquipmentManager>();
            //foreach (GameObject spellPrefab in book.SpellPrefabs)
            BookBehavior book = null;
            for(int i = 0; i < equipmentManager.Equipment.Count; i++)
            {
                if(equipmentManager.Equipment[i].GetComponent<BookBehavior>() != null)
                {
                    book = equipmentManager.Equipment[i].GetComponent<BookBehavior>();
                    break;
                }
            }
            if (book != null)
            {
                for (int i = 0; i < book.SpellPrefabs.Length; i++)
                {
                    if (book.SpellPrefabs[i] == unlockableSpellPrefab)
                    {
                        book.UnlockSpell(i, false);
                        Debug.Log("PagePickup::OnCollisionEnter(Collision)::Page picked up, unlock spell");
                        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerSoundManager>().SpellPickup();
                        Destroy(gameObject);
                        break;
                    }
                }
            }
            else
            {
                throw new System.Exception("PagePickup::OnCollisionEnter(Collision)::Book is null");
            }
        }
    }
    */
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<CharacterMovement>() != null)
        {
            EquipmentManager equipmentManager = FindObjectOfType<EquipmentManager>();
            //foreach (GameObject spellPrefab in book.SpellPrefabs)
            BookBehavior book = null;
            for (int i = 0; i < equipmentManager.Equipment.Count; i++)
            {
                if (equipmentManager.Equipment[i].GetComponent<BookBehavior>() != null)
                {
                    book = equipmentManager.Equipment[i].GetComponent<BookBehavior>();
                    break;
                }
            }
            if (book != null)
            {
                for (int i = 0; i < book.SpellPrefabs.Length; i++)
                {
                    if (book.SpellPrefabs[i] == unlockableSpellPrefab)
                    {
                        book.UnlockSpell(i, false);
                        Debug.Log("PagePickup::OnCollisionEnter(Collision)::Page picked up, unlock spell");
                        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerSoundManager>().SpellPickup();
                        Destroy(gameObject);
                        break;
                    }
                }
            }
            else
            {
                throw new System.Exception("PagePickup::OnCollisionEnter(Collision)::Book is null");
            }
        }
    }
}
