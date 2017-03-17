using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
public class Persister : MonoBehaviour
{
    #region Constants
    private const string SaveFile = "Save.sav";
    private const string SaveDirectory = "Save";
    #endregion

    private string _saveDirectory;
    private static List<string[]> _inMemory = new List<string[]>();
    private string _savePath;

    //private Inventory Inventory;
    // the two values we're interested in are current HP and which abilities the player have
    private PlayerAttributes _playerAttributes;

    // the game manager keeps track of which levels the player have beaten, and save them to file/memory
    private static SortedDictionary<int, bool> _completedLevels = new SortedDictionary<int, bool>();

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
        _completedLevels[buildIndex] = true;
    }

    public bool BossLevelAvailable()
    {
        // here we want to return true if we have beaten the previous levels, I think we can just check for their buildIndex at this point but might have to redo in the future
        return _completedLevels[2] && _completedLevels[3];
        //return false;
    }

    public void SaveToMemory()
    {
        /*
        _inMemory.Clear();
        var stringifiedInventorySlots = new string[Inventory.SlotCount];
        for (int i = 0; i < stringifiedInventorySlots.Length; ++i)
        {
            stringifiedInventorySlots[i] = JsonUtility.ToJson(Inventory.GetSlot(i));
        }
        _inMemory.Add(stringifiedInventorySlots);
        */

        _inMemory.Clear();
        var stringifiedPlayer = new string[_playerAttributes.Abilities.Count + 1]; // + 1 because we need a spot for HP too
        for (int i = 0; i < stringifiedPlayer.Length; i++)
        {
            stringifiedPlayer[i] = JsonUtility.ToJson(_playerAttributes.Abilities[i]);
        }
        // last element is HP
        stringifiedPlayer[stringifiedPlayer.Length-1] = JsonUtility.ToJson(_playerAttributes.MaxHP); // change to current HP later

        _inMemory.Add(stringifiedPlayer);
        
        // next we're gonna save level progression
        var stringifiedProgression = new string[_completedLevels.Count];
        for (int i = 0; i < stringifiedProgression.Length; i++)
        {
            // we only need to save the keys, since we know that if they are in here they are true
            stringifiedProgression[i] = JsonUtility.ToJson(_completedLevels.Keys.ToArray()[i]);
        }
        _inMemory.Add(stringifiedProgression);
    }

    public void LoadFromMemory()
    {
        /*
        if (!HasMemoryData) return;

        var stringifiedInventorySlots = _inMemory[0];
        for (int i = 0; i < stringifiedInventorySlots.Length; ++i)
        {
            var inventorySlot = JsonUtility.FromJson<InventorySlot>(stringifiedInventorySlots[i]);
            if (inventorySlot.StackCount > 0)
                Inventory.SetSlotData(i, inventorySlot.Item, inventorySlot.StackCount);
            else
            {
                Inventory.ClearSlot(i);
            }
        }*/

        if (!HasMemoryData) return;

        // do I need to clear the abilites and level progression when calling this?

        var stringifiedData1 = _inMemory[0];
        for (int i = 0; i < stringifiedData1.Length-1; i++)
        {
            var abilityData = JsonUtility.FromJson<Ability>(stringifiedData1[i]);
            _playerAttributes.Abilities.Add(abilityData);
        }
        var hpData = JsonUtility.FromJson<int>(stringifiedData1[stringifiedData1.Length - 1]);
        _playerAttributes.MaxHP = hpData;

        var stringifiedData2 = _inMemory[2];
        for (int i = 0; i < stringifiedData2.Length; i++)
        {
            var levelData = JsonUtility.FromJson<int>(stringifiedData2[i]);
            _completedLevels[levelData] = true;
        }
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
        get { return _inMemory.Count > 0; }
    }

    private void SaveToFile()
    {
        /*
        var stringifiedInventorySlots = new string[Inventory.SlotCount];
        for (int i = 0; i < stringifiedInventorySlots.Length; ++i)
        {
            var inventorySlot = Inventory.GetSlot(i);
            var inventorySlotSave = new InventorySlotSaveData();
            if (inventorySlot.StackCount > 0)
            {
                // NOTE: Unity wants us to use forward slashes.
                inventorySlotSave.ItemAssetPath = ItemFolder + "/" + inventorySlot.Item.name;
                inventorySlotSave.StackCount = inventorySlot.StackCount;
            }

            stringifiedInventorySlots[i] = JsonUtility.ToJson(inventorySlotSave);
        }

        File.WriteAllLines(_savePath, stringifiedInventorySlots);*/
        
        
        /*
        // nothing is saved with this approach
        var stringifiedPlayer = new string[_playerAttributes.Abilities.Count + 1];
        var stringifiedSaveData = new string[_playerAttributes.Abilities.Count + 1 + _completedLevels.Count];
        for (int i = 0; i < stringifiedPlayer.Length-1; i++)
        {
            //stringifiedPlayer[i] = _playerAttributes.Abilities[i].ToString();
            //stringifiedSaveData[i] = _playerAttributes.Abilities[i].ToString();
            stringifiedSaveData[i] = JsonUtility.ToJson(_playerAttributes.Abilities[i]);
        }
        //stringifiedPlayer[stringifiedPlayer.Length - 1] = _playerAttributes.MaxHP.ToString();
        //stringifiedSaveData[stringifiedPlayer.Length - 1] = _playerAttributes.MaxHP.ToString();
        stringifiedSaveData[stringifiedPlayer.Length - 1] = JsonUtility.ToJson(_playerAttributes.MaxHP);

        //var stringifiedLevels = new string[_completedLevels.Count];
        for (int i = stringifiedPlayer.Length; i < stringifiedSaveData.Length; i++)
        {
            //stringifiedLevels[i] = _completedLevels.Keys.ToArray()[i].ToString();
            //stringifiedSaveData[i] = _completedLevels.Keys.ToArray()[i].ToString();
            stringifiedSaveData[i] = JsonUtility.ToJson(_completedLevels.Keys.ToArray()[i]);
        }
        
        File.WriteAllLines(_savePath, stringifiedSaveData);
        */
        
        _playerAttributes.Abilities.Add(new JumpAbility());
        BeatLevel(2);
        BeatLevel(3);

        // this only saves the HP, evne if we add values to the lists manually
        PlayerSaveData temp = new PlayerSaveData();
        temp.Abilities = _playerAttributes.Abilities;
        temp.HP = _playerAttributes.MaxHP;
        temp.BeatenLevels = _completedLevels;
        

        var stringifySaveData = JsonUtility.ToJson(temp);
        //File.WriteAllLines(_savePath, stringifySaveData);
        File.WriteAllText(_savePath, stringifySaveData); // this only saves the HP field

        Debug.Log("saved at " + _savePath);
    }

    private void LoadFromFile()
    {
        // NOTE: This is not a fail-safe way to deal with things since the file can be deleted between
        // this check and when we actually tries to use it. For simplicity we disregard proper error
        // handling and such in this tutorial.

        /*if (!File.Exists(_savePath)) return;

        var stringifiedInventorySlots = File.ReadAllLines(_savePath);
        for (int i = 0; i < stringifiedInventorySlots.Length; ++i)
        {
            var inventorySlotSave = JsonUtility.FromJson<InventorySlotSaveData>(stringifiedInventorySlots[i]);
            if (inventorySlotSave.StackCount > 0)
            {
                var item = Resources.Load<Item>(inventorySlotSave.ItemAssetPath);
                Inventory.SetSlotData(i, item, inventorySlotSave.StackCount);
            }
            else
            {
                Inventory.ClearSlot(i);
            }
        }*/
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SaveToFiles();
        }
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
        Debug.Log(_playerAttributes);
        // Every time a level is loaded we can do the init stuff here, like reading from file/memory

        // we need to do some logic stuff here, like depending on which level we need check certain things
        // if we load level 1 then we need to check which abilities we have and delete those from the level
        // if we load hub then we need to check if the boss level is open
    }
}
