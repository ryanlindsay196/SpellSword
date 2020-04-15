using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    public enum DamageType { sword, fire, ice, corrosion, electric};
    public enum StatusType { none, freeze, burn, shock, plague};

    [SerializeField]
    AudioSource audioSource;

    [SerializeField]
    AudioClip deathSound;
    public AudioClip DeathSound
    {
        get { return deathSound; }
    }

    [SerializeField]
    AudioClip hitSound;
    public AudioClip HitSound
    {
        get { return hitSound; }
    }

    public int FreezeStacks
    {
        get
        {
            int stacks = 0;
            for(int i = 0; i < statuses.Count; i++)
            {
                if(statuses[i].status == StatusType.freeze)
                {
                    stacks++;
                }
            }
            return stacks;
        }
    }

    public bool IsParalyzed
    {
        get
        {
            for (int i = 0; i < statuses.Count; i++)
            {
                if (statuses[i].status == StatusType.shock)
                    return true;
            }
            return false;
        }
    }

    public struct StatusTracker
    {
        public StatusType status;
        public float timer;

        public void SetStatus(StatusType in_Status)
        {
            status = in_Status;
        }
        public void SetTimer(float in_Time)
        {
            timer = in_Time;
        }
        public void SubtractTime(float in_Time)
        {
            timer -= in_Time;
        }
    }
    public void AddStatus(float in_Time, StatusType in_Status)
    {
        statuses.Add(new StatusTracker());
        statuses[statuses.Count - 1].SetStatus(in_Status);
        statuses[statuses.Count - 1].SetTimer(in_Time);
    }

    List<StatusTracker> statuses;

    public float health;
    float secondsInDeath = 0.0f;
    [SerializeField]
    float maxHealth;

    [SerializeField]
    float swordDamagePercent, fireDamagePercent, iceDamagePercent, corrosionDamagePercent, shockDamagePercent;


    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
        statuses = new List<StatusTracker>();
    }

    // Update is called once per frame
    void Update()
    {
        if (health <= 0)
        {
            if(gameObject.tag == "Plant")
            {
                transform.parent.gameObject.GetComponent<Animator>().SetBool("isDead", true);
            }
            if (gameObject.tag == "Ghoul")
            {
                gameObject.GetComponent<CapsuleCollider>().enabled = false;
                foreach (Transform child in transform)
                {
                    child.gameObject.SetActive(false);
                }
            }
            secondsInDeath += Time.deltaTime;
            if (secondsInDeath >= 1.0)
            {
                Destroy(gameObject);
            }
        }

        for (int i = 0; i < statuses.Count; i++)
        {
            if (Mathf.Floor(statuses[i].timer) == Mathf.Ceil(statuses[i].timer))
            {//status effects tick
                switch (statuses[i].status)
                {
                    case StatusType.burn:
                        health -= fireDamagePercent / 100;
                        break;
                    case StatusType.freeze:
                        break;
                    case StatusType.plague:
                        health -= corrosionDamagePercent / 100;
                        break;
                    case StatusType.shock:
                        break;
                }
            }
            statuses[i].SubtractTime(Time.deltaTime);
            if (statuses[i].timer <= 0)
            {
                statuses.RemoveAt(i);
            }
        }
    }

    public void DamageEnemy(float in_Damage, DamageType in_DamageType)
    {
        DamageEnemy(in_Damage, in_DamageType, 0);
    }

    public void DamageEnemy(float in_Damage, DamageType in_DamageType, float statusTime)
    {
        StatusType statusType = StatusType.none;
        float tempDamageMultiplier = 1;
        if (in_DamageType == DamageType.sword)
        {
            tempDamageMultiplier = swordDamagePercent;
            if(this.tag == "Plant")
            {
                gameObject.GetComponent<AI_Sentry>().enraged = true;
            }
        }
        if (in_DamageType == DamageType.fire)
        {
            tempDamageMultiplier = fireDamagePercent;
            statusType = StatusType.burn;
        }
        if (in_DamageType == DamageType.ice)
        {
            tempDamageMultiplier = iceDamagePercent;
            statusType = StatusType.freeze;
        }
        if (in_DamageType == DamageType.corrosion)
        {
            tempDamageMultiplier = corrosionDamagePercent;
            statusType = StatusType.plague;
        }
        if (in_DamageType == DamageType.electric)
        {
            tempDamageMultiplier = shockDamagePercent;
            statusType = StatusType.shock;
        }
        if(statusTime > 0)
            AddStatus(statusTime * tempDamageMultiplier / 100, statusType);

        tempDamageMultiplier /= 100;
        Debug.Log("EnemyStats::DamageEnemy(float,DamageType)::Dealt " + in_Damage * tempDamageMultiplier + " " + in_DamageType.ToString() + " damage");
        health -= in_Damage * tempDamageMultiplier;

        if (hitSound != null)
        {
            audioSource.clip = hitSound;
            Debug.Log("Hitsound changed");
        }

        if (audioSource.clip != null)
        {
            audioSource.Play();
            Debug.Log("Hitsound played");
        }
    }
}
