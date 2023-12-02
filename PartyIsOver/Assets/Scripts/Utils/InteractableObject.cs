using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class InteractableObject : MonoBehaviourPun
{
    Rigidbody _rb;
    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }


    public enum Damage
    {
        Ignore = 0,
        Default = 1,
        Object = 2,
        Punch = 3,
        DropKick = 4,
        Headbutt = 5,
        Knockout = 6,
        Special= 7,

        PowerUp = 8,
        Burn = 9,
        Slow = 10,
        Ice = 11,
        Shock = 12,
        Stun = 13,
        Drunk = 14,
        Balloon = 15,
    }

    public Damage damageModifier = Damage.Default;


    public void PullingForceTrigger(Vector3 dir ,float power)
    {
        photonView.RPC("ApplyPullingForce", RpcTarget.All, dir, power);
    }

    [PunRPC]
    public void ApplyPullingForce(Vector3 dir, float power)
    {
        if (!photonView.IsMine)
            return;
        //Vector3 force = (vel - _rb.velocity) * _rb.mass; // �ӵ� ���̿� ������ ���Ͽ� ���� ���
        //_rb.AddForce(force, ForceMode.VelocityChange); // Impulse ��带 ����Ͽ� ���������� ���� ����
        _rb.AddForce(Vector3.ClampMagnitude(dir.normalized * power, 100f), ForceMode.VelocityChange);
    }

    [PunRPC]
    public void ChangeUseTypeTrigger(float waitTime, float useTime)
    {
        StartCoroutine(ChangeUseType(waitTime, useTime));
    }
    IEnumerator ChangeUseType(float waitTime, float useTime)
    {
        Damage tempDamage = damageModifier;
        yield return new WaitForSeconds(waitTime);

        if (GetComponent<Item>() != null)
        {
            damageModifier = GetComponent<Item>().ItemData.UseDamageType;
        }
        else
        {
            damageModifier = Damage.Object;
        }

        yield return new WaitForSeconds(useTime);
        damageModifier = tempDamage;
    }
}
