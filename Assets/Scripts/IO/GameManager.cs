using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

/**
 * List of levels and their IDs:
 * 0 Bootstrapper
 * 1 Hub
 * 2 Level1
 * 3 Level2
 * 4 BossLevel
 */


/// <summary>
/// This is for saving/loading data but also for keeping track of certain game elements
/// Might need to break this up because saving/loading and keeping track of progress are not the same thing 
/// </summary>
public class GameManager : MonoBehaviour
{
    #region Constants
    private const string SaveFile = "Save.sav";
    private const string SaveDirectory = "Save";
    #endregion

    private string _saveDirectory;
    private static readonly List<string[]> InMemory = new List<string[]>();
    private string _savePath;

    //private Inventory Inventory;
    // the two values we're interested in are current HP and which abilities the player have
    private PlayerAttributes _playerAttributes;
    public Scene Level1;
    public Scene Level2;

    // the game manager keeps track of which levels the player have beaten, and save them to file/memory
    // TODO should the persistor have this field? The persistor is now in charge of saving/loading and keeping track of progress
    private static readonly SortedDictionary<int, bool> CompletedLevels = new SortedDictionary<int, bool>();

    private void Awake()
    {
        DontDestroyOnLoad(this);;
        _saveDirectory = Path.Combine(Application.dataPath, SaveDirectory);
        //Inventory = GameObject.Find("Player").GetComponent<Inventory>();

        if (!Directory.Exists(_saveDirectory))
        {
            Directory.CreateDirectory(_saveDirectory);
        }

        _savePath = Path.Combine(_saveDirectory, SaveFile);

        // we can do this since this script is only in the bootstrap scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void BeatLevel(int buildIndex)
    {
        // add the level to _completedLevels
        CompletedLevels[buildIndex] = true;
    }

    public bool BossLevelAvailable()
    {
        // here we want to return true if we have beaten the previous levels, I think we can just check for their buildIndex at this point but might have to redo in the future
        // TODO This is very hacky, we need a way to do this in a better manner
        // we have to manually set which two levels we need to beat before the boss level unlocks
        return CompletedLevels[2] && CompletedLevels[3];
        //return CompletedLevels[level1.buildIndex] && CompletedLevels[level2.buildIndex];
        //return false;
    }

    public bool LevelTwoAvailable()
    {
        return CompletedLevels[2];
    }

    public void SaveToMemory()
    {

        InMemory.Clear();

        // this all works currently, at least saving to memory. next we need to clean it up and split it into seperate parts
        // we're gonna have 1 slot for storing abilities, 1 slot for storing player stats and 1 slot for saving beaten levels

        var playerSaveData = new PlayerSaveData();
        var abilityNames = new string[_playerAttributes.Abilities.Count];
        for (int i = 0; i < _playerAttributes.Abilities.Count; i++)
        {
            abilityNames[i] = _playerAttributes.Abilities[i].name;
        }

        var beatenLevels = CompletedLevels.Keys.ToArray();
        playerSaveData.Abilities = abilityNames;
        playerSaveData.HP = _playerAttributes.currentHealth; // TODO TEMP
        playerSaveData.BeatenLevels = beatenLevels;
        var stringifiedPlayer = new string[1]; // TODO TEMP
        
        stringifiedPlayer[0] = JsonUtility.ToJson(playerSaveData);
        InMemory.Add(stringifiedPlayer);
    }

    public bool LoadFromMemory()
    {
        if (!HasMemoryData) return false;

        // do I need to clear the abilites and level progression when calling this?
        var stringifiedData = InMemory[0]; // TODO POSSIBLY SCARY

        var playerSaveData = JsonUtility.FromJson<PlayerSaveData>(stringifiedData[0]);
        _playerAttributes.currentHealth = playerSaveData.HP; // TODO FIX THIS SHIT

        foreach (int i in playerSaveData.BeatenLevels)
            BeatLevel(i);

        // here we need to create abilities depending on the ones we currently have in the save data
        _playerAttributes.Abilities.Clear();
        foreach (string s in playerSaveData.Abilities)
        {
            var abb = Resources.Load<Ability>("Abilities\\" + s);
            _playerAttributes.Abilities.Add(abb);
        }

        return true;
    }

    public void SaveToFiles()
    {
        SaveToFile();
        GC.Collect();
    }

    public void LoadFromFiles()
    {
        LoadFromFile();
        GC.Collect();
    }

    private bool HasMemoryData
    {
        get { return InMemory.Count > 0; }
    }

    private void SaveToFile()
    {
        var playerSaveData = new PlayerSaveData();
        var abilityNames = new string[_playerAttributes.Abilities.Count];
        for (int i = 0; i < _playerAttributes.Abilities.Count; i++)
        {
            abilityNames[i] = _playerAttributes.Abilities[i].name;
        }

        var beatenLevels = CompletedLevels.Keys.ToArray();
        playerSaveData.Abilities = abilityNames;
        playerSaveData.HP = _playerAttributes.currentHealth; // TODO TEMP
        playerSaveData.BeatenLevels = beatenLevels;
        var stringifiedPlayer = new string[1]; // TODO TEMP

        stringifiedPlayer[0] = JsonUtility.ToJson(playerSaveData);
        File.WriteAllLines(_savePath, stringifiedPlayer);
    }

    private bool LoadFromFile()
    {
        // NOTE: This is not a fail-safe way to deal with things since the file can be deleted between
        // this check and when we actually tries to use it. For simplicity we disregard proper error
        // handling and such in this tutorial.

        if (!File.Exists(_savePath)) return false; // TODO SCARY SHIT

        var stringifiedData = File.ReadAllLines(_savePath);

        var playerSaveData = JsonUtility.FromJson<PlayerSaveData>(stringifiedData[0]);
        _playerAttributes.currentHealth = playerSaveData.HP; // TODO FIX THIS SHIT

        foreach (int i in playerSaveData.BeatenLevels)
            BeatLevel(i);

        // here we need to create abilities depending on the ones we currently have in the save data
        _playerAttributes.Abilities.Clear();
        foreach (string s in playerSaveData.Abilities)
        {
            var abb = Resources.Load<Ability>("Abilities\\" + s);
            _playerAttributes.Abilities.Add(abb);
        }

        return true;
    }


    // TODO REMOVE THIS SHIT
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha7))
            SaveToFiles();
        else if (Input.GetKeyDown(KeyCode.Alpha8))
            LoadFromFiles();

        if (Input.GetKeyDown(KeyCode.Alpha9))
            SaveToMemory();
        else if (Input.GetKeyDown(KeyCode.Alpha0))
            LoadFromMemory();
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
    }

    void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("level loaded " + scene.name);
        _playerAttributes = GameObject.Find("Player").GetComponent<PlayerAttributes>();
        if (_playerAttributes)
        {
            Debug.Log(_playerAttributes);
            if (!LoadFromMemory())
                LoadFromFile();
            /*LoadFromMemory();
            switch (scene.buildIndex)
            {
                case 1:
                    if (BossLevelAvailable())
                    {
                        
                    }
                    break;
            }*/
        }
        // Every time a level is loaded we can do the init stuff here, like reading from file/memory

        // we need to do some logic stuff here, like depending on which level we need check certain things
        // if we load level 1 then we need to check which abilities we have and delete those from the level
        // if we load hub then we need to check if the boss level is open
    }
}
