using UnityEditor;
using UnityEngine;
using UFB.Network;

[CustomEditor(typeof(NetworkTester))]
public class NetworkTesterEditor : Editor
{

    private int _effectChoice = 0;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        NetworkTester networkTester = (NetworkTester)target;


        if (GUILayout.Button("Register Client"))
        {
            networkTester.RegisterClient();
        }
    }

}