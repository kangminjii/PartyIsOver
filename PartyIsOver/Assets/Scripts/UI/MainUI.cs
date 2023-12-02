using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using UnityEngine.SceneManagement;



public class MainUI : MonoBehaviour
{
    public GameObject StartObject;
    public GameObject CancelPanel;
    public GameObject LoadingPanel;
    public GameObject StoryBoardPanel;
    public GameObject StoryEndingPanel;
    public GameObject SettingsPanel;
    public GameObject CreditPanel;

    public Text NickName;
    public Animator Animator;
    public Image LoadingBar;

    private bool _gameStartFlag;
    private bool _loadingFlag;
    private bool _loadingDelayFlag;
    private bool _keyBoardOn;
    private bool _creditOn;
    private bool _storyBoardOn;

    private float _angle = 0f;
    private float _delayTime = 0.0f;
    private float _loadingTime = 2f;
    
    private int _clickedNumber = 0;
    private string[] _savedStory = new string[8];

    private Transform _storyBoard;
    private Text _storyText;

    private GameObject _mainObject;
    private GameObject _creditObject;
    private GameObject _keyBoardObject;


    private void Start()
    {
        CancelPanel.SetActive(false);
        LoadingPanel.SetActive(false);
        StoryBoardPanel.SetActive(false);
        StoryEndingPanel.SetActive(false);
        SettingsPanel.SetActive(false);
        CreditPanel.SetActive(false);

        NickName.text = PhotonNetwork.NickName;

        _mainObject = GameObject.Find("Main Object");
        _creditObject = GameObject.Find("Credit Object");
        _creditObject.SetActive(false);
        _keyBoardObject = SettingsPanel.transform.GetChild(5).GetChild(0).gameObject;
        _keyBoardObject.SetActive(false);


        _storyBoard = StoryBoardPanel.transform.GetChild(0);
        _storyText = _storyBoard.GetChild(10).GetComponentInChildren<Text>();

        _savedStory[0] = "�� ���� ���� �������� ���� <����Ʈ>���� ��Ƽ�� ���Ƚ��ϴ�!\n���� ��ΰ� �԰� ���ð� ��ſ� ���̳׿�.";
        _savedStory[1] = "�׷��� �� �ϳ��� �ذ� ������.\n��Ƽ�� ��� ���͵��� �ῡ ������ϴ�.";
        _savedStory[2] = "������ ��ħ, ���͵��� ���� ������ ���ư����� �մϴ�.";
        _savedStory[3] = "������, �Ұ����ϴٴ� ���� �� �˰� �Ǿ���.";
        _savedStory[4] = "������ ���� <����Ʈ>�� ���� ���� ���͵��� ���� ���� ������ ��ٸ��� �� ��ٷȽ��ϴ�.";
        _savedStory[5] = "������ �������� ���� ���� �ʾҰ�.\n< ����Ʈ > �� ���ĵ� ���Ǿ� ���� �����߽��ϴ�.";
        _savedStory[6] = "���ָ��� ��ģ ���͵��� ���� ���� ���� ��Ƽ ������ ������ ������ ���ϱ�� �Ͽ����ϴ�.";
        _savedStory[7] = "������ �¸� ��Ģ�� �� �ϳ�, �������� ��Ƴ��� ���Ͱ� �� ��!\n��Ƽ ������ ������ ���ϱ� ���� ��ġ �ο��� ���� ���۵˴ϴ�!";


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
                        if(_creditOn)
                        {
                            OnClickCreditExit();
                            _creditOn = false;
                        }

                        if (_keyBoardOn)
                        {
                            _keyBoardObject.SetActive(false);
                            _keyBoardOn = false;
                        }

                        if(_storyBoardOn)
                        {
                            _clickedNumber = 0;
                            StoryBoardPanel.SetActive(false);
                            _storyBoardOn = false;
                        }
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

    // Game Quit
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

    // Settings
    public void OnClickSettings()
    {
        SettingsPanel.SetActive(true);
    }
    
    public void OnClickSettingsOK()
    {
        // �ٲ� �������

        SettingsPanel.SetActive(false);
    }

    public void OnClickSettingsCancel()
    {
        SettingsPanel.SetActive(false);
    }

    public void OnClickSettingsKeyboard()
    {
        _keyBoardObject.SetActive(true);
        _keyBoardOn = true;
    }

    // Credit
    public void OnClickCredit()
    {
        _mainObject.SetActive(false);
        _creditObject.SetActive(true);
        _creditOn = true;
        CreditPanel.SetActive(true);
    }

    public void OnClickCreditExit()
    {
        _mainObject.SetActive(true);
        _creditObject.SetActive(false);
        CreditPanel.SetActive(false);
    }

    // StoryBoard
    public void OnClickStoryBoard()
    {
        StoryBoardPanel.SetActive(true);
        _storyBoard.GetChild(0).gameObject.SetActive(true);
        _storyText.text = _savedStory[0];

        _clickedNumber = 0;
        _clickedNumber++;
        _storyBoardOn = true;
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
        _clickedNumber = 0;
        StoryEndingPanel.SetActive(false);
    }
}
