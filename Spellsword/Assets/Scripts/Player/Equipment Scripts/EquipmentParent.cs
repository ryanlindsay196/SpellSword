using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentParent : MonoBehaviour
{
    [SerializeField]
    protected PlayerSoundManager playerSoundManager;


    [SerializeField]
    protected Animator playerArms;
    public SaveFileLoader saveFileLoader;

    protected void Start()
    {
        //playerSoundManager = FindObjectOfType<PlayerSoundManager>();
    }
    private void Update()
    {

        if (saveFileLoader == null)
        {
            saveFileLoader = GameObject.FindObjectOfType<SaveFileLoader>();
        }
    }

    public virtual void UseEquipment()
    {
        Debug.Log("EquipmentParent::UseEquipment()::This function doesn't exist in the " + this.GetType().FullName + " class");
    }

    public virtual bool SwitchingEquipment()
    {
        playerArms.SetBool("IsSwitchingEquipment", true);
        //Debug.Log("EquipmentParent::SwitchEquipment()::This function doesn't exist in the " + this.GetType().FullName + " class");
        Invoke("StopSwitchingEquipment", 0.1f);
        return true;
    }

    void StopSwitchingEquipment()
    {
        playerArms.SetBool("IsSwitchingEquipment", false);
    }
}
