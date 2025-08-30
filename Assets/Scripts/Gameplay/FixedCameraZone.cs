using Sanicball.Gameplay;
using UnityEngine;

[RequireComponent(typeof(Collider)), DisallowMultipleComponent]
public class FixedCameraZone : MonoBehaviour
{
    public Transform fixedTransform;
    public bool overrideFixedCamera;
    public bool lerpMode;
    public float lerpSpeed;
    void Start()
    {
        GetComponent<Collider>().isTrigger = true;
        if (!fixedTransform) fixedTransform = transform;
    }
    private void OnTriggerEnter(Collider other)
    {
        FixedCamera.Instance?.SetPosition(fixedTransform.position);
        FixedCamera.Instance?.SetDirection(fixedTransform.rotation);
        if (!overrideFixedCamera) return;
        FixedCamera.Instance.LerpMode = lerpMode;
        FixedCamera.Instance.LerpSpeed = lerpSpeed;
    }
}
