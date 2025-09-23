using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

public class poopyBakeryEditor : EditorWindow
{
    public static LightingSettings lightingSettings;
    public static SceneAsset overrideScene;

    public const string BakeOnSceneLoadKey = "BakeOnSceneLoad";
    [MenuItem("freakycheesy/Bakery")]
    public static void ShowExample()
    {
        poopyBakeryEditor wnd = GetWindow<poopyBakeryEditor>();
        wnd.titleContent = new GUIContent("shitty Bakery");
    }

    protected virtual void OnGUI()
    {
        GUILayout.Label("The shitty Bakery!");
        lightingSettings = (LightingSettings)EditorGUILayout.ObjectField("Lighting Settings", lightingSettings, typeof(LightingSettings), false);
        EditorPrefs.SetBool(BakeOnSceneLoadKey, GUILayout.Toggle(EditorPrefs.GetBool(BakeOnSceneLoadKey, true), "Enable Bake On Scene Load"));
        overrideScene = (SceneAsset)EditorGUILayout.ObjectField("Override Current Scene", overrideScene, typeof(SceneAsset), false);
        if (GUILayout.Button("Bake")) Bake(overrideScene);
    }

    private void Bake(SceneAsset scene)
    {
        Lightmapping.bakeOnSceneLoad = EditorPrefs.GetBool(BakeOnSceneLoadKey) ? Lightmapping.BakeOnSceneLoadMode.IfMissingLightingData : Lightmapping.BakeOnSceneLoadMode.Never;
        if (!scene)
        {
            Lightmapping.SetLightingSettingsForScene(SceneManager.GetActiveScene(), lightingSettings);
            Lightmapping.Bake();
        }
        else
        {
            string path = AssetDatabase.GetAssetPath(scene);
            Lightmapping.SetLightingSettingsForScene(SceneManager.GetSceneByPath(path), lightingSettings);
            
            Lightmapping.BakeMultipleScenes(new[] { path });
        }
    }
}
