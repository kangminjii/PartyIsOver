using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CoolTimeMnaager
{
    private Dictionary<string, float> cooldowns = new Dictionary<string, float>();

    public event Action<string> CooldownExpired;

    //���¿� ���� ��Ÿ�� ����
    public void SetCooldown(string state, float duration)
    {
        if(cooldowns.ContainsKey(state))
        {
            cooldowns[state] = duration;
        }
        else
        {
            cooldowns.Add(state, duration);
        }
    }

    //��Ÿ�� ����
    public void UpdateCooldowns(float deltaTime)
    {
        foreach(var state in cooldowns.Keys.ToList())
        {
            cooldowns[state] = Mathf.Max(0f, cooldowns[state] - deltaTime);

            if (cooldowns[state] <= 0f)
            {
                OnCooldownExpired(state);
            }
        }
    }

    protected virtual void OnCooldownExpired(string state)
    {
        CooldownExpired?.Invoke(state);
    }

    //��Ÿ�� Ȯ��
    public bool IsCooldownActive(string state)
    {
        return cooldowns.ContainsKey(state) && cooldowns[state] > 0f;
    }

}
