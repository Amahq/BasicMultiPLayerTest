using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using System.IO;
using System;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PTRoomManager : MonoBehaviourPunCallbacks, IPunObservable
{
    [SerializeField] private DifficultySetting Easy;
    [SerializeField] private DifficultySetting Medium;
    [SerializeField] private DifficultySetting Hard;
    [SerializeField, HideInInspector]private DifficultySetting _currentDifficulty;
    private PhotonView PV;
    [SerializeField,HideInInspector]private Player _localPlayer;
    [SerializeField, HideInInspector] private Player _remotePlayer;
    private int _selecteddifficulty = 1;

    public event EventHandler OnDifficultyChange;

    public static PTRoomManager Instance;


    [SerializeField]
    public DifficultySetting CurrentDifficulty 
    {
        get 
        { 
            if(_currentDifficulty == null)
            {
                _currentDifficulty = Medium;
            }
            return _currentDifficulty; 
        }

        set
        {
            _currentDifficulty = value;
        }
    }

    private void OnDestroy()
    {
        SceneManager.LoadScene("Main Menu");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Destroy(MenuManager.Instance);
        Destroy(PTLauncher.Instance);
        Destroy(gameObject);
    }

    public override void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, Hashtable changedProps)
    {
        if(!PV.IsMine)
        {
            try
            {
                _selecteddifficulty = (int)changedProps["CurrentDifficulty"];
                switch (_selecteddifficulty)
                {
                    case 0:
                        _currentDifficulty = Easy;
                        break;
                    case 1:
                        _currentDifficulty = Medium;
                        break;
                    case 2:
                        _currentDifficulty = Hard;
                        break;
                    default:
                        _currentDifficulty = Medium;
                        break;
                }
                EventHandler handler = OnDifficultyChange;
                GameOptionsEvents args = new GameOptionsEvents();
                args.difficulty = CurrentDifficulty;
                handler?.Invoke(this, args);
            }
            catch { }
        }
    }

    public Player LocalPlayer { get => _localPlayer; set => _localPlayer = value; }
    public Player RemotePlayer { get => _remotePlayer; set => _remotePlayer = value; }

    private void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        Instance = this;
        PV = GetComponent<PhotonView>();
    }

    public override void OnEnable()
    {

        base.OnEnable();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public override void OnDisable()
    {

        base.OnDisable();
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        GameObject player;
        if (GlobalData.matchmode == MatchModes.SinglePlayer)
        {
            return;
        }
        if (scene.name == "Game")
        {
            PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerManager"), Vector3.zero, Quaternion.identity);
            player = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Player"), Vector3.zero, Quaternion.identity);
            if (PV.IsMine)
            {
                player.tag = "LocalPlayer";
                _localPlayer = player.GetComponent<Player>();
            }
            else
            {
                player.tag = "RemotePlayer";
                _remotePlayer = player.GetComponent<Player>();
            }

        }
    }

    // Start is called before the first frame update
    void Start()
    {
        //Set default difficulty
        SetDifficulty(Medium.name);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetDifficulty(string difficulty)
    {
        switch (difficulty.ToLower())
        {
            case "easy":
                _currentDifficulty = Easy;
                _selecteddifficulty = 0;
                break;
            case "medium":
                _currentDifficulty = Medium;
                _selecteddifficulty = 1;
                break;
            case "hard":
                _currentDifficulty = Hard;
                _selecteddifficulty = 2;
                break;
            default:
                _currentDifficulty = Medium;
                _selecteddifficulty = 1;
                break;
        }
        if (PV.IsMine)
        {
            Hashtable hash = new Hashtable();
            hash.Add("CurrentDifficulty", _selecteddifficulty);
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
            EventHandler handler = OnDifficultyChange;
            GameOptionsEvents args = new GameOptionsEvents();
            args.difficulty = CurrentDifficulty;
            handler?.Invoke(this, args);
        }

    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        //Debug.Log("Something");
    }
}
