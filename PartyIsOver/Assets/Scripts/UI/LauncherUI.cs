using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;


public class LauncherUI : MonoBehaviour
{
    private GameObject _controlPanel;
    private GameObject _cancelPanel;
    private GameObject _errorPanel;
    private GameObject _feedbackPanel;
    private Regex specialRegex = new Regex(@"[~!@\#$%^&*\()\=+|\\/:;?""<>'\[\]]");
    private string _nickName;

    public Text ErrorText;


    void Awake()
    {
        Init();
    }
    void Init()
    {
        _controlPanel = GameObject.Find("Control Panel");
        _cancelPanel = GameObject.Find("Cancel Panel");
        _errorPanel = GameObject.Find("Error Panel");
        _feedbackPanel = GameObject.Find("Feedback Panel");

        _controlPanel.SetActive(true);
        _cancelPanel.SetActive(false);
        _errorPanel.SetActive(false);
        _feedbackPanel.SetActive(false);

        Managers.Input.KeyboardAction -= OnKeyboardEvent;
        Managers.Input.KeyboardAction += OnKeyboardEvent;
    }

    void OnKeyboardEvent(Define.KeyboardEvent evt)
    {
        switch (evt)
        {
            case Define.KeyboardEvent.Press:
                {
                    //if (Input.GetKeyDown(KeyCode.KeypadEnter))
                        //OnClickErrorOK();
                }
                break;
        }
    }

    public void OnClickGameStart()
    {
        if (PhotonNetwork.NickName.Length < 2 || PhotonNetwork.NickName.Length > 12)
        {
            _errorPanel.SetActive(true);
            ErrorText.text = "닉네임 글자 수가 너무 적거나 많습니다.";
            return;
        }

        if (specialRegex.IsMatch(PhotonNetwork.NickName))
        {
            _errorPanel.SetActive(true);
            ErrorText.text = "사용 불가능한 특수 문자가 포함되어 있습니다.";
            return;
        }

        _nickName = PhotonNetwork.NickName;
        _feedbackPanel.SetActive(true);
        _feedbackPanel.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = _nickName;
    }

    public void OnClickFeedbackOK()
    {
        PhotonManager.Instance.Connect();
        SceneManager.LoadSceneAsync("[2]Main");
    }
    public void OnClickFeedbackPanelCancel()
    {
        _feedbackPanel.SetActive(false);
    }

    public void OnClickErrorOK()
    {
        _errorPanel.SetActive(false);
    }

    public void OnClickCancelPanel()
    {
        _cancelPanel.SetActive(true);
    }
    public void OnClickCancelPanelCancel()
    {
        _cancelPanel.SetActive(false);
    }

    public void OnClickGameQuit()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }


    // 빌드시 삭제할 부분
    public void OnClickShortcut()
    {
        PhotonManager.Instance.Connect();
        PhotonManager.Instance.LoadNextScene("[3]Lobby");
        SceneManager.LoadSceneAsync("[3]Lobby");
    }
}
