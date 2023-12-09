using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Context : MonoBehaviourPun
{
    private List<IState> _currentStateList = new List<IState>();

    public void SetState(IState state)
    {
        state.MyActor = GetComponent<Actor>();

        Debug.Log("���� ��ȯ");
        _currentStateList.Add(state);
        state.EnterState();
    }

    private void FixedUpdate()
    {
        for(int i = 0; i < _currentStateList.Count; i++)
        {
            var state = _currentStateList[i];

            if(state != null)
            {
                if(state.CoolTime > 0f)
                    state.CoolTime -= Time.deltaTime;

                if(state.CoolTime <= 0f)
                {
                    state.ExitState();
                    _currentStateList[i] = null;
                }

                state.UpdateState();
            }
        }
        _currentStateList.RemoveAll(state => state == null);
    }

    public void ChangeState(IState newState, float time)
    {
        Debug.Log("���� �ٲٶ�� ��û�� ���Ծ��!!");
        
        //���� ���°� �ߺ��Ǹ� ���� �ø��� �ͺ��� �׳� �ִ� ���� ������ �� ���� �����̸� return
        foreach(var state in _currentStateList)
        {
            if (state == newState && state != null)
            {
                Debug.Log("���� ���¿��� �׳� ����");
                return;
            }
        }

        newState.CoolTime = time;

        Debug.Log("���� ��ȯ ��û");
        SetState(newState);
    }
}
