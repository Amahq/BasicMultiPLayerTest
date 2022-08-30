using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class Player : MonoBehaviourPunCallbacks
{

    public MatchManager mm;
    private float _score;
    public Camera cam;
    private string _username;
    public UIManagerIngame ui;

    PhotonView PV;

    public string Username
    {
        get { return _username; }
        set
        {
            _username = value;
            ui.updateUsername(Username);
        }
    }

    public float Score
    {
        get { return _score; }
        set
        {
            _score = value;
            Hashtable hash = new Hashtable();
            hash.Add("score", _score);
            hash.Add("nickname", PhotonNetwork.NickName);
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
            GlobalData.CurrentMM.UpdateUI();
            if (_score >= 100)
            {
                GlobalData.CurrentMM.PlayerWin(this);
                return;
            }
        }
    }

    public override void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, Hashtable changedProps)
    {
        //Debug.Log("PlayerPropertiesChanged catched by ("+this.Username+") - triggered by (" + changedProps["nickname"].ToString() + ") - with value : " + (float)changedProps["score"]);
        if(this.Username == changedProps["nickname"].ToString())
        {
            _score = (float)changedProps["score"];
            GlobalData.CurrentMM.UpdateUI();
            if(_score >= 100)
            {
                GlobalData.CurrentMM.PlayerWin(this);
            }
        }
        //if (!PV.IsMine)
        //{
        //    Debug.Log("Score changed: " + (float)changedProps["score"]);
        //}
    }

    private void Awake()
    {
        PV = GetComponent<PhotonView>();
    }

    // Start is called before the first frame update
    void Start()
    {
        if (!PV.IsMine)
        {
            GlobalData.CurrentMM.PlayerSetupMP(this, PlayerType.Remote);
            //Destroy(GetComponentInChildren<Camera>().gameObject);
        }
        else
        {
            GlobalData.CurrentMM.PlayerSetupMP(this, PlayerType.Local);
        }
    }

    private void FixedUpdate()
    {
        if (GlobalData.matchmode == MatchModes.MultiPlayer)
        {
            if (!PV.IsMine)
            {
                return;
            }
        }

        Vector3 v3;
        v3 = GetLockedPoint();

        v3.z = 8;
        transform.position = v3;
    }

    private Vector3 GetLockedPoint()
    {
        Vector3 vp = cam.ScreenToViewportPoint(Input.mousePosition);
        Vector3 viewport = new Vector3(Mathf.Clamp(vp.x, -1, 1), Mathf.Clamp(vp.y, -1, 1), 0);
        Vector3 world = cam.ViewportToWorldPoint(viewport);
        return world;
    }

    // Update is called once per frame
    void Update()
    {
        HandleClicks();
    }

    private void HandleClicks()
    {
        if(gameObject.tag == "RemotePlayer")
        {
            return;
        }
        InteractableObject io;
        if (Input.GetMouseButtonDown(0))
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray;
                ray = new Ray(transform.position, transform.forward);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    //Select stage    
                    if (hit.transform.tag == gameObject.tag)
                    {
                        try
                        {
                            io = hit.transform.root.GetComponent<TargetObject>();
                            io.OnClick();
                        }
                        catch
                        {
                            io = hit.transform.root.GetComponent<InteractableObject>();
                            io.OnClick();
                        }

                    }
                }
            }
        }
    }
}
