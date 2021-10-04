using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System;
using UnityEngine;
using Random = System.Random;
using Scrypt;

public class StartGameCryptoStore : MonoBehaviour
{
    [TextArea]
    public string NewAccount = string.Empty;
    public string NewAddress =  string.Empty;

    public string PasswordGUIToUse = "This is a test!";
    
    public static byte[] GetBytes(string input)
    {
        var byteArray = new byte[input.Length * sizeof(char)];
        Buffer.BlockCopy(input.ToCharArray(), 0, byteArray, 0, byteArray.Length);
        return byteArray;
    }

    public static string GetString(byte[] byteArray)
    {
        var characters = new char[byteArray.Length / sizeof(char)];
        Buffer.BlockCopy(byteArray, 0, characters, 0, byteArray.Length);
        return new string(characters);
    }


    // Start is called before the first frame update
    void CreateStart()
    {
        ScryptEncoder encoder = new ScryptEncoder();

        string hashsedPassword = encoder.Encode("mypassword");
        
        // This is the hash algorithm to use
        var algorithm = HashAlgorithmName.SHA256;

        // ikm, info, and salt can be variable length - for an example we're just going to generate some random bytes
        /*
        var rng = new Random();
        var ikm = new byte[32];
        var salt = new byte[32];
        var info = new byte[4];
        rng.NextBytes(ikm);
        rng.NextBytes(salt);
        rng.NextBytes(info);
        */
        var l = 0;
        var ikm = GetBytes(hashsedPassword);
        var salt = GetBytes("12345678901234567890123456789012");
        var info = GetBytes("algo");

        // Our HKDF can be calculated as follows.
        var hkdf = new HKDF(algorithm, ikm, info, l, salt);
        var hash = hkdf.hash;
        Debug.Log(Convert.ToBase64String(hash, 0, hash.Length));

        /*
        var ikm = StringToByteArray("0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b");
        var salt = StringToByteArray("000102030405060708090a0b0c");
        var info = StringToByteArray("f0f1f2f3f4f5f6f7f8f9");
        var L = 42;
        */

        var StringToCrypt = GetBytes("This is a test");

        //Crypt using AESGCM
        var crypted = AESGCM.AesGcmEncrypt(StringToCrypt, hash);
        Debug.Log(Convert.ToBase64String(crypted, 0, crypted.Length));
        var cryptednew = Convert.FromBase64String(Convert.ToBase64String(crypted, 0, crypted.Length));
        //Decrypt using AESGCM
        //Verify all ok in crypt and decrypt and from this code create function for AlgorandManager
        var decrypted = AESGCM.AesGcmDecrypt(cryptednew, hash);
        Debug.Log(Convert.ToBase64String(decrypted, 0, decrypted.Length));
        Debug.Log(GetString(decrypted));
    }

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
            Debug.Log("Save Algorand Account in encrypted PlayerPrefs: " + AlgorandManager.Instance.SaveAccountInPlayerPrefs(AlgorandManager.Instance.GetPrivateKey(), PasswordGUIToUse));
        }
        else
        {
            //Load Algorand Account from encrypted PlayerPrefs
            NewAddress = AlgorandManager.Instance.LoadAccountFromPlayerPrefs(PasswordGUIToUse);
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
    }
}
