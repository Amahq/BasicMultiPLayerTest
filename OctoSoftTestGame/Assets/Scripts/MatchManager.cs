using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using System;

public class MatchManager : MonoBehaviour
{
    [Tooltip("Duration time in seconds")]
    public float MatchDuration = 120;
    public float WinConditionScore = 100;
    public MatchModes mode;
    public Camera SinglePlayerCamera;
    public Camera MultiPlayerCameraLocal;
    public Camera MultiPlayerCameraRemote;
    public GameObject PlayerPrefab;
    private Player _localplayer;
    private Player _remoteplayer;
    public UIManagerIngame ui;
    public UIManagerIngame uimp;
    public GameObject WinScreen;
    public TMPro.TMP_Text txtRemainingTime;
    public TMPro.TMP_Text txtGameOutcome;
    public TMPro.TMP_Text txtWinnerName;
    public bool isgamefinished = false;
    private System.Diagnostics.Stopwatch st;
    private TimeSpan remainingtime;
    TimeSpan MatchDurationTime;



    public void ReturnToMainMenu()
    {
        if(GlobalData.matchmode == MatchModes.MultiPlayer)
        {
            try
            {
                foreach (Photon.Realtime.Player p in PhotonNetwork.PlayerList)
                {
                    PhotonNetwork.DestroyPlayerObjects(System.Convert.ToInt32(p.UserId));
                }
            }
            catch { }

            PhotonNetwork.Disconnect();
        }
        else
        {
            Destroy(MenuManager.Instance);
            Destroy(PTLauncher.Instance);
            Destroy(PTRoomManager.Instance.gameObject);
        }
    }

    public void GameOver()
    {
        isgamefinished = true;
        WinScreen.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        txtGameOutcome.text = "Game Over";
        txtWinnerName.text = "You Lose";
    }

    public void PlayerWin(Player winner)
    {
        isgamefinished = true;
        WinScreen.SetActive(true);
        txtWinnerName.text = winner.Username + " - Won";
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void PlayerSetupSP()
    {
        _localplayer = Instantiate<GameObject>(PlayerPrefab, SinglePlayerCamera.transform.position, SinglePlayerCamera.transform.rotation).GetComponent<Player>();
        _localplayer.cam = SinglePlayerCamera;
        ui.gameObject.GetComponent<Canvas>().worldCamera = SinglePlayerCamera;
        _localplayer.tag = "LocalPlayer";
        _localplayer.mm = this;
        _localplayer.ui = ui;
        ui.player = _localplayer;
    }

    public void PlayerSetupMP(Player player, PlayerType playertype)
    {
        if (Photon.Pun.PhotonNetwork.IsMasterClient)
        {
            if (playertype == PlayerType.Local)
            {
                _localplayer = player;
                _localplayer.cam = MultiPlayerCameraLocal;
                ui.gameObject.GetComponent<Canvas>().worldCamera = MultiPlayerCameraLocal;
                uimp.gameObject.GetComponent<Canvas>().worldCamera = MultiPlayerCameraRemote;
                _localplayer.tag = "LocalPlayer";
                _localplayer.mm = this;
                _localplayer.ui = ui;
                ui.player = _localplayer;
                _localplayer.Username = PhotonNetwork.NickName;
            }
            else
            {
                _remoteplayer = player;
                _remoteplayer.tag = "RemotePlayer";
                _remoteplayer.mm = this;
                _remoteplayer.cam = MultiPlayerCameraRemote;
                _remoteplayer.ui = uimp;
                uimp.player = _remoteplayer;
                foreach(KeyValuePair<int, Photon.Realtime.Player> kv in PhotonNetwork.CurrentRoom.Players)
                {

                    if(PhotonNetwork.LocalPlayer.NickName != kv.Value.NickName)
                    {
                        _remoteplayer.Username = kv.Value.NickName;
                        continue;
                    }
                }
            }
        }
        else
        {
            if (playertype == PlayerType.Local)
            {
                _localplayer = player;
                _localplayer.cam = MultiPlayerCameraRemote;
                ui.gameObject.GetComponent<Canvas>().worldCamera = MultiPlayerCameraRemote;
                uimp.gameObject.GetComponent<Canvas>().worldCamera = MultiPlayerCameraLocal;
                _localplayer.tag = "LocalPlayer";
                _localplayer.mm = this;
                _localplayer.ui = ui;
                ui.player = _localplayer;
                _localplayer.Username = PhotonNetwork.NickName;
            }
            else
            {
                _remoteplayer = player;
                _remoteplayer.tag = "RemotePlayer";
                _remoteplayer.mm = this;
                _remoteplayer.cam = MultiPlayerCameraLocal;
                _remoteplayer.ui = uimp;
                uimp.player = _remoteplayer;
                foreach (KeyValuePair<int, Photon.Realtime.Player> kv in PhotonNetwork.CurrentRoom.Players)
                {

                    if (PhotonNetwork.LocalPlayer.NickName != kv.Value.NickName)
                    {
                        _remoteplayer.Username = kv.Value.NickName;
                        continue;
                    }
                }
            }

        }
    }

    private void Awake()
    {
        GlobalData.CurrentMM = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        bool _ismultiplayer = false;

        if (GlobalData.matchmode == MatchModes.MultiPlayer)
        {
            _ismultiplayer = true;
        }
        else
        {
            PlayerSetupSP();
        }

        SinglePlayerCamera.gameObject.SetActive(!_ismultiplayer);
        MultiPlayerCameraLocal.gameObject.SetActive(_ismultiplayer);
        MultiPlayerCameraRemote.gameObject.SetActive(_ismultiplayer);
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;
        st = System.Diagnostics.Stopwatch.StartNew();
        MatchDurationTime = new TimeSpan(0, 0, (int)MatchDuration);
        Timer();
    }

    private void Timer()
    {
        if (isgamefinished)
        {
            return;
        }
        remainingtime = MatchDurationTime - st.Elapsed;
        txtRemainingTime.text = remainingtime.ToString("mm':'ss':'fff");

        if(remainingtime.TotalSeconds <= 0 && GlobalData.matchmode == MatchModes.SinglePlayer)
        {
            GameOver();
        }
        else if(remainingtime.TotalSeconds <= 0 && GlobalData.matchmode == MatchModes.MultiPlayer)
        {
            txtGameOutcome.text = "Congratulations!!";

            if (_localplayer.Score > _remoteplayer.Score)
            {
                PlayerWin(_localplayer);
            }
            else
            {
                GameOver();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        Timer();
    }

    public void ModifyScore(float value, PlayerType type)
    {
        if(type == PlayerType.Local)
        {
            _localplayer.Score += value;
        }
    }

    public void UpdateUI()
    {
        try
        {
            _localplayer.ui.Refresh();
        }
        catch
        {

        }
        try
        {
            _remoteplayer.ui.Refresh();
        }
        catch { }
    }
}
