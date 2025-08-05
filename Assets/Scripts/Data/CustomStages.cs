using Sanicball.Data;
using UnityEngine;
using UnityEngine.Audio;
using Sanicball;

#if UNITY_EDITOR
using UnityEditor;
[CustomEditor(typeof(CustomStages),true)]
public class CustomStagesEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var customStages = (CustomStages)target;
        if (GUILayout.Button("Fix Barcodes"))
        {
            EditorUtility.SetDirty(customStages);
            customStages.FixBarcode();
            AssetDatabase.SaveAssetIfDirty(customStages);
        }
    }

}
#endif

[CreateAssetMenu(fileName = "CustomStages", menuName = "Sanicball/CustomStages")]
public class CustomStages : ScriptableObject
{
    public string Author;
    public StageInfo[] Stages;
    public Song[] Playlist;
    void OnValidate()
    {
        if (string.IsNullOrEmpty(Author)) Author = Application.companyName;        
    }
    public void FixBarcode()
    {
        foreach (var stage in Stages)
        {
            stage.BARCODE = $"{Author}.{name}.{stage.name}";
        }
        foreach (var song in Playlist)
        {
            song.BARCODE = $"{Author}.{name}.{song.name}";
        }
    }
}

