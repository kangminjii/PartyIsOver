using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UIElements;

public class PhotonManager : MonoBehaviourPunCallbacks
{
    private readonly string _version = "1.0";
    private string _userID = "Inha";
    public string _logText;

    //��Ʈ��ũ�� ���� ������� �𸣴ϱ� awake���� ����
    private void Awake()
    {
        Screen.SetResolution(800, 480, false);
        _logText = "Connecting to Master";
        Debug.Log("Connecting to Master");
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    //���� ������ �ٷ� �ݹ���
    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        _logText = "Connected Master";
        Debug.Log(_logText);
        //�ϴ� ���� ����
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        base.OnJoinRandomFailed(returnCode, message);
        _logText = "Failed to join random room";
        Debug.Log(_logText);
        CreateRoom();
    }

    void CreateRoom()
    {
        _logText = "Creating Room";
        Debug.Log(_logText);
        RoomOptions roomOptions = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = 20 };
        PhotonNetwork.CreateRoom("TT_Test Room", roomOptions);
    }

    //�����ϴ� �����ϸ�
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        base.OnCreateRoomFailed(returnCode, message);
        _logText = "Failed to Create room... try again";
        Debug.Log(_logText);
        CreateRoom();
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        _logText = "OnJoinedRoom";
        Debug.Log(_logText);
        PhotonNetwork.Instantiate("Player/Ragdoll", Vector3.zero, Quaternion.identity);
        //Managers.Resource.PhotonNetworkItemInstantiate("Cube");

    }

    private void OnGUI()
    {
        GUI.TextArea(new Rect(10, 10, 300, 20), _logText);
    }

}
