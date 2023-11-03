using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
//using System.Numerics;
using UnityEngine;
using UnityEngine.UI;
using static InteractableObject;
using static UnityEngine.Rendering.VirtualTexturing.Debugging;

public class StatusHandler : MonoBehaviour
{

    private float _damageModifer = 1f;

    public Actor actor;

    public bool invulnerable = false;

    [SerializeField]
    private float _health;
    public float Health { get { return _health; } set { _health = value; } }

    private float _maxHealth = 200f;
    private float _healthDamage;
    private bool _isDead;

    private float _maxUnconsciousTime=5f;
    private float _minUnconsciousTime=3f;
    private float _unconsciousTime = 0f;

    private float _knockoutThreshold=20f;

    private List<float> _xPosSpringAry = new List<float>();
    private List<float> _yzPosSpringAry = new List<float>();


    // ����Ʈ ����
    private bool hasObject = false;

    void Start()
    {
        actor = transform.GetComponent<Actor>();
        _health = _maxHealth;

        actor.BodyHandler.BodySetup();

        for (int i = 0; i < actor.BodyHandler.BodyParts.Count; i++)
        {
            if (i == 3)
            {
                continue;
            }
            _xPosSpringAry.Add(actor.BodyHandler.BodyParts[i].PartJoint.angularXDrive.positionSpring);
            _yzPosSpringAry.Add(actor.BodyHandler.BodyParts[i].PartJoint.angularYZDrive.positionSpring);
        }
    }

    void Update()
    {
        if (_healthDamage != 0f)
            UpdateHealth();

        //CheckConscious();
        DebuffAction();
    }

    private void OnGUI()
    {
        if(this.name == "Ragdoll2")
        {
            GUI.contentColor = Color.red;
            GUI.Label(new Rect(0, 0, 200, 200), "��������:" + actor.debuffState.ToString());
            GUI.Label(new Rect(0, 30, 200, 200), "�׼ǻ���:" + actor.actorState.ToString());
        }
    }

    // ����� ��������(trigger)
    public void AddDamage(InteractableObject.Damage type, float damage, GameObject causer)
    {
        Debug.Log("AddDamage");

        // ������ üũ
        damage *= _damageModifer;
        if (!invulnerable && actor.actorState != Actor.ActorState.Dead && actor.actorState != Actor.ActorState.Unconscious)
        {
            _healthDamage += damage;
        }

        // �����̻� üũ
        DebuffCheck(type);
    }

    public void DebuffCheck(InteractableObject.Damage type)
    {
        // ����
        if (type == Damage.Ice)
        {
            actor.debuffState |= Actor.DebuffState.Ice; // ���� ����� Ȱ��ȭ

            // �ٸ� ����� üũ
            foreach (Actor.DebuffState state in System.Enum.GetValues(typeof(Actor.DebuffState)))
            {
                // ���� �̿��� ���°� ������ ����
                if(state != Actor.DebuffState.Ice && (actor.debuffState & state) != 0)
                {
                    actor.debuffState &= ~state;
                }
            }
        }
        
        if (actor.debuffState == Actor.DebuffState.Ice) return;

        // ����
        if(type == Damage.ElectricShock)
        {
            actor.debuffState |= Actor.DebuffState.ElectricShock; // ���� ����� Ȱ��ȭ
        }

        // ����
        if(actor.actorState == Actor.ActorState.Unconscious || type == Damage.Knockout) // ����� ������ ��ƿ� ��
        {
            actor.debuffState |= Actor.DebuffState.Unconscious; // ���� ����� Ȱ��ȭ
        }

    }


    public void DebuffAction()
    {
        switch (actor.debuffState)
        {
            case Actor.DebuffState.Default:
                break;
            case Actor.DebuffState.Balloon:
                break;
            case Actor.DebuffState.Unconscious:
                StartCoroutine("ResetBodySpring");
                StartCoroutine("RestoreBodySpring");
                break;
            case Actor.DebuffState.Drunk:
                break;
            case Actor.DebuffState.ElectricShock:
                StartCoroutine("ElectricShock");
                break;
            case Actor.DebuffState.Ice:
                StartCoroutine("Ice");
                break;
            case Actor.DebuffState.Fire:
                break;
            case Actor.DebuffState.Invisible:
                break;
            case Actor.DebuffState.Strong:
                break;
        }
    }

    IEnumerator Ice()
    {
        Debug.Log("����!");
        yield return new WaitForSeconds(0.2f);

        // [����]
        actor.actorState = Actor.ActorState.Debuff;

        // ����Ʈ ����
        if (!hasObject)
        {
            hasObject = true;
        }
        
        for (int i = 0; i < actor.BodyHandler.BodyParts.Count; i++)
        {
            actor.BodyHandler.BodyParts[i].PartRigidbody.isKinematic = true;
        }

        yield return new WaitForSeconds(3f);

        // [���� ����]
        actor.actorState = Actor.ActorState.Stand;
        actor.debuffState &= ~Actor.DebuffState.Ice;

        // ����Ʈ ����
        if (hasObject)
        {
            hasObject = false;
        }

        for (int i = 0; i < actor.BodyHandler.BodyParts.Count; i++)
        {
            actor.BodyHandler.BodyParts[i].PartRigidbody.isKinematic = false;
        }
    }


    IEnumerator ElectricShock()
    {
        Debug.Log("����!");
        yield return new WaitForSeconds(0.2f);

        // ����
        StartCoroutine("ResetBodySpring");


        float startTime = Time.time;
        float shockDuration = 3f; // �����ð�
        while (Time.time - startTime < shockDuration)
        {
            if (actor.debuffState == Actor.DebuffState.Ice)
            {
                actor.actorState = Actor.ActorState.Stand;
                StartCoroutine("RestoreBodySpring");
                StopCoroutine("ElectricShock");
            }

            int number = Random.Range(0, 10);
            
            if (number > 7)
            {
                for (int i = 1; i < 15; i++)
                {
                    if (i == 3) continue;
                    actor.BodyHandler.BodyParts[i].transform.rotation = Quaternion.Euler(20, 0, 0);
                }
            }
            else
            {
                for (int i = 1; i < 15; i++)
                {
                    if (i == 3) continue;
                    actor.BodyHandler.BodyParts[i].transform.rotation = Quaternion.Euler(-20, 0, 0);
                }
            }
            yield return null;
        }

        yield return new WaitForSeconds(2.0f);

        // ���� ����
        actor.actorState = Actor.ActorState.Stand;
        actor.debuffState &= ~Actor.DebuffState.ElectricShock;
        StartCoroutine("RestoreBodySpring");
    }


    public void UpdateHealth()
    {
        if (_isDead)
            return;

        //���� ü�� �޾ƿ���
        float tempHealth = _health;


        //�������°� �ƴҶ��� ������ ����
        if (tempHealth > 0f && !invulnerable)
            tempHealth -= _healthDamage;


        float realDamage = _health - tempHealth;


        //�������°� �ƴҶ� ���� �̻��� �������� ������ ����
        if (actor.actorState != Actor.ActorState.Unconscious)
        {
            if (realDamage >= _knockoutThreshold)
            {
                if (actor.debuffState == Actor.DebuffState.Ice)
                    return;

                Debug.Log("�������� ���Ƽ� ����");
                _maxUnconsciousTime = Mathf.Clamp(_maxUnconsciousTime + 1.5f, _minUnconsciousTime, 20f);
                _unconsciousTime = _maxUnconsciousTime;
                actor.actorState = Actor.ActorState.Unconscious;
                actor.debuffState = Actor.DebuffState.Unconscious;
                EnterUnconsciousState();
            }
        }

        //����� ü���� 0���� ������ Death��
        if (tempHealth <= 0f)
        {
            KillPlayer();
            EnterUnconsciousState();
        }

        _health = Mathf.Clamp(tempHealth, 0f, _maxHealth);
        _healthDamage = 0f;
    }

    void KillPlayer()
    {

    }


    void EnterUnconsciousState()
    {
        //������ ����Ʈ�� ���� ���� �߰�

        //actor.BodyHandler.ResetLeftGrab();
        //actor.BodyHandler.ResetRightGrab();
        actor.BodyHandler.LeftHand.PartRigidbody.collisionDetectionMode = CollisionDetectionMode.Discrete;
        actor.BodyHandler.LeftForarm.PartRigidbody.collisionDetectionMode = CollisionDetectionMode.Discrete;
        actor.BodyHandler.RightHand.PartRigidbody.collisionDetectionMode = CollisionDetectionMode.Discrete;
        actor.BodyHandler.RightForarm.PartRigidbody.collisionDetectionMode = CollisionDetectionMode.Discrete;
    }

    IEnumerator ResetBodySpring()
    {
        JointDrive angularXDrive;
        JointDrive angularYZDrive;

        for (int i = 0; i < actor.BodyHandler.BodyParts.Count; i++)
        {
            if (i == 3)
                continue;

            angularXDrive = actor.BodyHandler.BodyParts[i].PartJoint.angularXDrive;
            angularXDrive.positionSpring = 0f;
            actor.BodyHandler.BodyParts[i].PartJoint.angularXDrive = angularXDrive;

            angularYZDrive = actor.BodyHandler.BodyParts[i].PartJoint.angularYZDrive;
            angularYZDrive.positionSpring = 0f;
            actor.BodyHandler.BodyParts[i].PartJoint.angularYZDrive = angularYZDrive;
        }

        yield return null;
    }

    IEnumerator RestoreBodySpring()
    {
        yield return new WaitForSeconds(2.0f);

        JointDrive angularXDrive;
        JointDrive angularYZDrive;

        float startTime = Time.time;
        float springLerpDuration = 2f;

        while (Time.time < startTime + springLerpDuration)
        {
            float elapsed = Time.time - startTime;
            float percentage = elapsed / springLerpDuration;
            int j = 0;

            for (int i = 0; i < actor.BodyHandler.BodyParts.Count; i++)
            {
                if (i == 3)
                {
                    continue;
                }
                angularXDrive = actor.BodyHandler.BodyParts[i].PartJoint.angularXDrive;
                angularXDrive.positionSpring = _xPosSpringAry[j] * percentage;

                actor.BodyHandler.BodyParts[i].PartJoint.angularXDrive = angularXDrive;

                angularYZDrive = actor.BodyHandler.BodyParts[i].PartJoint.angularYZDrive;
                angularYZDrive.positionSpring = _yzPosSpringAry[j] * percentage;
                actor.BodyHandler.BodyParts[i].PartJoint.angularYZDrive = angularYZDrive;
                j++;

                yield return null;
            }
        }

        actor.debuffState &= ~Actor.DebuffState.Unconscious;
    }


}

// �Ⱦ��� �Լ� ������

//public void CheckConscious()
//{
//    if (actor.actorState != Actor.ActorState.Unconscious && _unconsciousTime > 0f)
//    {
//        _unconsciousTime = Mathf.Clamp(_unconsciousTime - Time.deltaTime, 0f, _maxUnconsciousTime);
//    }

//    // �����϶�
//    if (actor.actorState == Actor.ActorState.Unconscious)
//    {
//        _unconsciousTime = Mathf.Clamp(_unconsciousTime - Time.deltaTime, 0f, _maxUnconsciousTime);

//        StartCoroutine("ResetBodySpring");

//        // ���� ����
//        if (_unconsciousTime <= 0f)
//        {
//            actor.actorState = Actor.ActorState.Stand;
//            StartCoroutine("RestoreBodySpring");
//            _unconsciousTime = 0f;
//        }
//    }
//}
