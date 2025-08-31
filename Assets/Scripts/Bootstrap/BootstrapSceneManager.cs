using System;
using FishNet;
using FishNet.Managing.Scened;
using FishNet.Object;
using UnityEngine;
using SceneManager = UnityEngine.SceneManagement.SceneManager;
public class BootstrapSceneManager : MonoBehaviour
{
    void Awake()
    {
        DontDestroyOnLoad(this);
    }

    void Start()
    {
        InstanceFinder.SceneManager.OnLoadEnd += OnLoadEnd;
    }

    private static void OnLoadEnd(SceneLoadEndEventArgs args)
    {
        currentScene = args.QueueData.SceneLoadData.SceneLookupDatas[0];
        if (SceneManager.GetActiveScene().name.Contains("Moved") && SceneManager.GetActiveScene().name.Contains("Objects"))
        {
            SceneManager.SetActiveScene(SceneManager.GetSceneAt(1));
        }
    }
    public static SceneLookupData currentScene = new();

    [Server]
    public static void LoadScene(object sceneKey)
    {
        SceneLookupData sld = new((string)sceneKey);
        SceneLoadData data = new SceneLoadData(sld)
        {
            MovedNetworkObjects = new NetworkObject[0],
            ReplaceScenes = ReplaceOption.All,
        };
        InstanceFinder.SceneManager.LoadGlobalScenes(data);
    }

    [Server]
    public static void UnloadScene(object sceneKey)
    {
        SceneLookupData sld = new((string)sceneKey);
        SceneUnloadData data = new SceneUnloadData(sld);
        InstanceFinder.SceneManager.UnloadGlobalScenes(data);
    }

    [Server]
    public static void ReplaceCurrentScene(object sceneKey)
    {
        UnloadCurrentScene();
        LoadScene(sceneKey);
    }

    [Server]
    public static void UnloadCurrentScene()
    {
        InstanceFinder.SceneManager.UnloadGlobalScenes(new(currentScene));
    }
}
