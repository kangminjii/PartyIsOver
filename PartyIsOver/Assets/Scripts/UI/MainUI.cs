using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using UnityEngine.SceneManagement;



public class MainUI : MonoBehaviour
{
    public Text NickName;
    public GameObject StartObject;
    public GameObject CancelPanel;
    public GameObject LoadingPanel;
    public GameObject StoryBoardPanel;
    public GameObject StoryEndingPanel;
    public Animator Animator;
    public Image LoadingBar;

    private bool _gameStartFlag;
    private bool _loadingFlag;
    private bool _loadingDelayFlag;
    private float _angle = 0f;
    private float _delayTime = 0.0f;
    private float _loadingTime = 2f;

    private int _clickedNumber = 0;
    private Transform _storyBoard;
    private Text _storyText;
    private string[] _savedStory = new string[8];

    private GameObject _mainObject;
    private GameObject _creditObject;


    private void Start()
    {
        NickName.text = PhotonNetwork.NickName;
        CancelPanel.SetActive(false);
        LoadingPanel.SetActive(false);
        StoryBoardPanel.SetActive(false);
        StoryEndingPanel.SetActive(false);

        _mainObject = GameObject.Find("Main Object");
        _creditObject = GameObject.Find("Credit Object");
        _creditObject.SetActive(false);


        _storyBoard = StoryBoardPanel.transform.GetChild(0);
        _storyText = _storyBoard.GetChild(10).GetComponentInChildren<Text>();

        _savedStory[0] = "한 해의 멋진 마무리를 위해 <아지트>에서 파티가 열렸습니다!\n몬스터 모두가 먹고 마시고 즐거워 보이네요.";
        _savedStory[1] = "그렇게 또 하나의 해가 저물고….\n파티를 즐긴 몬스터들은 잠에 들었습니다.";
        _savedStory[2] = "다음날 아침, 몬스터들은 이제 집으로 돌아가려고 합니다.";
        _savedStory[3] = "하지만, 불가능하다는 것을 곧 알게 되었죠.";
        _savedStory[4] = "폭설로 인해 <아지트>에 갇혀 버린 몬스터들은 눈이 녹을 때까지 기다리고 또 기다렸습니다.";
        _savedStory[5] = "하지만 오래도록 눈은 녹지 않았고….\n< 아지트 > 의 음식도 고갈되어 가기 시작했습니다.";
        _savedStory[6] = "굶주림에 지친 몬스터들은 논의 끝에 남은 파티 음식의 주인을 격투로 정하기로 하였습니다.";
        _savedStory[7] = "격투의 승리 규칙은 단 하나, 마지막에 살아남은 몬스터가 될 것!\n파티 음식의 주인을 정하기 위한 눈치 싸움이 이제 시작됩니다!";


        Managers.Input.KeyboardAction -= OnKeyboardEvent;
        Managers.Input.KeyboardAction += OnKeyboardEvent;
    }

    void OnKeyboardEvent(Define.KeyboardEvent evt)
    {
        switch (evt)
        {
            case Define.KeyboardEvent.Click:
                {
                    if (Input.GetKeyDown(KeyCode.Escape))
                    {
                        OnClickCreditExit();
                    }
                }
                break;
        }
    }


    void Update()
    {
        if(_gameStartFlag)
        {
            _angle -= 1f;
            StartObject.transform.rotation = Quaternion.Euler(_angle, 180, 0);

            if (_angle <= -90)
            {
                _gameStartFlag = false;
                _loadingFlag = true;
            }
        }

        if (_loadingFlag)
            DelayTime();
    }

    void DelayTime()
    {
        _delayTime += Time.deltaTime;

        if (_delayTime >= 1 && _loadingDelayFlag == false)
        {
            LoadingPanel.SetActive(true);
            _loadingDelayFlag = true;
        }

        LoadingBar.fillAmount = (_delayTime - 1) / _loadingTime;

        if ((_delayTime - 1) >= _loadingTime)
        {
            _delayTime = 0;
            _gameStartFlag = false;
            _loadingFlag = false;
            PhotonManager.Instance.Connect();
            PhotonManager.Instance.LoadNextScene("[3]Lobby");
            SceneManager.LoadSceneAsync("[3]Lobby");
        }
    }

    public void OnClickStart()
    {
        _gameStartFlag = true;
        Animator.SetBool("Pose", true);
    }

    public void OnClickPopup()
    {
        CancelPanel.SetActive(true);
    }

    public void OnClickPopUpCancel()
    {
        CancelPanel.SetActive(false);
    }

    public void OnClickPopUpGameQuit()
    {
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #else
                Application.Quit();
        #endif
    }


    public void OnClickCredit()
    {
        _mainObject.SetActive(false);
        _creditObject.SetActive(true);
    }

    public void OnClickCreditExit()
    {

    }

    public void OnClickSettings()
    {

    }

    public void OnClickStoryBoard()
    {
        StoryBoardPanel.SetActive(true);
        _storyBoard.GetChild(0).gameObject.SetActive(true);
        _storyText.text = _savedStory[0];
        _clickedNumber++;
    }

    public void OnClickStoryBoardNext()
    {
        if (_clickedNumber >= 8)
        {
            StoryBoardPanel.SetActive(false);
            StoryEndingPanel.SetActive(true);
        }

        for (int i = 0; i < 8; i++)
        {
            if(i == _clickedNumber)
            {
                _storyBoard.GetChild(i).gameObject.SetActive(true);
                _storyText.text = _savedStory[i];
            }
            else
                _storyBoard.GetChild(i).gameObject.SetActive(false);
        }


        _clickedNumber++;
    }

    public void OnClickStoryEnding()
    {
        StoryEndingPanel.SetActive(false);
    }
}
