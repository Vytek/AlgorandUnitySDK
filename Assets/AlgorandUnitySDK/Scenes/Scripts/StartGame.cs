using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;

public class StartGame : MonoBehaviour
{
    [TextArea]
    public string NewAccount = string.Empty;
    public string NewAddress =  string.Empty;
    
    // Start is called before the first frame update
    void Start()
    {
        //Check if encrypted Playprefs is loaded
        if (!PlayerPrefs.HasKey("AlgorandAccountSDK"))
        {
            //Show AlgorandManager SDK Version
            Debug.Log("AlgorandManager SDK Version: "+AlgorandManager.Instance.Version());
            //Create new account
            NewAccount = AlgorandManager.Instance.GenerateAccount();
            //Show Mnemonic Algorand Account Passphrase
            Debug.Log(NewAccount);
            //Reload Algorand Account using Passphrase in AlgorandManager Instance
            NewAddress = AlgorandManager.Instance.LoadAccountFromPassphrase(NewAccount);
            //Show Algorand Account Address
            Debug.Log(NewAddress);
            //Get Mnemonic Algorand Account Passphrase 
            Debug.Log(AlgorandManager.Instance.GetMnemonicPassphrase());
            //Get Algorand Account Address from AlgorandManager Instances
            Debug.Log(AlgorandManager.Instance.GetAddressAccount());
            //Verify Algorand Account Address passed
            Debug.Log("Valid Algorand Address: " + AlgorandManager.Instance.AddressIsValid(NewAddress));
            //Save Algorand Account using Passphrase in AlgorandManager Instance in encrypted PlayerPrefs
            Debug.Log("Save Algorand Account in encrypted PlayerPrefs: " + AlgorandManager.Instance.SaveAccountInPlayerPrefs(NewAccount));
        }
        else
        {
            //Load Algorand Account from encrypted PlayerPrefs
            NewAddress = AlgorandManager.Instance.LoadAccountFromPlayerPrefs();
            //Show Algorand Account Address
            Debug.Log(NewAddress);
            //Get Mnemonic Algorand Account Passphrase 
            Debug.Log(AlgorandManager.Instance.GetMnemonicPassphrase());
            NewAccount = AlgorandManager.Instance.GetMnemonicPassphrase();
            //Get Algorand Account Address from AlgorandManager Instances
            Debug.Log(AlgorandManager.Instance.GetAddressAccount());
            //Verify Algorand Account Address passed
            Debug.Log("Valid Algorand Address: " + AlgorandManager.Instance.AddressIsValid(NewAddress));
            //Show URL
            Debug.Log("URL ENDPOINT: "+AlgorandManager.Instance.ALGOD_URL_ENDPOINT);
        } 
        //Connect to Node
        AlgorandManager.Instance.ConnectToNode(AlgorandManager.Instance.ALGOD_URL_ENDPOINT, AlgorandManager.Instance.ALGOD_TOKEN);
        //Get Wallet Amount
        var amountnow = AlgorandManager.Instance.GetWalletAmount(AlgorandManager.Instance.ALGOD_URL_ENDPOINT, AlgorandManager.Instance.ALGOD_TOKEN, AlgorandManager.Instance.GetAddressAccount());
        if (amountnow>1)
        {
            //Transfert Algo
            Debug.Log("TxID: "+AlgorandManager.Instance.MakePaymentTransaction(AlgorandManager.Instance.ALGOD_URL_ENDPOINT, 
            AlgorandManager.Instance.ALGOD_TOKEN,
            "KV2XGKMXGYJ6PWYQA5374BYIQBL3ONRMSIARPCFCJEAMAHQEVYPB7PL3KU", 0.01, "Test using AlgorandUnitySDK: "+AlgorandManager.Instance.Version()));
        }
        else
        {
            Debug.LogWarning("Insufficient Algo amount to make a transaction!");
        }
        //Get Healthy ( Free 1 req/ 1 sec Purestack)
        //Debug.Log("Algorand Health: "+AlgorandManager.Instance.GetHealth(AlgorandManager.Instance.ALGOD_URL_ENDPOINT_INDEXER, AlgorandManager.Instance.ALGOD_TOKEN));
        //Get Account
        /*
        Debug.Log("Account Amount (KV2XGKMXGYJ6PWYQA5374BYIQBL3ONRMSIARPCFCJEAMAHQEVYPB7PL3KU): "+
        AlgorandManager.Instance.GetAccount(AlgorandManager.Instance.ALGOD_URL_ENDPOINT_INDEXER,
        AlgorandManager.Instance.ALGOD_TOKEN,
        "KV2XGKMXGYJ6PWYQA5374BYIQBL3ONRMSIARPCFCJEAMAHQEVYPB7PL3KU"), false);
        */
        //GetAsset
        /*
        var jsonResult = AlgorandManager.Instance.GetAsset(AlgorandManager.Instance.ALGOD_URL_ENDPOINT_INDEXER, 
            AlgorandManager.Instance.ALGOD_TOKEN,
            15187601);
        //https://wiki.unity3d.com/index.php/SimpleJSON
        var N = JSON.Parse(jsonResult);
        //Show Asset Total Example
        Debug.Log("Asset Total: "+N["asset"]["params"]["total"]);
        //Show Creator Example
        Debug.Log("Creator: "+N["asset"]["params"]["creator"]);
        */
        //Search last 5 Transactions
        var jsonResult = AlgorandManager.Instance.SearchTransactions(AlgorandManager.Instance.ALGOD_URL_ENDPOINT_INDEXER, 
            AlgorandManager.Instance.ALGOD_TOKEN, "KV2XGKMXGYJ6PWYQA5374BYIQBL3ONRMSIARPCFCJEAMAHQEVYPB7PL3KU");
        //https://wiki.unity3d.com/index.php/SimpleJSON
        var N = JSON.Parse(jsonResult);
        //Debug.Log(jsonResult);
        //Show Current Round Example
        Debug.Log("Current Round: "+N["current-round"]);
        //Assets
    }
}
