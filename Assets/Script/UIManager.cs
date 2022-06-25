using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
// use web3.jslib
using System.Runtime.InteropServices;

public class UIManager : MonoBehaviour
{

    // text in the button
    public Text ButtonText;

    [DllImport("__Internal")]
    private static extern string Connect();

    // use WalletAddress function from web3.jslib
    [DllImport("__Internal")]
    private static extern string WalletAddress();
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void onConnectWallet(){
        Debug.Log("Wallet Connection!");
        Connect();
        Debug.Log(WalletAddress());
        ButtonText.text = WalletAddress();

        if(WalletAddress() != ""){
            SceneManager.LoadScene("DemoScene");
        }

    }
}
