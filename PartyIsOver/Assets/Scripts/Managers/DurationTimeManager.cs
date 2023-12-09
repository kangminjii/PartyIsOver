using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Video;

public class DurationTimeManager : MonoBehaviour 
{
    private Dictionary<string, float> DurationTimes = new Dictionary<string, float>();

    //���� �ð� ���� �̺�Ʈ ����
    public event Action<string> DurationTimeExpired;

    //���� �ð� ����
    public void SetDurationTime(string state ,float duration)
    {
        if(DurationTimes.ContainsKey(state))
        {
            DurationTimes[state] = duration;
        }
        else
        {
            DurationTimes.Add(state, duration);
        }
    }

    //���� �ð� ���� �� ���� �̺�Ʈ �߻�
    public void UpdateDurationTimes(float deltaTime)
    {
        //���� ó���� �ʿ�� �ϴ� ������ ������ ������ ����� �ߺ����� ������ ������
        foreach(var state in DurationTimes.Keys.ToList()) 
        {
            DurationTimes[state] = Mathf.Max(0f, DurationTimes[state] - deltaTime);
            Debug.Log("Time : " + DurationTimes[state]);
            if (DurationTimes[state] <= 0f)
            {
                Debug.Log("���� �̺�Ʈ ���� ����");
                OnDurationTimesExpired(state);
                Debug.Log("���� �̺�Ʈ ���� ��");
            }
        }
        Debug.Log("���� �̺�Ʈ ++��¥++ ���� ��");
    }

    //���ӽð� ���� �̺�Ʈ �߻�
    protected virtual void OnDurationTimesExpired(string state)
    {
        //���� ������ �̺�Ʈ ���� ������ Stun
        DurationTimeExpired?.Invoke(state);
    }


}
