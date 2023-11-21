using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    // : Start
    public Text RoomName;

    public GameObject LobbyPanel;
    public GameObject RoomPanel;

    // : RoomItem
    // RoomItem(�� ����) ����Ʈ
    List<RoomItem> RoomItemsList = new List<RoomItem>();
    public RoomItem RoomItemPrefab;
    public Transform ContentObject; // Inspector�� �ִ� viewport > content

    //OnRoomListUpdate�� 2�� �̻� �Ҹ��� ���� ������ Ÿ�̸�
    public float TimeBetweenUpdate = 1.5f;
    float NextUpdateTime;


    // : PlayerItem
    // PlayerItem(�÷��̾� ����) ����Ʈ
    List<PlayerItem> PlayerItemsList = new List<PlayerItem>();
    public PlayerItem PlayerItemPrefab;
    public Transform PlayerItemParent; // Inspector�� �ִ� PlayerListing


    // : Game Start
    // ���� ���� ��ư
    public GameObject PlayButton;



    // Lobby Panel


    // Room Panel



    // Create Room Panel
    public GameObject CreateRoomPanel;
    public void OnClickOK()
    {
        CreateRoomPanel.SetActive(false);
        // �游���
    }

    public void OnClickCancel()
    {
        CreateRoomPanel.SetActive(false);
    }

    public InputField RoomInputField;

    public void OnClickCreate()
    {
        if (RoomInputField.text.Length >= 1)
        {
            PhotonNetwork.CreateRoom(RoomInputField.text, new RoomOptions() { MaxPlayers = 6 });

        }
    }


    public Sprite PrivateOn;
    public Sprite PrivateOff;
    public Sprite PrivateButton;
    private bool _isClicked;

    public void OnClickPrivate()
    {
        _isClicked = !_isClicked;

        if(_isClicked)
            PrivateButton = PrivateOn;
        else
            PrivateButton = PrivateOff;
    }



    private void Start()
    {
        PhotonNetwork.JoinLobby();
        LobbyPanel.SetActive(true);
        RoomPanel.SetActive(false);

    }

   

    // �뿡 ������ �� ȣ��Ǵ� �ݹ� �Լ�
    public override void OnJoinedRoom()
    {
        LobbyPanel.SetActive(false);
        RoomPanel.SetActive(true);
        RoomName.text = "Room Name: " + PhotonNetwork.CurrentRoom.Name;

        UpdatePlayerList();
    }

    // : RoomItem
    // �κ� �� ��� ������Ʈ
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        //UpdateRoomList(roomList); // > 2�� �̻� �ҷ��� ��찡 ����

        if (Time.time >= NextUpdateTime)
        {
            UpdateRoomList(roomList);
            NextUpdateTime = Time.time + TimeBetweenUpdate;
        }

        if (roomList.Count == 0 && PhotonNetwork.InLobby)
        {
            RoomItemsList.Clear();
        }
    }

    void UpdateRoomList(List<RoomInfo> list)
    {
        // ��������� �ִ� room list ����
        // ����Ʈ�� �ִ� ����� ���� ���� �� 
        foreach (RoomItem item in RoomItemsList)
        {
            Destroy(item.gameObject);
        }
        RoomItemsList.Clear();

        // ���Ӱ� �߰�
        foreach (RoomInfo room in list)
        {
            if (room.PlayerCount == 0)
                continue;

            RoomItem newRoom = Instantiate(RoomItemPrefab, ContentObject);
            newRoom.SetRoomName(room.Name);
            RoomItemsList.Add(newRoom);
        }
    }

    // �濡 �����ϱ�
    public void JoinRoom(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
    }

    // �濡�� ������
    public void OnClickLeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        RoomPanel.SetActive(false);
        LobbyPanel.SetActive(true);
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }


    // : PlayerItem
    // �κ� �÷��̾� ������ ������Ʈ
    void UpdatePlayerList()
    {
        // ��������� �ִ� player list ����
        // ����Ʈ�� �ִ� ����� ���� ���� �� 
        foreach (PlayerItem item in PlayerItemsList)
        {
            Destroy(item.gameObject);
        }
        PlayerItemsList.Clear();

        // �濡 �ִ��� Ȯ�� ��
        if(PhotonNetwork.CurrentLobby == null)
        {
            return;
        }

        // ���Ӱ� �߰�
        foreach (KeyValuePair<int, Player> player in PhotonNetwork.CurrentRoom.Players)
        {
            PlayerItem newPlayerItem = Instantiate(PlayerItemPrefab, PlayerItemParent);
            newPlayerItem.SetPlayerInfo(player.Value);

            ////���� �濡 �ִ� player�� ������ �ִ� Localplayer�� ��ȭ�� �ҷ�����
            if (player.Value == PhotonNetwork.LocalPlayer)
            {
                newPlayerItem.ApplyLocalChanges();
            }

            PlayerItemsList.Add(newPlayerItem);
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        UpdatePlayerList();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdatePlayerList();
    }


    // : Game Start
    private void Update()
    {
        // ���� ��� && �ּ� �����ο� �����ø� ���� ���� ��ư Ȱ��ȭ
        if (PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.PlayerCount >= 2)
        {
            PlayButton.SetActive(true);
        }
        else
        {
            PlayButton.SetActive(false);
        }
    }
    
}
