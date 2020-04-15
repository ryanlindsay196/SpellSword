using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;

public class SaveFileLoader : MonoBehaviour
{
    public static List<int> enchantmentLevels, spellLevels;
    public static List<string> enchantmentNames, spellNames;
    static int saveFileIndex;
    public static int SaveFileIndex
    {
        get { return saveFileIndex; }
        set { saveFileIndex = value; }
    }

    public static List<int> EnchantmentLevels
    {
        get { return enchantmentLevels; }
    }
    public static List<int> SpellLevels
    {
        get { return spellLevels; }
    }

    static int obeliskNumber;
    public static int ObeliskNumber
    {
        get { return obeliskNumber; }
        set { obeliskNumber = value; }
    }

    enum SaveGroups { None, CurrentLevel, Spells, Enchantments, Stats, LevelData }
    static SaveGroups saveGroup = SaveGroups.None;

    struct ObjectData
    {
        public string name;
        public int state;
    }

    struct LevelData
    {
        public string LevelName;
        public List<ObjectData> objectData;
    }

    static List<LevelData> levelData;

    static string LevelToLoad;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("SaveFileLoader::Start()");
        //DontDestroyOnLoad(this.gameObject);
    }

    public static void LoadFile(string path, int in_SaveFileIndex)
    {
        saveFileIndex = in_SaveFileIndex;
        enchantmentLevels = new List<int>();
        enchantmentNames = new List<string>();
        spellLevels = new List<int>();
        spellNames = new List<string>();
        levelData = new List<LevelData>();

        StreamReader saveFileStreamReader;
        try
        {
            saveFileStreamReader = new StreamReader(path);
        }
        catch
        {//Load generic save file
            //Tis is a fallback in case nothing works
            saveFileStreamReader = new StreamReader(path.Remove(path.Length - 5, 1));
        }
        while (!saveFileStreamReader.EndOfStream)
        {
            string currentLine = saveFileStreamReader.ReadLine();

            //parse strings
            if (currentLine.StartsWith("---"))
                continue;
            else if (currentLine.StartsWith("[["))
            {
                currentLine = currentLine.Trim('[', ']');
                if (currentLine.ToLower() == "spells")
                    saveGroup = SaveGroups.Spells;
                else if (currentLine.ToLower() == "enchantments")
                    saveGroup = SaveGroups.Enchantments;
                else if (currentLine.ToLower() == "stats")
                    saveGroup = SaveGroups.Stats;
                else if (currentLine.ToLower() == "leveltoload")
                {
                    saveGroup = SaveGroups.CurrentLevel;
                }
                else if (currentLine.ToLower() == "leveldata")
                    saveGroup = SaveGroups.LevelData;
            }
            else if(currentLine.StartsWith("["))
            {
                currentLine = currentLine.Trim('[', ']');
                string[] keysValuePairs = currentLine.Split('/');

                for (int i = 0; i < keysValuePairs.Length; i++)
                {
                    string currentKey = keysValuePairs[i].Split('=')[0];
                    string currentValue = keysValuePairs[i].Split('=')[1];

                    switch (currentKey)
                    {
                        case "Name":
                            if (saveGroup == SaveGroups.Spells)
                                spellNames.Add(currentValue);
                            else if (saveGroup == SaveGroups.Enchantments)
                                enchantmentNames.Add(currentValue);
                            else if (saveGroup == SaveGroups.CurrentLevel)
                                LevelToLoad = currentValue;
                            break;
                        case "Level":
                            if (saveGroup == SaveGroups.Spells)
                                spellLevels.Add(int.Parse(currentValue));
                            else if (saveGroup == SaveGroups.Enchantments)
                                enchantmentLevels.Add(int.Parse(currentValue));
                            break;
                        case "LevelName"://for loading level data
                            if (saveGroup == SaveGroups.LevelData)
                            {
                                LevelData newLevelData = new LevelData();
                                newLevelData.LevelName = currentValue;
                                levelData.Add(newLevelData);
                            }
                            break;
                        case "ObjectName":
                            if(saveGroup == SaveGroups.LevelData)
                            {
                                ObjectData newObject = new ObjectData();
                                newObject.name = currentValue;
                                LevelData modifyLevelData = levelData[levelData.Count - 1];
                                if (modifyLevelData.objectData == null)
                                    modifyLevelData.objectData = new List<ObjectData>();
                                modifyLevelData.objectData.Add(newObject);
                                levelData[levelData.Count - 1] = modifyLevelData;
                            }
                            break;
                        case "State"://object state
                            {
                                //Get the latest object in the latest level data entry
                                ObjectData newObject = levelData[levelData.Count - 1].objectData[levelData[levelData.Count - 1].objectData.Count - 1];
                                newObject.state = int.Parse(currentValue);
                                levelData[levelData.Count - 1].objectData[levelData[levelData.Count - 1].objectData.Count - 1] = newObject;
                            }
                            break;
                        case "Obelisk":
                            {
                                obeliskNumber = int.Parse(currentValue);
                                break;
                            }
                    }
                }
            }
        }

        int LevelToLoadInt = -1;

        try
        {
            LevelToLoadInt = int.Parse(LevelToLoad);
        }
        catch
        { }

        if(LevelToLoadInt != -1)
            SceneManager.LoadScene(LevelToLoadInt);
        else
            SceneManager.LoadScene(LevelToLoad);
    }

    public static int GetObjectState(string objectName)
    {//Get the state of an object in the current level
        for(int i = 0; i < levelData.Count; i++)
        {
            if(levelData[i].LevelName == SceneManager.GetActiveScene().name)
            {
                for(int j = 0; j < levelData[i].objectData.Count; j++)
                {
                    if(levelData[i].objectData[j].name == objectName)
                    {
                        return levelData[i].objectData[j].state;
                    }
                }
            }

        }

        //if no object with the name is found, add it to the list of objects for the current level
        for(int i = 0; i < levelData.Count; i++)
        {
            if(levelData[i].LevelName == SceneManager.GetActiveScene().name)
            {
                ObjectData newObject;
                newObject.name = objectName;
                newObject.state = 0;
                levelData[i].objectData.Add(newObject);
            }
        }

        return 0;
    }

    public static void SetObjectState(string in_ObjectName, int in_ObjectState)
    {
        for(int i = 0; i < levelData.Count; i++)
        {
            if(levelData[i].LevelName == SceneManager.GetActiveScene().name)
            {
                for(int j = 0; j < levelData[i].objectData.Count; j++)
                {
                    if (levelData[i].objectData[j].name == in_ObjectName)
                    {
                        ObjectData objectToModify = levelData[i].objectData[j];

                        objectToModify.state = in_ObjectState;
                        levelData[i].objectData[j] = objectToModify;
                        Debug.Log("Set " + objectToModify.name + " state to " + objectToModify.state);
                        return;
                    }
                }
            }
        }
    }

    public static void SaveGame(int obeliskIndex)
    {
        StreamWriter saveFileStreamWriter = new StreamWriter("Assets/Resources/Save" + saveFileIndex + ".txt");

        //Write spell levels
        saveFileStreamWriter.WriteLine("[[Spells]]");
        for(int i = 0; i < spellLevels.Count; i++)
        {
            saveFileStreamWriter.WriteLine("[Name=" + spellNames[i] + "/Level=" + spellLevels[i] + "]");
        }
        saveFileStreamWriter.WriteLine();
        saveFileStreamWriter.WriteLine();

        //Write enchantment levels
        saveFileStreamWriter.WriteLine("[[Enchantments]]");
        for(int i = 0; i < enchantmentLevels.Count; i++)
        {
            saveFileStreamWriter.WriteLine("[Name=" + enchantmentNames[i] + "/Level=" + enchantmentLevels[i] + "]");
        }
        saveFileStreamWriter.WriteLine();
        saveFileStreamWriter.WriteLine();

        //Write Stats
        PlayerStats playerStats = FindObjectOfType<PlayerStats>();
        float currentHealth = playerStats.CurrentHealth;
        saveFileStreamWriter.WriteLine("[[Stats]]");
        saveFileStreamWriter.WriteLine("[Health=" + currentHealth + "]");
        saveFileStreamWriter.WriteLine();
        saveFileStreamWriter.WriteLine();

        //Write Current Level
        saveFileStreamWriter.WriteLine("[[LevelToLoad]]");
        saveFileStreamWriter.WriteLine("[Name=" + SceneManager.GetActiveScene().name + "/Obelisk=" + obeliskIndex + "]");
        saveFileStreamWriter.WriteLine();
        saveFileStreamWriter.WriteLine();

        //Write LevelData
        saveFileStreamWriter.WriteLine("[[LevelData]]");
        for(int i = 0; i < levelData.Count; i++)
        {
            saveFileStreamWriter.WriteLine("[LevelName=" + levelData[i].LevelName + "]");
            for(int j = 0; j < levelData[i].objectData.Count; j++)
            {
                saveFileStreamWriter.WriteLine("[ObjectName=" + levelData[i].objectData[j].name + "/State=" + levelData[i].objectData[j].state + "]");
            }
        }


        saveFileStreamWriter.Close();
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("SaveFileLoader::Update()::" + gameObject.name);
    }
}
