using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AlgorandManager))]
public class AlgorandManagerEditor : Editor
{
    [MenuItem("GameObject/Create Other/AlgorandManager")]
    static void AddAlgorandManager()
    {
        if (GameObject.FindObjectOfType<AlgorandManager>() == null)
        {
            GameObject gameObject = new GameObject();
            gameObject.name = "AlgorandManager";
            gameObject.AddComponent<AlgorandManager>();
        }
        else
        {
            Debug.LogError("AlgorandManager already added to the scene.");
        }
    }

    public override void OnInspectorGUI()
    {
        base.DrawDefaultInspector();
    }
}
