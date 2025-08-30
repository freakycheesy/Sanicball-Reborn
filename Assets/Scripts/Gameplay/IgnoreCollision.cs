using UnityEngine;

[RequireComponent(typeof(Collider))]
public class IgnoreCollision : MonoBehaviour
{
    public Collider[] collidersToIgnore;
    public bool IgnoreOnAwake = true;
    private Collider _collider;
    void Awake()
    {
        if (IgnoreOnAwake && TryGetComponent(out _collider))
        {
            Ignore();
        }
    }

    public void UnIgnore()
    {
        foreach (var col in collidersToIgnore)
        {
            if (col != null)
            {
                Physics.IgnoreCollision(_collider, col, false);
            }
        }
    }

    public void Ignore()
    {
        foreach (var col in collidersToIgnore)
        {
            if (col != null)
            {
                Physics.IgnoreCollision(_collider, col, true);
            }
        }
    }
}
