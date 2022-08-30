using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;

public class PlayerListItem :  MonoBehaviourPunCallbacks
{
    [SerializeField] TMP_Text text;
    Photon.Realtime.Player player;

    public void Setup(Photon.Realtime.Player _player)
    {
        player = _player;
        text.text = player.NickName;
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        if(player == otherPlayer)
        {
            Destroy(gameObject);
        }
    }

    public override void OnLeftRoom()
    {
        Destroy(gameObject);
    }
}
