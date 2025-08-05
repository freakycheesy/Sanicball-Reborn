using Sanicball.Data;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
[CustomEditor(typeof(CustomStages),true)]
public class CustomStagesEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var customStages = (CustomStages)target;
        if (GUILayout.Button("Fix Barcodes in Stages")) customStages.FixStagesBarcode();
    }

}
#endif

[CreateAssetMenu(fileName = "CustomStages", menuName = "Sanicball/CustomStages")]
public class CustomStages : ScriptableObject
{
    public string Author = Application.companyName;
    public StageInfo[] Stages;
    public void FixStagesBarcode()
    {
        foreach (var stage in Stages)
        {
            stage.BARCODE = $"{Author}.{stage.name}";
        }
    }
}

