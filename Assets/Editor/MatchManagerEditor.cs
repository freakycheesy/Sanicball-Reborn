using Newtonsoft.Json;
using Sanicball.Logic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MatchManager))]
public class MatchManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Show Match Settings Panel"))
        {
            MatchManager.ShowMatchSettingsPanel();
        }
        base.OnInspectorGUI();
        EditorGUI.BeginDisabledGroup(true);
        var settings = MatchManager.Instance?MatchManager.Instance.State.CurrentSettings:new();
        GUILayout.Label("Match Settings");
        EditorGUILayout.TextArea(JsonConvert.SerializeObject(settings).Replace(",", ",\n"));
        EditorGUI.EndDisabledGroup();
    }

}
