using Newtonsoft.Json;
using Sanicball.Logic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MatchManager), true)]
public class MatchManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var manager = target as MatchManager;
        var matchSettings = JsonConvert.SerializeObject(manager.CurrentSettings).Replace(",","\n");
        base.OnInspectorGUI();
        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.TextArea(matchSettings);
        EditorGUI.EndDisabledGroup();
    }

}
