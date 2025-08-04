using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;

public class PostProcessAdder : MonoBehaviour
{
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        FindCamerasAndAddLayers();
        SceneManager.sceneLoaded += (_, _) => FindCamerasAndAddLayers();
    }

    void FindCamerasAndAddLayers()
    {
        var cameras = FindObjectsByType<Camera>(FindObjectsSortMode.None);
        foreach (var camera in cameras)
        {
            if (camera.TryGetComponent(out PostProcessLayer layer)) return;
            layer = camera.gameObject.AddComponent<PostProcessLayer>();
            layer.volumeTrigger = layer.transform;
            layer.volumeLayer = gameObject.layer;
            layer.antialiasingMode = PostProcessLayer.Antialiasing.FastApproximateAntialiasing;
            layer.fastApproximateAntialiasing.fastMode = true;
        }
    }
}
