using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Stun : MonoBehaviourPun ,IState
{
    private bool _isStun = false;
    public Actor MyActor { get; set; }

    private List<float> _xPosSpringAry = new List<float>();
    private List<float> _yzPosSpringAry = new List<float>();

    public void EnterState()
    {
        _isStun = true;

        for (int i = 0; i < MyActor.BodyHandler.BodyParts.Count; i++)
        {
            if (i == (int)Define.BodyPart.Hip)
                continue;

            _xPosSpringAry.Add(MyActor.BodyHandler.BodyParts[i].PartJoint.angularXDrive.positionSpring);
            _yzPosSpringAry.Add(MyActor.BodyHandler.BodyParts[i].PartJoint.angularYZDrive.positionSpring);
        }

        MyActor.debuffState = Actor.DebuffState.Stun;
        //TODO :: ����Ʈ �߰� 
        Debug.Log("Start ResetBodySpring");
        StartCoroutine("ResetBodySpring");
        Debug.Log($"���¸� ��ȯ �߽��ϴ�. {_isStun}");
    }

    public void UpdateState()
    {

    }

    public void ExitState()
    {
        _isStun = false;

        Debug.Log("���¸� �������� �����ϴ� : " + _isStun);
        //StartCoroutine�̿��� ���ķ� ���Ƽ� ��� �ٽ� ���ƿ�
        StartCoroutine(RestoreBodySpring(0.07f));
        MyActor.actorState = Actor.ActorState.Stand;
        MyActor.debuffState &= ~Actor.DebuffState.Stun;
        MyActor.InvokeStatusChangeEvent();
        //TODO :: ����Ʈ ����
    }

    IEnumerator ResetBodySpring()
    {
        photonView.RPC("SetJointSpring", RpcTarget.All, 0f);
        yield return null;
    }

    IEnumerator RestoreBodySpring(float _springLerpTime = 1f)
    {
        Debug.Log("Start RestoreBodySpring");
        float startTime = Time.time;
        float springLerpDuration = _springLerpTime;

        while (Time.time < startTime + springLerpDuration)
        {
            float elapsed = Time.time - startTime;
            float percentage = elapsed / springLerpDuration;
            //SetJointSpring(percentage);
            photonView.RPC("SetJointSpring", RpcTarget.All, percentage);
            Debug.Log("RestoreBodySpring+++++++++++++++");
            yield return null;
        }
        Debug.Log("End RestoreBodySpring");
    }

    [PunRPC]
    void SetJointSpring(float percentage)
    {
        JointDrive angularXDrive;
        JointDrive angularYZDrive;
        int j = 0;

        Debug.Log("Start SetJointSpring");

        //������ ȸ���� ��� ���� �����ÿ� �ۼ�Ƽ���� 0�����ؼ� ���
        for (int i = 0; i < MyActor.BodyHandler.BodyParts.Count; i++)
        {
            if (i == (int)Define.BodyPart.Hip)
                continue;

            angularXDrive = MyActor.BodyHandler.BodyParts[i].PartJoint.angularXDrive;
            angularXDrive.positionSpring = _xPosSpringAry[j] * percentage;
            MyActor.BodyHandler.BodyParts[i].PartJoint.angularXDrive = angularXDrive;

            angularYZDrive = MyActor.BodyHandler.BodyParts[i].PartJoint.angularYZDrive;
            angularYZDrive.positionSpring = _yzPosSpringAry[j] * percentage;
            MyActor.BodyHandler.BodyParts[i].PartJoint.angularYZDrive = angularYZDrive;

            j++;
        }

        Debug.Log("End SetJointSpring");

    }
}
