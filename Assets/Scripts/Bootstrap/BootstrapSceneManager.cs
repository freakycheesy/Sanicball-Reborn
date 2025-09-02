using Mirror;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceProviders;

public class BootstrapSceneManager : MonoBehaviour
{
    void Awake()
    {
        DontDestroyOnLoad(this);
    }

    /*
    private static void OnLoadEnd(SceneLoadEndEventArgs args)
    {
        currentScene = args.QueueData.SceneLoadData.SceneLookupDatas[0];
        if (SceneManager.GetActiveScene().name.Contains("Moved") && SceneManager.GetActiveScene().name.Contains("Objects"))
        {
            if(SceneManager.GetSceneAt(1) != null) SceneManager.SetActiveScene(SceneManager.GetSceneAt(1));
        }
    }
    */

    [Server]
    public static void LoadScene(object sceneKey)
    {
        if (sceneKey is AssetReference) sceneKey = ((AssetReference)sceneKey).RuntimeKey;
        NetworkManager.singleton.ServerChangeScene((string)sceneKey);
    }

}
