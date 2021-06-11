using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using SimpleJSON;
using TMPro;

public class StartGameASATest : MonoBehaviour
{
    [TextArea]
    public string NewAccount = string.Empty;
    public string NewAddress = string.Empty;
    [SerializeField] public TextMeshPro mTextGreen;
    [SerializeField] public TextMeshPro mTextYellow;
    public long? AmountGreen = 0;
    public long? AmountYellow = 0;

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
            //Show URL ENDPOINT ALGOD
            Debug.Log("URL ENDPOINT: " + AlgorandManager.Instance.ALGOD_URL_ENDPOINT);
            //Show URL ENDPOINT INDEXER
            Debug.Log("URL ENPOINT INDEXER: " + AlgorandManager.Instance.ALGOD_URL_ENDPOINT_INDEXER);
            //Show Token Used
            Debug.Log("Token Used: " + AlgorandManager.Instance.ALGOD_TOKEN);
            //Get Balances Account
            mTextGreen.SetText(AmountGreen.ToString());
            Debug.Log("Text: " + mTextGreen.text);
            mTextYellow.SetText(AmountYellow.ToString());
            Debug.Log("Text: " + mTextYellow.text);
            //Show Asset amount for: https://testnet.algoexplorer.io/asset/16038175
            UnityThreadQueue.Instance.Enqueue(() =>
            {
                //Green
                var jsonResult = AlgorandManager.Instance.GetAccount(AlgorandManager.Instance.ALGOD_URL_ENDPOINT_INDEXER,
                AlgorandManager.Instance.ALGOD_TOKEN, "PVT67ZSBADU5ATXRIYBRIDBWSOIJOJJR73FJPCUFSKPHXI4M7PIRS5SRRI");
                //Debug.Log(jsonResult);
                var N = JSON.Parse(jsonResult);
                //Show Asset Total Example
                Debug.Log("Total Amount for user Green: "+N["account"]["assets"][6]["amount"]);
                AmountGreen = N["account"]["assets"][6]["amount"];
                //PAUSE for PureStake limitation
                Thread.Sleep(2000);
                //Yellow
                jsonResult = AlgorandManager.Instance.GetAccount(AlgorandManager.Instance.ALGOD_URL_ENDPOINT_INDEXER,
                AlgorandManager.Instance.ALGOD_TOKEN, "F52PF5E2GNMUZN2JYPXS4ANMXUY23F6RVE6VEJH4ZZYHMDUZPYUFKWYX6Q");
                N = JSON.Parse(jsonResult);
                //Show Asset Total Example
                Debug.Log("Total Amount for user Yellow: "+N["account"]["assets"][3]["amount"]);
                AmountYellow = N["account"]["assets"][3]["amount"];
            });
        }
    }

    // Update is called once per frame
    void Update()
    {
        {
            mTextGreen.text = "T:" + AmountGreen.ToString();
            mTextYellow.text = "T:" + AmountYellow.ToString();
        }
    }
}
