using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
public class StartAssetGame : MonoBehaviour
{
    public string NewAccount = string.Empty;
    public string NewAddress = string.Empty;

    // Start is called before the first frame update
    void Start()
    {
        //Check if encrypted Playprefs is loaded
        if (!PlayerPrefs.HasKey("AlgorandAccountSDK"))
        {
            Debug.LogError("Nothing to do here ;-) PlayerPrefs AlgorandAccountSDK NOT saved.");
        }
        else
        {
            //Load Algorand Account from encrypted PlayerPrefs
            NewAddress = AlgorandManager.Instance.LoadAccountFromPlayPrefs();
            //Show Algorand Account Address
            Debug.Log(NewAddress);
            //Get Mnemonic Algorand Account Passphrase 
            Debug.Log(AlgorandManager.Instance.GetMnemonicPassphrase());
            NewAccount = AlgorandManager.Instance.GetMnemonicPassphrase();
            //Get Algorand Account Address from AlgorandManager Instances
            Debug.Log(AlgorandManager.Instance.GetAddressAccount());
            //Verify Algorand Account Address passed
            Debug.Log("Valid Algorand Address: " + AlgorandManager.Instance.AddressIsValid(NewAddress));
            //Show URL ENDPOINT ALGOD
            Debug.Log("URL ENDPOINT: " + AlgorandManager.Instance.ALGOD_URL_ENDPOINT);
            //Show URL ENDPOINT INDEXER
            Debug.Log("URL ENPOINT INDEXER: " + AlgorandManager.Instance.ALGOD_URL_ENDPOINT_INDEXER);
            //Show Token Used
            Debug.Log("Token Used: " + AlgorandManager.Instance.ALGOD_TOKEN);
            //Create Asset
            /*
            var AssetIDCreated = AlgorandManager.Instance.CreateAsset(
                AlgorandManager.Instance.ALGOD_URL_ENDPOINT,
                AlgorandManager.Instance.ALGOD_TOKEN,
                "AlgorandUnitySDKTest",
                "AUSDKT",
                10,
                0,
                "http://this.test.com",
                "16efaa3924a6fd9d3a4880099a4ac65d", //https://github.com/RileyGe/dotnet-algorand-sdk/issues/23#issuecomment-843929304
                "Test Tx for AlgorandUnitySDK: " + DateTime.Now.ToString()
            );
            */
            //Modify Asset (NOT WORKING)
            //** WARNING **: See Error: https://github.com/RileyGe/dotnet-algorand-sdk/issues/22
            /*
            var TxIDAssetMod = AlgorandManager.Instance.ModifyAsset(
                AlgorandManager.Instance.ALGOD_URL_ENDPOINT,
                AlgorandManager.Instance.ALGOD_TOKEN,
                NewAddress,
                NewAddress,
                NewAddress,
                NewAddress,
                "Test Tx for AlgorandUnitySDK2: "+DateTime.Now.ToString(),
                15856936
            );
            Debug.Log("Asset Modiy TxID: "+TxIDAssetMod);
            */
            /*
            Debug.Log("Starting Asset Opt-In");
            //Optin Asset (15206859)
            var TxIDAssetOptin = AlgorandManager.Instance.OptinAsset(
                AlgorandManager.Instance.ALGOD_URL_ENDPOINT,
                AlgorandManager.Instance.ALGOD_TOKEN,
                15206859,
                "Test Tx for AssetOptin: " + DateTime.Now.ToString()
            );
            Debug.Log("Asset Opt-in: " + TxIDAssetOptin);
            //Destroy Asset
            var TxIDAssetDestroyed = AlgorandManager.Instance.DestroyAsset(
                AlgorandManager.Instance.ALGOD_URL_ENDPOINT,
                AlgorandManager.Instance.ALGOD_TOKEN,
                Convert.ToInt32(AssetIDCreated),
                "Test Tx for AlgorandUnitySDK2: "+DateTime.Now.ToString()
            );
            Debug.Log("Asset Destroyed: "+TxIDAssetDestroyed);
            */
            //Asset Transfert
            //Example: https://goalseeker.purestake.io/algorand/testnet/transaction/CZAPOUEQQVEG7TNV4DEMU2YZTTOCAAY74MN5OLIFF7T6CT2W5YNQ
            var TxIDAssetTransfert = AlgorandManager.Instance.AssetTransfer(
                AlgorandManager.Instance.ALGOD_URL_ENDPOINT,
                AlgorandManager.Instance.ALGOD_TOKEN,
                "PVT67ZSBADU5ATXRIYBRIDBWSOIJOJJR73FJPCUFSKPHXI4M7PIRS5SRRI",
                1,
                15856936,
                "Test Tx for Asset Transfert: "+DateTime.Now.ToString()
            );
            Debug.Log("Asset Transfert: "+TxIDAssetTransfert);
        }
    }
}
