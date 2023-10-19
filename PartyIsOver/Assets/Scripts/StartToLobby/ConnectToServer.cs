using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ConnectToServer : MonoBehaviourPunCallbacks
{
    public InputField UsernameInput;
    public Text ButtonText;

    public void OnClickConnect()
    {
        if(UsernameInput.text.Length >= 1)
        {
            // ����� �̸� �Է� �� ������ ǥ��
            PhotonNetwork.NickName = UsernameInput.text; // ����� �г��� ����
            ButtonText.text = "Connecting ...";

            // ���� ����
            PhotonNetwork.ConnectUsingSettings();

            // �� ��ȯ�� �ʿ�
            PhotonNetwork.AutomaticallySyncScene = true;
        }
    }

    public override void OnConnectedToMaster()
    {
        SceneManager.LoadScene("Lobby");
    }


}
