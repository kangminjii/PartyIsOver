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
    public float _turnForce;

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
            // �ӽ� �ڵ�
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                Debug.Log("������ Ÿ��1");
                _itemType = 1;
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                Debug.Log("������ Ÿ��2");
                _itemType = 2;
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                Debug.Log("������ Ÿ��3");
                _itemType = 3;
            }

            // ��ġ�� ��ĥ �ʿ� ���� > Input.GetMouseButtonDown(0)
            if (Input.GetKeyDown(KeyCode.L))
            {
                switch (_itemType)
                {
                    case 1:
                        Item1(_grabGameObject);
                        break;
                    case 2:
                        Item2(_grabGameObject);
                        break;
                    case 3:
                        Item3(_grabGameObject);
                        break;
                }
            }

            // �⺻ ��� �ڼ�
            targetPosition = _grabGameObject.transform.position;
            _jointLeft.targetPosition = targetPosition + new Vector3(10, -10, 0);
            _jointRight.targetPosition = _jointLeft.targetPosition;

            _jointLeftArm.targetPosition = targetPosition;
            _jointRightArm.targetPosition = _jointLeftArm.targetPosition;

            _jointLeftArm.GetComponent<Rigidbody>().AddForce(new Vector3(0, 2.5f, 0));
            _jointRightArm.GetComponent<Rigidbody>().AddForce(new Vector3(0, 2.5f, 0));

           
        }
    }

    private void OnTriggerStay(Collider other)
    {
        //// �� ���
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

        
        // ������ ���
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
            else if (Input.GetMouseButtonDown(1))
            {
                Debug.Log("����");
               
                Destroy(_gameObjectJoint);
                Destroy(_gameObjectJointLeft);
                Destroy(_gameObjectJointRight);

                _grabGameObject = null;
            }

        }
    }

    // ����: ������ �Ʒ��� ������
    private void Item1(GameObject grabGameObj)
    {
        StartCoroutine("Item1Action", grabGameObj);
    }
    // �õ���ġ: 360�� ȸ��
    private void Item2(GameObject grabGameObj)
    {
        StartCoroutine("Item2Action", grabGameObj);
    }
    // ���̽� ����: ��� ������ �Ʒ��� �ֵθ���
    private void Item3(GameObject grabGameObj)
    {
        StartCoroutine("Item3Action", grabGameObj);
    }

    IEnumerator Item1Action(GameObject grabGameObj)
    {
        int forcingCount = 2000;

        _jointLeft.GetComponent<Rigidbody>().AddForce(new Vector3(0, _turnForce, 0));
        _jointRight.GetComponent<Rigidbody>().AddForce(new Vector3(0, _turnForce, 0));

        while (forcingCount > 0)
        {
            AlignToVector(_jointLeft.GetComponent<Rigidbody>(), _jointLeft.transform.position, new Vector3(0f, 0.2f, 0f), 0.1f, 4f);
            AlignToVector(_jointRight.GetComponent<Rigidbody>(), _jointRight.transform.position, new Vector3(0f, 0.2f, 0f), 0.1f, 4f);
            forcingCount--;
        }
        Debug.Log("�ڷ�ƾ ��");

        yield return 0;
    }
    IEnumerator Item2Action(GameObject grabGameObj)
    {
        int forcingCount = 10000;

        _jointLeft.GetComponent<Rigidbody>().AddForce(new Vector3(_turnForce * 2.5f, 0, 0));
        //_jointRight.GetComponent<Rigidbody>().AddForce(new Vector3(_turnForce * 5, 0, 0));
        _jointLeftArm.GetComponent<Rigidbody>().AddForce(new Vector3(_turnForce * 2.5f, 0, 0));
        //_jointRightArm.GetComponent<Rigidbody>().AddForce(new Vector3(_turnForce * 50, 0, 0));

        while (forcingCount > 0)
        {
            AlignToVector(_jointLeft.GetComponent<Rigidbody>(), _jointLeft.transform.position, new Vector3(1f, 0f, 0f), 0.1f, 2f);
            AlignToVector(_jointRight.GetComponent<Rigidbody>(), _jointRight.transform.position, new Vector3(1f, 0f, 0f), 0.1f, 2f);
            AlignToVector(_jointLeftArm.GetComponent<Rigidbody>(), _jointLeftArm.transform.position, new Vector3(1f, 0f, 0f), 0.1f, 2f);
            AlignToVector(_jointRightArm.GetComponent<Rigidbody>(), _jointRightArm.transform.position, new Vector3(1f, 0f, 0f), 0.1f, 2f);
            forcingCount--;
        }
        Debug.Log("�ڷ�ƾ ��");

        yield return 0;
    }

   



    //������ٵ� part�� alignmentVector������ targetVector�������� ȸ��
    public void AlignToVector(Rigidbody part, Vector3 alignmentVector, Vector3 targetVector, float stability, float speed)
    {
        if (part == null)
        {
            return;
        }
        Vector3 vector = Vector3.Cross(Quaternion.AngleAxis(part.angularVelocity.magnitude * 57.29578f * stability / speed, part.angularVelocity) * alignmentVector, targetVector * 10f);
        if (!float.IsNaN(vector.x) && !float.IsNaN(vector.y) && !float.IsNaN(vector.z))
        {
            part.AddTorque(vector * speed * speed);
            {
                Debug.DrawRay(part.position, alignmentVector * 0.2f, Color.red, 0f, depthTest: false);
                Debug.DrawRay(part.position, targetVector * 0.2f, Color.green, 0f, depthTest: false);
            }
        }
    }
}
