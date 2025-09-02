using Mirror;
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
            NetworkServer.Spawn(myBall.netIdentity.gameObject, NetworkServer.localConnection);
        }
    }
}
