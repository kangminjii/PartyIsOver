using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using System;


public class PhotonManager : MonoBehaviourPunCallbacks 
{
    static PhotonManager p_instance;

    const byte MAX_PLAYERS_PER_ROOM = 6;

    bool IsConnecting;
    string GameVersion = "1";
    string _gameCenterPath = "GameCenter";
    string _sceneMain = "[2]Main";
    string _sceneLobby = "[3]Lobby";
    string _sceneRoom = "[4]Room";

    List<RoomItem> RoomItemsList = new List<RoomItem>();
    float _nextUpdateTime = 1f;
    float _timeBetweenUpdate = 1.5f;

    public static PhotonManager Instance { get { return p_instance; } }
    public LobbyUI LobbyUI;


    void Awake()
    {
        Init();
    }

    #region Public Methods

    public void Connect()
    {
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.JoinLobby();
        }
        else
        {
            IsConnecting = PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.GameVersion = GameVersion;
        }

        SceneManagerEx sceneManagerEx = new SceneManagerEx();
        string currentSceneName = sceneManagerEx.GetCurrentSceneName();
        if (currentSceneName == _sceneLobby)
        {
            AudioSource[] _audioSources = new AudioSource[(int)Define.Sound.Maxcount];
            AudioClip audioClip = Managers.Resource.Load<AudioClip>("Sounds/Bgm/BongoBoogieMenuLOOPING");
            _audioSources[(int)Define.Sound.Bgm].clip = audioClip;
            Managers.Sound.Play(audioClip, Define.Sound.Bgm);
        }
    }

    public void LeaveLobby()
    {
        PhotonNetwork.LeaveLobby();
        StartCoroutine(LoadNextScene(_sceneMain));
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        StartCoroutine(LoadNextScene(_sceneLobby));
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
       
    }

    public void UpdateRoomList(List<RoomInfo> list)
    {
        Debug.Log("UpdateRoomList] IsLobby: " + PhotonNetwork.InLobby + ", Scene: " + SceneManager.GetActiveScene().name);

        if (SceneManager.GetActiveScene().name == _sceneLobby)
        {
            foreach (RoomItem item in RoomItemsList)
            {
                Destroy(item.gameObject);
            }
            RoomItemsList.Clear();

            foreach (RoomInfo room in list)
            {
                if (room.PlayerCount == 0)
                    continue;

                RoomItem newRoom = Instantiate(LobbyUI.RoomItemPrefab, LobbyUI.ContentObject);
                newRoom.SetRoomName(room.Name, room.PlayerCount);
                RoomItemsList.Add(newRoom);
            }
        }
    }

    #endregion

    #region Private Methods

    void Init()
    {
        if (p_instance == null)
        {
            GameObject _go = GameObject.Find("@Photon Manager");
            if (_go == null)
            {
                _go = new GameObject { name = "@Photon Manager" };
                _go.AddComponent<PhotonManager>();
            }
            DontDestroyOnLoad(_go);
            p_instance = _go.GetComponent<PhotonManager>();

            PhotonNetwork.AutomaticallySyncScene = true;

            PhotonNetwork.SerializationRate = 20;
            PhotonNetwork.SendRate = 20;

            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            Destroy(gameObject);
        }
    }




    IEnumerator LoadNextScene(string sceneName)
    {

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        if (sceneName == _sceneRoom)
            InstantiateGameCenter();

    }


    void InstantiateGameCenter()
    {
        if (GameCenter.LocalGameCenterInstance == null)
        {
            Managers.Resource.PhotonNetworkInstantiate(_gameCenterPath);
        }
    }

    #endregion

    #region Photon Callbacks

    public override void OnConnectedToMaster()
    {
        if (IsConnecting)
        {
            IsConnecting = false;
        }
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        IsConnecting = false;
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = MAX_PLAYERS_PER_ROOM });
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("[JoinLobby()] Load Lobby Scene");
        StartCoroutine(LoadNextScene(_sceneLobby));
    }

    public override void OnJoinedRoom()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(LoadNextScene(_sceneRoom));
        }
    }

    public override void OnLeftRoom()
    {
        Debug.Log("[OnLeftRoom()] LoadScene(2)");
        //SceneManager.LoadScene(2);
    }
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        if (Time.time >= _nextUpdateTime)
        {
            UpdateRoomList(roomList);
            _nextUpdateTime = Time.time + _timeBetweenUpdate;
        }

        if (roomList.Count == 0 && PhotonNetwork.InLobby)
        {
            RoomItemsList.Clear();
        }
    }

    public override void OnPlayerEnteredRoom(Player other)
    {
        //Debug.LogFormat("[OnPlayerEnteredRoom()] {0}", other.NickName);
    }

    public override void OnPlayerLeftRoom(Player other)
    {
        //Debug.LogFormat("[OnPlayerLeftRoom()] {0}", other.NickName);
    }

    #endregion
}
