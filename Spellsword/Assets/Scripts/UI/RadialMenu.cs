using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RadialMenu : MonoBehaviour
{
    [SerializeField]
    RadialMenuOption radialMenuOptionPrefab;

    [SerializeField]
    Image backgroundImage;

    [SerializeField]
    RectTransform selectorImage;
    Vector3 selectorImageInitialSize;

    ///This is the size of the screen in aspect 4:3
    Vector2 aspectRatio;

    Vector3 initialBackgroundImageSize;

    [SerializeField]
    Sprite[] spellSprites;
    public Sprite[] SpellSprites
    {
        get { return spellSprites; }
    }

    [SerializeField] [Tooltip("How far the radial menu options fan out")]
    float fanOutRadius;
    float fanOutPercentage;
    [SerializeField] [Tooltip("How fast the radial menu options fan out")]
    float fanOutPercentSpeed;

    int currentSubMenu;

    public bool SubMenuFannedOut
    {
        get
        {
            currentSubMenu = -1;
            bool subMenuFannedOut = false;
            for (int i = 0; i < subRadialMenus.Count; i++)
            {
                if (!subRadialMenus[i].menu.isFannedOut)
                {

                }
                else
                {
                    currentSubMenu = i;
                    subMenuFannedOut = true;
                    break;
                }
            }

            return subMenuFannedOut;
        }
    }

    bool isFannedOut;
    public bool IsFannedOut
    {
        set
        {
            if (isFannedOut != value)
            {
                selectionPosition = new Vector2();
            }
            isFannedOut = value;

            if (!isFannedOut)
            {
                for (int i = 0; i < spellRadialMenuSpritesDisplay.Count; i++)
                {
                    spellRadialMenuSpritesDisplay[i].IsSelected = false;
                }
                for (int i = 0; i < subRadialMenus.Count; i++)
                {
                    subRadialMenus[i].menu.IsFannedOut = false;
                }
            }
            else
            {
                if (spellRadialMenuSpritesDisplay == null)
                    InitializeMenu();
                if(currentSpellIndex != -1)
                    spellRadialMenuSpritesDisplay[currentSpellIndex].IsSelected = false;
                //currentSpellIndex = -1;
            }
        }
        get { return isFannedOut; }
    }

    int currentSpellIndex;
    public int CurrentSpellIndex
    {
        get { return currentSpellIndex; }
        set { currentSpellIndex = value; }
    }

    Vector2 selectionPosition;
    public Vector2 SelectionPosition
    {
        get { return selectionPosition; }
    }

    [SerializeField]
    float angleOffset;
    [SerializeField]
    float angleMultiplier;
    [SerializeField]
    float iconSizeMultiplier;
    List<RadialMenuOption> spellRadialMenuSpritesDisplay;
    public List<RadialMenuOption> SpellRadialMenuSpritesDisplay
    {
        get { return spellRadialMenuSpritesDisplay; }
    }

    [System.Serializable]
    public struct SubRadialMenuData
    {
        [SerializeField]
        public RadialMenu menu;
        [SerializeField]
        public int index;
    }

    [SerializeField]
    List<SubRadialMenuData> subRadialMenus;
    public List<SubRadialMenuData> SubRadialMenus
    {
        get { return subRadialMenus; }
    }

    public void InitializeMenu()
    {
        selectorImage.localScale = new Vector3();
        currentSpellIndex = -1;
        spellRadialMenuSpritesDisplay = new List<RadialMenuOption>();
        if (iconSizeMultiplier == 0)
            iconSizeMultiplier = 1;
        for (int i = 0; i < spellSprites.Length; i++)
        {
            spellRadialMenuSpritesDisplay.Add(Instantiate(radialMenuOptionPrefab, transform, false));
            spellRadialMenuSpritesDisplay[i].GetComponent<Image>().sprite = spellSprites[i];
            Rect tempRect = spellRadialMenuSpritesDisplay[i].GetComponent<RectTransform>().rect;
            spellRadialMenuSpritesDisplay[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);

            spellRadialMenuSpritesDisplay[i].SizeMultiplier = iconSizeMultiplier;
        }

        initialBackgroundImageSize = backgroundImage.transform.localScale;
        backgroundImage.transform.localScale = new Vector3();
    }

    // Start is called before the first frame update
    void Start()
    {
        InitializeMenu();
    }
    

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("BackgroundImage anchoredPosition: " + backgroundImage.rectTransform.anchoredPosition);
        if (spellRadialMenuSpritesDisplay == null)
            InitializeMenu();
        if (isFannedOut)
        {//this menu is fanned out
            if (!SubMenuFannedOut)
            {//but no sub menus are fanned out
                SetSelectedRadialOption();
                fanOutPercentage = Mathf.Lerp(fanOutPercentage, 1, fanOutPercentSpeed);
                backgroundImage.transform.localScale = Vector3.Lerp(backgroundImage.transform.localScale, initialBackgroundImageSize, fanOutPercentSpeed);
            }
            else
            {//if submenu is fanned out, unfan out this menu
                fanOutPercentage = Mathf.Lerp(fanOutPercentage, 0, fanOutPercentSpeed);
                backgroundImage.transform.localScale = Vector3.Lerp(backgroundImage.transform.localScale, new Vector3(), fanOutPercentSpeed);
            }
        }
        else
        {//if this menu is not fanned out
            fanOutPercentage = Mathf.Lerp(fanOutPercentage, 0, fanOutPercentSpeed);
            backgroundImage.transform.localScale = Vector3.Lerp(backgroundImage.transform.localScale, new Vector3(), fanOutPercentSpeed);
            for (int i = 0; i < subRadialMenus.Count; i++)
            {
                if(subRadialMenus[i].menu.IsFannedOut)
                {
                    subRadialMenus[i].menu.IsFannedOut = false;
                    subRadialMenus[i].menu.selectionPosition = new Vector2();
                }
            }
        }

        for(int i = 0; i < spellRadialMenuSpritesDisplay.Count; i++)
        {//set the radial options at the correct positions angles and distances, and opacity
            float angle = (Mathf.PI * 2 * ((i + 1) / (float)spellRadialMenuSpritesDisplay.Count)) + (Mathf.PI / 2);
            angle *= angleMultiplier;
            angle += angleOffset;
            spellRadialMenuSpritesDisplay[i].GetComponent<RectTransform>().anchoredPosition = fanOutRadius * fanOutPercentage * new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
            if (IsFannedOut)
                spellRadialMenuSpritesDisplay[i].GetComponent<Image>().color = new Color(1, 1, 1, fanOutPercentage);
            else
                spellRadialMenuSpritesDisplay[i].GetComponent<Image>().color = new Color(1, 1, 1, 0);
        }

        selectorImage.localScale = backgroundImage.transform.localScale;
        //Debug.Log("Selection Position = " + selectionPosition);
        if(currentSpellIndex > -1)
            selectorImage.localEulerAngles = new Vector3(selectorImage.localEulerAngles.x, selectorImage.localEulerAngles.y, Mathf.Rad2Deg * Mathf.Atan2(spellRadialMenuSpritesDisplay[currentSpellIndex].GetComponent<RectTransform>().anchoredPosition.y * 50, spellRadialMenuSpritesDisplay[currentSpellIndex].GetComponent<RectTransform>().anchoredPosition.x * 50) - (60));
        
    }

    public void UpdateSelectionPosition(Vector2 changePositionBy)
    {
        if (!SubMenuFannedOut)
        {
            if (changePositionBy.magnitude > 0.5f)
                selectionPosition += changePositionBy;
            if (selectionPosition.magnitude > 1)
            {
                selectionPosition = selectionPosition.normalized * 1;
            }
        }
        else if(currentSubMenu != -1 && fanOutPercentage <= 0.2f)//currentSubMenu is set in the SubMenuFannedOut function
        {
            subRadialMenus[currentSubMenu].menu.UpdateSelectionPosition(changePositionBy);
        }
    }

    public void SetRadialOption(int index)
    {
        currentSpellIndex = index;
    }

    public void SetSelectedRadialOption()
    {
        if (selectionPosition.magnitude > .7f)
        {
            Debug.Log("Select radial option");
            if(currentSpellIndex != -1)
                spellRadialMenuSpritesDisplay[currentSpellIndex].IsSelected = false;
            currentSpellIndex = -1;
            int tempCurrentSpellIndex = 0;
            float tempDistanceFromRadialOption = 2000;
            for(int i = 0; i < spellRadialMenuSpritesDisplay.Count; i++)
            {
                    tempDistanceFromRadialOption = (spellRadialMenuSpritesDisplay[i].GetComponent<RectTransform>().anchoredPosition.normalized - selectionPosition).magnitude;
                    //Debug.Log("Radial option distance " + tempDistanceFromRadialOption + ":::" + i);
                if((spellRadialMenuSpritesDisplay[tempCurrentSpellIndex].GetComponent<RectTransform>().anchoredPosition.normalized - selectionPosition).magnitude > tempDistanceFromRadialOption)
                {
                    //Debug.Log("NEW OPTION");
                    tempCurrentSpellIndex = i;
                    //if (currentSpellIndex == 3)
                    //{
                    //    Debug.Log("POOP");
                    //}
                    //else
                    //    Debug.Log("IIII::::" + i);

                    //if(subRadialMenus.Count == 0)
                    //Debug.Log("Current spell index = " + tempCurrentSpellIndex);


                    //break;
                }
            }
            for (int j = 0; j < subRadialMenus.Count; j++)
            {//search through subradial menus to see if the indices match
                if (subRadialMenus[j].index == tempCurrentSpellIndex)
                {
                    subRadialMenus[j].menu.IsFannedOut = true;
                }
            }

            currentSpellIndex = tempCurrentSpellIndex;
            //selectionPosition = new Vector2();
            spellRadialMenuSpritesDisplay[currentSpellIndex].IsSelected = true;
        }
    }
}
