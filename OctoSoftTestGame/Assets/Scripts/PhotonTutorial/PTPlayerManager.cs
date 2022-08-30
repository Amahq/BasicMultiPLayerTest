using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;

public class PTPlayerManager : MonoBehaviour
{

    PhotonView PV;

    private void Awake()
    {
        PV = GetComponent<PhotonView>();
    }

    // Start is called before the first frame update
    void Start()
    {
        if(GlobalData.matchmode == MatchModes.SinglePlayer)
        {
            return;
        }
        if (PV.IsMine)
        {
            CreateController();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void CreateController()
    {
        Vector3 position = Vector3.zero;
        
        Debug.Log("Controller Created");
        if (!PV.IsMine)
        {
            
        }
        else
        {

        }
        PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Player"), position, Quaternion.identity);
    }
}
