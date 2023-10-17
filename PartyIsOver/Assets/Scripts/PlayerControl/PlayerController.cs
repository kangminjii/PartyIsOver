using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Photon.Pun;

public class PlayerController : MonoBehaviourPun
{
    [Header("�յ� �ӵ�")]
    public float Speed;
    [Header("�¿� �ӵ�")]
    public float StrafeSpeed;
    [Header("���� ��")]
    public float JumpForce;

    private GameObject _hipGameObject;
    private Rigidbody _hipRigidbody;
    public bool IsGrounded;

    void Start()
    {
        _hipGameObject = GameObject.Find("pelvis");
        _hipRigidbody = _hipGameObject.GetComponent<Rigidbody>();

        //�Ǽ��� �����ϱ� ���ؼ� ������ �ι� �����°� ����
        Managers.Input.KeyAction -= OnKeyboard;
        //� Ű�� ������ ������û �ع���
        Managers.Input.KeyAction += OnKeyboard;
    }


    private void OnKeyboard()
    {
        if (Input.GetKey(KeyCode.W))
            if (Input.GetKey(KeyCode.LeftShift))
                _hipRigidbody.AddForce(transform.forward * Speed * 2f);
            else
                _hipRigidbody.AddForce(transform.forward * Speed);

        if (Input.GetKey(KeyCode.S))
            if (Input.GetKey(KeyCode.LeftShift))
                _hipRigidbody.AddForce(-transform.forward * Speed * 2f);
            else
                _hipRigidbody.AddForce(-transform.forward * Speed);

        if (Input.GetKey(KeyCode.A))
            if (Input.GetKey(KeyCode.LeftShift))
                _hipRigidbody.AddForce(-transform.right * StrafeSpeed * 2f);
            else
                _hipRigidbody.AddForce(-transform.right * StrafeSpeed);

        if (Input.GetKey(KeyCode.D))
            if (Input.GetKey(KeyCode.LeftShift))
                _hipRigidbody.AddForce(transform.right * StrafeSpeed * 2f);
            else
                _hipRigidbody.AddForce(transform.right * StrafeSpeed);

        if (Input.GetAxis("Jump") > 0)
        {
            if (IsGrounded)
            {
                _hipRigidbody.AddForce(new Vector3(0, JumpForce, 0));
                IsGrounded = false;
            }
        }

    }

}