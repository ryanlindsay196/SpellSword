﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;

public class MenuSelection : MonoBehaviour
{
    [SerializeField]
    AudioSource audioSource;
    [SerializeField]
    AudioClip btnHighlightAudioClip, btnSelectAudioClip;

    [SerializeField]
    SaveFileLoader saveFileLoader;

    [SerializeField]
    MenuOption optionPrefab;
    [SerializeField]
    float distanceBetweenOptions;
    [SerializeField]
    bool isHorizontal;

    [SerializeField]
    bool renderOnSceneLoad;

    GameObject previousMenu;

    int currentlyOptionIndex;
    
    List<MenuOption> menuOptions;

    [System.Serializable]
    struct MenuOptionValues
    {
        public enum OptionType { loadMenu, newGame, loadSave, changeSetting, back, quit }
        public string text;
        public OptionType optionType;
        public GameObject linkedObject;
    }

    public int CurrentOptionOverride
    {
        get
        {
            for (int i = 0; i < menuOptions.Count; i++)
            {
                if (menuOptions[i].IsHovered)
                    return i;
            }
            return currentlyOptionIndex;
        }
    }

    [SerializeField]
    //string[] menuOptionsText;
    MenuOptionValues[] menuOptionValues;


    [SerializeField]
    float newOptionMaxTime;
    float newOptionTimer;


    // Start is called before the first frame update
    void Start()
    {
        if(!renderOnSceneLoad)
        {
            gameObject.SetActive(false);
            //Debug.Log("MenuSelection::Start()::culling CanvasRenderer: " + name);
        }

        menuOptions = new List<MenuOption>();
        for(int i = 0; i < menuOptionValues.Length; i++)
        {
            menuOptions.Add(Instantiate<MenuOption>(optionPrefab, GetComponent<RectTransform>().anchoredPosition + new Vector2(distanceBetweenOptions * i * (isHorizontal ? 1 : 0), distanceBetweenOptions * i * (!isHorizontal ? 1 : 0)), Quaternion.identity, gameObject.transform));
            menuOptions[i].SetText(menuOptionValues[i].text);
            //menuOptionss[i].option = Instantiate<MenuOption>(optionPrefab, GetComponent<RectTransform>().anchoredPosition + new Vector2(distanceBetweenOptions * i * (isVertical ? 1 : 0), distanceBetweenOptions * i * (!isVertical ? 1 : 0)), Quaternion.identity, gameObject.transform);
            //menuOptionss[i].option.SetText(menuOptionss[i].text);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (saveFileLoader == null)
        {
            saveFileLoader = GameObject.FindObjectOfType<SaveFileLoader>();
        }

        #region navigate between menu options
        if (true)//!GetComponent<CanvasRenderer>().cull)
        {
            if (newOptionTimer < newOptionMaxTime)
                newOptionTimer += Time.deltaTime;
            else if (Mathf.Abs(Input.GetAxis("Vertical")) >= 0.4f)
            {
                newOptionTimer = 0;
                menuOptions[currentlyOptionIndex].StopSelected();
                currentlyOptionIndex -= (int)Mathf.Sign(Input.GetAxis("Vertical"));
                if (currentlyOptionIndex < 0)
                    currentlyOptionIndex += menuOptions.Count;
                currentlyOptionIndex = Mathf.Abs(currentlyOptionIndex % menuOptions.Count);
                
                audioSource.clip = btnHighlightAudioClip;
                audioSource.Play();
            }
            menuOptions[CurrentOptionOverride].MoveToSelectedPosition();

            for(int i = 0; i < menuOptions.Count; i++)
            {
                if(CurrentOptionOverride != i)
                {
                    menuOptions[i].StopSelected();
                }
            }
            #endregion

            #region Select menu option
            if (Input.GetButtonDown("Jump") || Input.GetMouseButtonDown(0)/* && newOptionTimer >= newOptionMaxTime*/)
            {
                audioSource.clip = btnSelectAudioClip;
                audioSource.Play();
                Debug.Log("MenuSelection::Update()::Selected option " + CurrentOptionOverride + " in " + name);
                if (menuOptionValues[CurrentOptionOverride].optionType == MenuOptionValues.OptionType.loadMenu)
                {
                    ActivateNewMenu();
                }
                if (menuOptionValues[CurrentOptionOverride].optionType == MenuOptionValues.OptionType.loadSave)
                {
                    SaveFileLoader.LoadFile("Assets/Resources/Save" + (CurrentOptionOverride + 1) + ".txt", CurrentOptionOverride + 1);
                }
                if(menuOptionValues[CurrentOptionOverride].optionType == MenuOptionValues.OptionType.quit)
                {
                    Debug.Log("Trying to quit application");
                    Application.Quit();
                }
                if(menuOptionValues[CurrentOptionOverride].optionType == MenuOptionValues.OptionType.back)
                {
                    ActivatePreviousMenu();
                }
                if(menuOptionValues[CurrentOptionOverride].optionType == MenuOptionValues.OptionType.newGame)
                {
                    SaveFileLoader.LoadFile("Assets/Resources/Save.txt", -1);
                    SaveFileLoader.SaveFileIndex = CurrentOptionOverride + 1;
                }
            }
            else if(Input.GetButtonDown("BackButton"))
            {
                ActivatePreviousMenu();
            }
        }
        #endregion
    }

    public void ActivateNewMenu()
    {
        //if the linkedObject of the current menu option is a new MenuSelection, set it to active and make it's previous menu this menu
        if (menuOptionValues[CurrentOptionOverride].linkedObject != null)
        {
            menuOptionValues[CurrentOptionOverride].linkedObject.SetActive(true);
            menuOptionValues[CurrentOptionOverride].linkedObject.GetComponent<MenuSelection>().previousMenu = gameObject;
            gameObject.SetActive(false);
        }
    }

    public void ActivatePreviousMenu()
    {
        if (previousMenu != null)
        {
            audioSource.clip = btnSelectAudioClip;
            audioSource.Play();
            Debug.Log("MenuSelection::ActivatePreviousMenu " + previousMenu.name);
            previousMenu.SetActive(true);
            gameObject.SetActive(false);
        }
        else
        {
            Debug.Log("MenuSelection::ActivatePreviousMenu::no previous menu to activate");
        }
    }
}
