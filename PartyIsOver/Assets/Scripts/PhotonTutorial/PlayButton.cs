using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayButton : MonoBehaviour
{
    Button ButtonPlay;

    void Start()
    {
        ButtonPlay = transform.GetComponent<Button>();
        ButtonPlay.onClick.AddListener(PhotonManager.Instance.JoinRoom);
    }
}
