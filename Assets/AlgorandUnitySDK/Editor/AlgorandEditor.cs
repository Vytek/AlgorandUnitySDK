using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class AlgorandEditor : EditorWindow
{
    //Vars
    public static GameObject tempObject;
    string[] options = new string[]
    {
        "MAINNET", "TESTNET", "BETANET",
    };
    Texture2D logo; //The Logo Texture

    // Add menu named "Custom Window" to the menu.
    [MenuItem("Window/Algorand Manager Editor")]
    /// <summary>
    /// Method called to show window.
    /// </summary>
    public static void ShowWindow()
    {
        // Get existing open window or if none, make a new one:
        var window = GetWindow(typeof(AlgorandEditor), false, "Algorand Manager Editor", true);
        //Minsize Unity Window Editor
        window.minSize = new Vector2(450, 300);
        window.Show();
    }

    /// <summary>
    /// Method called 
    /// </summary>
    private void OnEnable()
    {
        MonoScript script = MonoScript.FromScriptableObject(this);
        string path = AssetDatabase.GetAssetPath(script);
        path = path.Substring(0, path.Length - 17) + "algorand_full_logo_white.png";
        logo = (Texture2D)AssetDatabase.LoadAssetAtPath(path, typeof(Texture));     //Setting the Algorand logo texture
    }

    /// <summary>
    /// Unity method that renders editor window.
    /// </summary>
    private void OnGUI()
    {
        EditorGUILayout.LabelField("Algorand Manager Editor", EditorStyles.boldLabel);

        //Algorand Logo
        GUIStyle centeredStyle = GUI.skin.GetStyle("Label");    //Creating the GUIStyle to center-align the Logo
        centeredStyle.alignment = TextAnchor.UpperCenter;
        centeredStyle.fixedHeight = 100;
        GUILayout.Label(logo, centeredStyle);

        EditorGUILayout.Space();

        AlgorandSave.AlgorandURLEndpoint = EditorGUILayout.TextField("Algorand URL/Endpoint:", AlgorandSave.AlgorandURLEndpoint);
        AlgorandSave.AlgorandURLEndpoint = EditorGUILayout.TextField("Algorand URL/End.Indexer:", AlgorandSave.AlgorandURLEndpointIndexer);
        AlgorandSave.AlgorandApiKey = EditorGUILayout.TextField("Algorand API Key:", AlgorandSave.AlgorandApiKey);
        AlgorandSave.AlgorandNetwork = EditorGUILayout.Popup("Algorand Network:", AlgorandSave.AlgorandNetwork, options);

        if (AlgorandSave.AlgorandNetwork == 0)
        {
            AlgorandSave.AlgorandURLEndpoint = "https://mainnet-algorand.api.purestake.io/ps2";
            AlgorandSave.AlgorandURLEndpointIndexer = "https://mainnet-algorand.api.purestake.io/idx2";
        }
        if (AlgorandSave.AlgorandNetwork == 1)
        {
            AlgorandSave.AlgorandURLEndpoint = "https://testnet-algorand.api.purestake.io/ps2";
            AlgorandSave.AlgorandURLEndpointIndexer = "https://testnet-algorand.api.purestake.io/idx2";
        }
        if (AlgorandSave.AlgorandNetwork == 2)
        {
            AlgorandSave.AlgorandURLEndpoint = "https://betanet-algorand.api.purestake.io/ps2";
            AlgorandSave.AlgorandURLEndpointIndexer = "https://betanet-algorand.api.purestake.io/idx2";
        }

        //Help
        EditorGUILayout.HelpBox("Help: Choose the Algorand network to use and the related data to interface with the Algorand API Rest", MessageType.Info);
        EditorGUILayout.Separator();

        GUI.backgroundColor = Color.red;
        EditorGUILayout.Space();

        if (GUILayout.Button("Reset"))
        {
            AlgorandSave.AlgorandURLEndpoint = "https://testnet-algorand.api.purestake.io/ps2";
            AlgorandSave.AlgorandURLEndpointIndexer = "https://testnet-algorand.api.purestake.io/idx2";
            AlgorandSave.AlgorandApiKey = "IkwGyG4qWg8W6VegMFfCa3iIIj06wi0x6Vn7FO5j";
            AlgorandSave.AlgorandNetwork = 1;

        }

        GUI.backgroundColor = Color.white;

        if (GUILayout.Button("Save Algorand config in PlayerPrefs"))
        {
            PlayerPrefs.SetString("AlgorandAccountSDKURL", AlgorandSave.AlgorandURLEndpoint);
            PlayerPrefs.SetString("AlgorandAccountSDKURLIndexer", AlgorandSave.AlgorandURLEndpointIndexer);
            PlayerPrefs.SetString("AlgorandSDKToken", AlgorandSave.AlgorandApiKey);
            Debug.Log("Algorand Manager config Saved.");
        }

        if (GUILayout.Button("Insert Algorand Manager"))
        {
            Debug.Log("Inserting Algorand Manager...");
            InstantiateManager("AlgorandManager");
        }

        if (GUILayout.Button("Open Developer PureStake"))
        {
            Application.OpenURL("https://developer.purestake.io/");
        }
    }

    private void InstantiateManager(string objectName)
    {
        //Assets/AlgorandUnitySDK/Prefabs/AlgorandManager.prefab
        objectName = "AlgorandManager";
        string fileLocation = "Assets/AlgorandUnitySDK/Prefabs/" + objectName + ".prefab";

        // Try and load an existing prefab, if it's null then it does not exist.
        GameObject ManagerPrefab = AssetDatabase.LoadAssetAtPath(fileLocation, typeof(GameObject)) as GameObject;

        if (ManagerPrefab == null)
        {
            Object tempObject = PrefabUtility.CreateEmptyPrefab(fileLocation);
            ManagerPrefab = PrefabUtility.ReplacePrefab(new GameObject(objectName), tempObject, ReplacePrefabOptions.ConnectToPrefab);
        }

        if (ManagerPrefab != null)
        {
            PrefabUtility.InstantiatePrefab(ManagerPrefab);
            Debug.Log("Algorand Manager: inserted.");
        }
        else
        {
            Debug.LogError("Unable to insert AlgorandManager.");
        }
    }
}
