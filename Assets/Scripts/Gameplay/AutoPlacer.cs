using System;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;


#if UNITY_EDITOR
using UnityEditor;
[CustomEditor(typeof(AutoPlacer), true)]
public class AutoPlacerEditor : Editor
{
    public IEnumerable<AutoPlacer> placers => targets.Cast<AutoPlacer>();
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Place"))
            PlaceAllPlacers();
    }

    private void PlaceAllPlacers() => placers.ToList().ForEach(AutoPlacer.PlacePlacer);
}

public class AutoPlacerWindow : EditorWindow
{
    [MenuItem("freakycheesy/AutoPlacer")]
    public static void OpenWindow()
    {
        AutoPlacerWindow wnd = GetWindow<AutoPlacerWindow>();
        wnd.titleContent = new GUIContent("Auto Placer Window");
    }
    private void OnGUI()
    {
        if (GUILayout.Button("Place All"))
        {
            FindObjectsByType<AutoPlacer>(FindObjectsSortMode.None).ToList().ForEach(AutoPlacer.PlacePlacer);
        }
    }
}
#endif

public class AutoPlacer : MonoBehaviour
{
    void Awake()
    {
        Place();
    }
    public static void PlacePlacer(AutoPlacer placer) => placer.Place();

    [ContextMenu("Place")]
    public void Place()
    {
        TransformData.SetTransform(transform, TransformData.CalcTargetPlacement(transform));
    }

    [SerializeField]
    public struct TransformData
    {
        [SerializeField]
        public Vector3 Position;
        [SerializeField]
        public Quaternion Rotation;

        public TransformData(Vector3 position, Quaternion rotation)
        {
            Position = position;
            Rotation = rotation;
        }

        public TransformData(Transform transform)
        {
            Position = transform.position;
            Rotation = transform.rotation;
        }

        public static void SetTransform(Transform transform, TransformData data)
        {
            transform.position = data.Position;
            transform.rotation = data.Rotation;
        }

        public static TransformData CalcTargetPlacement(Transform transform)
        {
            var colliders = Physics.OverlapSphere(transform.position, 0.1f, -1, QueryTriggerInteraction.Ignore);
            foreach (var collider in colliders)
            {
                if (!transform.GetComponentsInChildren<Collider>().ToList().Find(x => x == collider)) return new(transform);
            }
            Ray ray = new Ray(transform.position, transform.rotation * Vector3.down);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100, -1))
            {
                TransformData placement = new TransformData();
                placement.Position = hit.point;

                Quaternion alongNormal = Quaternion.FromToRotation(Vector3.up, hit.normal);
                float angle = transform.rotation.eulerAngles.y;
                placement.Rotation = Quaternion.AngleAxis(angle, hit.normal) * alongNormal;

                return placement;
            }
            return new TransformData(transform.position, transform.rotation);
        }
    }
}