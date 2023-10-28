using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Numerics;
using UnityEngine;
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
    private float _healthRegeneration;
    private float _healthDamage;
    private bool _isDead;

    private float _maxUnconsciousTime=5f;
    private float _minUnconsciousTime=3f;
    private float _unconsciousTime = 0f;

    private float _knockoutThreshold=20f;

    private BodyHandler bodyHandler;

    void Start()
    {
        bodyHandler = GetComponent<BodyHandler>();
        actor = transform.GetComponent<Actor>();
        _health = _maxHealth;
    }

    void Update()
    {
        if (_healthDamage != 0f)
            UpdateHealth();

        CheckConscious();
    }

    public void AddDamage(InteractableObject.Damage type, float damage, GameObject causer)
    {
        Debug.Log("AddDamage");

        damage *= _damageModifer;
        if (!invulnerable && actor.actorState != Actor.ActorState.Dead && actor.actorState != Actor.ActorState.Unconscious)
        {
            //if (numOfDamageTypes < damageTypes.Length)
            //{
            //    damageTypes[numOfDamageTypes] = new Damage(type, damage, causer);
            //    numOfDamageTypes++;
            //    hasDamageTypes = true;
            //}
            _healthDamage += damage;
        }

        Debug.Log($"������ Ÿ��: {type}");
        DebuffCheck(type);
    }

    // �����̻� üũ
    public void DebuffCheck(InteractableObject.Damage type)
    {
        switch (type)
        {
            case Damage.Ice:
                StartCoroutine("Ice"); // ����
                break;
            case Damage.ElectricShock:
                StartCoroutine("ElectricShock"); // ����
                break;
        }
    }

    IEnumerator Ice()
    {
        bodyHandler.LeftHand.PartRigidbody.isKinematic = true;
        bodyHandler.LeftForarm.PartRigidbody.isKinematic = true;
        bodyHandler.LeftArm.PartRigidbody.isKinematic = true;
        bodyHandler.LeftFoot.PartRigidbody.isKinematic = true;
        bodyHandler.LeftLeg.PartRigidbody.isKinematic = true;
        bodyHandler.LeftThigh.PartRigidbody.isKinematic = true;

        bodyHandler.RightHand.PartRigidbody.isKinematic = true;
        bodyHandler.RightForarm.PartRigidbody.isKinematic = true;
        bodyHandler.RightArm.PartRigidbody.isKinematic = true;
        bodyHandler.RightFoot.PartRigidbody.isKinematic = true;
        bodyHandler.RightLeg.PartRigidbody.isKinematic = true;
        bodyHandler.RightThigh.PartRigidbody.isKinematic = true;

        bodyHandler.Head.PartRigidbody.isKinematic = true;
        bodyHandler.Chest.PartRigidbody.isKinematic = true;
        bodyHandler.Waist.PartRigidbody.isKinematic = true;
        bodyHandler.Hip.PartRigidbody.isKinematic = true;
        
        Debug.Log("����!");
        yield return new WaitForSeconds(2.0f);

        bodyHandler.LeftHand.PartRigidbody.isKinematic = false;
        bodyHandler.LeftForarm.PartRigidbody.isKinematic = false;
        bodyHandler.LeftArm.PartRigidbody.isKinematic = false;
        bodyHandler.LeftFoot.PartRigidbody.isKinematic = false;
        bodyHandler.LeftLeg.PartRigidbody.isKinematic = false;
        bodyHandler.LeftThigh.PartRigidbody.isKinematic = false;

        bodyHandler.RightHand.PartRigidbody.isKinematic = false;
        bodyHandler.RightForarm.PartRigidbody.isKinematic = false;
        bodyHandler.RightArm.PartRigidbody.isKinematic = false;
        bodyHandler.RightFoot.PartRigidbody.isKinematic = false;
        bodyHandler.RightLeg.PartRigidbody.isKinematic = false;
        bodyHandler.RightThigh.PartRigidbody.isKinematic = false;

        bodyHandler.Head.PartRigidbody.isKinematic = false;
        bodyHandler.Chest.PartRigidbody.isKinematic = false;
        bodyHandler.Waist.PartRigidbody.isKinematic = false;
        bodyHandler.Hip.PartRigidbody.isKinematic = false;
    }

    IEnumerator ElectricShock()
    {
        Debug.Log("����!");
        yield return 0;
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
                Debug.Log("����");

                _maxUnconsciousTime = Mathf.Clamp(_maxUnconsciousTime + 1.5f, _minUnconsciousTime, 20f);
                _unconsciousTime = _maxUnconsciousTime;
                actor.actorState = Actor.ActorState.Unconscious;
                //CheckForDamageAchievements();
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
        //damageCausers.Clear();
        //ClearDamageTypes();
    }

    void KillPlayer()
    {

    }



    public void CheckConscious()
    {
        if (actor.actorState != Actor.ActorState.Unconscious && _unconsciousTime > 0f)
        {
            _unconsciousTime = Mathf.Clamp(_unconsciousTime - Time.deltaTime, 0f, _maxUnconsciousTime);
        }
        if (actor.actorState == Actor.ActorState.Unconscious)
        {
            _unconsciousTime = Mathf.Clamp(_unconsciousTime - Time.deltaTime, 0f, _maxUnconsciousTime);
            if (_unconsciousTime <= 0f)
            {
                actor.actorState = Actor.ActorState.Stand;
                actor.PlayerControll.RestoreSpringTrigger();
                _unconsciousTime = 0f;
            }
        }
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
}
