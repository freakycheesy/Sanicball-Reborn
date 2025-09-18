using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;
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
    public static void LoadScene(Object newScene)
    {
        LoadScene(newScene.name);
    }

    [Server]
    public static void LoadScene(string newSceneName)
    {
        NetworkManager.singleton.ServerChangeScene(newSceneName);
    }

}
