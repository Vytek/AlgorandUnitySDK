using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using SimpleJSON;
using Siccity.GLTFUtility;
public class StartNFTGameTest : MonoBehaviour
{
    public Transform contentContainer;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Start NFT Test...");
        {
            //Show URL ENDPOINT ALGOD
            Debug.Log("URL ENDPOINT: " + AlgorandManager.Instance.ALGOD_URL_ENDPOINT);
            //Show URL ENDPOINT INDEXER
            Debug.Log("UR((L ENPOINT INDEXER: " + AlgorandManager.Instance.ALGOD_URL_ENDPOINT_INDEXER);
            //Show Token Used
            Debug.Log("Token Used: " + AlgorandManager.Instance.ALGOD_TOKEN);
            //Verify Algod / Purestack Noed connection
            //Connect to Node
            AlgorandManager.Instance.ConnectToNode(AlgorandManager.Instance.ALGOD_URL_ENDPOINT, AlgorandManager.Instance.ALGOD_TOKEN);
            //Get info About NFT Asset
            var jsonResult = AlgorandManager.Instance.GetAsset(AlgorandManager.Instance.ALGOD_URL_ENDPOINT_INDEXER,
                AlgorandManager.Instance.ALGOD_TOKEN,
                15942738);
            //Debug.Log(jsonResult); //DEBUG
            //https://wiki.unity3d.com/index.php/SimpleJSON
            var N = JSON.Parse(jsonResult);
            //Show Asset Total Example
            Debug.Log("Asset Total: " + N["asset"]["params"]["total"]);
            //Show Creator Example
            Debug.Log("Creator: " + N["asset"]["params"]["creator"]);
            //Get Asset Algorand NFT
            var UrlToNFT = N["asset"]["params"]["url"];
            Debug.Log("NFT Url: " + UrlToNFT);
            //Load GLTF using GLTFast (Not in this example!)
            //StartCoroutine(SendRequest(UrlToNFT));
            //Load GLTF using GLTFUtility
            StartCoroutine(LoadGLTFRoutine(UrlToNFT));
        }
    }
    IEnumerator SendRequest(string url)
    {
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.isNetworkError || request.isHttpError)
        {
            Debug.LogError(string.Format("Error: {0}", request.error));
        }
        else
        {
            // Response can be accessed through: request.downloadHandler.text
            Debug.Log(request.downloadHandler.text);

            // Or retrieve results as binary data
            byte[] results = request.downloadHandler.data;
            //WARNING: THIS IS ONLY A STUB to UNDERSTAND the pipeline!
            //See: https://goalseeker.purestake.io/algorand/testnet/asset/15942738
            //https://tinyurl.com/ausdknft is a redirect to 
            //https://gateway.pinata.cloud/ipfs/QmdyoYaeMdWM4xFgoMwSU5NLQekSwk5vybAFccuRJ6gaKc/BottleIPFS.glb
        }
    }

    IEnumerator LoadGLTFRoutine(string uri)
    {
        UnityWebRequest webRequest = new UnityWebRequest(uri);
        webRequest.downloadHandler = new DownloadHandlerBuffer();
        yield return webRequest.SendWebRequest();

        if (webRequest.isNetworkError || webRequest.isHttpError)
        {
            Debug.Log(webRequest.error);
        }
        else
        {
            // Or retrieve results as binary data
            byte[] results = webRequest.downloadHandler.data;
            AnimationClip[] clips;
            GameObject result = Importer.LoadFromBytes(results, new ImportSettings() { useLegacyClips = true }, out clips);
            result.transform.SetParent(contentContainer.transform, false);
        }
    }
}
