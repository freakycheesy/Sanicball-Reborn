using UnityEngine;
using UnityEditor;
using Sanicball.Data;
using Newtonsoft.Json;


[CustomEditor(typeof(ActiveData))]
public class ActiveDataEditor : Editor
{
    static string CustomStagesPallets;
    static string Stages;
    static string Powerups;
    static string Characters;

    public override void OnInspectorGUI()
    {
        try
        {
            CustomStagesPallets = JsonConvert.SerializeObject(ActiveData.singleton.pallets).Replace(",", "\n");
            Stages = JsonConvert.SerializeObject(ActiveData.singleton.stages).Replace(",", "\n");
            Powerups = JsonConvert.SerializeObject(ActiveData.singleton.powerups).Replace(",", "\n");
            Characters = JsonConvert.SerializeObject(ActiveData.singleton.characters).Replace(",", "\n");
        }
        catch
        {
            
        }
        base.OnInspectorGUI();
        EditorGUI.BeginDisabledGroup(true);
        TextArea("Pallets", CustomStagesPallets);
        TextArea("Stages", Stages);
        TextArea("Powerups", Powerups);
        TextArea("Characters", Characters);
        EditorGUI.EndDisabledGroup();
    }

    public static void TextArea(string label, string content, int pixels = 5, bool containColon = true)
    {
        if (containColon) label += ":";
        GUILayout.Label(label);
        EditorGUILayout.TextArea(content);
        GUILayout.Space(pixels);
    }

}
