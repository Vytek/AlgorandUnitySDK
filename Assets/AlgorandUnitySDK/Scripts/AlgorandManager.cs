/*
MIT License

Copyright (c) 2021 enrico.speranza@gt50.org

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;
using UnityEngine;
using UnityCipher;

//ALGORAND
using Algorand;
using Algorand.V2;
using Algorand.Client;
using Algorand.V2.Model;
using Account = Algorand.Account;

//Scrypt
using Scrypt;

public class AlgorandManager : Singleton<AlgorandManager>
{
    [Header("Player Configuration:")]
    [SerializeField]
    protected string m_PlayerName;
    protected string _Version = "0.18 Alfa";
    protected Account _AMAccount = null;
    private const string _InternalPassword = "0sIhlNRkMfDH8J9cC0Ky";

    [Header("Algorand Configuration:")]
    [Tooltip("ALGOD/PureStake URL Endpoint")]
    [SerializeField]
    public string ALGOD_URL_ENDPOINT = string.Empty;
    [Tooltip("ALGOD/PureStake Token")]
    [SerializeField]
    public string ALGOD_TOKEN = string.Empty;
    [Tooltip("INDEXER/PureStake URL Endpoint")]
    [SerializeField]
    public string ALGOD_URL_ENDPOINT_INDEXER = string.Empty;

    // OnEnable is called before Start
    protected virtual void OnEnable()
    {
        Debug.Log("Starting Algorand Manager...");
        this.ALGOD_URL_ENDPOINT = PlayerPrefs.GetString("AlgorandAccountSDKURL", "https://testnet-algorand.api.purestake.io/ps2");
        this.ALGOD_TOKEN = PlayerPrefs.GetString("AlgorandSDKToken", "IkwGyG4qWg8W6VegMFfCa3iIIj06wi0x6Vn7FO5j");
        this.ALGOD_URL_ENDPOINT_INDEXER = PlayerPrefs.GetString("AlgorandAccountSDKURLIndexer", "https://testnet-algorand.api.purestake.io/idx2");
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

    /// <summary>
    /// Get Actual Player Name
    /// </summary>
    /// <returns>Player Name</returns>
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
    /// <returns>Algorand Account Address generated</returns>
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
    /// Generate a new Algorand Account, save crypted in PlayerPrefs and in AlgorandManager instance crypted with Password by User
    /// </summary>
    /// <param name="Password">Password passed from UI by User</param>
    /// <returns>Algorand Account Address generated</returns>
    public string GenerateAccountAndSave(string Password)
    {
        if (_AMAccount == null)
        {
            _AMAccount = new Account();
            //Save encrypted Mnemonic Algorand Account in PlayPrefs
            if (!PlayerPrefs.HasKey("AlgorandAccountSDK"))
            {
                if (!String.IsNullOrEmpty(Password))
                {
                    //Save hash                       
                    ScryptEncoder encoder = new ScryptEncoder();
                    PlayerPrefs.SetString("AlgorandAccountSDKHash", encoder.Encode(Password));

                    PlayerPrefs.SetString("AlgorandAccountSDK", RijndaelEncryption.Encrypt(_AMAccount.ToMnemonic().ToString(), Password + SystemInfo.deviceUniqueIdentifier + _InternalPassword));
                    return _AMAccount.Address.ToString();
                }
                else
                {
                    Debug.LogError("Password passed is Null or empty!");
                    throw new InvalidOperationException("Password passed is Null or empty!");
                }
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
    /// <param name="Passphrase">Mnemonic Algorand Account</param>
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
    /// Save Algorand Account in encrypted PlayPrefs crypted with Password by User
    /// </summary>
    /// <param name="Passphrase">Mnemonic Algorand Account</param>
    /// <param name="Password">Password passed from UI by User</param>
    /// <returns>True if saved</returns>
    public Boolean SaveAccountInPlayerPrefs(string Passphrase, string Password)
    {
        if (!String.IsNullOrEmpty(Passphrase))
        {
            //Save encrypted Mnemonic Algorand Account in PlayPrefs
            if (!PlayerPrefs.HasKey("AlgorandAccountSDK"))
            {
                //Save Hash using Scrypt
                if (!PlayerPrefs.HasKey("AlgorandAccountSDKHash"))
                {
                    if (!String.IsNullOrEmpty(Password))
                    {
                        //Save hash                       
                        ScryptEncoder encoder = new ScryptEncoder();
                        PlayerPrefs.SetString("AlgorandAccountSDKHash", encoder.Encode(Password));

                        //Crypt saved password
                        PlayerPrefs.SetString("AlgorandAccountSDK", RijndaelEncryption.Encrypt(Passphrase, Password + SystemInfo.deviceUniqueIdentifier + _InternalPassword));
                        return true;
                    }
                    else
                    {
                        Debug.LogError("Password passed is Null or empty!");
                        throw new InvalidOperationException("Password passed is Null or empty!");
                    }
                }
                else
                {
                    Debug.LogError("There is already an account hash saved in PlayerPrefs!");
                    throw new InvalidOperationException("There is already an account hash saved in PlayerPrefs!");
                }
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
    public string LoadAccountFromPlayerPrefs()
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
    /// Load Account from PlayPrefs and use in Algorand Manager instance decrypted with Password by User
    /// </summary>
    /// <param name="Password">Password passed from UI by User</param>
    /// <returns>Algorand Account Address saved in PlayPrefs</returns>
    public string LoadAccountFromPlayerPrefs(string Password)
    {
        //Load encrypted Mnemonic Algorand Account from PlayPrefs
        if (PlayerPrefs.HasKey("AlgorandAccountSDK"))
        {
            if (_AMAccount == null)
            {
                if (!String.IsNullOrEmpty(Password))
                {
                    if (PlayerPrefs.HasKey("AlgorandAccountSDKHash"))
                    {
                        //Check if password is correct!
                        ScryptEncoder encoder = new ScryptEncoder();
                        if (encoder.Compare(Password, PlayerPrefs.GetString("AlgorandAccountSDKHash")) == true)
                        {
                            _AMAccount = new Account(RijndaelEncryption.Decrypt(PlayerPrefs.GetString("AlgorandAccountSDK"), Password + SystemInfo.deviceUniqueIdentifier + _InternalPassword));
                            return _AMAccount.Address.ToString();
                        }
                        else
                        {
                            Debug.LogError("Entered password is incorrect!");
                            throw new InvalidOperationException("Entered password is incorrect!");
                        }
                    }
                    else
                    {
                        Debug.LogError("There is already an account hash saved in PlayerPrefs!");
                        throw new InvalidOperationException("There is already an account hash saved in PlayerPrefs!");
                    }
                }
                else
                {
                    Debug.LogError("Password passed is Null or empty!");
                    throw new InvalidOperationException("Password passed is Null or empty!");
                }
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
    /// Delete actual Algorand Account from PlayerPrefs
    /// WARNING: this method will irrevocably delete your account from PlayerPrefs!
    /// </summary>
    /// <returns>Boolean true if procedure went ok</returns>
    public bool DeleteAccountFromPlayerPrefs()
    {
        if (PlayerPrefs.HasKey("AlgorandAccountSDK"))
        {
            PlayerPrefs.DeleteKey("AlgorandAccountSDK");
            //Delete always hash too (If the key does not exist, DeleteKey has no impact.)
            PlayerPrefs.DeleteKey("AlgorandAccountSDKHash");
            return true;
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
    public void ConnectToNode(string AlgodURLEndpoint, string AlgodToken)
    {
        if (string.IsNullOrEmpty(AlgodURLEndpoint) || string.IsNullOrEmpty(AlgodToken))
        {
            Debug.LogError("AlgodURLEndpoint or AlgodToken are null or empty!");
            throw new ArgumentException("AlgodURLEndpoint or AlgodToken are null or empty!");
        }
        else
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
    }

    /// <summary>
    /// Get Wallet Amount in MicroAlgos
    /// </summary>
    /// <param name="AlgodURLEndpoint">URL/Endpoint Algod</param>
    /// <param name="AlgodToken">API Key token</param>
    /// <param name="AccountAddress">Algorand Address</param>
    /// <returns>MicroAlgos of Algorand Account</returns>
    public long? GetWalletAmount(string AlgodURLEndpoint, string AlgodToken, string AccountAddress)
    {
        if (string.IsNullOrEmpty(AlgodURLEndpoint) || string.IsNullOrEmpty(AlgodToken))
        {
            Debug.LogError("AlgodURLEndpoint or AlgodToken are null or empty!");
            throw new ArgumentException("AlgodURLEndpoint or AlgodToken are null or empty!");
        }
        else
        {
            AlgodApi algodApiInstance = new AlgodApi(AlgodURLEndpoint, AlgodToken);

            if (this.AddressIsValid(AccountAddress))
            {
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
            else
            {
                Debug.LogError("Passed AlgorandAccount is invalid!");
                throw new ArgumentException("Passed AlgorandAccount is invalid!");
            }
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
    public string MakePaymentTransaction(string AlgodURLEndpoint, string AlgodToken, string ToAccountAddress, double AlgoAmount, string Note)
    {
        if (string.IsNullOrEmpty(AlgodURLEndpoint) || string.IsNullOrEmpty(AlgodToken))
        {
            Debug.LogError("AlgodURLEndpoint or AlgodToken are null or empty!");
            throw new ArgumentException("AlgodURLEndpoint or AlgodToken are null or empty!");
        }
        else
        {
            //Check passed Arguments and internal objects
            if (_AMAccount != null)
            {
                Debug.Log("Algorand Account: " + _AMAccount.Address.ToString());
            }
            else
            {
                Debug.LogError("Account not generated yet!");
                throw new InvalidOperationException("Account not generated yet!");
            }
            if (Note.Length > 1000)
            {
                Debug.LogError("Note must be a maximum of 1000 bytes!");
                throw new ArgumentException("Note must be a maximum of 1000 bytes!", "Note");
            }
            if (!this.AddressIsValid(ToAccountAddress))
            {
                Debug.LogError("Passed ToAccountAddress is invalid!");
                throw new ArgumentException("Passed ToAccountAddress is invalid!");
            }
            else
            {
                AlgodApi algodApiInstance = new AlgodApi(AlgodURLEndpoint, AlgodToken);

                TransactionParametersResponse transParams;
                try
                {
                    transParams = algodApiInstance.TransactionParams();
                }
                catch (ApiException e)
                {
                    Debug.LogError("Could not get params: " + e.Message);
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
    }

    /// <summary>
    /// Get https://developer.algorand.org/docs/reference/rest-apis/indexer/#get-health
    /// </summary>
    /// <param name="AlgodURLEndpointIndexer">URL/Endpoint Algod Indexer</param>
    /// <param name="AlgodToken">API Key token</param>
    /// <returns>Message from Aldgorand Indexer</returns>
    public string GetHealth(string AlgodURLEndpointIndexer, string AlgodToken)
    {
        if (string.IsNullOrEmpty(AlgodURLEndpointIndexer) || string.IsNullOrEmpty(AlgodToken))
        {
            Debug.LogError("AlgodURLEndpointIndexer or AlgodToken are null or empty!");
            throw new ArgumentException("AlgodURLEndpointIndexer or AlgodToken are null or empty!");
        }
        else
        {
            IndexerApi indexer = new IndexerApi(AlgodURLEndpointIndexer, AlgodToken);
            HealthCheck health = null;
            try
            {
                health = indexer.MakeHealthCheck();
                Debug.Log("Make Health Check: " + health.ToJson());
            }
            catch (ApiException e)
            {
                // This is generally expected, but should give us an informative error message.
                Debug.LogError("Exception when calling Indexer#GetHealth: " + e.Message);
            }
            return health.Message;
        }
    }

    /// <summary>
    /// Get https://developer.algorand.org/docs/reference/rest-apis/indexer/#get-v2accountsaccount-id
    /// </summary>
    /// <param name="AlgodURLEndpointIndexer">URL/Endpoint Algod Indexer</param>
    /// <param name="AlgodToken">API Key token</param>
    /// <param name="AlgorandAccount">Valid Algorand Account Address</param>
    /// <param name="JsonOrString">Booleand to return Json (True, default) Or String (False)</param>
    /// <returns>Amount Account or JSON Account Info</returns>
    public string GetAccount(string AlgodURLEndpointIndexer, string AlgodToken, string AlgorandAccount, bool JsonOrString = true)
    {
        if (string.IsNullOrEmpty(AlgodURLEndpointIndexer) || string.IsNullOrEmpty(AlgodToken) || string.IsNullOrEmpty(AlgorandAccount))
        {
            Debug.LogError("AlgodURLEndpointIndexer or AlgodToken or AlgorandAccount are null or empty!");
            throw new ArgumentException("AlgodURLEndpointIndexer or AlgodToken or AlgorandAccount are null or empty!");
        }
        else
        {
            if (this.AddressIsValid(AlgorandAccount))
            {
                IndexerApi indexer = new IndexerApi(AlgodURLEndpointIndexer, AlgodToken);
                AccountResponse acctInfo = null;
                try
                {
                    acctInfo = indexer.LookupAccountByID(AlgorandAccount);
                    Debug.Log("Account Info: " + acctInfo.ToJson());
                }
                catch (ApiException e)
                {
                    // This is generally expected, but should give us an informative error message.
                    Debug.LogError("Exception when calling Indexer#GetAccount: " + e.Message);
                }
                if (JsonOrString)
                {
                    return acctInfo.ToJson();
                }
                else
                {
                    return acctInfo.Account.Amount.ToString();
                }
            }
            else
            {
                Debug.LogError("Passed AlgorandAccount is invalid!");
                throw new ArgumentException("Passed AlgorandAccount is invalid!");
            }
        }
    }

    /// <summary>
    /// Get https://developer.algorand.org/docs/reference/rest-apis/indexer/#get-v2assetsasset-id
    /// </summary>
    /// <param name="AlgodURLEndpointIndexer">URL/Endpoint Algod Indexer</param>
    /// <param name="AlgodToken">API Key token</param>
    /// <param name="AssetID">Algorand Asset ID</param>
    /// <param name="JsonOrString">Booleand to return Json (True, default) Or String (False)</param>
    /// <returns>Complete JSON response or single string Index</returns>
    public string GetAsset(string AlgodURLEndpointIndexer, string AlgodToken, long? AssetID, bool JsonOrString = true)
    {
        if (string.IsNullOrEmpty(AlgodURLEndpointIndexer) || string.IsNullOrEmpty(AlgodToken) || (!AssetID.HasValue))
        {
            Debug.LogError("AlgodURLEndpointIndexer or AlgodToken or AssetID are null or empty!");
            throw new ArgumentException("AlgodURLEndpointIndexer or AlgodToken or AssetID are null or empty!");
        }
        else
        {
            IndexerApi indexer = new IndexerApi(AlgodURLEndpointIndexer, AlgodToken);
            AssetResponse assetInfo = null;
            try
            {
                assetInfo = indexer.LookupAssetByID(AssetID);
                Debug.Log("Asset Info: " + assetInfo.ToJson());
            }
            catch (ApiException e)
            {
                // This is generally expected, but should give us an informative error message.
                Debug.LogError("Exception when calling Indexer#GetAsset: " + e.Message);
            }
            if (JsonOrString)
            {
                return assetInfo.ToJson();
            }
            else
            {
                return assetInfo.Asset.Index.ToString();
            }
        }
    }

    /// <summary>
    /// Get Asset info from PureStack see Issue https://github.com/RileyGe/dotnet-algorand-sdk/issues/22
    /// </summary>
    /// <param name="AlgodURLEndpointIndexer">URL/Endpoint Algod Indexer</param>
    /// <param name="AlgodToken">API Key token</param>
    /// <param name="AssetID">Algorand Asset ID</param>
    /// <returns>Asset object</returns>
    Asset GetAssetFromPureStack(string AlgodURLEndpointIndexer, string AlgodToken, long? AssetID)
    {
        if (string.IsNullOrEmpty(AlgodURLEndpointIndexer) || string.IsNullOrEmpty(AlgodToken))
        {
            Debug.LogError("AlgodURLEndpointIndexer or AlgodToken or AssetID are null or empty!");
            throw new ArgumentException("AlgodURLEndpointIndexer or AlgodToken or AssetID are null or empty!");
        }
        else
        {
            IndexerApi indexer = new IndexerApi(AlgodURLEndpointIndexer, AlgodToken);
            AssetResponse assetInfo = null;
            try
            {
                assetInfo = indexer.LookupAssetByID(AssetID);
                Debug.Log("Asset Info for: " + assetInfo.Asset.Index.ToString());
            }
            catch (ApiException e)
            {
                // This is generally expected, but should give us an informative error message.
                Debug.LogError("Exception when calling Indexer#GetAsset: " + e.Message);
            }
            return assetInfo.Asset;
        }
    }

    /// <summary>
    /// Get https://developer.algorand.org/docs/reference/rest-apis/indexer/#get-v2accountsaccount-idtransactions
    /// </summary>
    /// <param name="AlgodURLEndpointIndexer">URL/Endpoint Algod Indexer</param>
    /// <param name="AlgodToken">API Key token</param>
    /// <param name="AlgorandAccount"></param>
    /// <returns>Structured result in JSON format</returns>
    public string SearchTransactions(string AlgodURLEndpointIndexer, string AlgodToken, string AlgorandAccount)
    {
        if (string.IsNullOrEmpty(AlgodURLEndpointIndexer) || string.IsNullOrEmpty(AlgodToken) || string.IsNullOrEmpty(AlgorandAccount))
        {
            Debug.LogError("AlgodURLEndpointIndexer or AlgodToken or AlgorandAccount are null or empty!");
            throw new ArgumentException("AlgodURLEndpointIndexer or AlgodToken or AlgorandAccount are null or empty!");
        }
        else
        {
            if (this.AddressIsValid(AlgorandAccount))
            {
                IndexerApi indexer = new IndexerApi(AlgodURLEndpointIndexer, AlgodToken);
                TransactionsResponse transInfos = null;
                try
                {
                    transInfos = indexer.LookupAccountTransactions(AlgorandAccount, 5);
                    //Debug.Log("Transactions Info: " + transInfos.ToJson());
                }
                catch (ApiException e)
                {
                    // This is generally expected, but should give us an informative error message.
                    Debug.LogError("Exception when calling Indexer#SearchTransactions: " + e.Message);
                }
                //Return
                return transInfos.ToJson();
            }
            else
            {
                Debug.LogError("Passed AlgorandAccount is invalid!");
                throw new ArgumentException("Passed AlgorandAccount is invalid!");
            }
        }
    }
    //Manage Algorand Assets
    /// <summary>
    /// Create Asset https://developer.algorand.org/docs/features/asa/
    /// </summary>
    /// <param name="AlgodURLEndpoint">URL/Endpoint Algod</param>
    /// <param name="AlgodToken">API Key token</param>
    /// <param name="AssetName">The name of the asset. Supplied on creation. Example: Tether</param>
    /// <param name="AssetUnitName">The name of a unit of this asset. Supplied on creation. Example: USDT</param>
    /// <param name="TotalAssetCount">The total number of base units of the asset to create. This number cannot be changed.</param>
    /// <param name="AssetDecimals">The number of digits to use after the decimal point when displaying the asset. If 0, the asset is not divisible. If 1, the base unit of the asset is in tenths. If 2, the base unit of the asset is in hundredths</param>
    /// <param name="AssetURL">Specifies a URL where more information about the asset can be retrieved. Max size is 32 bytes.</param>
    /// <param name="AssetmetadataHash">This field is intended to be a 32-byte hash of some metadata that is relevant to your asset and/or asset holders. The format of this metadata is up to the application. This field can only be specified upon creation. An example might be the hash of some certificate that acknowledges the digitized asset as the official representation of a particular real-world asset.</param>
    /// <param name="AssetTxMessage">Message to insert in creation transaction. Max size is 1000 bytes.</param>
    /// <returns>AssetID created</returns>
    public string CreateAsset(string AlgodURLEndpoint, string AlgodToken, string AssetName, string AssetUnitName,
    ulong? TotalAssetCount, long? AssetDecimals, string AssetURL, string AssetmetadataHash, string AssetTxMessage)
    {
        if (string.IsNullOrEmpty(AlgodURLEndpoint) || string.IsNullOrEmpty(AlgodToken))
        {
            Debug.LogError("AlgodURLEndpoint or AlgodToken are null or empty!");
            throw new ArgumentException("AlgodURLEndpoint or AlgodToken are null or empty!");
        }
        else
        {
            AlgodApi algodApiInstance = new AlgodApi(AlgodURLEndpoint, AlgodToken);

            TransactionParametersResponse transParams;
            try
            {
                transParams = algodApiInstance.TransactionParams();
            }
            catch (ApiException e)
            {
                Debug.LogError("Could not get params: " + e.Message);
                throw new Exception("Could not get params", e);
            }

            //Check passed Arguments and internal objects
            if (_AMAccount != null)
            {
                Debug.Log("Algorand Account: " + _AMAccount.Address.ToString());
            }
            else
            {
                Debug.LogError("Account not generated yet!");
                throw new InvalidOperationException("Account not generated yet!");
            }
            if (string.IsNullOrEmpty(AssetName))
            {
                Debug.LogError("AssetName are null or empty!");
                throw new ArgumentException("AssetName are null or empty!", "AssetName");
            }
            if (string.IsNullOrEmpty(AssetUnitName))
            {
                Debug.LogError("AssetUnitName are null or empty!");
                throw new ArgumentException("AssetName are null or empty!", "AssetUnitName");
            }
            if (!TotalAssetCount.HasValue)
            {
                Debug.LogError("TotalAssetCount has no value!");
                throw new ArgumentException("TotalAssetCount has no value!", "TotalAssetCount");
            }
            if (!AssetDecimals.HasValue)
            {
                Debug.LogError("AssetDecimals has no value!");
                throw new ArgumentException("AssetDecimals has no value!", "AssetDecimals");
            }
            if (String.IsNullOrEmpty(AssetURL))
            {
                Debug.LogError("AssetURL are null or empty!");
                throw new ArgumentException("AssetURL has no value!", "AssetURL");
            }
            if (AssetURL.Length > 32)
            {
                Debug.LogError("AssetURL must be a maximum of 32 bytes!");
                throw new ArgumentException("AssetURL must be a maximum of 32 bytes!", "AssetURL");
            }
            if (string.IsNullOrEmpty(AssetmetadataHash))
            {
                Debug.LogError("AssetmetadataHash are null or empty!");
                throw new ArgumentException("AssetmetadataHash has no value!", "AssetmetadataHash");
            }
            if (AssetmetadataHash.Length > 32)
            {
                Debug.LogError("AssetmetadataHash must be a maximum of 32 bytes!");
                throw new ArgumentException("AssetmetadataHash must be a maximum of 32 bytes!", "AssetmetadataHash");
            }
            if (string.IsNullOrEmpty(AssetTxMessage))
            {
                Debug.LogError("AssetTxMessage are null or empty!");
                throw new ArgumentException("AssetTxMessage has no value!", "AssetTxMessage");
            }
            if (AssetTxMessage.Length > 1000)
            {
                Debug.LogError("AssetTxMessage must be a maximum of 1000 bytes!");
                throw new ArgumentException("AssetTxMessage must be a maximum of 1000 bytes!", "AssetTxMessage");
            }
            // Create the Asset
            // Total number of this asset available for circulation            
            var ap = new AssetParams(creator: _AMAccount.Address.ToString(), name: AssetName, unitName: AssetUnitName, total: TotalAssetCount,
                decimals: AssetDecimals, url: AssetURL, metadataHash: Encoding.ASCII.GetBytes(AssetmetadataHash));
            // Specified address can change reserve, freeze, clawback, and manager
            // you can leave as default, by default the sender will be manager/reserve/freeze/clawback
            // the following code only set the freeze to acct1
            var tx = Utils.GetCreateAssetTransaction(ap, transParams, AssetTxMessage);

            // Sign the Transaction by sender
            SignedTransaction signedTx = _AMAccount.SignTransaction(tx);
            // send the transaction to the network and
            // wait for the transaction to be confirmed
            long? assetID = 0;
            try
            {
                var id = Utils.SubmitTransaction(algodApiInstance, signedTx);
                Debug.Log("Transaction ID: " + id.TxId);
                Debug.Log("Confirmed Round is: " +
                    Utils.WaitTransactionToComplete(algodApiInstance, id.TxId).ConfirmedRound);
                // Now that the transaction is confirmed we can get the assetID
                var ptx = algodApiInstance.PendingTransactionInformation(id.TxId);
                assetID = ptx.AssetIndex;
            }
            catch (Exception e)
            {
                Debug.LogError(e.StackTrace);
            }
            Debug.Log("Algorand transaction to Create Asset completed.");
            return assetID.ToString();
        }
    }
    //Modify asset config
    /// <summary>
    /// Modify Asset https://developer.algorand.org/docs/features/asa/#modifying-an-asset
    /// </summary>
    /// <param name="AlgodURLEndpoint">URL/Endpoint Algod</param>
    /// <param name="AlgodToken">API Key token</param>
    /// <param name="AddressManager">Modified Algorand Manager Address</param>
    /// <param name="AddressFreeze">Modified Algorand Freeze Address</param>
    /// <param name="AddressClawback">Modified Algorand Clawback Address</param>
    /// <param name="AddressReserve">Modified Algorand Reserve Address</param>
    /// <param name="AssetTxMessage">Message to insert in creation transaction. Max size is 1000 bytes.</param>
    /// <param name="AssetID">AssetID on which to make changes</param>
    /// <returns>Transaction ID (TxID)</returns>
    public string ModifyAsset(string AlgodURLEndpoint, string AlgodToken, string AddressManager, string AddressFreeze,
    string AddressClawback, string AddressReserve, string AssetTxMessage, long? AssetID)
    {
        if (string.IsNullOrEmpty(AlgodURLEndpoint) || string.IsNullOrEmpty(AlgodToken))
        {
            Debug.LogError("AlgodURLEndpoint or AlgodToken are null or empty!");
            throw new ArgumentException("AlgodURLEndpoint or AlgodToken are null or empty!");
        }
        else
        {
            //Check passed Arguments and internal objects
            if (_AMAccount != null)
            {
                Debug.Log("Algorand Account: " + _AMAccount.Address.ToString());
            }
            else
            {
                Debug.LogError("Account not generated yet!");
                throw new InvalidOperationException("Account not generated yet!");
            }
            //Check Address
            if (!this.AddressIsValid(AddressManager))
            {
                Debug.LogError("Passed AddressManager is invalid!");
                throw new ArgumentException("Passed AddressManager is invalid!");
            }
            if (!this.AddressIsValid(AddressClawback))
            {
                Debug.LogError("Passed AddressClawback is invalid!");

                throw new ArgumentException("Passed AddressClawback is invalid!");
            }
            if (!this.AddressIsValid(AddressFreeze))
            {
                Debug.LogError("Passed AddressFreeze is invalid!");
                throw new ArgumentException("Passed AddressFreeze is invalid!");
            }
            if (!this.AddressIsValid(AddressReserve))
            {
                Debug.LogError("Passed AddressReserve is invalid!");
                throw new ArgumentException("Passed AddressReserve is invalid!");
            }
            //Others Arguments
            if (AssetTxMessage.Length > 1000)
            {
                Debug.LogError("AssetTxMessage must be a maximum of 1000 bytes!");
                throw new ArgumentException("AssetTxMessage must be a maximum of 1000 bytes!", "AssetTxMessage");
            }
            if (!AssetID.HasValue)
            {
                Debug.LogError("AssetID has no value!");
                throw new ArgumentException("AssetID has no value!", "AssetID");
            }
            AlgodApi algodApiInstance = new AlgodApi(AlgodURLEndpoint, AlgodToken);

            TransactionParametersResponse transParams;
            try
            {
                transParams = algodApiInstance.TransactionParams();
            }
            catch (ApiException e)
            {
                Debug.LogError("Could not get params: " + e.Message);
                throw new Exception("Could not get params", e);
            }
            // Change Asset Configuration:
            // Next we will change the asset configuration
            // First we update standard Transaction parameters
            // To account for changes in the state of the blockchain
            transParams = algodApiInstance.TransactionParams();

            Algorand.Transaction tx = null;
            if (!AlgodURLEndpoint.Contains("algorand.api.purestake.io"))
            {
                Asset ast = algodApiInstance.GetAssetByID(AssetID);

                //Modify Asset
                ast.Params.Manager = AddressManager;
                ast.Params.Freeze = AddressFreeze;
                ast.Params.Clawback = AddressClawback;
                ast.Params.Reserve = AddressReserve;
                tx = Utils.GetConfigAssetTransaction(_AMAccount.Address, ast, transParams, AssetTxMessage);
            }
            else
            {
                Asset ast = this.GetAssetFromPureStack(this.ALGOD_URL_ENDPOINT_INDEXER, AlgodToken, AssetID);
                Debug.Log("Asset: " + ast.ToJson());

                //Modify Asset
                ast.Params.Manager = AddressManager;
                ast.Params.Freeze = AddressFreeze;
                ast.Params.Clawback = AddressClawback;
                ast.Params.Reserve = AddressReserve;
                //Debug.Log("AssetName: "+ ast.Params.Name);
                tx = Utils.GetConfigAssetTransaction(_AMAccount.Address, ast, transParams, AssetTxMessage);
            }

            var signedTx = _AMAccount.SignTransaction(tx);
            // send the transaction to the network and
            // wait for the transaction to be confirmed
            PostTransactionsResponse id = null;
            try
            {
                id = Utils.SubmitTransaction(algodApiInstance, signedTx);
                Debug.Log("Transaction ID: " + id.TxId);
                Debug.Log("Confirmed Round is: " +
                    Utils.WaitTransactionToComplete(algodApiInstance, id.TxId).ConfirmedRound);
            }
            catch (Exception e)
            {
                //e.printStackTrace();
                Debug.LogError(e.StackTrace);
            }
            /*
            // Next we will list the newly created asset
            // Get the asset information for the newly changed asset            
            ast = algodApiInstance.GetAssetByID(AssetID);
            */
            Debug.Log("Algorand transaction to Modify Asset completed.");
            return id.TxId;
        }
    }
    //Asset Opt-in
    /// <summary>
    /// Opt-in Asset https://developer.algorand.org/docs/features/asa/#receiving-an-asset
    /// </summary>
    /// <param name="AlgodURLEndpoint">URL/Endpoint Algod</param>
    /// <param name="AlgodToken">API Key token</param>
    /// <param name="AssetID">Asset ID to Opt-in</param>
    /// <param name="Note">Opt-In Transaction Note</param>
    /// <returns>Transaction ID (TxID)</returns>
    public string OptinAsset(string AlgodURLEndpoint, string AlgodToken, long? AssetID, string Note = "")
    {
        if (string.IsNullOrEmpty(AlgodURLEndpoint) || string.IsNullOrEmpty(AlgodToken))
        {
            Debug.LogError("AlgodURLEndpoint or AlgodToken are null or empty!");
            throw new ArgumentException("AlgodURLEndpoint or AlgodToken are null or empty!");
        }
        else
        {
            //Check passed Arguments and internal objects
            if (_AMAccount != null)
            {
                Debug.Log("Algorand Account: " + _AMAccount.Address.ToString());
            }
            else
            {
                Debug.LogError("Account not generated yet!");
                throw new InvalidOperationException("Account not generated yet!");
            }
            if (!AssetID.HasValue)
            {
                Debug.LogError("AssetID has no value!");
                throw new ArgumentException("AssetID has no value!", "AssetID");
            }
            if (Note.Length > 1000)
            {
                Debug.LogError("Note must be a maximum of 1000 bytes!");
                throw new ArgumentException("Note must be a maximum of 1000 bytes!", "Note");
            }

            AlgodApi algodApiInstance = new AlgodApi(AlgodURLEndpoint, AlgodToken);

            TransactionParametersResponse transParams;
            try
            {
                transParams = algodApiInstance.TransactionParams();
            }
            catch (ApiException e)
            {
                Debug.LogError("Could not get params: " + e.Message);
                throw new Exception("Could not get params", e);
            }

            var tx = Utils.GetAssetOptingInTransaction(_AMAccount.Address, AssetID, transParams, Note);
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
            Debug.Log("Algorand transaction to Opt-in Asset completed.");
            return id.TxId;
        }
    }
    //Asset Transfert
    /// <summary>
    /// Create a transaction ASA https://developer.algorand.org/docs/features/asa/#transferring-an-asset
    /// </summary>
    /// <param name="AlgodURLEndpoint">URL/Endpoint Algod</param>
    /// <param name="AlgodToken">API Key token</param>
    /// <param name="ToAccountAddress">Algorand Address Received ASA</param>
    /// <param name="AssetAmount">Amount ASA to send</param>
    /// <param name="AssetID">Asset ID to transfer</param>
    /// <param name="Note">Trnasfert Transaction Note</param>
    /// <returns>Transaction ID (TxID)</returns>
    public string AssetTransfer(string AlgodURLEndpoint, string AlgodToken, string ToAccountAddress,
    ulong AssetAmount, long? AssetID, string Note = "")
    {
        if (string.IsNullOrEmpty(AlgodURLEndpoint) || string.IsNullOrEmpty(AlgodToken))
        {
            Debug.LogError("AlgodURLEndpoint or AlgodToken are null or empty!");
            throw new ArgumentException("AlgodURLEndpoint or AlgodToken are null or empty!");
        }
        else
        {
            //Check passed Arguments and internal objects
            if (_AMAccount != null)
            {
                Debug.Log("Algorand Account: " + _AMAccount.Address.ToString());
            }
            else
            {
                Debug.LogError("Account not generated yet!");
                throw new InvalidOperationException("Account not generated yet!");
            }
            if (!AssetID.HasValue)
            {
                Debug.LogError("AssetID has no value!");
                throw new ArgumentException("AssetID has no value!", "AssetID");
            }
            if (Note.Length > 1000)
            {
                Debug.LogError("Note must be a maximum of 1000 bytes!");
                throw new ArgumentException("Note must be a maximum of 1000 bytes!", "Note");
            }
            if (!this.AddressIsValid(ToAccountAddress))
            {
                Debug.LogError("Passed ToAccountAddress is invalid!");
                throw new ArgumentException("Passed ToAccountAddress is invalid!");
            }

            AlgodApi algodApiInstance = new AlgodApi(AlgodURLEndpoint, AlgodToken);

            TransactionParametersResponse transParams;
            try
            {
                transParams = algodApiInstance.TransactionParams();
            }
            catch (ApiException e)
            {
                Debug.LogError("Could not get params: " + e.Message);
                throw new Exception("Could not get params", e);
            }
            //public static Transaction GetTransferAssetTransaction(Address from, Address to, ulong? assetId, ulong amount, TransactionParams trans, Address closeTo = null, string message = "");
            //public static Transaction GetTransferAssetTransaction(Address from, Address to, long? assetId, ulong amount, TransactionParametersResponse trans, Address closeTo = null, string message = "");
            //Address assetCloseTo = new Address();
            var tx = Utils.GetTransferAssetTransaction(_AMAccount.Address, new Address(ToAccountAddress), AssetID, AssetAmount, transParams, null, Note);
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
            Debug.Log("Algorand transaction to Transfert Asset completed.");
            return id.TxId;
        }
    }
    //Asset Freeze
    /// <summary>
    /// Freeze Asset https://developer.algorand.org/docs/features/asa/#freezing-an-asset
    /// </summary>
    /// <param name="AlgodURLEndpoint">URL/Endpoint Algod</param>
    /// <param name="AlgodToken">API Key token</param>
    /// <param name="AddressFreeze">Algorand Address Freeze Target</param>
    /// <param name="AssetID">Asset ID to transfer</param>
    /// <param name="Note">Trnasfert Transaction Note</param>
    /// <returns>Transaction ID (TxID)</returns>
    public string FreezeAsset(string AlgodURLEndpoint, string AlgodToken, string AddressFreeze, long? AssetID, string Note = "")
    {
        if (string.IsNullOrEmpty(AlgodURLEndpoint) || string.IsNullOrEmpty(AlgodToken))
        {
            Debug.LogError("AlgodURLEndpoint or AlgodToken are null or empty!");
            throw new ArgumentException("AlgodURLEndpoint or AlgodToken are null or empty!");
        }
        else
        {
            //Check passed Arguments and internal objects
            if (_AMAccount != null)
            {
                Debug.Log("Algorand Account: " + _AMAccount.Address.ToString());
            }
            else
            {
                Debug.LogError("Account not generated yet!");
                throw new InvalidOperationException("Account not generated yet!");
            }
            if (!AssetID.HasValue)
            {
                Debug.LogError("AssetID has no value!");
                throw new ArgumentException("AssetID has no value!", "AssetID");
            }
            if (Note.Length > 1000)
            {
                Debug.LogError("Note must be a maximum of 1000 bytes!");
                throw new ArgumentException("Note must be a maximum of 1000 bytes!", "Note");
            }
            if (!this.AddressIsValid(AddressFreeze))
            {
                Debug.LogError("Passed AddressFreeze is invalid!");
                throw new ArgumentException("Passed AddressFreeze is invalid!");
            }

            AlgodApi algodApiInstance = new AlgodApi(AlgodURLEndpoint, AlgodToken);

            TransactionParametersResponse transParams;
            try
            {
                transParams = algodApiInstance.TransactionParams();
            }
            catch (ApiException e)
            {
                Debug.LogError("Could not get params: " + e.Message);
                throw new Exception("Could not get params", e);
            }
            var tx = Utils.GetFreezeAssetTransaction(_AMAccount.Address, new Address(AddressFreeze), AssetID, true, transParams, Note);
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
            Debug.Log("Algorand transaction to Freeze Asset completed.");
            return id.TxId;
        }
    }
    //Asset Revoke
    /// <summary>
    /// Revoke Asset https://developer.algorand.org/docs/features/asa/#revoking-an-asset
    /// </summary>
    /// <param name="AlgodURLEndpoint">URL/Endpoint Algod</param>
    /// <param name="AlgodToken">API Key token</param>
    /// <param name="AddressRevoke">Algorand Address to revoke target</param>
    /// <param name="AssetAmount">Amount ASA to revoke</param>
    /// <param name="AssetID">Asset ID to transfer</param>
    /// <param name="Note">Trnasfert Transaction Note</param>
    /// <returns>Transaction ID (TxID)</returns>
    public string RevokeAsset(string AlgodURLEndpoint, string AlgodToken, string AddressRevoke, ulong AssetAmount, long? AssetID, string Note = "")
    {
        if (string.IsNullOrEmpty(AlgodURLEndpoint) || string.IsNullOrEmpty(AlgodToken))
        {
            Debug.LogError("AlgodURLEndpoint or AlgodToken are null or empty!");
            throw new ArgumentException("AlgodURLEndpoint or AlgodToken are null or empty!");
        }
        else
        {
            //Check passed Arguments and internal objects
            if (_AMAccount != null)
            {
                Debug.Log("Algorand Account: " + _AMAccount.Address.ToString());
            }
            else
            {
                Debug.LogError("Account not generated yet!");
                throw new InvalidOperationException("Account not generated yet!");
            }
            if (!AssetID.HasValue)
            {
                Debug.LogError("AssetID has no value!");
                throw new ArgumentException("AssetID has no value!", "AssetID");
            }
            if (Note.Length > 1000)
            {
                Debug.LogError("Note must be a maximum of 1000 bytes!");
                throw new ArgumentException("Note must be a maximum of 1000 bytes!", "Note");
            }
            if (!this.AddressIsValid(AddressRevoke))
            {
                Debug.LogError("Passed AddressRevoke is invalid!");
                throw new ArgumentException("Passed AddressRevoke is invalid!");
            }

            AlgodApi algodApiInstance = new AlgodApi(AlgodURLEndpoint, AlgodToken);

            TransactionParametersResponse transParams;
            try
            {
                transParams = algodApiInstance.TransactionParams();
            }
            catch (ApiException e)
            {
                Debug.LogError("Could not get params: " + e.Message);
                throw new Exception("Could not get params", e);
            }
            var tx = Utils.GetRevokeAssetTransaction(_AMAccount.Address, new Address(AddressRevoke), _AMAccount.Address, AssetID, AssetAmount, transParams, Note);
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
            Debug.Log("Algorand transaction to Revoke Asset completed.");
            return id.TxId;
        }
    }
    //Asset Destroy
    /// <summary>
    /// Destroy Asset https://developer.algorand.org/docs/features/asa/#destroying-an-asset
    /// </summary>
    /// <param name="AlgodURLEndpoint">URL/Endpoint Algod</param>
    /// <param name="AlgodToken">API Key token</param>
    /// <param name="AssetID">Asset ID to Opt-in</param>
    /// <param name="Note">Opt-In Transaction Note</param>
    /// <returns>Transaction ID (TxID)</returns>
    public string DestroyAsset(string AlgodURLEndpoint, string AlgodToken, long? AssetID, string Note = "")
    {
        if (string.IsNullOrEmpty(AlgodURLEndpoint) || string.IsNullOrEmpty(AlgodToken))
        {
            Debug.LogError("AlgodURLEndpoint or AlgodToken are null or empty!");
            throw new ArgumentException("AlgodURLEndpoint or AlgodToken are null or empty!");
        }
        else
        {
            //Check passed Arguments and internal objects
            if (_AMAccount != null)
            {
                Debug.Log("Algorand Account: " + _AMAccount.Address.ToString());
            }
            else
            {
                Debug.LogError("Account not generated yet!");
                throw new InvalidOperationException("Account not generated yet!");
            }
            if (!AssetID.HasValue)
            {
                Debug.LogError("AssetID has no value!");
                throw new ArgumentException("AssetID has no value!", "AssetID");
            }
            if (Note.Length > 1000)
            {
                Debug.LogError("Note must be a maximum of 1000 bytes!");
                throw new ArgumentException("Note must be a maximum of 1000 bytes!", "Note");
            }

            AlgodApi algodApiInstance = new AlgodApi(AlgodURLEndpoint, AlgodToken);

            TransactionParametersResponse transParams;
            try
            {
                transParams = algodApiInstance.TransactionParams();
            }
            catch (ApiException e)
            {
                Debug.LogError("Could not get params: " + e.Message);
                throw new Exception("Could not get params", e);
            }

            var tx = Utils.GetDestroyAssetTransaction(_AMAccount.Address, AssetID, transParams, Note);
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
            Debug.Log("Algorand transaction to Destroy Asset completed.");
            return id.TxId;
        }
    }

    //Rekey Transaction
    /// <summary>
    /// Rekey-to Transaction¶ https://developer.algorand.org/docs/features/accounts/rekey/
    /// </summary>
    /// <param name="AlgodURLEndpoint">URL/Endpoint Algod</param>
    /// <param name="AlgodToken">API Key token</param>
    /// <param name="ToReKeyAccountAddress">Algorand valid Address to rekey</param>
    /// <param name="Note">Rekey Transaction note</param>
    /// <returns>Transaction ID (TxID)</returns>
    public string MakeReKeyTransaction(string AlgodURLEndpoint, string AlgodToken, string ToReKeyAccountAddress, string Note)
    {
        if (string.IsNullOrEmpty(AlgodURLEndpoint) || string.IsNullOrEmpty(AlgodToken))
        {
            Debug.LogError("AlgodURLEndpoint or AlgodToken are null or empty!");
            throw new ArgumentException("AlgodURLEndpoint or AlgodToken are null or empty!");
        }
        else
        {
            //Check passed Arguments and internal objects
            if (_AMAccount != null)
            {
                Debug.Log("Algorand Account: " + _AMAccount.Address.ToString());
            }
            else
            {
                Debug.LogError("Account not generated yet!");
                throw new InvalidOperationException("Account not generated yet!");
            }
            if (Note.Length > 1000)
            {
                Debug.LogError("Note must be a maximum of 1000 bytes!");
                throw new ArgumentException("Note must be a maximum of 1000 bytes!", "Note");
            }
            if (!this.AddressIsValid(ToReKeyAccountAddress))
            {
                Debug.LogError("Passed ToReKeyAccountAddress is invalid!");
                throw new ArgumentException("Passed ToReKeyAccountAddress is invalid!");
            }
            else
            {
                AlgodApi algodApiInstance = new AlgodApi(AlgodURLEndpoint, AlgodToken);

                TransactionParametersResponse transParams;
                try
                {
                    transParams = algodApiInstance.TransactionParams();
                }
                catch (ApiException e)
                {
                    Debug.LogError("Could not get params: " + e.Message);
                    throw new Exception("Could not get params", e);
                }
                var amount = Utils.AlgosToMicroalgos(0.001);
                var tx = Utils.GetPaymentTransaction(_AMAccount.Address, _AMAccount.Address, amount, Note, transParams);
                tx.RekeyTo = new Address(ToReKeyAccountAddress);
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
                Debug.Log("Algorand transaction to ReKey Address completed.");
                return id.TxId;
            }
        }
    }
    //SmartContract
}