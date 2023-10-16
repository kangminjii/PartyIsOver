using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class PlayerItem : MonoBehaviour
{
    // Player �̸�
    public Text PlayerName;

    // ȭ�鿡 �������� Player ����
    Image BackgroundImage;
    public Color HighlightColor;
    public GameObject LeftArrowButton;
    public GameObject RightArrowButton;


    ExitGames.Client.Photon.Hashtable PlayerProperties = new ExitGames.Client.Photon.Hashtable();
    

    private void Start()
    {
        //BackgroundImage = GetComponent<Image>();
    }

    // Player�� �̸� ��������
    public void SetPlayerInfo(Player player)
    {
        PlayerName.text = player.NickName;
    }

    public void ApplyLocalChanges()
    {
        //BackgroundImage.color = HighlightColor;
        LeftArrowButton.SetActive(true);
        RightArrowButton.SetActive(true);
    }


}
