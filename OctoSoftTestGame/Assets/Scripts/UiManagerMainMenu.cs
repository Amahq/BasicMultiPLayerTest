using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class UiManagerMainMenu : MonoBehaviour
{

    public TMP_InputField username;

    // Start is called before the first frame update
    void Start()
    {
        username.text = GlobalData.Username;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SinglePlayerStart()
    {
        GlobalData.Username = username.text;
        GlobalData.matchmode = MatchModes.SinglePlayer;
        SceneManager.LoadSceneAsync("Lobby Room");
    }

    public void MultiPlayerStart()
    {
        GlobalData.Username = username.text;
        GlobalData.matchmode = MatchModes.MultiPlayer;
        SceneManager.LoadSceneAsync("Lobby Room");

    }
}
