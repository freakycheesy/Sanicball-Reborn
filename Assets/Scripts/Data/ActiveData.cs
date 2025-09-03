using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mirror;
using Newtonsoft.Json;
using Sanicball.Logic;
using Sanicball.Powerups;
using SanicballCore;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Audio;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace Sanicball.Data
{
    public class ActiveData : MonoBehaviour
    {
        #region Fields

        public List<RaceRecord> raceRecords = new List<RaceRecord>();

        //Pseudo-singleton pattern - this field accesses the current instance.
        private static ActiveData Instance;

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

        #endregion Fields

        #region Properties
        public static GameSettings GameSettings { get { return Instance.gameSettings; } }
        public static KeybindCollection Keybinds { get { return Instance.keybinds; } }
        public static MatchSettings MatchSettings = MatchSettings.CreateDefault();
        public static List<RaceRecord> RaceRecords { get { return Instance.raceRecords; } }

        public static List<SanicPallet> CustomStagesPallets = new List<SanicPallet>();
        public static List<StageInfo> Stages = new List<StageInfo>();
        public static List<PowerupLogic> Powerups = new List<PowerupLogic>();
        public static List<CharacterInfo> Characters = new List<CharacterInfo>();
        public static GameJoltInfo GameJoltInfo { get { return Instance.gameJoltInfo; } }
        public static GameObject ChristmasHat { get { return Instance.christmasHat; } }
        public static Material ESportsTrail { get { return Instance.eSportsTrail; } }
        public static GameObject ESportsHat { get { return Instance.eSportsHat; } }
        public static Song ESportsMusic { get { return Instance.eSportsMusic; } }
        public static ESportMode ESportsPrefab { get { return Instance.eSportsPrefab; } }

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
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                FindPallets();
            }
            else
            {
                Destroy(gameObject);
            }
        }
        public static AsyncOperationHandle<IList<SanicPallet>> PalletHandle = new();
        public static void FindPallets()
        {
            PalletHandle = Addressables.LoadAssetsAsync<SanicPallet>("mod", LoadPalletCallback);
            PalletHandle.Completed += PalletCompleted;
        }

        private static void PalletCompleted(AsyncOperationHandle<IList<SanicPallet>> _)
        {
            MatchSettings = MatchSettings.CreateDefault();
            foreach (var info in Stages)
            {
                AddressablesNetworkManager.AddSceneReference(info.scene);
            }
            Debug.Log("Completed Loading Pallet!");
        }

        public static void LoadPalletCallback(SanicPallet pallet)
        {
            CustomStagesPallets.Add(pallet);
            Stages.AddRange(pallet.Stages);
            for (int i = 0; i < Stages.Count; i++) Stages[i].id = i;
            MusicPlayer.Playlist.AddRange(pallet.Playlist);
            Characters.AddRange(pallet.Avatars);
            Powerups.AddRange(pallet.Powerups);

            Debug.Log($"Loaded Pallet: ({pallet.Author}.{pallet.name})");
        }
        public static bool TryGetStageByBarcode(string barcode, out StageInfo stage)
        {
            stage = GetStageByBarcode(barcode);
            return stage != null;
        }
        public static StageInfo GetRandomStage()
        {
            return Stages[Random.Range(0, Stages.Count - 1)];
        }
        public static int GetIndexFromStage(StageInfo stage)
        {
            return Stages.IndexOf(stage);
        }
        public static StageInfo GetStageByBarcode(string barcode)
        {
            barcode = barcode.ToLower();
            var selectedStage = Stages[0];
            foreach (var stage in Stages)
            {
                if (stage.BARCODE.ToLower().Contains(barcode)) selectedStage = stage;
            }
            return selectedStage;
        }

        public static void LoadLevel(StageInfo level, LoadSceneMode mode = LoadSceneMode.Single)
        {
            BootstrapSceneManager.LoadScene(level.scene.RuntimeKey);
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
            PalletHandle.Release();
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
