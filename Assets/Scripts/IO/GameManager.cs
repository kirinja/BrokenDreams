using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Prime31.TransitionKit;
using UnityEngine;
using UnityEngine.Audio;
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
    public AudioMixerSnapshot[] MusicSnapshots;

    private static readonly List<string[]> InMemory = new List<string[]>();

    // the game manager keeps track of which levels the player have beaten, and save them to file/memory
    private static readonly Dictionary<string, bool> CompletedLevels = new Dictionary<string, bool>();
    private bool _paused;

    // the two values we're interested in are current HP and which abilities the player have
    private PlayerAttributes _playerAttributes;
    private string _saveDirectory;
    private string _savePath;

    //Audio
    private AudioSource _effectSource;
    private AudioSource[] _musicSources;
    private int _currentAudioSource;

    // Values are only used when going back to hub from other levels
    private string _previousLevelName;
    private bool _clearedLevelFirstTime;

    public static GameManager Instance { get; private set; }

    public Texture2D MaskTexture;

    public bool Paused
    {
        get
        {
            return _paused;
        }
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


    public void SoftPause()
    {
        _paused = true;
    }


    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(this);

        _saveDirectory = Path.Combine(Application.dataPath, SaveDirectory);

        if (!Directory.Exists(_saveDirectory))
            Directory.CreateDirectory(_saveDirectory);

        _savePath = Path.Combine(_saveDirectory, SaveFile);

        // we can do this since this script is only in the bootstrap scene
        // we have to change this if we're gonna use a start menu, stil play from bootstrap
        //SceneManager.LoadScene("Start");

        var audioSources = GetComponents<AudioSource>();
        _effectSource = audioSources[0];
        _musicSources = new AudioSource[2];
        _musicSources[0] = audioSources[1];
        _musicSources[1] = audioSources[2];
        _currentAudioSource = 0;
    }


    private void Update()
    {
        if (Input.GetButtonDown("Pause"))
            if (!Paused)
            {
                if (SceneManager.GetActiveScene().name.Equals("Start")) return;
                GetComponent<MenuHandler>().ShowPauseMenu();
            }
            else
                GetComponent<MenuHandler>().HideMenus();
    }


    public void NewGame()
    {
        // maybe use transition kit here as well?
        DeleteSaveFile();
        //SceneManager.LoadScene("Hub");
        var mask = new ImageMaskTransition()
        {
            maskTexture = MaskTexture,
            backgroundColor = Color.black,
            duration = 1.5f,
            nextScene = "Hub"
        };
        TransitionKit.instance.transitionWithDelegate(mask);
    }


    public void LoadGame()
    {
        // belive we can just move to hub right away and the manager will try to load data if there is any
        //SceneManager.LoadScene("Hub");
        var mask = new ImageMaskTransition()
        {
            maskTexture = MaskTexture,
            backgroundColor = Color.black,
            duration = 1.5f,
            nextScene = "Hub"
        };
        TransitionKit.instance.transitionWithDelegate(mask);
    }


    // Returns true if this is the first time the level is cleared
    public bool BeatLevel(string levelName)
    {
        var cleared = LevelCleared(levelName);
        CompletedLevels[levelName] = true;
        return !cleared;
    }


    public bool LevelCleared(string levelName)
    {
        return CompletedLevels.ContainsKey(levelName) && CompletedLevels[levelName];
    }


    public bool LevelAvailable(string levelName)
    {
        // HACK This is very hacky
        switch (levelName)
        {
            case "Level_01":
                return true;
            case "Level_02_2D":
                return LevelCleared("Level_01");
            case "Boss_01":
                return LevelCleared("Level_01") &&
                       LevelCleared("Level_02_2D");
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
        if (File.Exists(_savePath))
            File.Delete(_savePath);
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


    // the following encrypt and decrypt code is taken from http://tekeye.biz/2015/encrypt-decrypt-c-sharp-string
    //Encrypt
    public static string EncryptString(string plainText, string passPhrase)
    {
        byte[] initVectorBytes = Encoding.UTF8.GetBytes(InitVector);
        byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);
        PasswordDeriveBytes password = new PasswordDeriveBytes(passPhrase, null);
        byte[] keyBytes = password.GetBytes(KeySize / 8);
        RijndaelManaged symmetricKey = new RijndaelManaged();
        symmetricKey.Mode = CipherMode.CBC;
        ICryptoTransform encryptor = symmetricKey.CreateEncryptor(keyBytes, initVectorBytes);
        MemoryStream memoryStream = new MemoryStream();
        CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);
        cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
        cryptoStream.FlushFinalBlock();
        byte[] cipherTextBytes = memoryStream.ToArray();
        memoryStream.Close();
        cryptoStream.Close();
        return Convert.ToBase64String(cipherTextBytes);
    }
    //Decrypt
    public static string DecryptString(string cipherText, string passPhrase)
    {
        byte[] initVectorBytes = Encoding.UTF8.GetBytes(InitVector);
        byte[] cipherTextBytes = Convert.FromBase64String(cipherText);
        PasswordDeriveBytes password = new PasswordDeriveBytes(passPhrase, null);
        byte[] keyBytes = password.GetBytes(KeySize / 8);
        RijndaelManaged symmetricKey = new RijndaelManaged();
        symmetricKey.Mode = CipherMode.CBC;
        ICryptoTransform decryptor = symmetricKey.CreateDecryptor(keyBytes, initVectorBytes);
        MemoryStream memoryStream = new MemoryStream(cipherTextBytes);
        CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
        byte[] plainTextBytes = new byte[cipherTextBytes.Length];
        int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
        memoryStream.Close();
        cryptoStream.Close();
        return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
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

        var stringifiedPlayer = JsonUtility.ToJson(playerSaveData);
        var encrypted = EncryptString(stringifiedPlayer, KeyPhrase);
        //File.WriteAllLines(_savePath, stringifiedPlayer);
        File.WriteAllText(_savePath, encrypted);
    }


    private bool LoadFromFile()
    {
        // NOTE: This is not a fail-safe way to deal with things since the file can be deleted between
        // this check and when we actually tries to use it. For simplicity we disregard proper error
        // handling and such in this tutorial.

        if (!File.Exists(_savePath)) return false; // TODO SCARY SHIT

        //var stringifiedData = File.ReadAllLines(_savePath);
        var stringifiedData = File.ReadAllText(_savePath);
        var decrypted = DecryptString(stringifiedData, KeyPhrase);
        // there is no safety here, so if the save file has been edited there will be errors

        //var playerSaveData = JsonUtility.FromJson<PlayerSaveData>(stringifiedData[0]);
        var playerSaveData = JsonUtility.FromJson<PlayerSaveData>(decrypted);
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

    // HACK: Pretty much all of this is super hacky
    public void ReturnToHub(string name, bool firstTime)
    {
        SceneManager.sceneLoaded += HubLoaded;
        _previousLevelName = name;
        _clearedLevelFirstTime = firstTime;

        var fishEye = new FishEyeTransition
        {
            nextScene = "Hub",
            duration = 2.0f,
            size = 0.2f,
            zoom = 100.0f,
            colorSeparation = 0.1f
        };
        TransitionKit.instance.transitionWithDelegate(fishEye);


        //GameObject.FindGameObjectWithTag("Player").transform.position = Vector3.zero;
    }


    private void HubLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= HubLoaded;

        if (_clearedLevelFirstTime)
        {
            StartCoroutine(RunHubAnimations());
        }

        switch (_previousLevelName)
        {
            case "Level_01":
            {
                var player = GameObject.FindGameObjectWithTag("Player");
                player.transform.position = new Vector3(GameObject.Find("Hub Door 1").transform.position.x, player.transform.position.y, player.transform.position.z);
                player.GetComponent<Controller3D>().SetSpawn(player.transform.position);
                
                break;
            }
            case "Level_02_2D":
            {
                var player = GameObject.FindGameObjectWithTag("Player");
                player.transform.position = new Vector3(GameObject.Find("Hub Door 2").transform.position.x,
                    player.transform.position.y, player.transform.position.z);
                    player.GetComponent<Controller3D>().SetSpawn(player.transform.position);

                break;
            }
        }
    }


    private IEnumerator RunHubAnimations()
    {
        switch (_previousLevelName)
        {
            case "Level_01":
            {
                GameObject.Find("Hub Door 1").transform.Find("Portal").GetComponent<HubPortal>().ShouldInitialize =
                    false;
                GameObject.Find("Hub Door 1").transform.Find("Portal").GetComponent<HubPortal>().Done =
                    false;
                GameObject.Find("Hub Door 2").transform.Find("Portal").GetComponent<HubPortal>().ShouldInitialize =
                    false;
                GameObject.Find("Hub Door 2").transform.Find("Portal").GetComponent<HubPortal>().Hide();

                SoftPause();

                yield return new WaitForSeconds(1f);

                GameObject.Find("Hub Door 1").transform.Find("Portal").GetComponent<HubPortal>().Done =
                    true;

                    yield return new WaitForSeconds(0.2f);

                GameObject.Find("Hub Door 2").transform.Find("Portal").gameObject.SetActive(true);
                    GameObject.Find("Hub Door 2").transform.Find("Portal").GetComponent<HubPortal>().ShouldAppear = true;

                Paused = false;

                break;
            }
            case "Level_02_2D":
            {
                GameObject.Find("Hub Door 2").transform.Find("Portal").GetComponent<HubPortal>().ShouldInitialize =
                    false;
                GameObject.Find("Hub Door 2").transform.Find("Portal").GetComponent<HubPortal>().Done =
                    false;
                GameObject.Find("Boss Trapdoor").GetComponent<BossDoor>().ShouldInitialize = false;

                    SoftPause();

                yield return new WaitForSeconds(1f);

                Paused = false;

                GameObject.Find("Hub Door 2").transform.Find("Portal").GetComponent<HubPortal>().Done =
                    true;

                
                    //GameObject.Find("Boss Trapdoor").GetComponent<BossDoor>().Play();

                break;
            }
        }

        yield return null;
    }

    public void PlayOneShot(AudioClip clip)
    {
        _effectSource.PlayOneShot(clip);
    }

    public void Play(AudioClip clip)
    {
        _effectSource.clip = clip;
        _effectSource.Play();
    }


    public void PlayMusic(AudioClip clip, float transitionTime)
    {
        if (_musicSources[_currentAudioSource].clip == clip)
            return;

        ++_currentAudioSource;
        _currentAudioSource %= 2;

        _musicSources[_currentAudioSource].clip = clip;
        _musicSources[_currentAudioSource].Play();
        MusicSnapshots[_currentAudioSource].TransitionTo(transitionTime);
    }

    #region Constants

    private const string SaveFile = "Save.sav";
    private const string SaveDirectory = "Save";
    private const float RegularTimeScale = 1f;

    // This size of the IV (in bytes) must = (KeySize / 8).  Default KeySize is 256, so the IV must be
    // 32 bytes long.  Using a 16 character string here gives us 32 bytes when converted to a byte array.
    private const string InitVector = "pemgail9uzpgzl88";
    // This constant is used to determine the KeySize of the encryption algorithm
    private const int KeySize = 256;

    // storing the encrypt/descrypt key inside the actual program is dangerous since you can read the memory and find the key
    // it doesnt matter in this case however since all we want to do is change the output file from english to gibberish (so the player cant easily edit it)
    private const string KeyPhrase = "Broken Dreams";

    #endregion
}