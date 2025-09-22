using UnityEngine;
using UnityEditor;
using Sanicball.Data;
using Newtonsoft.Json;


[CustomEditor(typeof(ActiveData))]
public class ActiveDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var CustomStagesPallets = JsonConvert.SerializeObject(ActiveData.CustomStagesPallets).Replace(",", "\n");
        var Stages = JsonConvert.SerializeObject(ActiveData.Stages).Replace(",", "\n");
        var Powerups = JsonConvert.SerializeObject(ActiveData.Powerups).Replace(",", "\n");
        var Characters = JsonConvert.SerializeObject(ActiveData.Characters).Replace(",", "\n");
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
