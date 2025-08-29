using FishNet;
using Sanicball.Gameplay;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Ball))]
public class BallEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        Ball myBall = (Ball)target;
        if (GUILayout.Button("Respawn"))
        {
            myBall.RequestRespawn();
        }
        if (GUILayout.Button("Spawn On Network"))
        {
            InstanceFinder.NetworkManager.ServerManager.Spawn(myBall.NetworkObject, InstanceFinder.ClientManager.Connection);
        }
    }
}
