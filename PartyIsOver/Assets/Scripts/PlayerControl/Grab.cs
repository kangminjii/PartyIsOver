using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine;

public class Grab : MonoBehaviourPun
{
    private GameObject _grabGameObject;
    private Rigidbody _grabRigidbody;

    private FixedJoint _gameObjectJoint;

    // ��� �߰�
    private FixedJoint _gameObjectJointLeft;
    private FixedJoint _gameObjectJointRight;
    private Rigidbody _grabRigidbody2;

    // ��� �ڼ� �߰�
    private ConfigurableJoint _jointLeft;
    private ConfigurableJoint _jointRight;
    private ConfigurableJoint _jointLeftArm;
    private ConfigurableJoint _jointRightArm;
    Vector3 targetPosition;

    // ������ ����
    private int _itemType;
    public float _turnSpeed;

    void Start()
    {
        // ��� �߰�
        _grabRigidbody = GetComponent<Rigidbody>();
        _grabRigidbody2 = GameObject.Find("GreenFistR").GetComponent<Rigidbody>();

        _jointLeft = GetComponent<ConfigurableJoint>();
        _jointRight = GameObject.Find("GreenFistR").GetComponent<ConfigurableJoint>();

        _jointLeftArm = GameObject.Find("GreenForeArmL").GetComponent<ConfigurableJoint>();
        _jointRightArm = GameObject.Find("GreenForeArmR").GetComponent<ConfigurableJoint>();
    }

    void Update()
    {
        if(_grabGameObject != null)
        {
            if (Input.GetMouseButton(1))
            {
                if (_itemType == 1)
                    Item1(_grabGameObject);
            }

            // �⺻ ��� �ڼ�
            targetPosition = _grabGameObject.transform.position;
            _jointLeft.targetPosition = targetPosition + new Vector3(10, -10, 0);
            _jointRight.targetPosition = targetPosition + new Vector3(10, -10, 10);

            _jointLeftArm.targetPosition = targetPosition;
            _jointRightArm.targetPosition = targetPosition;
            
            _jointLeftArm.GetComponent<Rigidbody>().AddForce(new Vector3(0, 2.5f, 0));
            _jointRightArm.GetComponent<Rigidbody>().AddForce(new Vector3(0, 2.5f, 0));

           
        }
    }

    private void OnTriggerStay(Collider other)
    {
        //if (other.gameObject.layer == LayerMask.NameToLayer("Player2"))
        //{
        //    Debug.Log("player2 grab");
        //    if (Input.GetMouseButton(0))
        //    {
        //        if (_grabGameObject == null)
        //        {
        //            _grabGameObject = other.gameObject;

        //            _gameObjectJoint = _grabGameObject.AddComponent<FixedJoint>();
        //            _gameObjectJoint.connectedBody = _grabRigidbody;
        //            _gameObjectJoint.breakForce = 9001;
        //        }
        //    }
        //    else if (_grabGameObject != null)
        //    {
        //        Destroy(_grabGameObject.GetComponent<FixedJoint>());
        //        _gameObjectJoint = null;
        //        _grabGameObject = null;
        //    }
        //}


        // ��� �߰�
        if (other.gameObject.CompareTag("Item"))
        {
            // �Ѽ�
            if (Input.GetKey(KeyCode.F))
            {
                if (_grabGameObject == null)
                {
                    Debug.Log("�Ѽ� ��Ҵ�");

                    _grabGameObject = other.gameObject;

                    _gameObjectJoint = _grabGameObject.AddComponent<FixedJoint>();
                    _gameObjectJoint.connectedBody = _grabRigidbody;
                    _gameObjectJoint.breakForce = 9001;
                }
            }
            // ���
            else if (Input.GetKey(KeyCode.G))
            {
                if (_grabGameObject == null)
                {
                    Debug.Log("��� ��Ҵ�");

                    _grabGameObject = other.gameObject;

                    _gameObjectJointLeft = _grabGameObject.AddComponent<FixedJoint>();
                    _gameObjectJointLeft.connectedBody = _grabRigidbody;
                    _gameObjectJointLeft.breakForce = 9001;

                    _gameObjectJointRight = _grabGameObject.AddComponent<FixedJoint>();
                    _gameObjectJointRight.connectedBody = _grabRigidbody2;
                    _gameObjectJointRight.breakForce = 9001;
                }
            }
            // ����
            else if (Input.GetKey(KeyCode.E))
            {
                Debug.Log("����");
               
                Destroy(_gameObjectJoint);
                Destroy(_gameObjectJointLeft);
                Destroy(_gameObjectJointRight);

                _grabGameObject = null;
            }

            // ������ ���� �ĺ�
            if(other.name == "Item1")
            {
                _itemType = 1;
            }

        }
    }


    private void Item1(GameObject grabGameObj)
    {
        Debug.Log("CLicked");

        _jointLeftArm.GetComponent<Rigidbody>().AddForce(new Vector3(0, 2.5f, 0));
        _jointRightArm.GetComponent<Rigidbody>().AddForce(new Vector3(0, 2.5f, 0));

        Vector3 center = GameObject.Find("GreenHip").GetComponent<Transform>().position;

        grabGameObj.transform.RotateAround(center, Vector3.up, _turnSpeed);

        //GameObject hip = Util.FindChild(gameObject, "pelvis", true);
        //hip.GetComponent<ConfigurableJoint>().targetPosition = grabGameObj.transform.position;
    }
}
