using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static Define;
using static UnityEditor.Progress;
using static UnityEngine.GraphicsBuffer;

public class Grab : MonoBehaviourPun
{
    private TargetingHandler _targetingHandler;
    private BodyHandler _bodyHandler;
    private InteractableObject _searchTarget;
    private Actor _actor;

    bool _isRightGrab = false;
    bool _isLeftGrab = false;

    float _grabDelayTimer = 0.5f;

    public bool _isGrabbing {get; private set;}


    public GameObject GrabItem;
    public Transform RangeWeaponSkin;

    private Rigidbody _leftHandRigid;
    private Rigidbody _rightHandRigid;

    private FixedJoint _grabJointLeft;
    private FixedJoint _grabJointRight;

    private ConfigurableJoint _jointLeft;
    private ConfigurableJoint _jointRight;
    private ConfigurableJoint _jointLeftForeArm;
    private ConfigurableJoint _jointRightForeArm;
    private ConfigurableJoint _jointLeftUpperArm;
    private ConfigurableJoint _jointRightUpperArm;
    private ConfigurableJoint _jointChest;

    Vector3 targetPosition;

    

    public GrabObjectType GrabObjectType = GrabObjectType.None;

    // ������ ����
    private int _itemType;
    public float _turnForce;

    public enum Side
    {
        Left = 0,
        Right = 1,
        Both = 2,
    }

    
    void Start()
    {
        _bodyHandler = transform.root.GetComponent<BodyHandler>();
        _targetingHandler = GetComponent<TargetingHandler>();
        _actor = GetComponent<Actor>();
        _bodyHandler.BodySetup();


        _leftHandRigid = _bodyHandler.LeftHand.PartRigidbody;
        _rightHandRigid = _bodyHandler.RightHand.PartRigidbody;

        _jointLeft = _bodyHandler.LeftHand.PartJoint;
        _jointRight = _bodyHandler.RightHand.PartJoint;

        _jointLeftForeArm = _bodyHandler.LeftForarm.PartJoint;
        _jointRightForeArm = _bodyHandler.RightForarm.PartJoint;

        _jointLeftUpperArm = _bodyHandler.LeftArm.PartJoint;
        _jointRightUpperArm = _bodyHandler.RightArm.PartJoint;

        _jointChest = _bodyHandler.Chest.PartJoint;
    }

    void Update()
    {
        _grabDelayTimer -= Time.deltaTime;

    }

    public void OnMouseEvent_EquipItem(Define.MouseEvent evt)
    {
        switch (evt)
        {
            case Define.MouseEvent.PointerDown:
                {
                }
                break;
            case Define.MouseEvent.Press:
                {

                }
                break;
            case Define.MouseEvent.PointerUp:
                {
                }
                break;
            case Define.MouseEvent.Click:
                {
                    if (Input.GetMouseButtonUp(0))
                    {
                        //if(GrabItem.GetComponent<Item>().ItemType == ItemType.TwoHanded ||
                        //    GrabItem.GetComponent<Item>().ItemType == ItemType.OneHanded)
                        //    _actor.PlayerController.PunchAndGrab();
                        if (GrabItem.GetComponent<Item>().ItemType == ItemType.TwoHanded)
                            StartCoroutine(HorizontalAttack());
                        else if(GrabItem.GetComponent<Item>().ItemType == ItemType.OneHanded)
                            StartCoroutine(VerticalAttack());

                    }
                    if (Input.GetMouseButtonUp(1))
                    {
                        GrabReset();
                    }
                }
                break;
        }
    }



    public void GrabPose()
    {
        if(GrabItem.GetComponent<Item>().ItemType == ItemType.Ranged)
        {
            _jointLeft.targetPosition = GrabItem.GetComponent<Item>().TwoHandedPos.position;
            _jointRight.targetPosition = GrabItem.GetComponent<Item>().OneHandedPos.position;
        }
        else if(GrabItem.GetComponent<Item>().ItemType == ItemType.OneHanded)
        {
            _jointRight.targetPosition = GrabItem.GetComponent<Item>().OneHandedPos.position;
        }

        // �⺻ ��� �ڼ�
        //targetPosition = _grabItem.transform.position;
        //_jointLeft.targetPosition = targetPosition + new Vector3(0, 0, 20);
        //_jointRight.targetPosition = _jointLeft.targetPosition;

        //_jointLeftForeArm.targetPosition = targetPosition;
        //_jointRightForeArm.targetPosition = _jointLeftForeArm.targetPosition;
    }

    public void GrabReset()
    {
        _isGrabbing = false;
        if(GrabItem != null)
        {
            GrabItem.gameObject.layer = LayerMask.NameToLayer("Item");
            GrabItem.GetComponent<Item>().Body.gameObject.SetActive(true);
            RangeWeaponSkin.gameObject.SetActive(false);
            GrabItem = null;
            _isRightGrab = false;
            _isLeftGrab = false;
        }
        DestroyJoint();
    }

    public void Grabbing()
    {
        if (_grabDelayTimer > 0f || _isRightGrab)
            return;
        
        //Ÿ�ټ�ġ �±׼��� �����Ұ�
        _searchTarget = _targetingHandler.SearchTarget();

        //�߰��� ������Ʈ�� ������ ����
        if (_searchTarget == null)
            return;

        _isGrabbing = true;

        //��ġŸ���� �������̰�, ���� �Ÿ� �̳��� ������
        if (_searchTarget.GetComponent<Item>() != null && Vector3.Distance(_searchTarget.transform.position, _bodyHandler.Chest.transform.position) <= 1.5f)
        {
            Item item = _searchTarget.GetComponent<Item>();
            HandleItemGrabbing(item);  
        }
        else
        {
            //��ġŸ���� �������� �ƴ� ��



            //Ÿ���� ���� ����� �������� ���� ��� ���˽� �׷����·� ��
            //Ÿ���� ��ġ�� �Ÿ��� ���� ��ձ׷�, �Ѽձ׷��� ��
        }
    }

 


    void HandleItemGrabbing(Item item)
    {
        switch (item.ItemType)
        {
            case ItemType.OneHanded:
                {
                    Vector3 dir = item.OneHandedPos.position - _rightHandRigid.transform.position;
                    _rightHandRigid.AddForce(dir * 80f);

                    if (ItemGrabCheck(item, Side.Right))
                        ItemRotate(item.transform, false);
                    else
                        return;
                    //�����ۿ� �°� �������� �Լ� �߰��ؾ���

                    JointFix(Side.Right);
                }
                break;
            case ItemType.TwoHanded:
                {
                    //������ ������� ������ �����̸� ���������� ��� ����
                    TwoHandedGrab(item);
                }
                break;
            case ItemType.Ranged:
                {
                    TwoHandedGrab(item);
                }
                break;
        }
    }

    void TwoHandedGrab(Item item)
    {
        if (ItemDirCheck(item))
        {
            Vector3 dir = item.OneHandedPos.position - _rightHandRigid.transform.position;
            _rightHandRigid.AddForce(dir * 90f);

            dir = item.TwoHandedPos.position - _leftHandRigid.transform.position;
            _leftHandRigid.AddForce(dir * 90f);

            if (ItemGrabCheck(item, Side.Both))
                ItemRotate(item.transform, true);
            else
                return;
        }
        else
        {
            Vector3 dir = item.TwoHandedPos.position - _rightHandRigid.transform.position;
            _rightHandRigid.AddForce(dir * 90f);

            dir = item.OneHandedPos.position - _leftHandRigid.transform.position;
            _leftHandRigid.AddForce(dir * 90f);

            if (ItemGrabCheck(item, Side.Both))
                ItemRotate(item.transform, false);
            else
                return;
        }

        JointFix(Side.Left);
        JointFix(Side.Right);
    }


    /// <summary>
    /// ���� �����ۿ� ����� �����ߴ��� üũ �� ��������
    /// </summary>
    bool ItemGrabCheck(Item item,Side side)
    {
        //HandChecker ��ũ��Ʈ���� ��� �� �������� trigger�� ���������� ����
        if (GrabObjectType == GrabObjectType.Item && HandCollisionCheck(side))
        {
            _grabDelayTimer = 0.5f;
            GrabObjectType = GrabObjectType.None;
            GrabItem = item.transform.root.gameObject;
            return true;
        }
        return false;
    }


    bool HandCollisionCheck(Side side)
    {
        switch (side)
        {
            case Side.Left:
                if (_leftHandRigid.GetComponent<HandChecker>().isCheck)
                {
                    _isLeftGrab = true;
                    return true;
                }
                break;
            case Side.Right:
                if (_rightHandRigid.GetComponent<HandChecker>().isCheck)
                {
                    _isRightGrab = true;
                    return true;
                }
                break;
            case Side.Both:
                if (_rightHandRigid.GetComponent<HandChecker>().isCheck && _leftHandRigid.GetComponent<HandChecker>().isCheck)
                {
                    _isRightGrab = true;
                    _isLeftGrab = true;
                    return true;
                }
                break;
        }

        return false;
    }

    bool ItemDirCheck(Item item)
    {
        //�����հ� ������ ��ġ üũ�ؼ� ������ ���� ����
        Vector3 toItem = (item.TwoHandedPos.position - _jointChest.transform.position).normalized; // �÷��̾ �������� �ٶ󺸴� ����
        Vector3 toOneHandedHandle = (item.OneHandedPos.position - _jointChest.transform.position).normalized; // �������� ��ƾ��� oneHanded ������ ����
        Vector3 crossProduct = Vector3.Cross(toItem, toOneHandedHandle);

        if (crossProduct.y > 0) 
            return true;// ���ڵ� �����̰� ������
        else
            return false;// ���ڵ� �����̰� ����
    }


    /// <summary>
    /// �� ���⿡ �°� ������ �����̼� ����
    /// </summary>
    void ItemRotate(Transform item, bool isHeadLeft)
    {
        //item.GetComponent<Rigidbody>().isKinematic = true;
        //item.GetComponent<Rigidbody>().useGravity = false;
        //item.GetComponent<Collider>().enabled = false;

        Vector3 targetPosition = _jointChest.transform.forward;

        switch (item.GetComponent<Item>().ItemType)
        {
            case ItemType.TwoHanded:
                        //�������� ���κ��� �ش� ���⺤�͸� �ٶ󺸰�
                    if (isHeadLeft)
                        targetPosition = -_jointChest.transform.right;
                    else
                        targetPosition = _jointChest.transform.right;
                break;
            case ItemType.OneHanded:
                    targetPosition = _jointChest.transform.forward;
                break;
            case ItemType.Ranged:
                {
                    item.GetComponent<Item>().Body.gameObject.SetActive(false);
                    RangeWeaponSkin.gameObject.SetActive(true);
                    targetPosition = -_jointChest.transform.up;
                }
                break;
            case ItemType.Potion:
                    targetPosition = _jointChest.transform.forward;
                break;
        }
        //item.gameObject.layer = gameObject.layer;
        item.transform.right = -targetPosition.normalized;

        GrabPose();
    }


    void JointFix(Side side)
    {
        //��⿡ ����������� ���� ���� �� �Ϻ� ����
        if (side == Side.Left )
        {
            _grabJointLeft = GrabItem.AddComponent<FixedJoint>();
            _grabJointLeft.connectedBody = _leftHandRigid;
            _grabJointLeft.breakForce = 9001;

            _jointLeft.angularYMotion = ConfigurableJointMotion.Locked;
            _jointLeftForeArm.angularYMotion = ConfigurableJointMotion.Locked;
            _jointLeftUpperArm.angularYMotion = ConfigurableJointMotion.Locked;
            _jointLeft.angularZMotion = ConfigurableJointMotion.Locked;
            _jointLeftForeArm.angularZMotion = ConfigurableJointMotion.Locked;
            _jointLeftUpperArm.angularZMotion = ConfigurableJointMotion.Locked;
        }
        else if (side == Side.Right )
        {
            _grabJointRight = GrabItem.AddComponent<FixedJoint>();
            _grabJointRight.connectedBody = _rightHandRigid;
            _grabJointRight.breakForce = 9001;

            _jointRight.angularYMotion = ConfigurableJointMotion.Locked;
            _jointRightForeArm.angularYMotion = ConfigurableJointMotion.Locked;
            _jointRightUpperArm.angularYMotion = ConfigurableJointMotion.Locked;
            _jointRight.angularZMotion = ConfigurableJointMotion.Locked;
            _jointRightForeArm.angularZMotion = ConfigurableJointMotion.Locked;
            _jointRightUpperArm.angularZMotion = ConfigurableJointMotion.Locked;
        }
    }
    void DestroyJoint()
    {
        Destroy(_grabJointLeft);
        Destroy(_grabJointRight);

        // ���� ����
        _jointLeft.angularYMotion = ConfigurableJointMotion.Limited;
        _jointLeftForeArm.angularYMotion = ConfigurableJointMotion.Limited;
        _jointLeftUpperArm.angularYMotion = ConfigurableJointMotion.Limited;
        _jointLeft.angularZMotion = ConfigurableJointMotion.Limited;
        _jointLeftForeArm.angularZMotion = ConfigurableJointMotion.Limited;
        _jointLeftUpperArm.angularZMotion = ConfigurableJointMotion.Limited;

        _jointRight.angularYMotion = ConfigurableJointMotion.Limited;
        _jointRightForeArm.angularYMotion = ConfigurableJointMotion.Limited;
        _jointRightUpperArm.angularYMotion = ConfigurableJointMotion.Limited;
        _jointRight.angularZMotion = ConfigurableJointMotion.Limited;
        _jointRightForeArm.angularZMotion = ConfigurableJointMotion.Limited;
        _jointRightUpperArm.angularZMotion = ConfigurableJointMotion.Limited;
    }


    IEnumerator VerticalAttack()
    {
        int forcingCount = 2000;

        _jointLeft.GetComponent<Rigidbody>().AddForce(new Vector3(0, _turnForce, 0));
        _jointRight.GetComponent<Rigidbody>().AddForce(new Vector3(0, _turnForce, 0));

        while (forcingCount > 0)
        {
            AlignToVector(_jointLeft.GetComponent<Rigidbody>(), _jointLeft.transform.position, new Vector3(0f, 0.2f, 0f), 0.1f, 6f);
            AlignToVector(_jointRight.GetComponent<Rigidbody>(), _jointRight.transform.position, new Vector3(0f, 0.2f, 0f), 0.1f, 6f);
            forcingCount--;
        }
        Debug.Log("�ڷ�ƾ ��");

        yield return 0;
    }
    IEnumerator HorizontalAttack()
    {
        int forcingCount = 5000;

        _jointLeft.GetComponent<Rigidbody>().AddForce(new Vector3(_turnForce*3, 0, 0));
        _jointRight.GetComponent<Rigidbody>().AddForce(new Vector3(_turnForce*3, 0, 0));

        Debug.Log("h");
        while (forcingCount > 0)
        {
            AlignToVector(_jointLeft.GetComponent<Rigidbody>(), _jointLeft.transform.position, new Vector3(0.2f, 0f, 0f), 0.1f, 2f);
            AlignToVector(_jointLeftForeArm.GetComponent<Rigidbody>(), _jointLeftForeArm.transform.position, new Vector3(0.2f, 0f, 0f), 0.1f, 2f);
            AlignToVector(_jointLeftUpperArm.GetComponent<Rigidbody>(), _jointLeftUpperArm.transform.position, new Vector3(0.2f, 0f, 0f), 0.1f, 2f);

            AlignToVector(_jointRight.GetComponent<Rigidbody>(), _jointRight.transform.position, _jointLeft.transform.position, 0.1f, 2f);
            AlignToVector(_jointRightForeArm.GetComponent<Rigidbody>(), _jointRightForeArm.transform.position, _jointLeftForeArm.transform.position, 0.1f, 2f);
            AlignToVector(_jointRightUpperArm.GetComponent<Rigidbody>(), _jointRightUpperArm.transform.position, _jointLeftUpperArm.transform.position, 0.1f, 2f);

            AlignToVector(_jointChest.GetComponent<Rigidbody>(), _jointChest.transform.position, _jointLeft.transform.position, 0.1f, 2f);

            //AlignToVector(_jointRight.GetComponent<Rigidbody>(), _jointRight.transform.position, new Vector3(0.2f, 0f, 0f), 0.1f, 0.1f);
            //AlignToVector(_jointRightForeArm.GetComponent<Rigidbody>(), _jointRightForeArm.transform.position, new Vector3(0.2f, 0f, 0f), 0.1f, 0.1f);
            //AlignToVector(_jointRightUpperArm.GetComponent<Rigidbody>(), _jointRightUpperArm.transform.position, new Vector3(0.2f, 0f, 0f), 0.1f, 0.1f);

            forcingCount--;
        }

        yield return 0;
    }

    void UsePotion()
    {
        _jointLeft.GetComponent<Rigidbody>().AddForce(new Vector3(_turnForce * 3, 0, 0));
        _jointRight.GetComponent<Rigidbody>().AddForce(new Vector3(_turnForce * 3, 0, 0));

        Debug.Log("h");

        AlignToVector(_jointLeft.GetComponent<Rigidbody>(), _jointLeft.transform.position, new Vector3(0.2f, 0f, 0f), 0.1f, 2f);
        AlignToVector(_jointLeftForeArm.GetComponent<Rigidbody>(), _jointLeftForeArm.transform.position, new Vector3(0.2f, 0f, 0f), 0.1f, 2f);
        AlignToVector(_jointLeftUpperArm.GetComponent<Rigidbody>(), _jointLeftUpperArm.transform.position, new Vector3(0.2f, 0f, 0f), 0.1f, 2f);

        AlignToVector(_jointRight.GetComponent<Rigidbody>(), _jointRight.transform.position, _jointLeft.transform.position, 0.1f, 2f);
        AlignToVector(_jointRightForeArm.GetComponent<Rigidbody>(), _jointRightForeArm.transform.position, _jointLeftForeArm.transform.position, 0.1f, 2f);
        AlignToVector(_jointRightUpperArm.GetComponent<Rigidbody>(), _jointRightUpperArm.transform.position, _jointLeftUpperArm.transform.position, 0.1f, 2f);

        AlignToVector(_jointChest.GetComponent<Rigidbody>(), _jointChest.transform.position, _jointLeft.transform.position, 0.1f, 2f);

        //AlignToVector(_jointRight.GetComponent<Rigidbody>(), _jointRight.transform.position, new Vector3(0.2f, 0f, 0f), 0.1f, 0.1f);
        //AlignToVector(_jointRightForeArm.GetComponent<Rigidbody>(), _jointRightForeArm.transform.position, new Vector3(0.2f, 0f, 0f), 0.1f, 0.1f);
        //AlignToVector(_jointRightUpperArm.GetComponent<Rigidbody>(), _jointRightUpperArm.transform.position, new Vector3(0.2f, 0f, 0f), 0.1f, 0.1f);

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
