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
    
    // Start is called before the first frame update
    void Start()
    {
        actor = transform.GetComponent<Actor>();
        _health = _maxHealth;
    }

    // Update is called once per frame
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


    public void EnterUnconsciousState()
    {
        //������ ����Ʈ�� ���� ���� �߰�

        _maxUnconsciousTime = Mathf.Clamp(_maxUnconsciousTime + 1.5f, _minUnconsciousTime, 20f);
        _unconsciousTime = _maxUnconsciousTime;
        actor.actorState = Actor.ActorState.Unconscious;


        //actor.BodyHandler.ResetLeftGrab();
        //actor.BodyHandler.ResetRightGrab();
        actor.BodyHandler.LeftHand.PartRigidbody.collisionDetectionMode = CollisionDetectionMode.Discrete;
        actor.BodyHandler.LeftForarm.PartRigidbody.collisionDetectionMode = CollisionDetectionMode.Discrete;
        actor.BodyHandler.RightHand.PartRigidbody.collisionDetectionMode = CollisionDetectionMode.Discrete;
        actor.BodyHandler.RightForarm.PartRigidbody.collisionDetectionMode = CollisionDetectionMode.Discrete;
    }
}
