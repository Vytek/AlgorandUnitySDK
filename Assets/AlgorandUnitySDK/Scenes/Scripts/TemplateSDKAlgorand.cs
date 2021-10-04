using UnityEngine;

public class TemplateSDKAlgorand : MonoBehaviour
{
    [TextArea]
    public string NewAccount = string.Empty;
    public string NewAddress = string.Empty;
    //Internal Password
    private const string _InternalPassword = "0sIhlNRkMfDH8J9cC0Ky";
    
    // Start is called before the first frame update
    void Start()
    {
        //Check if encrypted Playprefs is loaded
        if (!PlayerPrefs.HasKey("AlgorandAccountSDK"))
        {
            string TestPassPhrase = "course claim donor embark section tribe latin quiz few version solid pizza thought snake often mandate brand act useful reopen speak palace disorder ability amount";
            //Load Algorand Account from encrypted PlayerPrefs
            AlgorandManager.Instance.LoadAccountFromPassphrase(TestPassPhrase);
            NewAddress = AlgorandManager.Instance.GetAddressAccount();
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
            //SaveData
            AlgorandManager.Instance.SaveAccountInPlayerPrefs(NewAccount);
        }
        else
        {
            //Delete all PlayPrefs
            AlgorandManager.Instance.DeleteAccountFromPlayerPrefs();
            string TestPassPhrase = "course claim donor embark section tribe latin quiz few version solid pizza thought snake often mandate brand act useful reopen speak palace disorder ability amount";
            //Load Algorand Account from encrypted PlayerPrefs
            AlgorandManager.Instance.LoadAccountFromPassphrase(TestPassPhrase);
            NewAddress = AlgorandManager.Instance.GetAddressAccount();
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
            //SaveData
            AlgorandManager.Instance.SaveAccountInPlayerPrefs(NewAccount);
        }
    }
}
