using Sanicball.Data;
using UnityEngine;
using UnityEngine.Audio;
using Sanicball;
using UnityEngine.AddressableAssets;
using System.Collections.Generic;
using Sanicball.Powerups;

#if UNITY_EDITOR
using UnityEditor;
[CustomEditor(typeof(SanicPallet),true)]
public class CustomStagesEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var customStages = (SanicPallet)target;
        if (GUILayout.Button("Fix Barcodes"))
        {
            EditorUtility.SetDirty(customStages);
            customStages.FixBarcode();
            AssetDatabase.SaveAssetIfDirty(customStages);
        }
        if (GUILayout.Button("Add Song"))
        {
            EditorUtility.SetDirty(customStages);
            customStages.AddSong();
            AssetDatabase.SaveAssetIfDirty(customStages);
        }
    }

}
#endif

[CreateAssetMenu(fileName = "SanicPallet", menuName = "Sanicball/SanicPallet")]
public class SanicPallet : ScriptableObject
{
    public string Author;
    public List<StageInfo> Stages;
    public List<Song> Playlist;
    public List<Sanicball.Data.CharacterInfo> Avatars;
    public List<PowerupLogic> Powerups;
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

    public void AddSong()
    {
        Song newSong = new();
        newSong.resource = song;
        newSong.name = song.editorAsset.name;
        newSong.BARCODE = $"{Author}.{name}.{newSong.name}";
        if (!Playlist.Contains(newSong))
            Playlist.Add(newSong);
    }
#endif
}

