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

}
