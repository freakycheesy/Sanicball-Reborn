using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using SanicballCore;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Sanicball.Data
{
    public class ActiveData : MonoBehaviour
    {
        #region Fields

        public List<RaceRecord> raceRecords = new List<RaceRecord>();

        //Pseudo-singleton pattern - this field accesses the current instance.
        private static ActiveData instance;

        //This data is saved to a json file
        private GameSettings gameSettings = new GameSettings();

        private KeybindCollection keybinds = new KeybindCollection();
        private MatchSettings matchSettings = MatchSettings.CreateDefault();

        //This data is set from the editor and remains constant
        [Header("Static data")]
        [SerializeField]
        private List<CustomStages> customStagesPallets;

        [SerializeField]
        private CharacterInfo[] characters;

        [SerializeField]
        private GameJoltInfo gameJoltInfo;

        [SerializeField]
        private GameObject christmasHat;
        [SerializeField]
        private Material eSportsTrail;
        [SerializeField]
        private GameObject eSportsHat;
        [SerializeField]
        private AudioClip eSportsMusic;
        [SerializeField]
        private ESportMode eSportsPrefab;

        #endregion Fields

        #region Properties

        public static GameSettings GameSettings { get { return instance.gameSettings; } }
        public static KeybindCollection Keybinds { get { return instance.keybinds; } }
        public static MatchSettings MatchSettings { get { return instance.matchSettings; } set { instance.matchSettings = value; } }
        public static List<RaceRecord> RaceRecords { get { return instance.raceRecords; } }

        public static CustomStages[] CustomStagesPallets { get { return instance.customStagesPallets.ToArray(); } }
        public static StageInfo[] Stages { get {
                List<StageInfo> dumped = new();
                foreach (var stages in CustomStagesPallets) {
                    foreach (var stage in stages.Stages) dumped.Add(stage);
                }
                return dumped.ToArray();
                } }
        public static CharacterInfo[] Characters { get { return instance.characters; } }
        public static GameJoltInfo GameJoltInfo { get { return instance.gameJoltInfo; } }
        public static GameObject ChristmasHat { get { return instance.christmasHat; } }
        public static Material ESportsTrail { get { return instance.eSportsTrail; } }
        public static GameObject ESportsHat { get { return instance.eSportsHat; } }
        public static AudioClip ESportsMusic { get { return instance.eSportsMusic; } }
        public static ESportMode ESportsPrefab { get { return instance.eSportsPrefab; } }

        public static bool ESportsFullyReady
        {
            get
            {
                bool possible = false;
                if (GameSettings.eSportsReady)
                {
                    Sanicball.Logic.MatchManager m = FindObjectOfType<Sanicball.Logic.MatchManager>();
                    if (m)
                    {
                        var players = m.Players;
                        foreach (var p in players)
                        {
                            if (p.CtrlType != SanicballCore.ControlType.None)
                            {
                                if (p.CharacterId == 13)
                                {
                                    possible = true;
                                }
                                else
                                {
                                    return false;
                                }
                            }
                        }
                    }
                }
                return possible;
            }
        }

        #endregion Properties

        #region Unity functions

        //Make sure there is never more than one GameData object
        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
                FindLevels();          
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public static void FindLevels()
        {
            var levels = Addressables.LoadAssetsAsync<CustomStages>("level", LoadLevelCallback);
        }

        public static void LoadLevelCallback(CustomStages stages)
        {
            foreach (var scene in stages.Stages)
            {
                scene.scene.LoadAssetAsync<Object>();
            }
            instance.customStagesPallets.Add(stages);
        }

        private void OnEnable()
        {
            LoadAll();
            gameJoltInfo.Init();
        }

        private void OnApplicationQuit()
        {
            SaveAll();
        }

        #endregion Unity functions

        #region Saving and loading

        public void LoadAll()
        {
            Load("GameSettings.json", ref gameSettings);
            Load("GameKeybinds.json", ref keybinds);
            Load("MatchSettings.json", ref matchSettings);
            Load("Records.json", ref raceRecords);
        }

        public void SaveAll()
        {
            Save("GameSettings.json", gameSettings);
            Save("GameKeybinds.json", keybinds);
            Save("MatchSettings.json", matchSettings);
            Save("Records.json", raceRecords);
        }

        private void Load<T>(string filename, ref T output)
        {
            string fullPath = Application.persistentDataPath + "/" + filename;
            if (File.Exists(fullPath))
            {
                //Load file contents
                string dataString;
                using (StreamReader sr = new StreamReader(fullPath))
                {
                    dataString = sr.ReadToEnd();
                }
                //Deserialize from JSON into a data object
                try
                {
                    var dataObj = JsonConvert.DeserializeObject<T>(dataString);
                    //Make sure an object was created, this would't end well with a null value
                    if (dataObj != null)
                    {
                        output = dataObj;
                        Debug.Log(filename + " loaded successfully.");
                    }
                    else
                    {
                        Debug.LogError("Failed to load " + filename + ": file is empty.");
                    }
                }
                catch (JsonException ex)
                {
                    Debug.LogError("Failed to parse " + filename + "! JSON converter info: " + ex.Message);
                }
            }
            else
            {
                Debug.Log(filename + " has not been loaded - file not found.");
            }
        }

        private void Save(string filename, object objToSave)
        {
            var data = JsonConvert.SerializeObject(objToSave);
            using (StreamWriter sw = new StreamWriter(Application.persistentDataPath + "/" + filename))
            {
                sw.Write(data);
            }
            Debug.Log(filename + " saved successfully.");
        }
        #endregion Saving and loading

        public static StageInfo GetStage(int index)
        {
            foreach (var stage in Stages)
            {
                if (stage == Stages[index]) return stage;
            }
            return null;
        }
    }
}
