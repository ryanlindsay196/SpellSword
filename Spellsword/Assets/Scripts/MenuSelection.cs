using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
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
    public struct MenuOptionValues
    {
        public enum OptionType { loadMenu, newGame, loadSave, changeSetting, back, quit, resume, quitToMenu }
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
        Time.timeScale = 1;
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

            if(menuOptionValues[i].linkedObject != null && menuOptionValues[i].linkedObject.GetComponent<Slider>() != null)
            {
                menuOptionValues[i].linkedObject.transform.SetParent(menuOptions[i].transform);
                menuOptionValues[i].linkedObject.transform.position = menuOptions[i].transform.position;
                menuOptionValues[i].linkedObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -20);// = menuOptions[i].GetComponent<RectTransform>().anchoredPosition;
            }
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
            //Debug.Log("Timescale = " + Time.timeScale);
            //if (Time.timeScale < 0.5)
            //    newOptionTimer = newOptionMaxTime;
            if (newOptionTimer < newOptionMaxTime)
            {
                newOptionTimer += Time.deltaTime;
                
                if (Time.timeScale == 0)
                {
                    Debug.Log("Timescale == 0");
                    newOptionTimer += 0.02f;
                }
            }
            else if (Mathf.Abs(Input.GetAxisRaw("Vertical")) >= 0.4f)
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

            if (menuOptionValues[CurrentOptionOverride].optionType == MenuOptionValues.OptionType.changeSetting && menuOptionValues[CurrentOptionOverride].linkedObject.GetComponent<Slider>() != null)
            {
                if(Mathf.Abs(Input.GetAxis("Horizontal")) >= 0.4f)
                {
                    menuOptionValues[CurrentOptionOverride].linkedObject.GetComponent<Slider>().value += Mathf.Sign(Input.GetAxis("Horizontal")) / 100;
                    SettingsManager.UpdateSetting(menuOptionValues[CurrentOptionOverride].linkedObject.GetComponent<Setting>().SettingType, menuOptionValues[CurrentOptionOverride].linkedObject.GetComponent<Slider>().value);
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
                if(menuOptionValues[CurrentOptionOverride].optionType == MenuOptionValues.OptionType.resume)
                {
                    menuOptionValues[CurrentOptionOverride].linkedObject.SetActive(false);
                    Time.timeScale = 1;
                }
                if(menuOptionValues[CurrentOptionOverride].optionType == MenuOptionValues.OptionType.quitToMenu)
                {
                    SceneManager.LoadScene("MainMenu");
                }
                //if(menuOptionValues[CurrentOptionOverride].optionType == MenuOptionValues.OptionType.changeSetting)
                //{

                //}
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
