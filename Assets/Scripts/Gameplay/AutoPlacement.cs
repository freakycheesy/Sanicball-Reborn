using System;
using UnityEngine;

public class AutoPlacement : MonoBehaviour
{
    [SerializeField]
    private LayerMask placementLayers;
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
            placement.Position = hit.point;

            Quaternion alongNormal = Quaternion.FromToRotation(Vector3.up, hit.normal);
            float angle = transform.rotation.eulerAngles.y;
            placement.Rotation = Quaternion.AngleAxis(angle, hit.normal) * alongNormal;

            return placement;
        }
        return new PosRot(transform.position, transform.rotation);
    }

}

public struct PosRot
{
    public Vector3 Position { get; set; }
    public Quaternion Rotation { get; set; }

    public PosRot(Vector3 position, Quaternion rotation)
    {
        Position = position;
        Rotation = rotation;
    }
}
