using Sanicball.Gameplay;
using SanicballCore;
using UnityEngine;

public class FixedCamera : MonoBehaviour, IBallCamera
{
    public static FixedCamera Instance { get; private set; }
    private Vector3 _targetPosition;
    public bool LerpMode;
    public float LerpSpeed;
    public Rigidbody Target { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

    public Camera AttachedCamera => throw new System.NotImplementedException();
    public ControlType CtrlType { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
    private void Start()
    {
        Instance = this;
    }
    public void Remove()
    {
        throw new System.NotImplementedException();
    }

    public void SetDirection(Quaternion dir)
    {
        transform.rotation = dir;
    }

    public void SetPosition(Vector3 pos)
    {
        _targetPosition = pos;
    }

    void Update()
    {
        if (LerpMode)
        {
            transform.position = Vector3.Lerp(transform.position, _targetPosition, LerpSpeed * Time.deltaTime);
        }
        else transform.position = _targetPosition;
    }
}
