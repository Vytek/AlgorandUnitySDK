using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using MHLab.Utilities;
using SimpleJSON;

public class ClickOnObjectGreen : MonoBehaviour
{
    [Header("The bottle was already present in the object pooler")]
    public int bottleIndex = 0;
    [Header("This bottle will be added to the object pooler when the game begins")]
    public GameObject differentBottle;
    private int differentIndex;
    private ObjectPooler OP;
    public Transform SpawnPoint;
    public BackgroundTasksProcessor Processor;
    private string TxIDPayment = string.Empty;
    public GameObject tempTXT;
    private long? Amount = 0;

    private void Start()
    {
        OP = ObjectPooler.SharedInstance;
        differentIndex = OP.AddObject(differentBottle);
        Random.InitState((int)System.DateTime.Now.Ticks);
    }
    void OnMouseDown()
    {
        Debug.Log("Transfer 1 ASA to Green User");
        GameObject bottle = OP.GetPooledObject(bottleIndex);
        bottle.transform.rotation = SpawnPoint.transform.rotation;
        //float xPos = Random.Range(-5f, 5f);
        //float zPos = Random.Range(-5f, 5f);
        bottle.transform.position = SpawnPoint.transform.position;
        bottle.SetActive(true);
        //Transfert ASA
        Processor.Process(
                () =>
                {
                    //Do here Algorand ASA Transaction
                    TxIDPayment = AlgorandManager.Instance.AssetTransfer(
                    AlgorandManager.Instance.ALGOD_URL_ENDPOINT,
                    AlgorandManager.Instance.ALGOD_TOKEN,
                    "PVT67ZSBADU5ATXRIYBRIDBWSOIJOJJR73FJPCUFSKPHXI4M7PIRS5SRRI",
                    1,
                    16038175,
                    "Test Tx for Asset Transfert: " + System.DateTime.Now.ToString());
                    Debug.Log("TxID: " + TxIDPayment);
                    //PAUSE for PureStake limitation
                    Thread.Sleep(4000);
                    //Update Balance
                    var jsonResult = AlgorandManager.Instance.GetAccount(AlgorandManager.Instance.ALGOD_URL_ENDPOINT_INDEXER,
                    AlgorandManager.Instance.ALGOD_TOKEN, "PVT67ZSBADU5ATXRIYBRIDBWSOIJOJJR73FJPCUFSKPHXI4M7PIRS5SRRI");
                    var N = JSON.Parse(jsonResult);
                    //Show Asset Total Example
                    Debug.Log("Total Amount for user Green: " + N["account"]["assets"][6]["amount"]);
                    Amount = N["account"]["assets"][6]["amount"];
                    return Amount;
                },
                (result) =>
                {
                    Debug.Log("Total Amount Green: " + result);
                    //Update Blue Balance
                    tempTXT.GetComponent<StartGameASATest>().AmountGreen = Amount;
                }
            );
    }

    // Update is called once per frame
    void Update()
    {
        if (!Processor.IsReady)
        {
            return;
        }
    }
}
