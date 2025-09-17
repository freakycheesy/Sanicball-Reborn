using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mirror;
using ModTool;
using Newtonsoft.Json;
using Sanicball.Logic;
using Sanicball.Powerups;
using SanicballCore;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Sanicball.Data
{
    public class ActiveData : MonoBehaviour
    {
        #region Fields

        public List<RaceRecord> raceRecords = new List<RaceRecord>();

        public static ActiveData Instance;

        //This data is saved to a json file
        private GameSettings gameSettings = new GameSettings();

        private KeybindCollection keybinds = new KeybindCollection();

        //This data is set from the editor and remains constant
        [Header("Static data")]
        [SerializeField]
        private GameJoltInfo gameJoltInfo;

        [SerializeField]
        private GameObject christmasHat;
        [SerializeField]
        private Material eSportsTrail;
        [SerializeField]
        private GameObject eSportsHat;
        [SerializeField]
        private Song eSportsMusic;
        [SerializeField]
        private ESportMode eSportsPrefab;

        public Object LobbyScene;

        #endregion Fields

        #region Properties
        public static GameSettings GameSettings { get { return Instance.gameSettings; } }
        public static KeybindCollection Keybinds { get { return Instance.keybinds; } }
        public static MatchSettings MatchSettings = new();
        public static List<RaceRecord> RaceRecords { get { return Instance.raceRecords; } }

        public List<SanicPallet> Pallets = new List<SanicPallet>();
        public List<StageInfo> Stages = new List<StageInfo>();
        public List<PowerupLogic> Powerups = new List<PowerupLogic>();
        public List<CharacterInfo> Characters = new List<CharacterInfo>();
        public GameObject ChristmasHat { get { return Instance.christmasHat; } }
        public Material ESportsTrail { get { return Instance.eSportsTrail; } }
        public GameObject ESportsHat { get { return Instance.eSportsHat; } }
        public Song ESportsMusic { get { return Instance.eSportsMusic; } }
        public ESportMode ESportsPrefab { get { return Instance.eSportsPrefab; } }

        public static bool ESportsFullyReady
        {
            get
            {
                return GameSettings.eSportsReady;
            }
        }

        #endregion Properties

        #region Unity functions

        //Make sure there is never more than one GameData object
        private void Awake()
        {
            if (Instance != this)
            {
                if(Instance) Destroy(Instance.gameObject);
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            FindPallets();
        }
        public static string ModsPath => Path.Combine(Application.persistentDataPath, "Mods");
        public void FindPallets()
        {
            Pallets.ForEach(LoadPalletCallback);

            if (!Directory.Exists(ModsPath)) Directory.CreateDirectory(ModsPath);
            ModManager.AddSearchDirectory(ModsPath);
            ModManager.Refresh();
            ModManager.ModFound += LoadModCallback;
            ModManager.ModsChanged += PalletCompleted;
        }

        private void PalletCompleted()
        {
            MatchSettings = new();
            foreach (var info in Stages)
            {
                //AddressablesNetworkManager.AddSceneReference(info.scene);
            }
            Debug.Log("Completed Loading Pallet!");
        }

        public void LoadModCallback(Mod mod)
        {
            var pallets = mod.GetAssets<SanicPallet>();
            pallets.ToList().ForEach(LoadPalletCallback);
        }

        public void LoadPalletCallback(SanicPallet pallet)
        {
            if(!Pallets.Contains(pallet)) Pallets.Add(pallet);
            Stages.AddRange(pallet.Stages);
            for (int i = 0; i < Stages.Count; i++) Stages[i].id = i;
            MusicPlayer.Playlist.AddRange(pallet.Playlist);
            Characters.AddRange(pallet.Avatars);
            Powerups.AddRange(pallet.Powerups);
            Debug.Log($"Loaded Pallet: ({pallet.Author}.{pallet.name})");
        }
        public bool TryGetStageByBarcode(string barcode, out StageInfo stage)
        {
            stage = GetStageByBarcode(barcode);
            return stage != null;
        }
        public StageInfo GetRandomStage()
        {
            return Stages[Random.Range(0, Stages.Count - 1)];
        }
        public int GetIndexFromStage(StageInfo stage)
        {
            return Stages.IndexOf(stage);
        }
        public StageInfo GetStageByBarcode(string barcode)
        {
            if (barcode == null)
            {
                Debug.LogError("Barcode is null");
                barcode =  MatchSettings.DEFAULTSTAGE;
            }
            barcode = barcode.ToLower();
            var selectedStage = Stages.Find(x => x.BARCODE.ToLower() == barcode.ToLower());
            return selectedStage;
        }

        public void LoadLevel(StageInfo level, LoadSceneMode mode = LoadSceneMode.Single)
        {
            BootstrapSceneManager.LoadScene(level.scene);
            //level.LoadSceneAsync(mode);
            //Addressables.LoadSceneAsync(level, mode);
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
            Load("MatchSettings.json", ref MatchSettings);
            Load("Records.json", ref raceRecords);
        }

        public void SaveAll()
        {
            Save("GameSettings.json", gameSettings);
            Save("GameKeybinds.json", keybinds);
            Save("MatchSettings.json", MatchSettings);
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
    }
}
