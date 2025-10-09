using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Sanicball.Powerups;
using SanicballCore;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;

namespace Sanicball.Data
{
    public class ActiveData : MonoBehaviour
    {
        public static ActiveData singleton;

        
        [Header("Loaded Data")]
        public List<SanicPallet> pallets = new List<SanicPallet>();
        public List<StageInfo> stages = new List<StageInfo>();
        public List<PowerupLogic> powerups = new List<PowerupLogic>();
        public List<CharacterInfo> characters = new List<CharacterInfo>();
        [Header("Post Processing")]
        public Volume bloom;
        public Volume motionBlur;

        [Header("Static data")]
        public GameJoltInfo gameJoltInfo;
        public GameObject christmasHat;
        public Material eSportsTrail;
        public GameObject eSportsHat;
        public Song eSportsMusic;
        public ESportMode eSportsPrefab;
        public GameSettings gameSettings = new GameSettings();
        public KeybindCollection keybinds = new KeybindCollection();
        public List<RaceRecord> raceRecords = new List<RaceRecord>();
        public MatchSettings matchSettings = MatchSettings.CreateDefault();

        #region Unity functions

        //Make sure there is never more than one GameData object
        private void Awake()
        {
            FindObjectsByType<UniversalAdditionalCameraData>(FindObjectsSortMode.None).ToList().ForEach(AdditonalCameraDataCallback);
            if (singleton == null)
            {
                singleton = this;
                DontDestroyOnLoad(gameObject);
                FindPallets();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void AdditonalCameraDataCallback(UniversalAdditionalCameraData data)
        {
            data.renderPostProcessing = true;
            data.renderShadows = true;
        }

        public AsyncOperationHandle<IList<SanicPallet>> palletHandle = new();
        public List<AssetLabelReference> palletLabels = new();
        public async void FindPallets()
        {
            foreach (var label in palletLabels)
            {
                palletHandle = Addressables.LoadAssetsAsync<SanicPallet>(label, LoadPalletCallback);
                palletHandle.Completed += (_) => { matchSettings = MatchSettings.CreateDefault(); Debug.Log("Completed Loading Pallet!"); };
                await palletHandle.Task;
            }
        }

        public void LoadPalletCallback(SanicPallet pallet)
        {
            pallets.Add(pallet);
            stages.AddRange(pallet.Stages);
            for (int i = 0; i < stages.Count; i++) stages[i].id = i;
            MusicPlayer.Playlist.AddRange(pallet.Playlist);
            characters.AddRange(pallet.Avatars);
            powerups.AddRange(pallet.Powerups);

            Debug.Log($"Loaded Pallet: ({pallet.Author}.{pallet.name})");
        }
        public bool TryGetStageByBarcode(string barcode, out StageInfo stage)
        {
            stage = GetStageByBarcode(barcode);
            return stage != null;
        }
        public StageInfo GetRandomStage()
        {
            return stages[Random.Range(0, stages.Count - 1)];
        }
        public int GetIndexFromStageBarcode(string barcode)
        {
            return stages.IndexOf(GetStageByBarcode(barcode));
        }
        public int GetIndexFromStage(StageInfo stage)
        {
            return stages.IndexOf(stage);
        }
        public StageInfo GetStageByBarcode(string barcode)
        {
            barcode = barcode.ToLower();
            var selectedStage = stages[0];
            foreach (var stage in stages)
            {
                if (stage.BARCODE.ToLower().Contains(barcode)) selectedStage = stage;
            }
            return selectedStage;
        }

        public async void LoadLevel(SceneReference level, LoadSceneMode mode = LoadSceneMode.Single)
        {
            var handle = Addressables.LoadSceneAsync(level, mode);
            await handle.Task;
            FindObjectsByType<UniversalAdditionalCameraData>(FindObjectsSortMode.None).ToList().ForEach(AdditonalCameraDataCallback);
        }

        private void OnEnable()
        {
            LoadAll();
            gameJoltInfo.Init();
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= AddLensFlareLights;
        }

        private void AddLensFlareLights(Scene arg0, LoadSceneMode arg1)
        {
            FindObjectsByType<Light>(FindObjectsSortMode.None).ToList().ForEach(AddLensFlare);
        }

        private void AddLensFlare(Light light)
        {
            if(light.TryGetComponent(out LensFlareComponentSRP lensFlare))
            lensFlare = light.gameObject.AddComponent<LensFlareComponentSRP>();
        }

        private void OnApplicationQuit()
        {
            SaveAll();
            palletHandle.Release();
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
    }
}
