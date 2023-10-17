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

    public static int LayerCnt = 7;
    public string TestTag = "Item";

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
        ChangeLayerRecursively(gameObject, LayerCnt++);
        ChangeTagRecursively(gameObject, TestTag);
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

    private void ChangeLayerRecursively(GameObject obj, int layer)
    {
        obj.layer = layer;

        foreach (Transform child in obj.transform)
        {
            ChangeLayerRecursively(child.gameObject, layer);
        }
    }

    private void ChangeTagRecursively(GameObject obj, string tag)
    {
        obj.tag = tag;

        foreach (Transform child in obj.transform)
        {
            ChangeTagRecursively(child.gameObject, tag);
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