using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using Photon.Pun;

public class GameOptions : MonoBehaviour
{

    public void SetDifficulty(string difficulty)
    {
        PTRoomManager.Instance.SetDifficulty(difficulty);
        if (GlobalData.matchmode == MatchModes.SinglePlayer)
        {
            SceneManager.LoadScene("Game");
        }
    }

    //public override void OnJoinedRoom()
    //{
    //    if (GlobalData.matchmode == MatchModes.MultiPlayer)
    //    {
    //        foreach (Transform t in gameObject.GetComponentInChildren<Transform>())
    //        {
    //            t.gameObject.SetActive(PhotonNetwork.IsMasterClient);
    //        }
    //    }
    //}

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
