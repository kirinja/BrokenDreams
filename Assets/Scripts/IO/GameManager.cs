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
 * 2 Level_01
 * 3 Level_02
 * 4 Boss_01
 */


/// <summary>
///     This is for saving/loading data but also for keeping track of certain game elements
///     Might need to break this up because saving/loading and keeping track of progress are not the same thing
/// </summary>
public class GameManager : MonoBehaviour
{
    private static readonly List<string[]> InMemory = new List<string[]>();

    // the game manager keeps track of which levels the player have beaten, and save them to file/memory
    private static readonly Dictionary<string, bool> CompletedLevels = new Dictionary<string, bool>();
    private bool _paused;

    // the two values we're interested in are current HP and which abilities the player have
    private PlayerAttributes _playerAttributes;

    private string _saveDirectory;
    private string _savePath;


    public bool Paused
    {
        get { return _paused; }
        set
        {
            _paused = value;
            Time.timeScale = value ? 0f : RegularTimeScale;
        }
    }


    public bool UseCheckPoint { get; set; }


    private bool HasMemoryData
    {
        get { return InMemory.Count > 0; }
    }


    public static bool IsPaused()
    {
        var gm = Get();
        return gm && gm.Paused;
    }


    private void Awake()
    {
        DontDestroyOnLoad(this);

        _saveDirectory = Path.Combine(Application.dataPath, SaveDirectory);
        //AbilityGUI = GameObject.Find("Player").GetComponent<AbilityGUI>();

        if (!Directory.Exists(_saveDirectory))
            Directory.CreateDirectory(_saveDirectory);

        _savePath = Path.Combine(_saveDirectory, SaveFile);

        // we can do this since this script is only in the bootstrap scene
        SceneManager.LoadScene("Hub");

    }


    private void Update()
    {
        if (Input.GetButtonDown("Pause"))
            if (!Paused)
            {
                GetComponent<MenuHandler>().ShowPauseMenu();
            }
            else
            {
                GetComponent<MenuHandler>().HideMenus();
            }
    }


    public void BeatLevel(string levelName)
    {
        // add the level to _completedLevels
        CompletedLevels[levelName] = true;
    }


    public bool LevelAvailable(string levelName)
    {
        // HACK This is very hacky
        switch (levelName)
        {
            case "Level_01":
                return true;
            case "Level_02_2D":
                return CompletedLevels.ContainsKey("Level_01") && CompletedLevels["Level_01"];
            case "Boss_01":
                //return CompletedLevels.ContainsKey("Level_01") && CompletedLevels["Level_01"] && CompletedLevels.ContainsKey("Level_02") && CompletedLevels["Level_02"];
                return CompletedLevels.ContainsKey("Level_01") && CompletedLevels["Level_01"] &&
                       CompletedLevels.ContainsKey("Level_02_2D") && CompletedLevels["Level_02_2D"];
            default:
                return false;
        }
    }


    public void SaveToMemory()
    {
        InMemory.Clear();

        // this all works currently, at least saving to memory. next we need to clean it up and split it into seperate parts
        // we're gonna have 1 slot for storing abilities, 1 slot for storing player stats and 1 slot for saving beaten levels

        var playerSaveData = new PlayerSaveData();
        var abilityNames = new string[_playerAttributes.Abilities.Count];
        for (var i = 0; i < _playerAttributes.Abilities.Count; i++)
            abilityNames[i] = _playerAttributes.Abilities[i].name;

        var beatenLevels = CompletedLevels.Keys.ToArray();
        playerSaveData.Abilities = abilityNames;
        //playerSaveData.HP = _playerAttributes.currentHealth;
        playerSaveData.BeatenLevels = beatenLevels;
        playerSaveData.SpawnPoint = _playerAttributes.GetComponent<Controller3D>().SpawnPoint;
        var stringifiedPlayer = new string[1];

        stringifiedPlayer[0] = JsonUtility.ToJson(playerSaveData);
        InMemory.Add(stringifiedPlayer);
    }


    public bool LoadFromMemory()
    {
        if (!HasMemoryData) return false;

        // do I need to clear the abilites and level progression when calling this?
        var stringifiedData = InMemory[0]; // TODO POSSIBLY SCARY

        var playerSaveData = JsonUtility.FromJson<PlayerSaveData>(stringifiedData[0]);
        //_playerAttributes.currentHealth = playerSaveData.HP;
        //_playerAttributes.GetComponent<Controller3D>().SetSpawn(playerSaveData.SpawnPoint);

        foreach (var i in playerSaveData.BeatenLevels)
            BeatLevel(i);

        // here we need to create abilities depending on the ones we currently have in the save data
        _playerAttributes.Abilities.Clear();
        foreach (var s in playerSaveData.Abilities)
        {
            var abb = Resources.Load<Ability>("Abilities\\" + s);
            _playerAttributes.Abilities.Add(abb);
        }

        return true;
    }


    public void LoadCheckPointData()
    {
        if (!HasMemoryData) return;

        var stringifiedData = InMemory[0]; // TODO POSSIBLY SCARY
        var playerSaveData = JsonUtility.FromJson<PlayerSaveData>(stringifiedData[0]);
        _playerAttributes.GetComponent<Controller3D>().SetSpawn(playerSaveData.SpawnPoint);
    }


    public void DeleteSaveFile()
    {
        if (File.Exists(SaveFile))
            File.Delete(SaveFile);
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


    private void SaveToFile()
    {
        var playerSaveData = new PlayerSaveData();
        var abilityNames = new string[_playerAttributes.Abilities.Count];
        for (var i = 0; i < _playerAttributes.Abilities.Count; i++)
            abilityNames[i] = _playerAttributes.Abilities[i].name;

        var beatenLevels = CompletedLevels.Keys.ToArray();
        playerSaveData.Abilities = abilityNames;
        //playerSaveData.HP = _playerAttributes.currentHealth;
        playerSaveData.BeatenLevels = beatenLevels;
        var stringifiedPlayer = new string[1];

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
        //_playerAttributes.currentHealth = playerSaveData.HP;

        foreach (var i in playerSaveData.BeatenLevels)
            BeatLevel(i);

        // here we need to create abilities depending on the ones we currently have in the save data
        _playerAttributes.Abilities.Clear();
        foreach (var s in playerSaveData.Abilities)
        {
            var abb = Resources.Load<Ability>("Abilities\\" + s);
            _playerAttributes.Abilities.Add(abb);
        }

        return true;
    }


    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }


    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
    }


    private void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        //_playerAttributes = GameObject.Find("Player").GetComponent<PlayerAttributes>();
        var g = GameObject.FindGameObjectWithTag("Player");
        if (!g) return;

        _playerAttributes = g.GetComponent<PlayerAttributes>();
        if (!_playerAttributes) return;

        if (!LoadFromMemory())
            LoadFromFile();

        GetComponent<MenuHandler>().HideMenus();
        // Every time a level is loaded we can do the init stuff here, like reading from file/memory

        // we need to do some logic stuff here, like depending on which level we need check certain things
        // if we load level 1 then we need to check which abilities we have and delete those from the level
        // if we load hub then we need to check if the boss level is open
    }


    public static GameManager Get()
    {
        var gameObject = GameObject.FindGameObjectWithTag("Game Manager");
        if (gameObject)
            return gameObject.GetComponent<GameManager>();

        // create infinte amount of game manager, which is not what we want
        //var gm = Instantiate(new GameObject("GameManager"));
        //gm.AddComponent<GameManager>();
        //return gm.GetComponent<GameManager>();

        return null;
    }


    #region Constants

    private const string SaveFile = "Save.sav";
    private const string SaveDirectory = "Save";
    private const float RegularTimeScale = 1f;

    #endregion
}