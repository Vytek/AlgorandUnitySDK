using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityCipher;

//ALGORAND
using Algorand;
using Algorand.V2;
using Algorand.Client;
using Algorand.V2.Model;
using Account = Algorand.Account;

public class AlgorandManager : Singleton<AlgorandManager>
{
    [SerializeField]
    protected string m_PlayerName;
    protected string _Version = "0.6 Alfa";
    protected Account _AMAccount = null;
    private const string _InternalPassword = "0sIhlNRkMfDH8J9cC0Ky";

    [SerializeField]
    public string ALGOD_URL_ENDPOINT = string.Empty;
    [SerializeField]
    public string ALGOD_TOKEN = string.Empty;
    [SerializeField]
    public string ALGOD_URL_ENPOINT_INDEXER = string.Empty;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        Debug.Log("Starting Algorand Manager...");
        this.ALGOD_URL_ENDPOINT = PlayerPrefs.GetString("AlgorandAccountSDKURL", "https://testnet-algorand.api.purestake.io/ps2");
        this.ALGOD_TOKEN = PlayerPrefs.GetString("AlgorandSDKToken", "IkwGyG4qWg8W6VegMFfCa3iIIj06wi0x6Vn7FO5j");
        this.ALGOD_URL_ENPOINT_INDEXER = PlayerPrefs.GetString("AlgorandAccountSDKURLIndexer", "https://testnet-algorand.api.purestake.io/idx2");
#if !(DEVELOPMENT_BUILD || UNITY_EDITOR)
        	Debug.unityLogger.filterLogType = LogType.Exception;
#endif
    }

    /// <summary>
    /// Get AlgorandSDK Version
    /// </summary>
    /// <returns>AlgorandDSK Version</returns>
	public string Version()
    {
        return _Version;
    }
    public string GetPlayerName()
    {
        return m_PlayerName;
    }
    protected virtual void OnApplicationQuit()
    {
        Debug.Log("Algorand Manager stopped.");

    }

    //Publics Methods

    /// <summary>
    /// Generate new Algorand Account but not saved in Playprefs
    /// </summary>
    /// <returns>Algorand Account Mnemonic Passphrase</returns>
    public string GenerateAccount()
    {
        Account _Account = new Account();
        return _Account.ToMnemonic().ToString();
    }

    /// <summary>
    /// Load Algorand Account from Mnemonic Passphrase
    /// </summary>
    /// <param name="Passphrase"></param>
    /// <returns>Algorand Account Address</returns>
    public string LoadAccountFromPassphrase(string Passphrase)
    {
        if (_AMAccount == null)
        {
            _AMAccount = new Account(Passphrase);
            return _AMAccount.Address.ToString();
        }
        else
        {
            return _AMAccount.Address.ToString();
        }
    }

    /// <summary>
    /// Generate a new Algorand Account, save crypted in PlayerPrefs and in AlgorandManager instance
    /// </summary>
    /// <returns>Algorand Account Address</returns>
    public string GenerateAccountAndSave()
    {
        if (_AMAccount == null)
        {
            _AMAccount = new Account();
            //Save encrypted Mnemonic Algorand Account in PlayPrefs
            if (!PlayerPrefs.HasKey("AlgorandAccountSDK"))
            {
                PlayerPrefs.SetString("AlgorandAccountSDK", RijndaelEncryption.Encrypt(_AMAccount.ToMnemonic().ToString(), SystemInfo.deviceUniqueIdentifier + _InternalPassword));
                return _AMAccount.Address.ToString();
            }
            else
            {
                Debug.LogError("There is already an account saved in PlayerPrefs!");
                throw new InvalidOperationException("There is already an account saved in PlayerPrefs!");
            }
        }
        else
        {
            return _AMAccount.Address.ToString();
        }
    }

    /// <summary>
    /// Save Algorand Account in encrypted PlayPrefs
    /// </summary>
    /// <param name="Passphrase"></param>
    /// <returns>True if saved</returns>
    public Boolean SaveAccountInPlayerPrefs(string Passphrase)
    {
        if (!String.IsNullOrEmpty(Passphrase))
        {
            //Save encrypted Mnemonic Algorand Account in PlayPrefs
            if (!PlayerPrefs.HasKey("AlgorandAccountSDK"))
            {
                PlayerPrefs.SetString("AlgorandAccountSDK", RijndaelEncryption.Encrypt(Passphrase, SystemInfo.deviceUniqueIdentifier + _InternalPassword));
                return true;
            }
            else
            {
                Debug.LogError("There is already an account saved in PlayerPrefs!");
                throw new InvalidOperationException("There is already an account saved in PlayerPrefs!");
            }
        }
        else
        {
            Debug.LogError("Passphrase passed is Null or empty!");
            throw new InvalidOperationException("Passphrase passed is Null or empty!");
        }
    }

    /// <summary>
    /// Load Account from PlayPrefs and use in Algorand Manager instance
    /// </summary>
    /// <returns>Algorand Account Address saved in PlayPrefs</returns>
    public string LoadAccountFromPlayPrefs()
    {
        //Load encrypted Mnemonic Algorand Account from PlayPrefs
        if (PlayerPrefs.HasKey("AlgorandAccountSDK"))
        {
            if (_AMAccount == null)
            {
                _AMAccount = new Account(RijndaelEncryption.Decrypt(PlayerPrefs.GetString("AlgorandAccountSDK"), SystemInfo.deviceUniqueIdentifier + _InternalPassword));
                return _AMAccount.Address.ToString();
            }
            else
            {
                Debug.LogError("There is already an account loaded!");
                throw new InvalidOperationException("There is already an account loaded!");
            }
        }
        else
        {
            Debug.LogError("PlayPrefs does not contain a saved Algorand Account");
            throw new InvalidOperationException("PlayPrefs does not contain a saved Algorand Account");
        }
    }

    /// <summary>
    /// Get Actual Account Address initialized in AlgorandManager
    /// </summary>
    /// <returns>Algorand Account Address</returns>
	public string GetAddressAccount()
    {

        if (_AMAccount != null)
        {
            return _AMAccount.Address.ToString();
        }
        else
        {
            Debug.LogError("Account not generated yet!");
            throw new InvalidOperationException("Account not generated yet!");
        }
    }

    /// <summary>
    /// Get Actual Mnemonic Passphrase initialized in AlgorandManager
    /// </summary>
    /// <returns>Algorand Account Mnemonic Passphrase</returns>
	public string GetMnemonicPassphrase()
    {
        if (_AMAccount != null)
        {
            return _AMAccount.ToMnemonic().ToString();
        }
        else
        {
            Debug.LogError("Account not generated yet!");
            throw new InvalidOperationException("Account not generated yet!");
        }
    }

    /// <summary>
    /// Verify if Algorand Address is well formated
    /// </summary>
    /// <param name="AddressPassed"></param>
    /// <returns>Simple Boolean: True or False</returns>
    public bool AddressIsValid(string AddressPassed)
    {
        if (Address.IsValid(AddressPassed))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Connect to ALGOD / Purestack Node
    /// </summary>
    public void ConnectToNode (string AlgodURLEndpoint, string AlgodToken)
    {
        AlgodApi algodApiInstance = new AlgodApi(AlgodURLEndpoint, AlgodToken);
    
        try
        {
            var supply = algodApiInstance.GetSupply();
            Debug.Log("Total Algorand Supply: " + supply.TotalMoney);
            Debug.Log("Online Algorand Supply: " + supply.OnlineMoney);
        }

        catch (ApiException e)
        {
            Debug.LogError("Exception when calling algod#getSupply: " + e.Message);
        }
    }
    
    /// <summary>
    /// Get Wallet Amount in MicroAlgos
    /// </summary>
    /// <param name="AlgodURLEndpoint">URL/Endpoint Algod</param>
    /// <param name="AlgodToken">API Key token</param>
    /// <param name="AccountAddress">Algorand Address</param>
    /// <returns>MicroAlgos of Algorand Account</returns>
    public long? GetWalletAmount (string AlgodURLEndpoint, string AlgodToken, string AccountAddress)
    {
        AlgodApi algodApiInstance = new AlgodApi(AlgodURLEndpoint, AlgodToken);            

        try
        {
            var accountInfo = algodApiInstance.AccountInformation(AccountAddress);
            Debug.Log(string.Format("Account Balance: {0} microAlgos", accountInfo.Amount));
            return accountInfo.Amount;
        }

        catch (ApiException e)
        {
            Debug.LogError("Exception when calling algod#AccountInformation: " + e.Message);
            return 0;
        }
    }
    
    /// <summary>
    /// Create and send a payment Algorand Transaction
    /// </summary>
    /// <param name="AlgodURLEndpoint">URL/Endpoint Algod</param>
    /// <param name="AlgodToken">API Key token</param>
    /// <param name="ToAccountAddress">Algorand Address Received Algos</param>
    /// <param name="AlgoAmount">Amount Algo to send</param>
    /// <param name="Note">Note to insert in transaction max 1000 Bytes</param>
    /// <returns>TxID: Transaction ID</returns>
    public string MakePaymentTransaction (string AlgodURLEndpoint, string AlgodToken, string ToAccountAddress, double AlgoAmount, string Note)
    {
        AlgodApi algodApiInstance = new AlgodApi(AlgodURLEndpoint, AlgodToken);

        TransactionParametersResponse transParams;
        try
        {
            transParams = algodApiInstance.TransactionParams();
        }
        catch (ApiException e)
        {
            Debug.LogError("Could not get params: "+ e.Message);
            throw new Exception("Could not get params", e);
        }
        var amount = Utils.AlgosToMicroalgos(AlgoAmount);
        var tx = Utils.GetPaymentTransaction(_AMAccount.Address, new Address(ToAccountAddress), amount, Note, transParams);
        var signedTx = _AMAccount.SignTransaction(tx);

        Debug.Log("Signed transaction with txid: " + signedTx.transactionID);

        PostTransactionsResponse id = null;
        // send the transaction to the network
        try
        {
            id = Utils.SubmitTransaction(algodApiInstance, signedTx);
            Debug.Log("Successfully sent tx with id: " + id.TxId);
            var resp = Utils.WaitTransactionToComplete(algodApiInstance, id.TxId);
            Debug.Log("Confirmed Round is: " + resp.ConfirmedRound);
        }
        catch (ApiException e)
        {
            // This is generally expected, but should give us an informative error message.
            Debug.LogError("Exception when calling algod#rawTransaction: " + e.Message);
        }
        Debug.Log("Algorand transaction to Player completed.");
        return id.TxId;
    }
}