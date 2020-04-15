using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [SerializeField]
    PlayerSoundManager playerSoundManager;
    //[SerializeField]
    SaveObelisk lastSaveObelisk;
    SaveObelisk nearbySaveObelisk;
    public SaveObelisk NearbySaveObelisk
    {
        get { return nearbySaveObelisk; }
    }

    [SerializeField]
    float maxMana;
    public float MaxMana
    {
        get { return maxMana; }
    }

    public struct StatusTrackers
    {
        public void init(StatusType in_type, float in_time)
        {
            statusType = in_type;
            statusTimer = in_time;
        }

        public enum StatusType { defenseBoost, burn, corrode, freeze}
        public StatusType statusType;
        public float statusTimer;
    }

    List<StatusTrackers> statusTrackers;

    float currentMana;
    public float CurrentMana
    {
        get { return currentMana; }
        set
        {
            currentMana = Mathf.Min(value, maxMana);
        }
    }
    [SerializeField]
    float manaRegenPerSecond;

    [SerializeField]
    float maxHealth;
    float currentHealth;
    public float CurrentHealth
    {
        get { return currentHealth; }
        set { currentHealth = value; }
    }

    SaveObelisk[] allObelisks;
    SaveFileLoader saveFileLoader;

    // Start is called before the first frame update
    void Start()
    {
        statusTrackers = new List<StatusTrackers>();
        saveFileLoader = FindObjectOfType<SaveFileLoader>();
        allObelisks = FindObjectsOfType<SaveObelisk>();
        //playerSoundManager = GameObject.FindObjectOfType<PlayerSoundManager>();
        //lastSaveObelisk = GameObject.FindObjectOfType<SaveObelisk>();
        //lastSaveObelisk = allObelisks[0];
        Debug.Log("Total obelistsk: " + allObelisks.Length);
        for (int i = 0; i < allObelisks.Length; i++)
        {
            if (SaveFileLoader.ObeliskNumber == i)
            {
                lastSaveObelisk = allObelisks[i];
                Debug.Log("Starting obelisk index: " + i);
                Respawn();
                break;
            }
        }

        currentMana = maxMana;
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        if (saveFileLoader == null)
        {
            saveFileLoader = FindObjectOfType<SaveFileLoader>();
        }
        if (statusTrackers != null)
        {
            for (int i = 0; i < statusTrackers.Count; i++)
            {
                StatusTrackers tempTracker = statusTrackers[i];
                tempTracker.statusTimer -= Time.deltaTime;
                statusTrackers[i] = tempTracker;

                if (statusTrackers[i].statusTimer <= 0)
                {
                    statusTrackers.RemoveAt(i);
                    i--;
                }
            }
        }
        if(GetComponent<EquipmentManager>().GetCurrentEquipment.GetComponent<BookBehavior>() == null)
        {
            if (currentMana < maxMana)
                currentMana += Time.deltaTime * manaRegenPerSecond;
            //Debug.Log("PlayerStats::Update()::currentMana = " + currentMana);
        }
        currentMana = Mathf.Clamp(currentMana, 0, maxMana);

        //if(currentHealth <= 0)
        //{
        //    Respawn();
        //}
    }

    public void AddNewStatus(StatusTrackers.StatusType in_Type, float in_time)
    {
        statusTrackers.Add(new StatusTrackers());
        statusTrackers[statusTrackers.Count - 1].init(in_Type, in_time);
    }

    public void Respawn()
    {
        Debug.Log("PlayerStats::Respawn()");
        if (lastSaveObelisk != null)
        {
            transform.position = lastSaveObelisk.RespawnTransform.transform.position;
            GetComponent<CharacterMovement>().CameraHolder.transform.rotation = lastSaveObelisk.RespawnTransform.transform.rotation;
        }
        //transform.rotation = lastSaveObelisk.RespawnTransform.transform.rotation;
        CurrentHealth = maxHealth;
    }

    public void UseMana(float manaCost)
    {
        currentMana -= manaCost;
        Debug.Log("PlayerStats::UseMana()::New mana reserves = " + currentMana);
    }

    public void DamagePlayer(float in_Damage)
    {
        for(int i = 0; i < statusTrackers.Count; i++)
        {
            if(statusTrackers[i].statusType == StatusTrackers.StatusType.defenseBoost)
            {
                in_Damage /= 2;
            }
        }

        playerSoundManager.PlaySound(2);
        Debug.Log("PlayerStats::DamagePlayer(float)::" + in_Damage + "damage taken");
        currentHealth -= in_Damage;
        //if (currentHealth <= 0)
        //{
        //    Respawn();
        //}
    }

    public void SaveNearestObelisk()
    {
        lastSaveObelisk = nearbySaveObelisk;
        Debug.Log("PlayerStats::SaveNearestObelisk()");

        int newObeliskIndex = 0;
        for(int i = 0; i < allObelisks.Length; i++)
        {
            if (lastSaveObelisk == allObelisks[i])
                newObeliskIndex = i;
        }
        currentHealth = maxHealth;
        lastSaveObelisk.ActivateSaveParticles();

        SaveFileLoader.SaveGame(newObeliskIndex);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<SaveObelisk>() != null)
        {
            nearbySaveObelisk = collision.gameObject.GetComponent<SaveObelisk>();
            nearbySaveObelisk.PlayerInRange = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.GetComponent<SaveObelisk>() != null)
        {
            nearbySaveObelisk.PlayerInRange = false;
            nearbySaveObelisk = null;
        }
    }
}
