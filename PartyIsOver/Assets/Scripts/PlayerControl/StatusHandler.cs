using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.UI;
using static InteractableObject;
using static UnityEngine.Rendering.VirtualTexturing.Debugging;

public class StatusHandler : MonoBehaviourPun
{
    private float _damageModifer = 1f;

    public Actor actor;

    public bool invulnerable = false;
    
    private float _healthDamage;
    private bool _isDead;

    // ���� �ð�
    private float _maxUnconsciousTime=5f;
    private float _minUnconsciousTime=3f;
    private float _unconsciousTime = 0f;

    private float _knockoutThreshold=20f;

    // �ʱ� ������
    private List<float> _xPosSpringAry = new List<float>();
    private List<float> _yzPosSpringAry = new List<float>();


    // ����Ʈ ����
    private bool hasObject = false;

    // �ʱ� �ӵ�
    private float _maxSpeed;

    // ���� Ȯ�ο� �÷���
    private bool _hasPowerUp;
    private bool _hasBurn;
    private bool _hasExhausted;
    private bool _hasSlow;
    private bool _hasFreeze;
    private bool _hasShock;
    private bool _hasStun;
    private bool _hasDrunk;
    private bool _hasBalloon;
    private bool _hasGhost;

   
    [Header("�Ҳ� �ð�")]
    [SerializeField]
    private float _powerUpTime;
    [Header("ȭ�� �ð�")]
    [SerializeField]
    private float _burnTime;
    [Header("��ħ �ð�")]
    [SerializeField]
    private float _exhaustedTime;
    [Header("��ȭ �ð�")]
    [SerializeField]
    private float _slowTime;
    [Header("���� �ð�")]
    [SerializeField]
    private float _freezeTime;
    [Header("���� �ð�")]
    [SerializeField]
    private float _shockTime;
    [Header("���� �ð�")]
    [SerializeField]
    private float _stunTime;

    [Header("���� ������")]
    [SerializeField]
    public float _iceDamage;
    [Header("ȭ�� ������")]
    [SerializeField]
    public float _burnDamage;


    void Start()
    {
        actor = transform.GetComponent<Actor>();
        _maxSpeed = actor.PlayerController.RunSpeed;

        actor.BodyHandler.BodySetup();

        for (int i = 0; i < actor.BodyHandler.BodyParts.Count; i++)
        {
            if (i == 3)
                continue;

            _xPosSpringAry.Add(actor.BodyHandler.BodyParts[i].PartJoint.angularXDrive.positionSpring);
            _yzPosSpringAry.Add(actor.BodyHandler.BodyParts[i].PartJoint.angularYZDrive.positionSpring);
        }
    }

    
    void Update()
    {

        // ��ħ ����� Ȱ��ȭ/��Ȱ��ȭ
        if(actor.Stamina == 0)
        {
            if(!_hasExhausted)
            {
                actor.debuffState |= Actor.DebuffState.Exhausted;
                StartCoroutine(Exhausted(_exhaustedTime));
            }
        }
    }

    private void OnGUI()
    {
        if (this.name == "Ragdoll2(Clone)" && photonView.IsMine)
        {
            GUI.contentColor = Color.red;
            GUI.Label(new Rect(0, 0, 200, 200), "��������:" + actor.debuffState.ToString());
            GUI.Label(new Rect(0, 20, 200, 200), "�׼ǻ���:" + actor.actorState.ToString());

            GUI.contentColor = Color.blue;
            GUI.Label(new Rect(0, 60, 200, 200), "ü��: " + actor.Health);
            GUI.Label(new Rect(0, 80, 200, 200), "���׹̳�: " + actor.Stamina);
        }
    }

    // ����� ��������(trigger)
    public void AddDamage(InteractableObject.Damage type, float damage, GameObject causer)
    {
        // ������ üũ
        damage *= _damageModifer;

        if (!invulnerable && actor.actorState != Actor.ActorState.Dead && actor.actorState != Actor.ActorState.Unconscious)
        {
            _healthDamage += damage;
        }

        // �����̻� üũ
        DebuffCheck(type);
        DebuffAction();

        //Debug.Log("[AddDamage()] _healthDamage " + photonView.ViewID + " - " + _healthDamage);

        if (_healthDamage != 0f)
            UpdateHealth();
    }

    public void DebuffCheck(InteractableObject.Damage type)
    {
        if (actor.debuffState == Actor.DebuffState.Ice) return;
        if (actor.debuffState == Actor.DebuffState.Balloon) return;

        switch (type)
        {
            case Damage.Ice: // ����
                actor.debuffState |= Actor.DebuffState.Ice;
                // �ٸ� ����� üũ
                foreach (Actor.DebuffState state in System.Enum.GetValues(typeof(Actor.DebuffState)))
                {
                    // ���� �̿��� ���°� ������ ����
                    if (state != Actor.DebuffState.Ice && (actor.debuffState & state) != 0)
                    {
                        actor.debuffState &= ~state;
                    }
                }
                break;
            case Damage.Balloon:
                break;
            case Damage.PowerUp: // �Ҳ�
                actor.debuffState |= Actor.DebuffState.PowerUp;
                break;
            case Damage.Burn: // ȭ��
                actor.debuffState |= Actor.DebuffState.Burn;
                break;
            case Damage.Shock: // ����
                actor.debuffState |= Actor.DebuffState.Shock;
                break;
            case Damage.Knockout: // ���� (����� Damage.Knockout ��)
                actor.debuffState |= Actor.DebuffState.Stun;
                break;
           
        }
    }

    public void DebuffAction()
    {
        foreach (Actor.DebuffState state in System.Enum.GetValues(typeof(Actor.DebuffState)))
        {
            Actor.DebuffState checking = actor.debuffState & state;
            
            switch (checking)
            {
                case Actor.DebuffState.Default:
                    break;
                case Actor.DebuffState.PowerUp:
                    if(!_hasPowerUp)
                        StartCoroutine(PowerUp(_powerUpTime));
                    break;
                case Actor.DebuffState.Burn:
                    if (!_hasBurn)
                        StartCoroutine(Burn(_burnTime));
                    break;
                case Actor.DebuffState.Slow:
                    if(!_hasSlow)
                        StartCoroutine(Slow(_slowTime));
                    break;
                case Actor.DebuffState.Ice:
                    if(!_hasFreeze)
                        StartCoroutine(Freeze(_freezeTime));
                    break;
                case Actor.DebuffState.Shock:
                    if (!_hasShock)
                        StartCoroutine(Shock(_shockTime));
                    break;
                case Actor.DebuffState.Stun:
                    if (!_hasStun)
                    {
                        StartCoroutine(ResetBodySpring());
                        StartCoroutine(Stun(_stunTime));
                    }
                    break;
                case Actor.DebuffState.Drunk:
                    break;
                case Actor.DebuffState.Balloon:
                    break;
                case Actor.DebuffState.Ghost:
                    break;
            }
        }
    }

    IEnumerator PowerUp(float delay)
    {
        // �Ҳ�
        _hasPowerUp = true;
        actor.actorState = Actor.ActorState.Debuff;
        actor.PlayerController.RunSpeed += _maxSpeed * 0.1f;

        yield return new WaitForSeconds(delay);

        // �Ҳ� ����
        _hasPowerUp = false;
        actor.actorState = Actor.ActorState.Stand;
        actor.debuffState &= ~Actor.DebuffState.PowerUp;
        actor.PlayerController.RunSpeed -= _maxSpeed * 0.1f;
    }
    IEnumerator Burn(float delay)
    {
        // ȭ��
        _hasBurn = true;
        actor.actorState = Actor.ActorState.Debuff;

        float elapsedTime = 0f;
        float lastBurnTime = Time.time;
        float startTime = Time.time;

        while (elapsedTime < delay)
        {
            if (actor.debuffState == Actor.DebuffState.Ice)
            {
                _hasBurn = false;
                actor.actorState = Actor.ActorState.Stand;
                StopCoroutine(Burn(delay));
            }

            if (Time.time - lastBurnTime >= 1.0f) // 1�ʰ� ������+�׼�
            {
                actor.Health -= _burnDamage;
                actor.BodyHandler.BodyParts[2].PartRigidbody.AddForce((actor.BodyHandler.Hip.transform.right) * 25, ForceMode.VelocityChange);
                actor.BodyHandler.BodyParts[3].PartRigidbody.AddForce((actor.BodyHandler.Hip.transform.right) * 25, ForceMode.VelocityChange);
                lastBurnTime = Time.time;
            }

            elapsedTime = Time.time - startTime;
            yield return null;
        }

        // ȭ�� ����
        _hasBurn = false;
        actor.actorState = Actor.ActorState.Stand;
        actor.debuffState &= ~Actor.DebuffState.Burn;
    }
    IEnumerator Exhausted(float delay)
    {
        // ��ħ
        _hasExhausted = true;
        actor.actorState = Actor.ActorState.Debuff;
        JointDrive angularXDrive;

        angularXDrive = actor.BodyHandler.BodyParts[0].PartJoint.angularXDrive;
        angularXDrive.positionSpring = 0f;
        actor.BodyHandler.BodyParts[0].PartJoint.angularXDrive = angularXDrive;

        float startTime = Time.time;
        while (Time.time < startTime + delay)
        {
            float elapsed = Time.time - startTime;
            float percentage = elapsed / delay;

            actor.Stamina = Mathf.Clamp(actor.MaxStamina * percentage, 0, actor.MaxStamina);
            yield return null;
        }

        // ��ħ ����
        _hasExhausted = false;
        actor.actorState = Actor.ActorState.Stand;
        actor.debuffState &= ~Actor.DebuffState.Exhausted;
        angularXDrive.positionSpring = _xPosSpringAry[0];

        actor.BodyHandler.BodyParts[0].PartJoint.angularXDrive = angularXDrive;
        actor.Stamina = 100;
    }
    IEnumerator Slow(float delay)
    {
        // ��ȭ
        _hasSlow = true;
        actor.actorState = Actor.ActorState.Debuff;
        actor.PlayerController.RunSpeed -= _maxSpeed * 0.1f;

        yield return new WaitForSeconds(delay);

        // ��ȭ ����
        _hasSlow = false;
        actor.actorState = Actor.ActorState.Stand;
        actor.debuffState &= ~Actor.DebuffState.Slow;
        actor.PlayerController.RunSpeed += _maxSpeed * 0.1f;
    }
    IEnumerator Freeze(float delay)
    {
        yield return new WaitForSeconds(0.2f);

        // ����
        _hasFreeze = true;
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
        _hasFreeze = false;
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
    IEnumerator Shock(float delay)
    {
        yield return new WaitForSeconds(0.2f);

        // ����
        _hasShock = true;
        actor.actorState = Actor.ActorState.Debuff;

        JointDrive angularXDrive;
        JointDrive angularYZDrive;

        for (int i = 4; i < actor.BodyHandler.BodyParts.Count; i++)
        {
            angularXDrive = actor.BodyHandler.BodyParts[i].PartJoint.angularXDrive;
            angularXDrive.positionSpring = 0f;
            actor.BodyHandler.BodyParts[i].PartJoint.angularXDrive = angularXDrive;

            angularYZDrive = actor.BodyHandler.BodyParts[i].PartJoint.angularYZDrive;
            angularYZDrive.positionSpring = 0f;
            actor.BodyHandler.BodyParts[i].PartJoint.angularYZDrive = angularYZDrive;
        }

        float startTime = Time.time;
      
        while (Time.time - startTime < delay)
        {
            if (actor.debuffState == Actor.DebuffState.Ice)
            {
                _hasShock = false;
                actor.actorState = Actor.ActorState.Stand;
                StartCoroutine(Stun(_stunTime));
                StopCoroutine(Shock(delay));
            }

            if (Random.Range(0, 20) > 17)
            {
                for (int i = 4; i < 14; i++)
                {
                    if (i == 9) continue;
                    actor.BodyHandler.BodyParts[i].transform.rotation = Quaternion.Euler(20, 0, 0);
                }
            }
            else
            {
                for (int i = 4; i < 14; i++)
                {
                    if (i == 9) continue;
                    actor.BodyHandler.BodyParts[i].transform.rotation = Quaternion.Euler(-20, 0, 0);
                }
            }
            yield return null;
        }

        // ���� ����
        _hasShock = false;
        StartCoroutine(ResetBodySpring());
        StartCoroutine(Stun(3));
        actor.actorState = Actor.ActorState.Stand;
        actor.debuffState &= ~Actor.DebuffState.Shock;
    }

    IEnumerator Stun(float delay)
    {
        _hasStun = true;
        yield return new WaitForSeconds(delay);
        yield return RestoreBodySpring();

        _hasStun = false;
        actor.actorState = Actor.ActorState.Stand;
        actor.debuffState &= ~Actor.DebuffState.Stun;
    }

    public void UpdateHealth()
    {
        if (_isDead)
            return;

        //���� ü�� �޾ƿ���
        float tempHealth = actor.Health;

        //�������°� �ƴҶ��� ������ ����
        if (tempHealth > 0f && !invulnerable)
            tempHealth -= _healthDamage;

        float realDamage = actor.Health - tempHealth;

        //�������°� �ƴҶ� ���� �̻��� �������� ������ ����
        if (actor.actorState != Actor.ActorState.Unconscious)
        {
            if (_unconsciousTime >= 0f)
                _unconsciousTime = Mathf.Clamp(_unconsciousTime - Time.deltaTime, 0f, _maxUnconsciousTime);
            
            if (realDamage >= _knockoutThreshold)
            {
                if (actor.debuffState == Actor.DebuffState.Ice)
                    return;

                _maxUnconsciousTime = Mathf.Clamp(_maxUnconsciousTime + 1.5f, _minUnconsciousTime, 20f);
                _unconsciousTime = _maxUnconsciousTime;
                actor.actorState = Actor.ActorState.Unconscious;
                actor.debuffState = Actor.DebuffState.Stun;
                EnterUnconsciousState();
            }
        }

        // �����϶�
        if (actor.actorState == Actor.ActorState.Unconscious)
        {
            _unconsciousTime = Mathf.Clamp(_unconsciousTime - Time.deltaTime, 0f, _maxUnconsciousTime);
            StartCoroutine(ResetBodySpring());
            StartCoroutine(Stun(_unconsciousTime));
            _unconsciousTime = 0f;
        }

        //����� ü���� 0���� ������ Death��
        if (tempHealth <= 0f)
        {
            KillPlayer();
            EnterUnconsciousState();
        }

        actor.Health = Mathf.Clamp(tempHealth, 0f, actor.MaxHealth);

        actor.HurtEventInvoke();

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
        actor.BodyHandler.LeftForearm.PartRigidbody.collisionDetectionMode = CollisionDetectionMode.Discrete;
        actor.BodyHandler.RightHand.PartRigidbody.collisionDetectionMode = CollisionDetectionMode.Discrete;
        actor.BodyHandler.RightForearm.PartRigidbody.collisionDetectionMode = CollisionDetectionMode.Discrete;
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