using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartGame : MonoBehaviour
{
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
            //Save Algorand Account using Passphrase in AlgorandManager Instance in encrypted PlayPrefs
            Debug.Log("Save Algorand Account in encrypted Playprefs: " + AlgorandManager.Instance.SaveAccountInPlayerPrefs(NewAccount));
        }
        else
        {
            //Load Algorand Account from encrypted PlayerPrefs
            NewAddress = AlgorandManager.Instance.LoadAccountFromPlayPrefs();
            //Show Algorand Account Address
            Debug.Log(NewAddress);
            //Get Mnemonic Algorand Account Passphrase 
            Debug.Log(AlgorandManager.Instance.GetMnemonicPassphrase());
            //Get Algorand Account Address from AlgorandManager Instances
            Debug.Log(AlgorandManager.Instance.GetAddressAccount());
            //Verify Algorand Account Address passed
            Debug.Log("Valid Algorand Address: " + AlgorandManager.Instance.AddressIsValid(NewAddress));
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
    }
}
