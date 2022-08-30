using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomMenu : Menu
{
    public GameObject RoomOptions;

    public override void Open()
    {
        RoomOptions.SetActive(Photon.Pun.PhotonNetwork.IsMasterClient);
        base.Open();
    }
}
