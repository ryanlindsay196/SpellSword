using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentManager : MonoBehaviour
{
    SaveFileLoader saveFileLoader;

    PlayerStats playerStats;

    CharacterMovement characterMovement;

    PlayerAimAssist playerAimAssist;
    public PlayerAimAssist PlayerAimAssist
    {
        get { return playerAimAssist; }
    }

    [SerializeField]
    List<EquipmentParent> equipment;
    public List<EquipmentParent> Equipment
    {
        get { return equipment; }
    }

    bool switchingEquipment;
    int equipmentIndex;

    public EquipmentParent GetCurrentEquipment
    {
        get { return equipment[equipmentIndex]; }
    }
    
    
    public void SwitchEquipment()
    {
        switchingEquipment = true;
        if (equipment[(equipmentIndex + 1) % equipment.Count].GetComponent<BookBehavior>() != null)
        {
            if (!equipment[(equipmentIndex + 1) % equipment.Count].GetComponent<BookBehavior>().AnyEnchantmentsUnlocked())
            {
                switchingEquipment = false;
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        saveFileLoader = FindObjectOfType<SaveFileLoader>();

        characterMovement = GetComponent<CharacterMovement>();
        playerAimAssist = FindObjectOfType<PlayerAimAssist>();
        playerStats = GetComponent<PlayerStats>();// GameObject.FindObjectOfType<PlayerStats>();
        switchingEquipment = false;
        for(int i = 1; i < equipment.Count; i++)
        {
            equipment[i].gameObject.SetActive(false);
        }

        for(int i = 0; i < equipment.Count; i++)
        {
            equipment[i].saveFileLoader = saveFileLoader;
            if (equipment[i].GetComponent<BookBehavior>() != null)
            {
                for (int j = 0; j < SaveFileLoader.SpellLevels.Count; j++)
                {
                    if (SaveFileLoader.SpellLevels[j] > 0)
                        equipment[i].GetComponent<BookBehavior>().UnlockSpell(j, true);
                }
            }
            if (equipment[i].GetComponent<SwordBehavior>() != null)
            {
                for (int j = 0; j < SaveFileLoader.EnchantmentLevels.Count; j++)
                {
                    if (SaveFileLoader.EnchantmentLevels[j] > 0)
                        equipment[i].GetComponent<SwordBehavior>().UnlockEnchantment(j, true);
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(saveFileLoader == null)
        {
            saveFileLoader = GameObject.FindObjectOfType<SaveFileLoader>();
        }
        //if (playerStats.NearbySaveObelisk == null)
        {
            if ((Input.GetButtonDown("SwitchEquipment")) && !switchingEquipment)
            {//deactivate current equipment and activate the next equipment
                Debug.Log("Switching equipment..." + equipmentIndex);
                SwitchEquipment();
                //
            }
            else if (switchingEquipment)
            {
                if (!equipment[equipmentIndex].SwitchingEquipment())
                {
                    Debug.Log("Finished switching equipment");
                    switchingEquipment = false;
                    equipment[equipmentIndex].gameObject.SetActive(false);
                    equipmentIndex++;
                    equipmentIndex = equipmentIndex % equipment.Count;
                    equipment[equipmentIndex].gameObject.SetActive(true);
                }
            }
        }
        if(Input.GetButtonDown("Interact") && playerStats.NearbySaveObelisk != null)
        {
            playerStats.SaveNearestObelisk();
        }
        #region auto aim
        if (playerAimAssist.TargetsInRange.Count > 0)
        {
            Debug.Log("Number of Targets: " + playerAimAssist.TargetsInRange.Count);
            playerAimAssist.RemoveNullTargets();
            Transform autoAimTarget = null;
            float aimAssistAngle = 2000;
            for (int i = 0; i < playerAimAssist.TargetsInRange.Count; i++)
            {//loop through all auto aim targets
                playerAimAssist.TargetsInRange[i].ResetGlow();
                float tempAimAssistAngle = Mathf.Acos(Vector3.Dot(transform.forward, (transform.position - playerAimAssist.TargetsInRange[i].transform.position).normalized));


                RaycastHit hitInfo;

                if (tempAimAssistAngle < aimAssistAngle && tempAimAssistAngle < playerAimAssist.MaxAngleForAutoAim &&
                    (Physics.Raycast(transform.position, transform.forward, out hitInfo, 200)))
                {//if the current auto aim target has a lower Acos(Dot), then save the current autoAim target
                    aimAssistAngle = tempAimAssistAngle;
                    autoAimTarget = playerAimAssist.TargetsInRange[i].transform;
                }
            }
            if (autoAimTarget != null)
            {
                autoAimTarget.GetComponent<Targetable>().isTargeted = true;
                autoAimTarget.GetComponent<Targetable>().LerpGlow();
            }
            for (int i = 0; i < playerAimAssist.TargetsInRange.Count; i++)
            {
                if (playerAimAssist.TargetsInRange[i].gameObject != autoAimTarget.gameObject)
                {
                    playerAimAssist.TargetsInRange[i].ResetGlow();
                }
            }

            if (Input.GetButtonDown("UseSpell"))
                characterMovement.SetAutoAimTarget(autoAimTarget);
        }
        else
        {
            //Debug.Log("EquipmentManager::Update()::No valid targets for auto aim");
        }
        #endregion
        //Debug.Log(Input.GetAxis("SwitchEquipment"));// || Mathf.Abs(Input.GetAxis("UseSpell")) > 0.4f
        if (Input.GetButtonDown("UseSpell"))
        {
            //if(GetCurrentEquipment.GetType(). == BookBehavior)
            //Debug.Log("EquipmentManager::Update()::Use Equipment");

            GetCurrentEquipment.UseEquipment();
        }
    }
}
