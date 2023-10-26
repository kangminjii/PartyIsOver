using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine;

public class Grab : MonoBehaviourPun
{
   


    private GameObject _grabGameObject;
    //private Rigidbody _grabRigidbody;

    private FixedJoint _gameObjectJoint;

    // ��� �߰�
    private FixedJoint _gameObjectJointLeft;
    private FixedJoint _gameObjectJointRight;
    //private Rigidbody _grabRigidbody2;

    // ��� �ڼ� �߰�
    private ConfigurableJoint _jointLeft;
    private ConfigurableJoint _jointRight;
    private ConfigurableJoint _jointLeftForeArm;
    private ConfigurableJoint _jointRightForeArm;
    private ConfigurableJoint _jointLeftUpperArm;
    private ConfigurableJoint _jointRightUpperArm;
    private ConfigurableJoint _jointChest;

    Vector3 targetPosition;

    // ������ ����
    private int _itemType;
    public float _turnForce;

    // dummy �߰��� ���� �۾�
    private TargetingHandler targetingHandler;
    [SerializeField]
    private BodyHandler bodyHandler;


    void Start()
    {
        targetingHandler = GetComponent<TargetingHandler>();
        bodyHandler = GetComponent<BodyHandler>();




        _jointRight = GameObject.Find("GreenFistR").GetComponent<ConfigurableJoint>();

        _jointLeftForeArm = GameObject.Find("GreenForeArmL").GetComponent<ConfigurableJoint>();
        _jointRightForeArm = GameObject.Find("GreenForeArmR").GetComponent<ConfigurableJoint>();

        _jointLeftUpperArm = GameObject.Find("GreenUpperArmL").GetComponent<ConfigurableJoint>();
        _jointRightUpperArm = GameObject.Find("GreenUpperArmR").GetComponent<ConfigurableJoint>();

        _jointChest = GameObject.Find("GreenChest").GetComponent<ConfigurableJoint>();


       

    }

    void Update()
    {
        if (_grabGameObject != null)
        {
            // ����
            if (Input.GetMouseButtonDown(1))
            {
                Debug.Log("����");

                Destroy(_gameObjectJoint);
                Destroy(_gameObjectJointLeft);
                Destroy(_gameObjectJointRight);

                _grabGameObject = null;

                // ���� ����
                bodyHandler.LeftHand.PartJoint.angularYMotion = ConfigurableJointMotion.Limited;
                _jointLeftForeArm.angularYMotion = ConfigurableJointMotion.Limited;
                _jointLeftUpperArm.angularYMotion = ConfigurableJointMotion.Limited;
                bodyHandler.LeftHand.PartJoint.angularZMotion = ConfigurableJointMotion.Limited;
                _jointLeftForeArm.angularZMotion = ConfigurableJointMotion.Limited;
                _jointLeftUpperArm.angularZMotion = ConfigurableJointMotion.Limited;

                _jointRight.angularYMotion = ConfigurableJointMotion.Limited;
                _jointRightForeArm.angularYMotion = ConfigurableJointMotion.Limited;
                _jointRightUpperArm.angularYMotion = ConfigurableJointMotion.Limited;
                _jointRight.angularZMotion = ConfigurableJointMotion.Limited;
                _jointRightForeArm.angularZMotion = ConfigurableJointMotion.Limited;
                _jointRightUpperArm.angularZMotion = ConfigurableJointMotion.Limited;
                return;
            }

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
            bodyHandler.LeftHand.PartJoint.targetPosition = targetPosition + new Vector3(0, 0, 20);
            _jointRight.targetPosition = bodyHandler.LeftHand.PartJoint.targetPosition;

            _jointLeftForeArm.targetPosition = targetPosition;
            _jointRightForeArm.targetPosition = _jointLeftForeArm.targetPosition;
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
            //���콺�� Ŭ���� ���� �տ� ������Ʈ�� Ž���ϰ�
            //targetingHandler.SearchTarget();


            // �Ѽ�
            if (Input.GetKey(KeyCode.F))
            {
                if (_grabGameObject == null)
                {
                    Debug.Log("�Ѽ� ��Ҵ�");

                    _grabGameObject = other.gameObject;

                    _gameObjectJoint = _grabGameObject.AddComponent<FixedJoint>();
                    _gameObjectJoint.connectedBody = bodyHandler.LeftHand.PartRigidbody;
                    _gameObjectJoint.breakForce = 9001;

                    // �Ѽ� �ֵθ��� ����� ���� ���� �κ����
                    bodyHandler.LeftHand.PartJoint.angularYMotion = ConfigurableJointMotion.Locked;
                    _jointLeftForeArm.angularYMotion = ConfigurableJointMotion.Locked;
                    _jointLeftUpperArm.angularYMotion = ConfigurableJointMotion.Locked;
                    bodyHandler.LeftHand.PartJoint.angularZMotion = ConfigurableJointMotion.Locked;
                    _jointLeftForeArm.angularZMotion = ConfigurableJointMotion.Locked;
                    _jointLeftUpperArm.angularZMotion = ConfigurableJointMotion.Locked;
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
                    _gameObjectJointLeft.connectedBody = bodyHandler.LeftHand.PartRigidbody;
                    _gameObjectJointLeft.breakForce = 9001;

                    _gameObjectJointRight = _grabGameObject.AddComponent<FixedJoint>();
                    _gameObjectJointRight.connectedBody = bodyHandler.RightHand.PartRigidbody;
                    _gameObjectJointRight.breakForce = 9001;

                    // ��� �ֵθ��� ����� ���� ���� �κ����
                    bodyHandler.LeftHand.PartJoint.angularYMotion = ConfigurableJointMotion.Locked;
                    _jointLeftForeArm.angularYMotion = ConfigurableJointMotion.Locked;
                    _jointLeftUpperArm.angularYMotion = ConfigurableJointMotion.Locked;
                    bodyHandler.LeftHand.PartJoint.angularZMotion = ConfigurableJointMotion.Locked;
                    _jointLeftForeArm.angularZMotion = ConfigurableJointMotion.Locked;
                    _jointLeftUpperArm.angularZMotion = ConfigurableJointMotion.Locked;

                    bodyHandler.RightHand.PartJoint.angularYMotion = ConfigurableJointMotion.Locked;
                    _jointRightForeArm.angularYMotion = ConfigurableJointMotion.Locked;
                    _jointRightUpperArm.angularYMotion = ConfigurableJointMotion.Locked;
                    bodyHandler.RightHand.PartJoint.angularZMotion = ConfigurableJointMotion.Locked;
                    _jointRightForeArm.angularZMotion = ConfigurableJointMotion.Locked;
                    _jointRightUpperArm.angularZMotion = ConfigurableJointMotion.Locked;
                }
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
        // ��� ��Ŭ��(��ġ)���� ��ü
        // void ArmActionPunching() �Լ����� ��ġ ����
        // rigidbody.AddForce(-(zero * 8f), ForceMode.VelocityChange); // ���� before 2 after 8
        // rigidbody2.AddForce(zero * 12f, ForceMode.VelocityChange);  // ���� before 3 after 12
    }
    // ���̽� ����: ��� ������ �Ʒ��� �ֵθ���
    private void Item3(GameObject grabGameObj)
    {
        StartCoroutine("Item3Action", grabGameObj);
    }

    IEnumerator Item1Action(GameObject grabGameObj)
    {
        int forcingCount = 2000;

        bodyHandler.LeftHand.PartRigidbody.AddForce(new Vector3(0, _turnForce, 0));
        bodyHandler.RightHand.PartRigidbody.AddForce(new Vector3(0, _turnForce, 0));

        while (forcingCount > 0)
        {
            AlignToVector(bodyHandler.LeftHand.PartRigidbody, _jointLeft.transform.position, new Vector3(0f, 0.2f, 0f), 0.1f, 6f);
            AlignToVector(bodyHandler.RightHand.PartRigidbody, _jointRight.transform.position, new Vector3(0f, 0.2f, 0f), 0.1f, 6f);
            forcingCount--;
        }
        Debug.Log("�Ʒ����� �ֵθ���");

        yield return 0;
    }

    IEnumerator Item3Action(GameObject grabGameObj)
    {
        int forcingCount = 2000;

        bodyHandler.LeftHand.PartRigidbody.AddForce(new Vector3(0, 0, _turnForce*2));
        bodyHandler.RightHand.PartRigidbody.AddForce(new Vector3(0, 0, _turnForce*2));

        while (forcingCount > 0)
        {
            AlignToVector(bodyHandler.LeftHand.PartRigidbody, bodyHandler.LeftHand.transform.position, new Vector3(0f, 0f, 0.2f), 0.1f, 6f);
            AlignToVector(bodyHandler.RightHand.PartRigidbody, bodyHandler.RightHand.transform.position, new Vector3(0f, 0f, 0.2f), 0.1f, 6f);
            forcingCount--;
        }
        Debug.Log("���");


        yield return new WaitForSeconds(0.5f);

        StartCoroutine("Item1Action", grabGameObj);

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
        }
    }
}
