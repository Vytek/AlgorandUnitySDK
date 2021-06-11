using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StartGameTest : MonoBehaviour
{
    [TextArea]
    public string NewAccount = string.Empty;
    public string NewAddress = string.Empty;
    [SerializeField] public TextMeshPro mTextRed;
    [SerializeField] public TextMeshPro mTextBlue;
    public long? AmountRed = 0;
    public long? AmountBlue = 0;
    // Start is called before the first frame update
    void Start()
    {
        //Count GameObject in Tracker List
        Debug.Log("Mumbers GameObjects: " + GameObjectTracker.AllGameObjects.Count);
        //Index a GameObject in List
        Debug.Log ("GameObject Name: " + GameObjectTracker.AllGameObjects [0].gameObject.name);  
        Debug.Log ("GameObject Name: " + GameObjectTracker.AllGameObjects [1].gameObject.name);
        Debug.Log ("GameObject Name: " + GameObjectTracker.AllGameObjects [2].gameObject.name);
        Debug.Log ("GameObject Name: " + GameObjectTracker.AllGameObjects [3].gameObject.name);
        //TextMeshPro mText = this.PlayerME.transform.Find("Head").gameObject.GetComponentInChildren<TextMeshPro>();
        //mText.text = this.AvatarName + " ; " + UID;
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
            mTextRed.SetText(AmountRed.ToString());
            Debug.Log("Text: "+ mTextRed.text); 
            //mTextBlue =  GameObjectTracker.AllGameObjects [1].gameObject.GetComponent<TextMeshPro>();
            mTextBlue.SetText(AmountBlue.ToString());
            Debug.Log("Text: "+ mTextBlue.text); 
            UnityThreadQueue.Instance.Enqueue(() =>
            {
                //Update Red Balance
                AmountRed = AlgorandManager.Instance.GetWalletAmount(
                    AlgorandManager.Instance.ALGOD_URL_ENDPOINT,
                    AlgorandManager.Instance.ALGOD_TOKEN,
                    "PVT67ZSBADU5ATXRIYBRIDBWSOIJOJJR73FJPCUFSKPHXI4M7PIRS5SRRI"
                );
                //Update blue Balance
                AmountBlue = AlgorandManager.Instance.GetWalletAmount(
                    AlgorandManager.Instance.ALGOD_URL_ENDPOINT,
                    AlgorandManager.Instance.ALGOD_TOKEN,
                    "F52PF5E2GNMUZN2JYPXS4ANMXUY23F6RVE6VEJH4ZZYHMDUZPYUFKWYX6Q"
                );
                Debug.Log("Amount Blue: "+AmountBlue);
                Debug.Log("Amount Red: "+AmountRed);
            });
        }  
    }
    void Update()
    {
        //if ((AmountRed == 0) || (AmountBlue == 0))
        {
            mTextRed.text = AmountRed.ToString();
            mTextBlue.text = AmountBlue.ToString();
        } 
    }
}
