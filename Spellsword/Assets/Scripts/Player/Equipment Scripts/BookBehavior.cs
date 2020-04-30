using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BookBehavior : EquipmentParent
{
    [System.Serializable]
    struct SoundIndices
    {
        //public bool bookCanOpen;
        public int swordDraw;
        public bool bookCanClose;
        public int bookClose;
    }
    [SerializeField]
    SoundIndices soundIndices;


    [SerializeField]
    List<Texture> pageImages;
    [SerializeField]
    List<Material> pageMaterials;
    [SerializeField]
    List<GameObject> pages;
    [SerializeField]
    ParticleSystem[] spellParticleSystems;

    [SerializeField]
    GameObject bookModel;

    float spellCooldownTimer;

    [SerializeField]
    Transform spellSpawnTransform;

    [SerializeField]
    GameObject[] spellPrefabs;
    public GameObject[] SpellPrefabs
    {
        get { return spellPrefabs; }
    }

    /// <summary>
    /// the overall page of the book. theoretically could be infinite
    /// </summary>
    [SerializeField]
    int currentPageIndex;
    
    /// <summary>
    /// which of the three pages in the scene is being shown/turned
    /// </summary>
    [SerializeField]
    int pageOffset;
    /// <summary>
    /// which of the three pages in the scene will be active after the current page finishes turning
    /// </summary>
    [SerializeField]
    int nextPageOffset;
    int pageTurnDirection;
    bool switchingPages;

    [SerializeField]
    bool[] spellUnlocked;
    [SerializeField]
    RadialMenu spellRadialMenu;

    [SerializeField]
    Sprite[] uiSpellSprites;

    float pageSwitchTimer;
    float pageSwitchMaxTime;

    public int CurrentPageIndex
    {
        get { return currentPageIndex; }
        set { currentPageIndex = value; }
    }

    [SerializeField]
    Sprite lockedSprite;

    public GameObject GetSpellPrefab(int i)
    {
        return spellPrefabs[i];
    }

    public void SwitchSpell(int targetPageIndex)
    {
        if (targetPageIndex > spellUnlocked.Length)
            return;
        if (spellUnlocked[targetPageIndex])
        {
            currentPageIndex = targetPageIndex;
            playerArms.SetBool("IsTurningPage", true);
            Invoke("StopTurningPageAnimation", 0.1f);
            Debug.Log("Successfully switched to spell: " + currentPageIndex);
        }
        else
            Debug.Log("Target spell not unlocked");
    }

    void StopTurningPageAnimation()
    {
        playerArms.SetBool("IsTurningPage", false);
    }

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        soundIndices.bookCanClose = true;
        pageSwitchMaxTime = 0.32f;
        pageSwitchTimer = pageSwitchMaxTime;

        if (spellRadialMenu.SpellRadialMenuSpritesDisplay == null)
            spellRadialMenu.InitializeMenu();

        for (int i = 0; i < spellRadialMenu.SpellRadialMenuSpritesDisplay.Count; i++)
            spellRadialMenu.SpellRadialMenuSpritesDisplay[i].GetComponent<Image>().overrideSprite = lockedSprite;

        pages[0].GetComponent<Page>().PageFront.GetComponent<MeshRenderer>().material = pageMaterials[0];
        pages[0].GetComponent<Page>().PageBack.GetComponent<MeshRenderer>().material = pageMaterials[1];
        pages[1].GetComponent<Page>().PageFront.GetComponent<MeshRenderer>().material = pageMaterials[2];
        pages[1].GetComponent<Page>().PageBack.GetComponent<MeshRenderer>().material = pageMaterials[3];
        pages[2].GetComponent<Page>().PageFront.GetComponent<MeshRenderer>().material = pageMaterials[4];
        pages[2].GetComponent<Page>().PageBack.GetComponent<MeshRenderer>().material = pageMaterials[5];
    }

    private void FixedUpdate()
    {
        if (playerArms.GetCurrentAnimatorClipInfo(0)[0].clip.name == "SwordIdle")
        {//if the player's animation is the sword, switch to book (because this is the BOOK script)
            Invoke("StopSwitchingEquipment", 0.1f);
            playerArms.SetBool("IsSwitchingEquipment", true);
        }
        if (spellCooldownTimer > 0)
        {
            spellCooldownTimer -= Time.deltaTime;
        }

        if(Mathf.Abs(Input.GetAxis("Mouse ScrollWheel")) >= 0.1f && !switchingPages)
        {
            switchingPages = true;
            currentPageIndex += (int)Mathf.Sign(Input.GetAxis("Mouse ScrollWheel"));
            if (currentPageIndex < 0 || currentPageIndex > pageImages.Count - 1)
            {//at first of last page
                currentPageIndex = Mathf.Clamp(currentPageIndex, 0, pageImages.Count - 1);
                switchingPages = false;
            }
            else
            {//successful page switch
                pageSwitchTimer = 0;
                pageTurnDirection = (int)Mathf.Sign(Input.GetAxis("Mouse ScrollWheel"));
                spellParticleSystems[currentPageIndex - pageTurnDirection].Stop();
                spellParticleSystems[currentPageIndex - pageTurnDirection].gameObject.SetActive(false);

                nextPageOffset = (pageOffset + pageTurnDirection) % (pages.Count);
                if (nextPageOffset <= -1)
                    nextPageOffset = pages.Count + nextPageOffset;
                //make the rotation of the page that's not visible equal to the one that's turning
                if (pageTurnDirection >= 1)
                    pages[nextPageOffset].transform.rotation = pages[pageOffset].transform.rotation;
                else if(nextPageOffset - pageOffset < -1)
                    pages[nextPageOffset].transform.localEulerAngles = new Vector3(-90,90,90);


                for (int i = 0; i < pages.Count; i++)
                    pages[i].transform.localPosition = new Vector3();

                if (pageTurnDirection >= 1)
                {
                    pages[pageOffset].transform.localPosition = new Vector3(0, 0.11f, 0);
                    pages[nextPageOffset].transform.localPosition = new Vector3(0, 0.11f, 0);
                    //pages[nextPageOffset].GetComponent<Page>().PageFront.GetComponent<MeshRenderer>().materials[0].mainTexture = pageImages[currentPageIndex * 2];
                    //pages[nextPageOffset].GetComponent<Page>().PageFront.GetComponent<MeshRenderer>().materials[0].mainTexture = pageImages[0];
                    pages[nextPageOffset].GetComponent<Page>().PageFront.GetComponent<MeshRenderer>().material = pageMaterials[(currentPageIndex * 2)];
                    pages[nextPageOffset].GetComponent<Page>().PageBack.GetComponent<MeshRenderer>().material = pageMaterials[(currentPageIndex * 2) + 1];
                    //pages[nextPageOffset].GetComponent<Page>().PageBack.GetComponent<MeshRenderer>().materials[0].mainTexture = pageImages[(currentPageIndex * 2) + 1];
                }
                else
                {
                    //pages[currentPageIndex].transform.position = new Vector3(0, -0.03f, 0);
                    pages[pageOffset].transform.localPosition = new Vector3(0, 0.08f, 0);
                    pages[nextPageOffset].transform.localPosition = new Vector3(0, 0.11f, 0);
                    //pages[nextPageOffset - 2].transform.localPosition = new Vector3(0, 0.11f, 0);
                }
                //make the image of the page that's not visible equal to the next or previous image in the book

                Debug.Log("BookBehavior::FixedUpdate()::Page Index: " + currentPageIndex);

                //pageOffset = nextPageOffset;
                //pageOffset = pageOffset % (pages.Count - 1);//Mathf.Clamp(pageOffset, 0, pages.Count - 1);
                Debug.Log("BookBehavior::FixedUpdate()::Page Offset: " + pageOffset);
            }
        }
        if(switchingPages)
        {
            pageSwitchTimer += Time.deltaTime;
            if (pageTurnDirection >= 1)//turning the right page to the left
                pages[pageOffset].transform.Rotate(0, 2 * pageTurnDirection, 0);
            else//turning the left page to the right
                pages[nextPageOffset].transform.Rotate(0, 2 * pageTurnDirection, 0);
            //Debug.Log();
            float targetPageRotationDifference = 0;
            if (pageTurnDirection >= 1)
                targetPageRotationDifference = ((pages[pageOffset].transform.localEulerAngles.x - 180) - (90 * Mathf.Sign(pageTurnDirection)));
            else
                targetPageRotationDifference = ((pages[nextPageOffset].transform.localEulerAngles.x - 180) - (90 * Mathf.Sign(pageTurnDirection)));
            //Debug.Log("BookBehavior::FixedUpdate()::Rotation difference: " + targetPageRotationDifference);
            if(Mathf.Abs(targetPageRotationDifference) <= 4 && pageSwitchTimer >= pageSwitchMaxTime)
            {
                if (pageTurnDirection <= -1)
                    pages[nextPageOffset].transform.localEulerAngles = new Vector3(90, 90, 90);
                if(pageTurnDirection >= 1)
                    pages[pageOffset].transform.localEulerAngles = new Vector3(90 * -Mathf.Sign(pageTurnDirection), 90, 90);
                spellParticleSystems[currentPageIndex].gameObject.SetActive(true);
                spellParticleSystems[currentPageIndex].Play();
                pageOffset = nextPageOffset;
                switchingPages = false;

                if (Mathf.Abs(Input.GetAxis("Mouse ScrollWheel")) >= 0.2f)
                    Debug.Log("BookBehavior::FixedUpdate()::Scroll wheel stuck. Are you using a track pad? That might be the problem.");
            }
        }
    }

    public override void UseEquipment()
    {
        //base.UseEquipment();
        //Subtract the mana value of the currently selected spell from the playerstats
        if (spellCooldownTimer <= 0 && GameObject.FindObjectOfType<PlayerStats>().CurrentMana > 0 && spellUnlocked[currentPageIndex])
        {
            playerArms.SetBool("IsUsingSpell", true);
            Invoke("SpawnSpell", 0.2f);
        }
    }
    void SpawnSpell()
    {
        if (playerArms.GetBool("IsUsingSpell"))
        {
            Instantiate(spellPrefabs[currentPageIndex], spellSpawnTransform.position, spellSpawnTransform.rotation);
            spellCooldownTimer = spellPrefabs[currentPageIndex].GetComponent<Spell>().CoolDownMaxTime;
            GameObject.FindObjectOfType<PlayerStats>().UseMana(spellPrefabs[currentPageIndex].GetComponent<Spell>().ManaCost);
            playerArms.SetBool("IsUsingSpell", false);
        }
    }

    public override bool SwitchingEquipment()
    {
        base.SwitchingEquipment();
        if (soundIndices.bookCanClose)
        {
            Invoke("BookCloseSound", 0.3f);
            Invoke("SwordDrawSound", 0.5f);
            soundIndices.bookCanClose = false;
        }
        bookModel.GetComponent<Animator>().SetTrigger("CloseBook");
        if (bookModel.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("FinishClosing"))
        {//if closing book animation is done playing
            soundIndices.bookCanClose = true;
            return false;
        }
        return true;
    }

    private void BookCloseSound()
    {
        //playerSoundManager.PlaySound(1);
        playerSoundManager.PlaySound(soundIndices.bookClose);
    }

    private void SwordDrawSound()
    {
        playerSoundManager.PlaySound(soundIndices.swordDraw);
    }

    public void UnlockSpell(int in_unlockableSpellIndex, bool gameJustStarted)
    {
        spellUnlocked[in_unlockableSpellIndex] = true;
        spellRadialMenu.SpellRadialMenuSpritesDisplay[in_unlockableSpellIndex].GetComponent<Image>().overrideSprite = spellRadialMenu.SpellSprites[in_unlockableSpellIndex];
        if(!gameJustStarted)
            SaveFileLoader.SpellLevels[in_unlockableSpellIndex]++;
    }

    public bool AnyEnchantmentsUnlocked()
    {
        for (int i = 0; i < spellUnlocked.Length; i++)
        {
            if (spellUnlocked[i])
                return true;
        }
        return false;
    }
}
