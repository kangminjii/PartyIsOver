using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine;

public class Context : MonoBehaviourPun
{
    private List<IDebuffState> _currentStateList = new List<IDebuffState>();

    public void SetState(IDebuffState state)
    {
        state.MyActor = GetComponent<Actor>();

        _currentStateList.Add(state);
        state.EnterState();
    }

    private void FixedUpdate()
    {
        for (int i = 0; i < _currentStateList.Count; i++)
        {
            var state = _currentStateList[i];

            if (state != null)
            {
                //��ħ ���¸��� 100�� �ƴ϶�� ��� ������Ʈ
                if (state.ToString().Contains("Exhausted") && state.MyActor.Stamina != 100)
                {
                    state.UpdateState();
                }
                //��ħ ���°� �ƴ϶�� �Ϲ� ���� ���ӽð����� Ȯ��
                else
                {
                    if(state.ToString().Contains("Exhausted") && state.MyActor.Stamina == 100)
                        state.ExitState();

                    if (state.CoolTime > 0f)
                        state.CoolTime -= Time.deltaTime;

                    if (state.CoolTime <= 0f)
                    {
                        state.ExitState();
                        _currentStateList[i] = null;
                    }

                    state.UpdateState();
                }
            }
        }
        _currentStateList.RemoveAll(state => state == null);
    }

    public void ChangeState(IDebuffState newState, float time = 0)
    {        
        //���� ���°� �ߺ��Ǹ� ���� �ø��� �ͺ��� �׳� �ִ� ���� ������ �� ���� �����̸� return
        foreach(var state in _currentStateList)
        {
            if (state == newState && state != null)
                return;
        }

        newState.CoolTime = time;

        SetState(newState);
    }
}
