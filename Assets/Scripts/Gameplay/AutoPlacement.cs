using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
[CustomEditor(typeof(AutoPlacement), true)]
public class AutoPlacementEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Place"))
        {
            (target as AutoPlacement).Place();
        }
    }

}
#endif
public class AutoPlacement : MonoBehaviour
{
    public LayerMask placementLayers;
    public Vector3 offset;
    public bool flip = false;
    public bool manual = false;
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        PosRot placement = CalcTargetPlacement();

        Gizmos.DrawLine(transform.position, placement.Position);

        Gizmos.matrix = Matrix4x4.TRS(placement.Position, placement.Rotation, Vector3.one);
        Gizmos.DrawCube(Vector3.zero, GetComponentInChildren<Renderer>().bounds.size);
        Gizmos.matrix = Matrix4x4.identity;
    }
    void Awake() => Place();

    public void Place()
    {
        if (manual && Application.isPlaying) return;
        PosRot placement = CalcTargetPlacement();
        transform.position = placement.Position;
        transform.rotation = placement.Rotation;
    }

    protected PosRot CalcTargetPlacement()
    {
        Ray ray = new Ray(transform.position, transform.rotation * Vector3.down);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100, placementLayers, QueryTriggerInteraction.Ignore))
        {
            PosRot placement = new PosRot();
            placement.Position = hit.point + offset;

            Quaternion alongNormal = Quaternion.FromToRotation(Vector3.up, hit.normal);
            float angle = transform.rotation.eulerAngles.y;
            placement.Rotation = Quaternion.AngleAxis(angle, hit.normal) * alongNormal;
            if(flip) placement.Rotation = Quaternion.Euler(placement.Euler.x, -placement.Euler.y, placement.Euler.z);
            return placement;
        }
        return new PosRot(transform.position, transform.rotation);
    }

}

public struct PosRot
{
    public Vector3 Position { get; set; }
    public Quaternion Rotation { get; set; }
    public Vector3 Euler { get { return Rotation.eulerAngles; }}

    public PosRot(Vector3 position, Quaternion rotation)
    {
        Position = position;
        Rotation = rotation;
    }
}