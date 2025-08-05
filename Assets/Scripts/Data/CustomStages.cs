using Sanicball.Data;
using UnityEngine;
using UnityEngine.Audio;
using Sanicball;
using UnityEngine.AddressableAssets;
using System.Collections.Generic;





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
        if (GUILayout.Button("Add Song"))
        {
            EditorUtility.SetDirty(customStages);
            customStages.AddSong(customStages.song);
            AssetDatabase.SaveAssetIfDirty(customStages);
        }
    }

}
#endif

[CreateAssetMenu(fileName = "CustomStages", menuName = "Sanicball/CustomStages")]
public class CustomStages : ScriptableObject
{
    public string Author;
    public List<StageInfo> Stages;
    public List<Song> Playlist;
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
#if UNITY_EDITOR
    public AssetReferenceT<AudioResource> song;
#endif

    public void AddSong(AssetReferenceT<AudioResource> song)
    {
        Song newSong = new();
        newSong.resource = song;
        newSong.name = song.editorAsset.name;
        newSong.BARCODE = $"{Author}.{name}.{newSong.name}";
        if (!Playlist.Contains(newSong))
            Playlist.Add(newSong);
    }
}

