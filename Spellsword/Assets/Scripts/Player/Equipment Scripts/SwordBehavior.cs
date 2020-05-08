using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwordBehavior : EquipmentParent
{
    bool isAttacking, isRecoiling;

    [SerializeField]
    Sprite lockedSprite;

    [SerializeField]
    GameObject bloodSplatterPrefab;


    /// <summary>
    /// After finishing the current attack, attack again if true
    /// </summary>
    bool toAttack;

    [SerializeField]
    bool[] enchantmentsUnlocked;
    public bool[] EnchantmentsUnlocked
    {
        get { return enchantmentsUnlocked; }
    }

    [SerializeField]
    PlayerStats playerStats;

    enum EnchantmentAttackTypes { None, DragonSkin, Fury, ManaDrain, DeleteGame }
    [SerializeField]
    EnchantmentAttackTypes currentEnchantment;

    [SerializeField]
    RadialMenu enchantmentRadialMenu;
    public RadialMenu EnchantmentRadialMenu
    {
        get { return enchantmentRadialMenu; }
    }

    [System.Serializable]
    struct SoundIndices
    {
        //public bool swordCanDraw;
        public int bookOpen;
        public bool swordCanSheath;
        public int swordSheath;
        public int swordSwing;
    }
    [SerializeField]
    SoundIndices soundIndices;

    //[SerializeField]
    //GameObject playerArms;

    [SerializeField]
    float baseDamage;

    Vector3 originalEulerAngles;
    // Start is called before the first frame update
    new void Start()
    {
        toAttack = false;
        base.Start();
        originalEulerAngles = transform.localEulerAngles;

        if (enchantmentRadialMenu.SpellRadialMenuSpritesDisplay == null)
            enchantmentRadialMenu.InitializeMenu();
        for (int i = 0; i < enchantmentRadialMenu.SpellRadialMenuSpritesDisplay.Count; i++)
            enchantmentRadialMenu.SpellRadialMenuSpritesDisplay[i].GetComponent<Image>().overrideSprite = lockedSprite;
    }

    // Update is called once per frame
    void Update()
    {
        #region Old attack code
        //if (isAttacking)
        //{
        //    Vector3 targetEulerAngles = new Vector3(90, transform.localEulerAngles.y, transform.localEulerAngles.z);
        //    transform.localEulerAngles = Vector3.Lerp(transform.localEulerAngles, targetEulerAngles, 0.2f);
        //
        //    //Debug.Log(Mathf.Acos(Vector3.Dot(targetEulerAngles.normalized, transform.localEulerAngles.normalized)));
        //    //if (Mathf.Acos(Vector3.Dot(targetEulerAngles.normalized, transform.localEulerAngles.normalized)) <= 0.002f)
        //    if(transform.localEulerAngles == targetEulerAngles)
        //    {
        //        //transform.localEulerAngles = targetEulerAngles;
        //        isAttacking = false;
        //        isRecoiling = true;
        //        GetComponent<Rigidbody>().detectCollisions = false;
        //    }
        //}
        //else if (isRecoiling)
        //{
        //    transform.localEulerAngles = Vector3.Lerp(transform.localEulerAngles, originalEulerAngles, 0.2f);
        //
        //    if(Mathf.Acos(Vector3.Dot(transform.localEulerAngles.normalized, originalEulerAngles.normalized)) <= 0.002f)
        //    {
        //        transform.localEulerAngles = originalEulerAngles;
        //        isAttacking = false;
        //        isRecoiling = false;
        //    }
        //}
        #endregion
        if (playerArms.GetCurrentAnimatorClipInfo(0)[0].clip.name == "BookIdle")
        {
            Invoke("StopSwitchingEquipment", 0.1f);
            playerArms.SetBool("IsSwitchingEquipment", true);
        }
        //currentEnchantment = (EnchantmentAttackTypes)enchantmentRadialMenu.CurrentSpellIndex;

        if(enchantmentRadialMenu.CurrentSpellIndex == -1)
        {
            playerArms.GetComponent<Animator>().SetBool("IsAttacking", false);
            Debug.Log("SwordBehavior::ResetEnchantmentMenu to 0");
            enchantmentRadialMenu.SetRadialOption(0);
        }
        //Debug.Log(playerArms.GetComponent<Animator>().GetCurrentAnimatorClipInfo(0)[0].clip.name);
        switch ((EnchantmentAttackTypes)enchantmentRadialMenu.CurrentSpellIndex)
        {
            case EnchantmentAttackTypes.None:
                //Debug.Log(playerArms.GetComponent<Animator>().GetCurrentAnimatorClipInfo(0).Length);
                if (playerArms.GetComponent<Animator>().GetCurrentAnimatorClipInfo(0)[0].clip.name.Contains("SwordSwing"))
                {
                    if(!toAttack)
                        playerArms.GetComponent<Animator>().SetBool("IsAttacking", false);
                }
                else if (playerArms.GetComponent<Animator>().GetCurrentAnimatorClipInfo(0)[0].clip.name.Contains("Idle"))
                {
                    toAttack = false;
                    //playerArms.GetComponent<Animator>().SetBool("IsAttacking", false);
                }
                break;
            case EnchantmentAttackTypes.DragonSkin:
                enchantmentRadialMenu.CurrentSpellIndex = (int)EnchantmentAttackTypes.None;
                if (playerStats.CurrentMana > 80)
                {//add visual indicator of dragonskin
                    playerStats.UseMana(80);
                    playerStats.AddNewStatus(PlayerStats.StatusTrackers.StatusType.defenseBoost, 30);
                }
                break;
            case EnchantmentAttackTypes.DeleteGame:
                break;
            case EnchantmentAttackTypes.Fury:
                break;
            case EnchantmentAttackTypes.ManaDrain:
                if(isAttacking)
                {
                    isAttacking = false;
                    playerStats.CurrentMana += 20;//TEMPORARY VALUE
                }
                break;
        }
    }

    void DisableAttackingAnimation()
    {
        playerArms.GetComponent<Animator>().SetBool("IsAttacking", false);
    }

    public override void UseEquipment()
    {
        toAttack = false;
        CancelInvoke("DisableAttackingAnimation");
        if (playerArms.GetComponent<Animator>().GetCurrentAnimatorClipInfo(0)[0].clip.name.Contains("SwordSwing"))
            toAttack = true;
        playerArms.GetComponent<Animator>().SetBool("IsAttacking", true);
        Invoke("DisableAttackingAnimation", playerArms.GetComponent<Animator>().GetCurrentAnimatorClipInfo(0)[0].clip.length * 0.7f * playerArms.GetComponent<Animator>().speed);
        Debug.Log("SwordBehavior::UseEquipment()::Start attacking");
        if(!isAttacking)
        {
            isAttacking = true;
            //GetComponent<Collider>().enabled = true;
        }
        if (enchantmentRadialMenu.CurrentSpellIndex != -1)
            currentEnchantment = (EnchantmentAttackTypes)enchantmentRadialMenu.CurrentSpellIndex;
        Debug.Log("SwordBehavior::UseEquipment()::Enchantment: " + ((EnchantmentAttackTypes)currentEnchantment).ToString());
            //playerSoundManager.PlaySound(soundIndices.swordSwing, 0, false);
    }

    private void OpenBookSound()
    {
        playerSoundManager.PlaySound(soundIndices.bookOpen);
    }

    private void SheathSwordSound()
    {
        playerSoundManager.PlaySound(soundIndices.swordSheath);
        soundIndices.swordCanSheath = true;
    }

    public void UnlockEnchantment(int in_enchantment, bool gameJustStarted)
    {
        if(!gameJustStarted)
            SaveFileLoader.EnchantmentLevels[in_enchantment]++;

        if(in_enchantment < enchantmentsUnlocked.Length)
            enchantmentsUnlocked[in_enchantment] = true;
        if(in_enchantment < enchantmentRadialMenu.SpellRadialMenuSpritesDisplay.Count)
            enchantmentRadialMenu.SpellRadialMenuSpritesDisplay[in_enchantment].GetComponent<Image>().overrideSprite = enchantmentRadialMenu.SpellSprites[in_enchantment];
    }

    public override bool SwitchingEquipment()
    {
        base.SwitchingEquipment();
        //if(soundIndices.swordCanSheath)
        {
            //soundIndices.swordCanSheath = false;
            Invoke("SheathSwordSound", 0.02f);
            Invoke("OpenBookSound", 0.2f);
            playerArms.GetComponent<Animator>().SetBool("IsAttacking", false);
            playerArms.SetBool("IsSwitchingEquipment", true);
        }
        //TODO: Update later when we have animations
        if (playerArms.GetCurrentAnimatorClipInfo(0)[0].clip.name.Contains("Attack"))
            return true;
        return false;
    }

    public bool AnyEnchantmentsUnlocked()
    {
        for(int i = 0; i < enchantmentsUnlocked.Length; i++)
        {
            if (enchantmentsUnlocked[i])
                return true;
        }
        return false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        //if (isAttacking)
        if (playerArms.GetComponent<Animator>().GetCurrentAnimatorClipInfo(0)[0].clip.name.Contains("SwordSwing"))
        {
            if (collision.gameObject.GetComponent<EnemyStats>() == null)
            {
                if (!playerSoundManager.AudioSourceIsPlaying(0))
                    playerSoundManager.PlaySound(6);
            }
            if (collision.gameObject.GetComponent<EnemyStats>() != null)
            {
                collision.gameObject.GetComponent<EnemyStats>().DamageEnemy(baseDamage, EnemyStats.DamageType.sword);
                playerStats.CurrentMana += playerStats.MaxMana * 0.2f;
                isAttacking = false;
            }
            if(collision.gameObject.activeInHierarchy && collision.gameObject.layer == 12)
                Instantiate(bloodSplatterPrefab, transform.position, transform.rotation);
        }
    }
}
