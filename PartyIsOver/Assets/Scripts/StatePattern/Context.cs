using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Context : MonoBehaviourPun
{
    private IState _currentState;
    private float time = 2f;

    public void SetState(IState state)
    {
        if(_currentState != null)
        {
            Debug.Log("�ٸ� ���°� ���Ծ�� �׷��� ���� �ִ� ���´� ���� �Ҳ���");
            _currentState.ExitState();
        }

        Debug.Log("���� ��ȯ");
        _currentState = state;
        _currentState.EnterState();
    }

    private void FixedUpdate()
    {
        if(_currentState != null)
            _currentState.UpdateState();
    }

    public void ChangeState(IState newState)
    {
        Debug.Log("���� �ٲٶ�� ��û�� ���Ծ��!!");

        if(_currentState != null)
        {
            Debug.Log("�� ���� ������");
            _currentState.ExitState();
        }

        Debug.Log("���� ��ȯ ��û");
        SetState(newState);
    }


}
