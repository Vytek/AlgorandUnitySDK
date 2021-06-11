using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using MHLab.Utilities;

[RequireComponent(typeof(AudioSource))]
public class CoinColliderBlueNew : MonoBehaviour
{
    public AudioClip CoinSound;
    public BackgroundTasksProcessor Processor;
    private AudioSource _audioSource;
    private string TxIDPayment = string.Empty;
    public GameObject tempTXT;
    private long? Amount = 0;
    // Start is called before the first frame update
    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!Processor.IsReady)
        {
            return;
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.tag == "AlgorandCoin")
        {
            Destroy(col.gameObject);
            //Play Sound
            _audioSource.clip = CoinSound;
            _audioSource.Play();
            Debug.Log("AlgorandCoin Collider!");
            //Doing Transaction
            //Using BackgroundTasksProcessor
            Processor.Process(
                () =>
                {
                    //Do here Algorand Transaction
                    TxIDPayment = AlgorandManager.Instance.MakePaymentTransaction(
                        AlgorandManager.Instance.ALGOD_URL_ENDPOINT,
                        AlgorandManager.Instance.ALGOD_TOKEN,
                        "F52PF5E2GNMUZN2JYPXS4ANMXUY23F6RVE6VEJH4ZZYHMDUZPYUFKWYX6Q",
                        0.01,
                        "Test Tx for Payment: " + DateTime.Now.ToString()
                    );
                    Debug.Log("TxID: " + TxIDPayment);
                    //Update Balance
                    Amount = AlgorandManager.Instance.GetWalletAmount(
                        AlgorandManager.Instance.ALGOD_URL_ENDPOINT,
                        AlgorandManager.Instance.ALGOD_TOKEN,
                        "F52PF5E2GNMUZN2JYPXS4ANMXUY23F6RVE6VEJH4ZZYHMDUZPYUFKWYX6Q"
                    );
                    return Amount;
                },
                (result) =>
                {
                    Debug.Log("Amount Blue: " + result);
                    //Update Blue Balance
                    tempTXT.GetComponent<StartGameTest>().AmountBlue = Amount;
                }
            );
        }
    }
}
