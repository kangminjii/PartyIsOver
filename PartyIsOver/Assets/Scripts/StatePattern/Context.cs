using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Context : MonoBehaviourPun
{
    private IState _currentState;
    [SerializeField]
    private float _stunTime = 2;
    private float _currentTime = 0;

    public void Start()
    {
        Managers.DurationTime.DurationTimeExpired += OnDurationTime;
    }

    public void SetState(IState state)
    {
        state.MyActor = GetComponent<Actor>();

        Debug.Log("���� ��ȯ");
        _currentState = state;
        _currentState.EnterState();
    }

    private void FixedUpdate()
    {
        if(_currentState != null)
        {
            _currentState.UpdateState();
            Managers.DurationTime.UpdateDurationTimes(Time.fixedDeltaTime);
        }
    }

    public void ChangeState(IState newState)
    {
        Debug.Log("���� �ٲٶ�� ��û�� ���Ծ��!!");

        if(_currentState != null)
        {
            Debug.Log("�� ���� ������");
            _currentState.ExitState();
        }
        //���� �ð� �߰� �ϱ�
        Managers.DurationTime.SetDurationTime(newState.ToString(), _stunTime);
        Debug.Log("���� ��ȯ ��û");
        SetState(newState);
        
    }

    //�̺�Ʈ�� ���Ḧ �ϴ� ��
    private void OnDurationTime(string state)
    {
        if(_currentState != null)
        {
            _currentState.ExitState();
        }
        _currentState = null;
    }


}
