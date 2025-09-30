using Sanicball;
using Sanicball.Gameplay;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AINode), true), CanEditMultipleObjects]
public class AINodeEditor : Editor
{
    public Checkpoint Checkpoint { get; set; }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        GUILayout.Label("Create AI Node");
        if (GUILayout.Button("AI Node Single")) CreateAINode<AINodeSingle>();
        if (GUILayout.Button("AI Node Splitter")) CreateAINode<AINodeSplitter>();
        if (targets.Length < 2)
        {
            Checkpoint = (Checkpoint)EditorGUILayout.ObjectField("Checkpoint", Checkpoint, typeof(Checkpoint), true);
            if (GUILayout.Button("Set Checkpoint AI Point as this"))
            {
                Checkpoint.FirstAINode = (AINode)target;
            }
        }
    }

    private void CreateAINode<T>() where T : AINode
    {
        var targetNode = (AINode)targets[0];
        var newNode = new GameObject($"{targetNode.name}").AddComponent<T>();
        newNode.transform.parent = targetNode.transform.parent;
        newNode.transform.position = targetNode.transform.position;
        Selection.activeGameObject = newNode.gameObject;
        Selection.activeObject = newNode;
        Selection.activeTransform = newNode.transform;
        foreach (var target in targets)
        {
            var node = target as AINode;
            node.AddNextNode(newNode);
        }
    }
}
