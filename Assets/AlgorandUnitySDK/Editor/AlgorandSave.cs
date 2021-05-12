/*
 * Copyright (c) GT50 S.r.l
 * Algorand Save Config Editor
 */

#if UNITY_EDITOR
using UnityEditor;
#endif

public class AlgorandSave
{
    public static string AlgorandURLEndpoint
    {
        get
        {
#if UNITY_EDITOR
            return EditorPrefs.GetString("AlgorandURLEndpoint", "https://testnet-algorand.api.purestake.io/ps2");
#else
            return false;
#endif
        }

        set
        {
#if UNITY_EDITOR
            EditorPrefs.SetString("AlgorandURLEndpoint", value);
#endif
        }
    }

    public static string AlgorandURLEndpointIndexer
    {
        get
        {
#if UNITY_EDITOR
            return EditorPrefs.GetString("AlgorandURLEndpointIndexer", "https://testnet-algorand.api.purestake.io/idx2");
#else
            return false;
#endif
        }

        set
        {
#if UNITY_EDITOR
            EditorPrefs.SetString("AlgorandURLEndpointIndexer", value);
#endif
        }
    }

    public static string AlgorandApiKey
    {
        get
        {
#if UNITY_EDITOR
            return EditorPrefs.GetString("AlgorandApiKey", "IkwGyG4qWg8W6VegMFfCa3iIIj06wi0x6Vn7FO5j");
#else
            return false;
#endif
        }

        set
        {
#if UNITY_EDITOR
            EditorPrefs.SetString("AlgorandApiKey", value);
#endif
        }
    }


    public static int AlgorandNetwork
    {
        get
        {
#if UNITY_EDITOR
            return EditorPrefs.GetInt("AlgorandNetwork", 1);
#else
            return false;
#endif
        }

        set
        {
#if UNITY_EDITOR
            EditorPrefs.SetInt("AlgorandNetwork", value);
#endif
        }
    }
}