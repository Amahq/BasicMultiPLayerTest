using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;
using System.Linq;
using System;
using UnityEngine.SceneManagement;
using Hashtable = ExitGames.Client.Photon.Hashtable;


public class PTLauncher : MonoBehaviourPunCallbacks
{
    public static PTLauncher Instance;

    [SerializeField] TMP_InputField roomNameInputField;
    [SerializeField] TMP_Text errorText;
    [SerializeField] TMP_Text roomNameText;
    [SerializeField] Transform roomListContent;
    [SerializeField] Transform playerListContent;
    [SerializeField] GameObject roomListItemPrefab;
    [SerializeField] GameObject playerListItemPrefab;
    [SerializeField] GameObject startGameButton;
    [SerializeField] TMP_Text currentDifficulty;

    private void Awake()
    {
        Instance = this;
    }



    private void OnChangedDifficulty(object sender, EventArgs e)
    {
        GameOptionsEvents ge = (GameOptionsEvents)e;
        currentDifficulty.text = "Difficulty: " + ge.difficulty.name;
    }

    // Start is called before the first frame update
    void Start()
    {
        if(SceneManager.GetActiveScene().name == "Lobby Room")
        {
            if (GlobalData.matchmode == MatchModes.MultiPlayer)
            {
                PhotonNetwork.ConnectUsingSettings();
                MenuManager.Instance.OpenMenu("loading");
                PTRoomManager.Instance.OnDifficultyChange += OnChangedDifficulty;
            }
            else
            {
                MenuManager.Instance.OpenMenu("game options");
            }
        }
    }

    public void QuickPlay()
    {
        RoomOptions ro = new RoomOptions();
        ro.MaxPlayers = 2;
        ro.IsOpen = true;
        PhotonNetwork.JoinRandomOrCreateRoom(null,2, MatchmakingMode.FillRoom,null,null,null,ro,null);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("Failed To Join Random:" + message);
        base.OnJoinRandomFailed(returnCode, message);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        MenuManager.Instance.OpenMenu("title");
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public void InitiateMPSequence()
    {

    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Joined Lobby");

        MenuManager.Instance.OpenMenu("title");
        PhotonNetwork.NickName = GlobalData.Username;
    }



    public void CreateRoom()
    {
        if (string.IsNullOrEmpty(roomNameInputField.text))
        {
            return;
        }
        RoomOptions ro = new RoomOptions();
        ro.MaxPlayers = 2;
        ro.IsOpen = true;
        PhotonNetwork.CreateRoom(roomNameInputField.text,ro);
        MenuManager.Instance.OpenMenu("loading");
    }

    public override void OnJoinedRoom()
    {
        roomNameText.text = PhotonNetwork.CurrentRoom.Name;
        MenuManager.Instance.OpenMenu("room");

        Photon.Realtime.Player[] players = PhotonNetwork.PlayerList;

        foreach(Transform child in playerListContent)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < players.Count(); i++)
        {
            Instantiate(playerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().Setup(players[i]);

        }
        startGameButton.SetActive(PhotonNetwork.IsMasterClient);
        PTRoomManager.Instance.SetDifficulty(PTRoomManager.Instance.CurrentDifficulty.name);
    }

    public override void OnMasterClientSwitched(Photon.Realtime.Player newMasterClient)
    {
        startGameButton.SetActive(PhotonNetwork.IsMasterClient);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        errorText.text = "Room Creation Failed: " + message;
        MenuManager.Instance.OpenMenu("error");
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        MenuManager.Instance.OpenMenu("loading");
    }

    public override void OnLeftRoom()
    {
        MenuManager.Instance.OpenMenu("title");
    }

    public void JoinRoom(RoomInfo info)
    {
        PhotonNetwork.JoinRoom(info.Name);
        MenuManager.Instance.OpenMenu("loading");
    }

    public void StartGame()
    {
        PTRoomManager.Instance.OnDifficultyChange -= OnChangedDifficulty;
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.LoadLevel("Game");
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach(Transform t in roomListContent)
        {
            Destroy(t.gameObject);
        }
        for (int i = 0; i < roomList.Count; i++)
        {
            if (roomList[i].RemovedFromList)
                continue;
            Instantiate(roomListItemPrefab, roomListContent).GetComponent<RoomListItem>().Setup(roomList[i]);
        }
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        Instantiate(playerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().Setup(newPlayer);
        PTRoomManager.Instance.SetDifficulty(PTRoomManager.Instance.CurrentDifficulty.name);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
