using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using static Define;

public class Grab : MonoBehaviourPun
{
    private TargetingHandler _targetingHandler;
    private InteractableObject _leftSearchTarget;
    private InteractableObject _rightSearchTarget;


    private Actor _actor;

    [SerializeField]
    bool _isRightGrab = false;
    [SerializeField]
    bool _isLeftGrab = false;


    [SerializeField]
    private float _throwingForce = 40f;

    float _grabDelayTimer = 0.5f;

    public bool _isGrabbingInProgress { get; private set; }


    public GameObject EquipItem;
    public GameObject LeftGrabObject;
    public GameObject RightGrabObject;

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


    public GameObject CollisionObject;

    // ������ ����
    private int _itemType;
    public float _turnForce;

    private List<ConfigurableJoint> _configurableJoints = new List<ConfigurableJoint>();
    private FixedJoint[] _armJoints = new FixedJoint[6];


    public enum Side
    {
        Left = 0,
        Right = 1,
        Both = 2,
    }


    void Start()
    {
        Init();
    }

    void Update()
    {
        _grabDelayTimer -= Time.deltaTime;
        GrabStateCheck();

    }

    void Init()
    {
        _actor = GetComponent<Actor>();
        _actor.BodyHandler = transform.root.GetComponent<BodyHandler>();
        _targetingHandler = GetComponent<TargetingHandler>();
        _actor.BodyHandler.BodySetup();


        _leftHandRigid = _actor.BodyHandler.LeftHand.PartRigidbody;
        _rightHandRigid = _actor.BodyHandler.RightHand.PartRigidbody;


        _configurableJoints.Add(_jointChest = _actor.BodyHandler.Chest.PartJoint);

        _configurableJoints.Add(_jointLeftUpperArm = _actor.BodyHandler.LeftArm.PartJoint);
        _configurableJoints.Add(_jointLeftForeArm = _actor.BodyHandler.LeftForearm.PartJoint);
        _configurableJoints.Add(_jointLeft = _actor.BodyHandler.LeftHand.PartJoint);

        _configurableJoints.Add(_jointRightUpperArm = _actor.BodyHandler.RightArm.PartJoint);
        _configurableJoints.Add(_jointRightForeArm = _actor.BodyHandler.RightForearm.PartJoint);
        _configurableJoints.Add(_jointRight = _actor.BodyHandler.RightHand.PartJoint);
    }

    void GrabStateCheck()
    {
        //PlayerLiftCheck();
        //photonView.RPC("PullingCheck", RpcTarget.All);

        //PullingCheck();
        if (EquipItem != null)
        {
            _actor.GrabState = GrabState.EquipItem;
            return;
        }
        ClimbCheck();
    }

    [PunRPC]
    void PullingCheck()
    {
        if (EquipItem != null)
            return;

        if(LeftGrabObject != null && LeftGrabObject.GetComponent<PhotonView>() != null)
        {
            LeftGrabObject.GetComponent<InteractableObject>().ApplyPullingForce(_leftHandRigid.velocity,_leftHandRigid.angularVelocity);
        }
        if (RightGrabObject != null && RightGrabObject.GetComponent<PhotonView>() != null)
        {
            RightGrabObject.GetComponent<InteractableObject>().ApplyPullingForce(_rightHandRigid.velocity, _rightHandRigid.angularVelocity);
        }

    }

    void ClimbCheck()
    {
        if (_isRightGrab && _isLeftGrab && LeftGrabObject != null && RightGrabObject != null)
        {
            //���߿� �������̳� �÷��̾ �ƴ� ������Ʈ�� Layer�� ClimbLayer ������ �����ϰ� ���� ���� �ٲ� �� ����
            if (LeftGrabObject.layer == (int)Define.Layer.ClimbObject && RightGrabObject.layer == (int)Define.Layer.ClimbObject)
            {
                _actor.GrabState = GrabState.Climb;
            }
        }
    }

    void PlayerLiftCheck()
    {
        if (_isRightGrab && _isLeftGrab && LeftGrabObject != null && RightGrabObject != null)
        {
            if (LeftGrabObject.GetComponent<CollisionHandler>() != null &&
                RightGrabObject.GetComponent<CollisionHandler>() != null)
            {
                _actor.GrabState = GrabState.PlayerLift;

                AlignToVector(_actor.BodyHandler.LeftArm.PartRigidbody, _actor.BodyHandler.LeftArm.PartTransform.forward, -_actor.BodyHandler.Waist.PartTransform.forward + _actor.BodyHandler.Chest.PartTransform.right / 2f + -_actor.PlayerController.MoveInput / 8f, 0.01f, 8f);
                AlignToVector(_actor.BodyHandler.LeftForearm.PartRigidbody, _actor.BodyHandler.LeftForearm.PartTransform.forward, -_actor.BodyHandler.Waist.PartTransform.forward, 0.01f, 8f);
                //_leftHandRigid.AddForce(Vector3.up*500);
                _leftHandRigid.AddForce(Vector3.up * 4, ForceMode.VelocityChange);

                //_actor.BodyHandler.Chest.PartRigidbody.AddForce(Vector3.down * 900);
                _actor.BodyHandler.Chest.PartRigidbody.AddForce(Vector3.down * 3, ForceMode.VelocityChange);

                AlignToVector(_actor.BodyHandler.RightArm.PartRigidbody, _actor.BodyHandler.RightArm.PartTransform.forward, -_actor.BodyHandler.Waist.PartTransform.forward + -_actor.BodyHandler.Chest.PartTransform.right / 2f + -_actor.PlayerController.MoveInput / 8f, 0.01f, 8f);
                AlignToVector(_actor.BodyHandler.RightForearm.PartRigidbody, _actor.BodyHandler.RightForearm.PartTransform.forward, -_actor.BodyHandler.Waist.PartTransform.forward, 0.01f, 8f);
                //_rightHandRigid.AddForce(Vector3.up*500);
                _rightHandRigid.AddForce(Vector3.up * 4, ForceMode.VelocityChange);
            }
        }
    }

    public void Climb()
    {
        GrabResetTrigger();
        //_rightHandRigid.AddForce(_rightHandRigid.transform.position + Vector3.down * 80f);
        //_leftHandRigid.AddForce(_rightHandRigid.transform.position + Vector3.down * 80f);


        _actor.BodyHandler.Hip.PartRigidbody.AddForce(Vector3.up * 100f, ForceMode.VelocityChange);
        _grabDelayTimer = 0.7f;

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
                    ItemType type = EquipItem.GetComponent<Item>().ItemData.ItemType;

                    if (Input.GetMouseButtonUp(0))
                    {
                        switch (type)
                        {
                            case ItemType.OneHanded:
                                StartCoroutine(OwnHandAttack());
                                break;
                            case ItemType.TwoHanded:
                                photonView.RPC("HorizontalAtkTrigger", RpcTarget.All);
                                //HorizontalAtkTrigger();
                                //StartCoroutine(HorizontalAttack());
                                break;
                            case ItemType.Gravestone:
                                StartCoroutine(VerticalAttack());
                                break;
                            case ItemType.Ranged:
                                photonView.RPC("UseItem", RpcTarget.All);
                                break;
                            case ItemType.Potion:
                                StartCoroutine(UsePotionAnim());
                                break;
                        }
                    }
                    if (Input.GetMouseButtonUp(1))
                    {
                        GrabResetTrigger();
                    }
                }
                break;
        }
    }

    public void OnMouseEvent_LiftPlayer(Define.MouseEvent evt)
    {
        switch (evt)
        {
            case Define.MouseEvent.PointerUp:
                {
                    if (Input.GetMouseButtonUp(0))
                    {
                        GrabResetTrigger();
                    }
                }
                break;
            case Define.MouseEvent.PointerDown:
                {
                    if (Input.GetMouseButtonDown(1))
                    {
                        Rigidbody rb1 = RightGrabObject.GetComponent<Rigidbody>();
                        Rigidbody rb2 = LeftGrabObject.GetComponent<Rigidbody>();
                        GrabResetTrigger();

                        rb1.AddForce(-_actor.BodyHandler.Chest.PartTransform.up * _throwingForce, ForceMode.VelocityChange);
                        rb2.AddForce(-_actor.BodyHandler.Chest.PartTransform.up * _throwingForce, ForceMode.VelocityChange);

                        rb1.AddForce(Vector3.up * _throwingForce * 1.5f, ForceMode.VelocityChange);
                        rb2.AddForce(Vector3.up * _throwingForce * 1.5f, ForceMode.VelocityChange);
                    }
                }
                break;
        }
    }

    [PunRPC]
    public void GrabPose(int itemViewID)
    {
        EquipItem = PhotonNetwork.GetPhotonView(itemViewID).gameObject;

        if (EquipItem.GetComponent<Item>().ItemData.ItemType == ItemType.Ranged)
        {
            _jointLeft.targetPosition = EquipItem.GetComponent<Item>().TwoHandedPos.position;
            _jointRight.targetPosition = EquipItem.GetComponent<Item>().OneHandedPos.position;
        }
        else if (EquipItem.GetComponent<Item>().ItemData.ItemType == ItemType.TwoHanded)
        {
            //_jointLeft.targetPosition = EquipItem.GetComponent<Item>().TwoHandedPos.position;
            //_jointRight.targetPosition = EquipItem.GetComponent<Item>().TwoHandedPos.position;
        }
        else if (EquipItem.GetComponent<Item>().ItemData.ItemType == ItemType.OneHanded)
        {
            _jointRight.targetPosition = EquipItem.GetComponent<Item>().OneHandedPos.position;
        }
        else if (EquipItem.GetComponent<Item>().ItemData.ItemType == ItemType.Gravestone)
        {
            _jointLeft.targetPosition = EquipItem.GetComponent<Item>().TwoHandedPos.position;
            _jointRight.targetPosition = EquipItem.GetComponent<Item>().OneHandedPos.position;
        }

        // �⺻ ��� �ڼ�
        //targetPosition = _grabItem.transform.position;
        //_jointLeft.targetPosition = targetPosition + new Vector3(0, 0, 20);
        //_jointRight.targetPosition = _jointLeft.targetPosition;

        //_jointLeftForeArm.targetPosition = targetPosition;
        //_jointRightForeArm.targetPosition = _jointLeftForeArm.targetPosition;
    }


   
    public void GrabResetTrigger()
    {
        photonView.RPC("GrabReset", RpcTarget.All);
    }


    [PunRPC]
    private void GrabReset()
    {
        int leftObjViewID = 0;
        int rightObjViewID = 0;

        if (LeftGrabObject != null && LeftGrabObject.transform.GetComponent<PhotonView>() != null)
            leftObjViewID = LeftGrabObject.transform.GetComponent<PhotonView>().ViewID;

        if (RightGrabObject != null && RightGrabObject.transform.GetComponent<PhotonView>() != null)
            rightObjViewID = RightGrabObject.transform.GetComponent<PhotonView>().ViewID;


        PhotonView leftPV = PhotonNetwork.GetPhotonView(leftObjViewID);
        PhotonView rightPV = PhotonNetwork.GetPhotonView(rightObjViewID);

        if (photonView.IsMine)
        {
            int playerID = PhotonNetwork.MasterClient.ActorNumber;
            if (leftPV != null)
                leftPV.TransferOwnership(playerID);
            if (rightPV != null)
                rightPV.TransferOwnership(playerID);
        }

        _isGrabbingInProgress = false;

        if (EquipItem != null)
        {
            EquipItem.gameObject.layer = LayerMask.NameToLayer("Item");
            EquipItem.GetComponent<Item>().Body.gameObject.SetActive(true);
            RangeWeaponSkin.gameObject.SetActive(false);
            EquipItem.GetComponent<Item>().Owner = null;
            if (EquipItem.GetComponent<Item>().ItemData.ItemType == ItemType.OneHanded ||
                EquipItem.GetComponent<Item>().ItemData.ItemType == ItemType.TwoHanded)
                EquipItem.GetComponent<InteractableObject>().damageModifier = InteractableObject.Damage.Default;
            EquipItem.GetComponent<Rigidbody>().mass = 10f;
            EquipItem = null;
        }


        DestroyJoint();

        _grabDelayTimer = 0.5f;
        _isRightGrab = false;
        _isLeftGrab = false;
        RightGrabObject = null;
        LeftGrabObject = null;
        _actor.GrabState = GrabState.None;
    }


    [PunRPC]
    private void SearchTarget()
    {
        //Ÿ�ټ�ġ �±׼��� �����Ұ�
        _leftSearchTarget = _targetingHandler.SearchTarget(Side.Left);
        _rightSearchTarget = _targetingHandler.SearchTarget(Side.Right);
    }
    
    public void Grabbing()
    {
        if (_grabDelayTimer > 0f)
            return;


        photonView.RPC("SearchTarget", RpcTarget.All);
        //Debug.Log(_leftSearchTarget);
        //Debug.Log(_rightSearchTarget);

        //�߰��� ������Ʈ�� ������ ����
        if (_leftSearchTarget == null && _rightSearchTarget == null)
            return;

        _isGrabbingInProgress = true;

        //Ÿ���� ���鿡 �ְ� �������϶�
        if (_leftSearchTarget == _rightSearchTarget && _leftSearchTarget.GetComponent<Item>() != null)
        {
            //���� �Ÿ� �̳��� ������ ����� ���������
            if (Vector3.Distance(_targetingHandler.FindClosestCollisionPoint(_leftSearchTarget.GetComponent<Collider>()),
                _actor.BodyHandler.Chest.transform.position) <= 1f
                  && !_isRightGrab && !_isLeftGrab)
            {
                Item item = _leftSearchTarget.GetComponent<Item>();
                HandleItemGrabbing(item);
                return;
            }
        }
        else//�������� �ƴҶ�
        {
            Vector3 dir;
            //Ÿ���� ������ �ƴҶ�
            if (_leftSearchTarget != null && !_isLeftGrab)
            {
                if (_actor.actorState == Actor.ActorState.Jump || _actor.actorState == Actor.ActorState.Fall)
                {
                    dir = ((_targetingHandler.FindClosestCollisionPoint(_leftSearchTarget.GetComponent<Collider>()) + Vector3.up * 2)
                        - _leftHandRigid.transform.position).normalized;
                }
                else
                {
                    dir = (_targetingHandler.FindClosestCollisionPoint(_leftSearchTarget.GetComponent<Collider>())
                        - _leftHandRigid.transform.position).normalized;
                }

                _leftHandRigid.AddForce(dir * 80f);
                if (HandCollisionCheck(Side.Left))
                {
                    int leftObjViewID = -1;
                    if (_leftSearchTarget.GetComponent<PhotonView>() != null)
                    {
                        leftObjViewID = _leftSearchTarget.transform.GetComponent<PhotonView>().ViewID;
                    }
                    photonView.RPC("JointFix", RpcTarget.All, (int)Side.Left, leftObjViewID);
                    _grabDelayTimer = 0.5f;
                }
            }

            if (_rightSearchTarget != null && !_isRightGrab)
            {
                if (_actor.actorState == Actor.ActorState.Jump || _actor.actorState == Actor.ActorState.Fall)
                {
                    dir = ((_targetingHandler.FindClosestCollisionPoint(_rightSearchTarget.GetComponent<Collider>()) + Vector3.up * 2)
                        - _rightHandRigid.transform.position).normalized;
                }
                else
                {
                    dir = (_targetingHandler.FindClosestCollisionPoint(_rightSearchTarget.GetComponent<Collider>())
                        - _rightHandRigid.transform.position).normalized;
                }

                _rightHandRigid.AddForce(dir * 80f);
                if (HandCollisionCheck(Side.Right))
                {
                    int rightObjViewID = -1;
                    if (_rightSearchTarget.GetComponent<PhotonView>() !=null)
                    {
                        rightObjViewID = _rightSearchTarget.transform.GetComponent<PhotonView>().ViewID;
                    }
                    photonView.RPC("JointFix", RpcTarget.All, (int)Side.Right, rightObjViewID);
                    _grabDelayTimer = 0.5f;
                }
            }
        }
    }




    void HandleItemGrabbing(Item item)
    {
        switch (item.ItemData.ItemType)
        {
            case ItemType.OneHanded:
                {
                    Vector3 dir = item.OneHandedPos.position - _rightHandRigid.transform.position;
                    _rightHandRigid.AddForce(dir.normalized * 80f);

                    if (IsHoldingItem(item, Side.Right))
                        ItemRotate(item.transform, false);
                    else
                        return;
                    //�����ۿ� �°� �������� �Լ� �߰��ؾ���

                    int rightObjViewID = _rightSearchTarget.transform.GetComponent<PhotonView>().ViewID;
                    photonView.RPC("JointFix", RpcTarget.All, (int)Side.Right, rightObjViewID);
                }
                break;
            case ItemType.TwoHanded:
                {
                    TwoHandedGrab(item);
                }
                break;
            case ItemType.Ranged:
                {
                    TwoHandedGrab(item);
                }
                break;
            case ItemType.Gravestone:
                {
                    TwoHandedGrab(item);
                }
                break;
            case ItemType.Potion:
                {
                    Vector3 dir = item.OneHandedPos.position - _rightHandRigid.transform.position;
                    _rightHandRigid.AddForce(dir.normalized * 80f);

                    if (IsHoldingItem(item, Side.Right))
                        ItemRotate(item.transform, false);
                    else
                        return;
                    //�����ۿ� �°� �������� �Լ� �߰��ؾ���

                    int rightObjViewID = _rightSearchTarget.transform.GetComponent<PhotonView>().ViewID;
                    photonView.RPC("JointFix", RpcTarget.All, (int)Side.Right, rightObjViewID);
                }
                break;
        }
    }

    void TwoHandedGrab(Item item)
    {
        //������ ������� ������ �����̸� ���������� ��� ����
        if (ItemDirCheck(item))
        {
            Vector3 dir = item.OneHandedPos.position - _rightHandRigid.transform.position;
            _rightHandRigid.AddForce(dir.normalized * 90f);

            dir = item.TwoHandedPos.position - _leftHandRigid.transform.position;
            _leftHandRigid.AddForce(dir.normalized * 90f);

            if (IsHoldingItem(item, Side.Both))
                ItemRotate(item.transform, true);
            else
                return;
        }
        else
        {
            Vector3 dir = item.TwoHandedPos.position - _rightHandRigid.transform.position;
            _rightHandRigid.AddForce(dir.normalized * 90f);

            dir = item.OneHandedPos.position - _leftHandRigid.transform.position;
            _leftHandRigid.AddForce(dir.normalized * 90f);

            if (IsHoldingItem(item, Side.Both))
                ItemRotate(item.transform, false);
            else
                return;
        }

        int leftObjViewID = _leftSearchTarget.transform.GetComponent<PhotonView>().ViewID;
        photonView.RPC("JointFix", RpcTarget.All, (int)Side.Left, leftObjViewID);
        int rightObjViewID = _rightSearchTarget.transform.GetComponent<PhotonView>().ViewID;
        photonView.RPC("JointFix", RpcTarget.All, (int)Side.Right, rightObjViewID);

        photonView.RPC("LockArmTrigger", RpcTarget.All);

    }


    /// <summary>
    /// ���� �����ۿ� ����� �����ߴ��� üũ �� ��������
    /// </summary>
    bool IsHoldingItem(Item item, Side side)
    {
        //HandChecker ��ũ��Ʈ���� ��� �� �������� �����̿� ���������� ����
        if (HandCollisionCheck(side))
        {
            EquipItem = item.transform.root.gameObject;
            int id = EquipItem.GetComponent<PhotonView>().ViewID;
            photonView.RPC("UsingItemSetting", RpcTarget.All, id);
            return true;
        }
        return false;
    }

    [PunRPC]
    void UsingItemSetting(int id)
    {
        _grabDelayTimer = 0.5f;
         

        PhotonView pv = PhotonNetwork.GetPhotonView(id);

        pv.GetComponent<Item>().Owner = GetComponent<Actor>();
        if (pv.GetComponent<Item>().ItemData.ItemType == ItemType.OneHanded ||
            pv.GetComponent<Item>().ItemData.ItemType == ItemType.TwoHanded)
            pv.GetComponent<InteractableObject>().damageModifier = pv.GetComponent<Item>().ItemData.UseDamageType;
        pv.GetComponent<Rigidbody>().mass = 0.1f;
    }

    bool HandCollisionCheck(Side side)
    {
        switch (side)
        {
            case Side.Left:
                if (_leftHandRigid.GetComponent<HandChecker>().CollisionObject != null &&
                    _leftHandRigid.GetComponent<HandChecker>().CollisionObject == _leftSearchTarget.gameObject)
                {
                    _isLeftGrab = true;
                    return true;
                }
                break;
            case Side.Right:
                if (_rightHandRigid.GetComponent<HandChecker>().CollisionObject != null &&
                    _rightHandRigid.GetComponent<HandChecker>().CollisionObject == _rightSearchTarget.gameObject)
                {
                    _isRightGrab = true;
                    return true;
                }
                break;
            case Side.Both:
                if (HandCollisionCheck(Side.Right) && HandCollisionCheck(Side.Left))
                {
                    return true;
                }
                else
                {
                    _isRightGrab = false;
                    _isLeftGrab = false;
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

        switch (item.GetComponent<Item>().ItemData.ItemType)
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
            case ItemType.Gravestone:
                {
                    if (isHeadLeft)
                        targetPosition = -_jointChest.transform.right;
                    else
                        targetPosition = _jointChest.transform.right;

                    item.transform.up = _jointChest.transform.up;
                }
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
        if(item.GetComponent<PhotonView>() != null)
        {
            int itemViewID = item.GetComponent<PhotonView>().ViewID;
            photonView.RPC("SyncGrapItemPosition", RpcTarget.All, targetPosition, itemViewID);
            photonView.RPC("GrabPose", RpcTarget.All, itemViewID);
        }

    }

    [PunRPC]
    void SyncGrapItemPosition( Vector3 targetPosition, int itemViewID)
    {
        Transform item = PhotonNetwork.GetPhotonView(itemViewID).transform;
        item.transform.right = -targetPosition.normalized;
        Debug.Log("SyncGrapItemPosition");
    }


    [PunRPC]
    void LockArmTrigger()
    {
        StartCoroutine(LockArmPosition());
    }

    IEnumerator LockArmPosition()
    {
        yield return new WaitForSeconds(0.5f);

        for (int i = 0; i < _armJoints.Length; i++)
        {
            _armJoints[i] = _configurableJoints[i + 1].AddComponent<FixedJoint>();

            if (i == 3)
                _armJoints[i].connectedBody = _configurableJoints[0].GetComponent<Rigidbody>();
            else
                _armJoints[i].connectedBody = _configurableJoints[i].GetComponent<Rigidbody>();
        }
    }

    [PunRPC]
    void UnlockArmPosition()
    {
        for (int i = 0; i < 6; i++)
        {
            Destroy(_armJoints[i]);
            _armJoints[i] = null;
        }
    }

    [PunRPC]
    void JointFix(int side, int objViewID = -1)
    {
        ItemType type = ItemType.None;
        if (EquipItem != null)
        {
            type = EquipItem.GetComponent<Item>().ItemData.ItemType;
            EquipItem.gameObject.layer = gameObject.layer;
        }

        //objViewID �� �׷�������Ʈ�� ID
        PhotonView pv = PhotonNetwork.GetPhotonView(objViewID);
        if (photonView.IsMine && pv != null && EquipItem != null)
        {
            int playerID = PhotonNetwork.LocalPlayer.ActorNumber;
            pv.TransferOwnership(playerID);
        }

        //��⿡ ����������� ���� ���� �� �Ϻ� ����
        if ((Side)side == Side.Left)
        {
            _grabJointLeft = _leftHandRigid.AddComponent<FixedJoint>();
            if (_leftSearchTarget == null)
                Debug.Log("lllllllllllllllllllll");
            _grabJointLeft.connectedBody = _leftSearchTarget.GetComponent<Rigidbody>();
            _grabJointLeft.breakForce = 9001;

            //if (pv != null)
            //    _leftSearchTarget = pv.transform.GetComponent<InteractableObject>();


            if (_leftSearchTarget != null)
                LeftGrabObject = _leftSearchTarget.gameObject;

            if (EquipItem != null && (type == ItemType.TwoHanded || type == ItemType.Ranged))
            {
                _jointLeft.angularYMotion = ConfigurableJointMotion.Locked;
                _jointLeftForeArm.angularYMotion = ConfigurableJointMotion.Locked;
                _jointLeftUpperArm.angularYMotion = ConfigurableJointMotion.Locked;
                _jointLeft.angularZMotion = ConfigurableJointMotion.Locked;
                _jointLeftForeArm.angularZMotion = ConfigurableJointMotion.Locked;
                _jointLeftUpperArm.angularZMotion = ConfigurableJointMotion.Locked;
            }

        }
        else if ((Side)side == Side.Right)
        {
            

            _grabJointRight = _rightHandRigid.AddComponent<FixedJoint>();

            //if (pv != null)
            //    _rightSearchTarget = pv.transform.GetComponent<InteractableObject>();
            if (_rightSearchTarget == null)
                Debug.Log("lllllllllllllllllllll");
            _grabJointRight.connectedBody = _rightSearchTarget.GetComponent<Rigidbody>();
            _grabJointRight.breakForce = 9001;

            if (_rightSearchTarget != null)
                RightGrabObject = _rightSearchTarget.gameObject;

            if (EquipItem != null && (type == ItemType.TwoHanded || type == ItemType.Ranged))
            {
                _jointRight.angularYMotion = ConfigurableJointMotion.Locked;
                _jointRightForeArm.angularYMotion = ConfigurableJointMotion.Locked;
                _jointRightUpperArm.angularYMotion = ConfigurableJointMotion.Locked;
                _jointRight.angularZMotion = ConfigurableJointMotion.Locked;
                _jointRightForeArm.angularZMotion = ConfigurableJointMotion.Locked;
                _jointRightUpperArm.angularZMotion = ConfigurableJointMotion.Locked;


            }
        }
    }

    
    void DestroyJoint()
    {
        

        Destroy(_grabJointLeft);
        Destroy(_grabJointRight);
        photonView.RPC("UnlockArmPosition", RpcTarget.All);

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
        yield return _actor.PlayerController.DropRip(PlayerController.Side.Right, 0.07f, 0.1f, 0.5f, 0.5f, 0.1f);
    }

    IEnumerator OwnHandAttack()
    {
        _jointLeft.GetComponent<Rigidbody>().AddForce(new Vector3(0, _turnForce, 0));
        _jointRight.GetComponent<Rigidbody>().AddForce(new Vector3(0, _turnForce, 0));
        yield return _actor.PlayerController.ItemOwnHand(PlayerController.Side.Right, 0.07f, 0.1f, 0.5f, 0.5f, 0.1f);
    }

    [PunRPC]
    void HorizontalAtkTrigger()
    {
        StartCoroutine(HorizontalAttack());
    }

    IEnumerator HorizontalAttack()
    {
        _jointLeft.GetComponent<Rigidbody>().AddForce(new Vector3(_turnForce * 3, 0, 0));
        _jointRight.GetComponent<Rigidbody>().AddForce(new Vector3(_turnForce * 3, 0, 0));

        yield return _actor.PlayerController.ItemTwoHand(PlayerController.Side.Right, 0.07f, 0.1f, 0.5f, 0.1f, 3f);
    }

    IEnumerator UsePotionAnim()
    {
        _jointLeft.GetComponent<Rigidbody>().AddForce(new Vector3(_turnForce * 3, 0, 0));
        _jointRight.GetComponent<Rigidbody>().AddForce(new Vector3(_turnForce * 3, 0, 0));

        yield return _actor.PlayerController.Potion(PlayerController.Side.Right, 0.07f, 0.1f, 0.5f, 0.5f, 0.1f);

        photonView.RPC("UseItem", RpcTarget.All);
        GrabResetTrigger();
    }

    [PunRPC]
    private void UseItem()
    {
        EquipItem.GetComponent<Item>().Use();
    }


    //������ٵ� part�� alignmentVector������ targetVector�������� ȸ��
    private void AlignToVector(Rigidbody part, Vector3 alignmentVector, Vector3 targetVector, float stability, float speed)
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
