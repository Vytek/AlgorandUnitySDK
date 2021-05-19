using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using MHLab.Utilities;

[RequireComponent(typeof(AudioSource))]
public class CoinColliderRedNew : MonoBehaviour
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
                        "PVT67ZSBADU5ATXRIYBRIDBWSOIJOJJR73FJPCUFSKPHXI4M7PIRS5SRRI",
                        0.01,
                        "Test Tx for Payment: " + DateTime.Now.ToString()
                    );
                    Debug.Log("TxID: " + TxIDPayment);
                    //Update Balance
                    Amount = AlgorandManager.Instance.GetWalletAmount(
                        AlgorandManager.Instance.ALGOD_URL_ENDPOINT,
                        AlgorandManager.Instance.ALGOD_TOKEN,
                        "PVT67ZSBADU5ATXRIYBRIDBWSOIJOJJR73FJPCUFSKPHXI4M7PIRS5SRRI"
                    );
                    return Amount;
                },
                (result) =>
                {
                    Debug.Log("Amount Red: " + result);
                    //Update Red Balance
                    tempTXT.GetComponent<StartGameTest>().AmountRed = Amount;
                }
            );
        }
    }
}
