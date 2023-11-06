using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameCenter : MonoBehaviourPunCallbacks
{
    #region Private Serializable Fields

    [SerializeField]
    RoomUI _roomUI;

    #endregion

    #region Private Fields

    string _arenaName = "PlayerMoveTest";
    // ������ ���
    string _playerPath = "Ragdoll2";

    #endregion

    #region Public Fields

    public static GameObject LocalGameCenterInstance = null;

    // ���� ����Ʈ 6�� ����
    public List<Vector3> _spawnPoints = new List<Vector3>
    {
        new Vector3(5f, 5f, 0f),
        new Vector3(2.5f, 5f, 4.33f),
        new Vector3(-2.5f, 5f, 4.33f),
        new Vector3(-5f, 5f, 0f),
        new Vector3(-2.5f, 5f, -4.33f),
        new Vector3(2.5f, 5f, -4.33f)
    };

    #endregion

    #region Private Methods

    void Awake()
    {
        if (photonView.IsMine)
        {
            LocalGameCenterInstance = this.gameObject;
        }
        DontDestroyOnLoad(this.gameObject);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == _arenaName)
        {
            Debug.Log("Arena Scene �ε� �Ϸ� �� �ʱ�ȭ");
            InstantiatePlayer();
        }
    }

    void InstantiatePlayer()
    {
        if (Actor.LocalPlayerInstance == null)
        {
            Debug.LogFormat("PhotonManager.cs => We are Instantiating LocalPlayer from {0}", SceneManagerHelper.ActiveSceneName);

            switch (PhotonNetwork.LocalPlayer.ActorNumber)
            {
                case 1:
                    Managers.Resource.PhotonNetworkInstantiate(_playerPath, pos: _spawnPoints[0]);
                    break;
                case 2:
                    Managers.Resource.PhotonNetworkInstantiate(_playerPath, pos: _spawnPoints[1]);
                    break;
                case 3:
                    Managers.Resource.PhotonNetworkInstantiate(_playerPath, pos: _spawnPoints[2]);
                    break;
                case 4:
                    Managers.Resource.PhotonNetworkInstantiate(_playerPath, pos: _spawnPoints[3]);
                    break;
                case 5:
                    Managers.Resource.PhotonNetworkInstantiate(_playerPath, pos: _spawnPoints[4]);
                    break;
                case 6:
                    Managers.Resource.PhotonNetworkInstantiate(_playerPath, pos: _spawnPoints[5]);
                    break;
            }
        }
    }

    void LoadArena()
    {
        Debug.Log("LoadArena()");

        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel(_arenaName);
        }
        else
        {
            Debug.LogError("PhotonNetwork : Trying to Load a level but we are not the master Client");
            return;
        }
    }

    void Start()
    {
        InitRoomUI();
    }

    void InitRoomUI()
    {
        Debug.Log("InitRoomUI() / name: " + gameObject.name + ", ViewId: " + photonView.ViewID + ", IsMine?: " + photonView.IsMine);

        _roomUI = GameObject.Find("Control Panel").transform.GetComponent<RoomUI>();

        if (PhotonNetwork.IsMasterClient)
        {
            _roomUI.IsReady = true;
            _roomUI.SetButtonActive("ready", false);
            _roomUI.AddButtonEvent("play", LoadArena);
            _roomUI.UpdateReadyCountText(_roomUI.IsReady);
            _roomUI.SetButtonInteractable("play", true);
            _roomUI.SetPlayerStatus("Wait for Other Players...");
        }
        else
        {
            _roomUI.SetButtonActive("play", false);
            _roomUI.AddButtonEvent("ready", Ready);
            _roomUI.SetPlayerStatus("Unready");
            photonView.RPC("EnteredRoom", RpcTarget.MasterClient);
        }
    }

    void UpdateMasterStatus()
    {
        if (_roomUI.PlayerReadyCount == PhotonNetwork.CurrentRoom.PlayerCount)
        {
            _roomUI.SetPlayerStatus("All Players Ready!");
            _roomUI.SetButtonInteractable("play", true);
        }
        else
        {
            _roomUI.SetPlayerStatus("Wait for Other Players...");
            _roomUI.SetButtonInteractable("play", false);
        }
    }

    void Ready()
    {
        _roomUI.IsReady = !_roomUI.IsReady;
        _roomUI.SetPlayerStatus();
        photonView.RPC("PlayerReady", RpcTarget.MasterClient, _roomUI.IsReady);
    }

    #endregion

    #region MonoBehaviourPunCallbacks Methods

    public override void OnEnable()
    {
        Debug.Log("GameCenter OnEnable");
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public override void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    #endregion

    #region PunRPC Methods

    [PunRPC]
    void EnteredRoom()
    {
        Debug.Log("[master received] EnteredRoom(void)");

        _roomUI.UpdateReadyCountText(_roomUI.PlayerReadyCount);
        UpdateMasterStatus();
        photonView.RPC("UpdateCount", RpcTarget.Others, _roomUI.PlayerReadyCount);
    }

    [PunRPC]
    void UpdateCount(int count)
    {
        Debug.Log("[except master received] UpdateCount(int): " + count);

        _roomUI.UpdateReadyCountText(count);
    }

    [PunRPC]
    void PlayerReady(bool isReady)
    {
        Debug.Log("[master received] PlayerReady(void): " + isReady);

        _roomUI.UpdateReadyCountText(isReady);
        UpdateMasterStatus();
        photonView.RPC("UpdateCount", RpcTarget.Others, _roomUI.PlayerReadyCount);
    }

    #endregion
}
