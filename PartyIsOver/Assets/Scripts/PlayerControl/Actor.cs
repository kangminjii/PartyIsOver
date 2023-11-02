using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor : MonoBehaviour
{
    public StatusHandler StatusHandler;
    public BodyHandler BodyHandler;
    public PlayerController PlayerControll;

    public enum ActorState
    {
        Dead = 0x1,
        Unconscious = 0x2,
        Stand = 0x4,
        Run = 0x8,
        Jump = 0x10,
        Fall = 0x20,
        Climb = 0x40,
        Debuff = 0x80,
    }

    public enum DebuffState
    {
        Default =   0x0,  // X
        // ����
        PowerUp =   0x1,  // �Ҳ�
        Invisible = 0x2,  // ����     >> X
        // �����
        Burn =      0x4,  // ȭ��
        Exhausted = 0x8,  // ��ħ
        Slow =      0x10, // ��ȭ
        Freeze =    0x20, // ����
        Shock =     0x40, // ����
        Stun =      0x80, // ����
        // ���º�ȭ
        Drunk =     0x100, // ����
        Balloon =   0x200, // ǳ��
        Ghost =     0x400, // ����
    }


    public ActorState actorState = ActorState.Stand;
    public ActorState lastActorState = ActorState.Run;

    public DebuffState debuffState = DebuffState.Default;


    private void Awake()
    {
        BodyHandler = GetComponent<BodyHandler>();
        StatusHandler = GetComponent<StatusHandler>();
        PlayerControll = GetComponent<PlayerController>();
    }
   

    private void FixedUpdate()
    {
        if (actorState != lastActorState)
        {
            PlayerControll.isStateChange = true;
            Debug.Log("stateChange");
        }
        else
        {
            PlayerControll.isStateChange = false;
        }


        switch (actorState)
        {
            case ActorState.Dead:
                break;
            case ActorState.Unconscious:
                break;
            case ActorState.Stand:
                PlayerControll.Stand();
                break;
            case ActorState.Run:
                PlayerControll.Move();
                break;
            case ActorState.Jump:
                PlayerControll.Jump();
                break;
            case ActorState.Fall:
                break;
            case ActorState.Climb:
                break;
        }

        lastActorState = actorState;
    }
}
