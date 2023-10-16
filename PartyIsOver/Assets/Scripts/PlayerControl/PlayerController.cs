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
    }

    private void FixedUpdate()
    {
        InputMove();
        InputJump();
    }

    

    private void InputMove()
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
    }
    private void InputJump()
    {
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


#region â��
/*
 



------------------------------------------------------------------------------------------------------------------
�ڵ�� �߰� �غ����ߴµ� ������
    private Vector3 _networkPosition;
    private Quaternion _networkRotation;

public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(_hipRigidbody.position);
            stream.SendNext(_hipRigidbody.rotation);
        }
        else
        {
            _networkPosition = (Vector3)stream.ReceiveNext();
            _networkRotation = (Quaternion)stream.ReceiveNext();
        }
    }

    private void Update()
    {
        if(!photonView.IsMine)
        {
            _hipRigidbody.position = _networkPosition;
            _hipRigidbody.rotation = _networkRotation;
        }
    }

 
 */
#endregion