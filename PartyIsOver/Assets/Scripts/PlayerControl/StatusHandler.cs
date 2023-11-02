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


    private float _beforeSpeed;
    private bool hasBuff;

    [Header("���� �ð�")]
    [SerializeField]
    private float _freezeTime = 3f;
    [Header("���� �ð�")]
    [SerializeField]
    private float _shockTime = 3f;
    [Header("���� �ð�")]
    [SerializeField]
    private float _stunTime = 3f;
    [Header("�Ҳ� �ð�")]
    [SerializeField]
    private float _powerUpTime = 3f;
    [Header("��ȭ �ð�")]
    [SerializeField]
    private float _slowTime = 3f;


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

    float startTime = 0;
    float endTime = 0;
    void Update()
    {
        if (_healthDamage != 0f)
            UpdateHealth();

        //CheckConscious();
    }

    private void OnGUI()
    {
        if(this.name == "Ragdoll2")
        {
            GUI.contentColor = Color.red;
            GUI.Label(new Rect(0, 0, 200, 200), "��������:" + actor.debuffState.ToString());
            GUI.Label(new Rect(0, 30, 200, 200), "�׼ǻ���:" + actor.actorState.ToString());
            GUI.Label(new Rect(0, 60, 200, 200), "����� �ɸ� �ð�:" + (endTime - startTime));
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
        DebuffAction();
    }

    public void DebuffCheck(InteractableObject.Damage type)
    {
        startTime = Time.time; // ����׿�

        // ����
        if (type == Damage.Freeze)
        {
            actor.debuffState |= Actor.DebuffState.Freeze;

            // �ٸ� ����� üũ
            foreach (Actor.DebuffState state in System.Enum.GetValues(typeof(Actor.DebuffState)))
            {
                // ���� �̿��� ���°� ������ ����
                if(state != Actor.DebuffState.Freeze && (actor.debuffState & state) != 0)
                {
                    actor.debuffState &= ~state;
                }
            }
        }
        
        if (actor.debuffState == Actor.DebuffState.Freeze) return;

        // �Ҳ�
        if(type == Damage.PowerUp)
        {
            actor.debuffState |= Actor.DebuffState.PowerUp;
        }

        // ����
        // ȭ��

        // ��ħ

        // ��ȭ
        if (type == Damage.Slow)
        {
            actor.debuffState |= Actor.DebuffState.Slow;
        }

        // ����
        if (type == Damage.Shock)
        {
            actor.debuffState |= Actor.DebuffState.Shock; 
        }

        // ����
        if(actor.actorState == Actor.ActorState.Unconscious || type == Damage.Knockout) // ����� Damage.Knockout ��
        {
            actor.debuffState |= Actor.DebuffState.Stun;
        }
    }


    public void DebuffAction()
    {
        // 2���� �ϵ��� �����մ��� Ȯ��
        // �����ִ°͸� �ൿ�ϰԲ� swtich���� 1������ �ൿ X
        // for 2~3~4�� �ൿ �� �������� 

        // �̹� �����ִ� �ൿ�̶��? flag ���� �̹� �����ִٰ� Ȯ�ν����ֱ�

        switch (actor.debuffState)
        {
            case Actor.DebuffState.Default:
                break;
            case Actor.DebuffState.PowerUp:
                if (!hasBuff)
                {
                    _beforeSpeed = actor.PlayerControll.RunSpeed;
                    StartCoroutine("PowerUp", _powerUpTime);
                }
                break;
            case Actor.DebuffState.Invisible:
                break;
            case Actor.DebuffState.Burn:
                break;
            case Actor.DebuffState.Exhausted:
                break;
            case Actor.DebuffState.Slow:
                if (!hasBuff)
                {
                    _beforeSpeed = actor.PlayerControll.RunSpeed;
                    StartCoroutine("Slow", _slowTime);
                }
                break;
            case Actor.DebuffState.Freeze:
                StartCoroutine("Freeze", _freezeTime);
                break;
            case Actor.DebuffState.Shock:
                StartCoroutine("Shock");
                break;
            case Actor.DebuffState.Stun:
                StartCoroutine("ResetBodySpring");
                StartCoroutine("Stun");
                break;
            case Actor.DebuffState.Drunk:
                break;
            case Actor.DebuffState.Balloon:
                break;
            case Actor.DebuffState.Ghost:
                break;
        }
    }

    IEnumerator PowerUp(float delay)
    {
        Debug.Log("�Ҳ�!");

        // �Ҳ�
        hasBuff = true;
        actor.actorState = Actor.ActorState.Debuff;
        actor.PlayerControll.RunSpeed = _beforeSpeed * 1.1f;

        yield return new WaitForSeconds(delay);

        // �Ҳ� ����
        actor.actorState = Actor.ActorState.Stand;
        actor.debuffState &= ~Actor.DebuffState.PowerUp;
        actor.PlayerControll.RunSpeed = _beforeSpeed;
        hasBuff = false;

        endTime = Time.time; // ����׿�
    }

    IEnumerator Slow(float delay)
    {
        Debug.Log("��ȭ!");

        // ��ȭ
        hasBuff = true;
        actor.actorState = Actor.ActorState.Debuff;
        actor.PlayerControll.RunSpeed = _beforeSpeed * 0.9f;

        yield return new WaitForSeconds(delay);

        // ��ȭ ����
        actor.actorState = Actor.ActorState.Stand;
        actor.debuffState &= ~Actor.DebuffState.Slow;
        actor.PlayerControll.RunSpeed = _beforeSpeed;
        hasBuff = false;

        endTime = Time.time; // ����׿�
    }

    IEnumerator Freeze(float delay)
    {
        Debug.Log("����!");
        yield return new WaitForSeconds(0.2f);

        // ����
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

        yield return new WaitForSeconds(delay);

        // ���� ����
        actor.actorState = Actor.ActorState.Stand;
        actor.debuffState &= ~Actor.DebuffState.Freeze;

        // ����Ʈ ����
        if (hasObject)
        {
            hasObject = false;
        }

        for (int i = 0; i < actor.BodyHandler.BodyParts.Count; i++)
        {
            actor.BodyHandler.BodyParts[i].PartRigidbody.isKinematic = false;
        }

        endTime = Time.time; // ����׿�
    }

    IEnumerator Shock()
    {
        Debug.Log("����!");
        yield return new WaitForSeconds(0.2f);

        // ����
        actor.actorState = Actor.ActorState.Debuff;
        StartCoroutine("ResetBodySpring");

        float startTime = Time.time;
      
        while (Time.time - startTime < _shockTime)
        {
            if (actor.debuffState == Actor.DebuffState.Freeze)
            {
                actor.actorState = Actor.ActorState.Stand;
                StartCoroutine("Stun");
                StopCoroutine("Shock");
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

        endTime = Time.time; // ����׿�

        // ���� ����
        actor.actorState = Actor.ActorState.Stand;
        actor.debuffState &= ~Actor.DebuffState.Shock;
        StartCoroutine("Stun");
    }

    IEnumerator Stun()
    {
        Debug.Log("����!");

        JointDrive angularXDrive;
        JointDrive angularYZDrive;

        float startTime = Time.time;
       
        while (Time.time < startTime + _stunTime)
        {
            float elapsed = Time.time - startTime;
            float percentage = elapsed / _stunTime;
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

        actor.debuffState &= ~Actor.DebuffState.Stun;
        endTime = Time.time;
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
                if (actor.debuffState == Actor.DebuffState.Freeze)
                    return;

                Debug.Log("�������� ���Ƽ� ����");
                _maxUnconsciousTime = Mathf.Clamp(_maxUnconsciousTime + 1.5f, _minUnconsciousTime, 20f);
                _unconsciousTime = _maxUnconsciousTime;
                actor.actorState = Actor.ActorState.Unconscious;
                actor.debuffState = Actor.DebuffState.Stun;
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
