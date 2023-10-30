using ExitGames.Client.Photon;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Define;

public class LobbyCenter : MonoBehaviourPunCallbacks
{
    #region Private Serializable Fields

    [Tooltip("�÷��̾� �̸� ǥ�� �ؽ�Ʈ")]
    [SerializeField]
    private TMP_Text _playerNameText;
    [Tooltip("�÷��̾� �غ� ���� ǥ�� �ؽ�Ʈ")]
    [SerializeField]
    private TMP_Text _playerStatusText;
    [Tooltip("�غ� �Ϸ� �÷��̾� �� ǥ�� �ؽ�Ʈ")]
    [SerializeField]
    private TMP_Text _playerReadyCountText;

    [Tooltip("�غ�/���� ��ư, Ŭ���̾�Ʈ ����")]
    [SerializeField]
    //GameObject _buttonReady;
    Button _buttonReady;
    [Tooltip("���� ��ư, ���� ����")]
    [SerializeField]
    //GameObject _buttonPlay;
    Button _buttonPlay;

    PhotonView _pv;

    #endregion

    #region Private Fields

    bool _isReady = false;

    #endregion

    #region Public Fields

    public static int _playerReadyCount = 1;

    #endregion

    #region Private Methods

    private void Start()
    {
        Init();
        Enter();
    }

    private void Init()
    {
        if (_buttonPlay == null)
        {
            _buttonPlay = transform.GetChild(1).GetComponent<Button>();
        }

        if (_buttonReady == null)
        {
            _buttonReady = transform.GetChild(0).GetComponent<Button>();
        }

        // ������ Play ��ư�� ǥ��, �ܴ̿� Ready ��ư�� ǥ��
        if (PhotonNetwork.IsMasterClient)
        {
            _buttonReady.gameObject.SetActive(false);
            _isReady = true;

            _buttonPlay.onClick.AddListener(PhotonManager.Instance.LoadArena);
            Debug.Log("1");
        }
        else
        {
            _buttonPlay.gameObject.SetActive(false);

            _buttonReady.onClick.AddListener(Ready);
            Debug.Log("2");
        }

        Debug.Log("3");
        ShowPlayerName();
        TogglePlayerStatus();
    }

    public void Announce(PhotonView pv)
    {
        pv.RPC("Enter", RpcTarget.All);
    }

    [PunRPC]
    public void Enter()
    {
        Debug.Log("Enter");
        _playerReadyCountText.text = _playerReadyCount.ToString() + "/" + PhotonNetwork.CurrentRoom.PlayerCount.ToString();
    }

    void ShowPlayerName()
    {
        Debug.Log("GetPlayerName(): " + PhotonNetwork.NickName);
        _playerNameText.text = PhotonNetwork.NickName;
    }

    void TogglePlayerStatus()
    {
        if (_isReady)
        {
            _playerStatusText.text = "Unready";
        }
        else
        {
            _playerStatusText.text = "Ready";
        }
    }

    #endregion

    #region Public Methods

    public void UpdatePlayerReadyCount()
    {

    }

    public void Ready()
    {
        if (!_isReady)
        {
            _isReady = true;
            _playerReadyCount++;
            TogglePlayerStatus();
        }
        else
        {
            _isReady = false;
            TogglePlayerStatus();
        }
    }

    #endregion
}
