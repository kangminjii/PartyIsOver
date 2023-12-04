using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static InteractableObject;

public class StatusHandler : MonoBehaviourPun
{
    private float _damageModifer = 1f;

    public Actor actor;

    public bool invulnerable = false;

    private float _healthDamage;
    public bool _isDead;


    private float _knockoutThreshold = 15f;

    // �ʱ� ������
    private List<float> _xPosSpringAry = new List<float>();
    private List<float> _yzPosSpringAry = new List<float>();

   

    // �ʱ� �ӵ�
    private float _maxSpeed;




    // ���� Ȯ�ο� �÷���
    private bool _hasPowerUp;
    private bool _hasBurn;
    private bool _hasExhausted;
    private bool _hasSlow;
    public bool _hasFreeze;
    public bool _hasShock;
    private bool _hasStun;
    public bool HasDrunk;

    public Transform playerTransform;
    public GameObject effectObject = null;
    int _burnCount = 0;

    AudioClip _audioClip = null;
    AudioSource _audioSource;

    [Header("Debuff Duration")]
    [SerializeField]
    private float _powerUpTime;
    [SerializeField]
    private float _burnTime;
    [SerializeField]
    private float _exhaustedTime;
    [SerializeField]
    private float _slowTime;
    [SerializeField]
    private float _freezeTime;
    [SerializeField]
    private float _shockTime;
    [SerializeField]
    private float _stunTime;

    [Header("Debuff Damage")]
    [SerializeField]
    public float _iceDamage;
    [SerializeField]
    public float _burnDamage;

    int Creatcount = 0;


    private void Awake()
    {
        playerTransform = this.transform.Find("GreenHip").GetComponent<Transform>();
        Transform SoundSourceTransform = transform.Find("GreenHip");
        _audioSource = SoundSourceTransform.GetComponent<AudioSource>();
    }

    void Start()
    {
        actor = transform.GetComponent<Actor>();
        _maxSpeed = actor.PlayerController.RunSpeed;

        actor.BodyHandler.BodySetup();

        for (int i = 0; i < actor.BodyHandler.BodyParts.Count; i++)
        {
            if (i == (int)Define.BodyPart.Hip)
                continue;

            _xPosSpringAry.Add(actor.BodyHandler.BodyParts[i].PartJoint.angularXDrive.positionSpring);
            _yzPosSpringAry.Add(actor.BodyHandler.BodyParts[i].PartJoint.angularYZDrive.positionSpring);
        }
    }


    void Update()
    {
        // ��ħ ����� Ȱ��ȭ/��Ȱ��ȭ
        if (actor.Stamina <= 0)
        {
            if (!_hasExhausted)
            {
                actor.debuffState |= Actor.DebuffState.Exhausted;
                photonView.RPC("Exhausted", RpcTarget.All, _exhaustedTime);
            }
        }
    }


    private void LateUpdate()
    {
        if (effectObject != null && effectObject.name == "Stun_loop")
            photonView.RPC("MoveEffect", RpcTarget.All);
        else if (effectObject != null && effectObject.name == "Fog_frost")
            photonView.RPC("MoveEffect", RpcTarget.All);
        else if (effectObject != null)
            photonView.RPC("PlayerEffect", RpcTarget.All);

    }

    // ����� ��������(trigger)
    public void AddDamage(InteractableObject.Damage type, float damage, GameObject causer=null)
    {
        // ������ üũ
        damage *= _damageModifer;

        if (!invulnerable && actor.actorState != Actor.ActorState.Dead && actor.actorState != Actor.ActorState.Unconscious)
        {
            _healthDamage += damage;
        }

        if (_healthDamage != 0f)
            UpdateHealth();

        if (actor.actorState != Actor.ActorState.Dead)
        {
            // �����̻� üũ
            DebuffCheck(type);
            DebuffAction();
            //CheckProjectile(causer);
        }

        photonView.RPC("InvulnerableState", RpcTarget.All, 0.5f);
        actor.InvokeStatusChangeEvent();
    }

    void CheckProjectile(GameObject go)
    {
        if (go.GetComponent<ProjectileStandard>() != null)
        {
            go.GetComponent<ProjectileStandard>().DestoryProjectileTrigger();
        }
    }

    [PunRPC]
    void PlayerDebuffSound(string path)
    {
        _audioClip = Managers.Sound.GetOrAddAudioClip(path);
        _audioSource.clip = _audioClip;
        _audioSource.volume = 0.2f;
        _audioSource.spatialBlend = 1;
        Managers.Sound.Play(_audioClip, Define.Sound.PlayerEffect, _audioSource);
    }

    public void DebuffCheck(InteractableObject.Damage type)
    {
        if (actor.debuffState != Actor.DebuffState.Default)
            return;

        if (actor.debuffState == Actor.DebuffState.Ice) return;
        //if (actor.debuffState == Actor.DebuffState.Balloon) return;

        switch (type)
        {
            case Damage.Ice: // ����
                actor.debuffState |= Actor.DebuffState.Ice;
                //foreach (Actor.DebuffState state in System.Enum.GetValues(typeof(Actor.DebuffState)))
                //{
                //    if (state != Actor.DebuffState.Ice && (actor.debuffState & state) != 0)
                //    {
                //        actor.debuffState &= ~state;
                //    }
                //}
                break;
            //case Damage.Balloon: // ǳ��
            //    {
            //        actor.debuffState |= Actor.DebuffState.Balloon;
            //        photonView.RPC("PlayerDebuffSound", RpcTarget.All, "PlayerEffect/Cartoon-UI-049");
            //        foreach (Actor.DebuffState state in System.Enum.GetValues(typeof(Actor.DebuffState)))
            //        {
            //            if (state != Actor.DebuffState.Balloon && (actor.debuffState & state) != 0)
            //            {
            //                actor.debuffState &= ~state;
            //            }
            //        }
            //    }
            //    break;
            case Damage.PowerUp: // �Ҳ�
                actor.debuffState |= Actor.DebuffState.PowerUp;
                break;
            case Damage.Burn: // ȭ��
                actor.debuffState |= Actor.DebuffState.Burn;
                break;
            case Damage.Shock: // ����
                if (actor.debuffState == Actor.DebuffState.Stun || actor.debuffState == Actor.DebuffState.Drunk)
                    break;
                else
                    actor.debuffState |= Actor.DebuffState.Shock;
                break;
            case Damage.Stun: // ����
                if (actor.debuffState == Actor.DebuffState.Shock || actor.debuffState == Actor.DebuffState.Drunk )
                    break;
                else
                    actor.debuffState |= Actor.DebuffState.Stun;
                break;
            case Damage.Drunk: // ����
                if (actor.debuffState == Actor.DebuffState.Stun || actor.debuffState == Actor.DebuffState.Shock)
                    break;
                else
                {
                    actor.debuffState |= Actor.DebuffState.Drunk;
                    photonView.RPC("PlayerDebuffSound", RpcTarget.All, "PlayerEffect/Cartoon-UI-049");
                }
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
                    if (!_hasPowerUp)
                        photonView.RPC("PowerUp", RpcTarget.All, _powerUpTime);
                    break;
                case Actor.DebuffState.Burn:
                    if (!_hasBurn)
                        photonView.RPC("Burn", RpcTarget.All, _burnTime);
                    break;
                case Actor.DebuffState.Slow:
                    if (!_hasSlow)
                        photonView.RPC("Slow", RpcTarget.All, _slowTime);
                    break;
                case Actor.DebuffState.Shock:
                    if (!_hasShock)
                        photonView.RPC("Shock", RpcTarget.All, _shockTime);
                    break;
                case Actor.DebuffState.Stun:
                    if (!_hasStun)
                    {
                        StartCoroutine(ResetBodySpring());
                        photonView.RPC("Stun", RpcTarget.All, _stunTime);
                    }
                    break;
                case Actor.DebuffState.Ghost:
                    break;
                case Actor.DebuffState.Drunk:
                    if(!HasDrunk)
                    {
                        photonView.RPC("PoisonCreate", RpcTarget.All);
                    }
                    break;
                case Actor.DebuffState.Ice:
                    if (!_hasFreeze)
                        photonView.RPC("Freeze", RpcTarget.All, _freezeTime);
                    break;
            }
        }
    }

    [PunRPC]
    IEnumerator PowerUp(float delay)
    {

        // �Ҳ�
        _hasPowerUp = true;
        actor.actorState = Actor.ActorState.Debuff;
        actor.PlayerController.RunSpeed += _maxSpeed * 0.1f;
        PlayerDebuffSound("PlayerEffect/Cartoon-UI-037");
        PowerUpCreate();

        yield return new WaitForSeconds(delay);

        // �Ҳ� ����
        _hasPowerUp = false;
        actor.actorState = Actor.ActorState.Stand;
        actor.debuffState &= ~Actor.DebuffState.PowerUp;
        actor.PlayerController.RunSpeed -= _maxSpeed * 0.1f;
        DestroyEffect("Aura_acceleration");

        actor.InvokeStatusChangeEvent();
        _audioClip = null;
    }

    [PunRPC]
    IEnumerator Burn(float delay)
    {
        // ȭ��
        _hasBurn = true;
        actor.actorState = Actor.ActorState.Debuff;
        photonView.RPC("TransferDebuffToPlayer", RpcTarget.MasterClient, (int)InteractableObject.Damage.Burn);


        PlayerDebuffSound("PlayerEffect/SFX_FireBall_Projectile");
        BurnCreate();

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
                actor.BodyHandler.Waist.PartRigidbody.AddForce((actor.BodyHandler.Hip.transform.right) * 25, ForceMode.VelocityChange);
                actor.BodyHandler.Hip.PartRigidbody.AddForce((actor.BodyHandler.Hip.transform.right) * 25, ForceMode.VelocityChange);
                lastBurnTime = Time.time;
            }

            elapsedTime = Time.time - startTime;
            yield return null;
        }
        _burnCount = 0;
        Creatcount = 0;
        // ȭ�� ����
        _hasBurn = false;
        photonView.RPC("TransferDebuffToPlayer", RpcTarget.MasterClient, (int)InteractableObject.Damage.Default);
        actor.actorState = Actor.ActorState.Stand;
        actor.debuffState &= ~Actor.DebuffState.Burn;

        DestroyEffect("Fire_large");
        actor.InvokeStatusChangeEvent();
        _audioClip = null;
    }

   
    [PunRPC]
    IEnumerator Exhausted(float delay)
    {
        // ��ħ
        _hasExhausted = true;
        WetCreate();
        actor.actorState = Actor.ActorState.Debuff;
        JointDrive angularXDrive;

        angularXDrive = actor.BodyHandler.BodyParts[(int)Define.BodyPart.Head].PartJoint.angularXDrive;
        angularXDrive.positionSpring = 0f;
        actor.BodyHandler.BodyParts[(int)Define.BodyPart.Head].PartJoint.angularXDrive = angularXDrive;

        while(actor.Stamina != 100)
        {
            yield return null;
        }

        // ��ħ ����
        _hasExhausted = false;
        DestroyEffect("Wet");

        actor.actorState = Actor.ActorState.Stand;
        actor.debuffState &= ~Actor.DebuffState.Exhausted;
        angularXDrive.positionSpring = _xPosSpringAry[0];

        actor.BodyHandler.BodyParts[(int)Define.BodyPart.Head].PartJoint.angularXDrive = angularXDrive;

        actor.InvokeStatusChangeEvent();
        _audioClip = null;
    }
    [PunRPC]
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

        actor.InvokeStatusChangeEvent();
    }

    [PunRPC]
    IEnumerator Freeze(float delay)
    {
        _hasFreeze = true;

        yield return new WaitForSeconds(0.2f);

        PlayerDebuffSound("PlayerEffect/Cartoon-UI-047");
        IceCubeCreate();
        IceSmokeCreate();

        // ����
        actor.actorState = Actor.ActorState.Debuff;

        for (int i = 0; i < actor.BodyHandler.BodyParts.Count; i++)
        {
            actor.BodyHandler.BodyParts[i].PartRigidbody.isKinematic = true;
        }

        yield return new WaitForSeconds(delay);

        // ���� ����
        _hasFreeze = false;
        actor.actorState = Actor.ActorState.Stand;
        actor.debuffState &= ~Actor.DebuffState.Ice;

        actor.InvokeStatusChangeEvent();
        DestroyEffect("Fog_frost");
        DestroyEffect("IceCube");
       
        for (int i = 0; i < actor.BodyHandler.BodyParts.Count; i++)
        {
            actor.BodyHandler.BodyParts[i].PartRigidbody.isKinematic = false;
        }
        _audioClip = null;
    }

    [PunRPC]
    IEnumerator Shock(float delay)
    {
        _hasShock = true;
        

        if (actor.debuffState == Actor.DebuffState.Ice)
            photonView.RPC("StopShock", RpcTarget.All, delay);

        
        // ����
        actor.actorState = Actor.ActorState.Debuff;
        photonView.RPC("TransferDebuffToPlayer", RpcTarget.MasterClient, (int)InteractableObject.Damage.Shock);
        PlayerDebuffSound("PlayerEffect/electronic_02");
        ShockCreate();

        JointDrive angularXDrive;
        JointDrive angularYZDrive;

        for (int i = 0; i < actor.BodyHandler.BodyParts.Count; i++)
        {
            if (i >= (int)Define.BodyPart.Hip && i <= (int)Define.BodyPart.Head) continue;
            if (i == (int)Define.BodyPart.Ball) continue;

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
                StartCoroutine(Stun(0.5f));
                //photonView.RPC("Stun", RpcTarget.All, _stunTime);
                photonView.RPC("StopShock", RpcTarget.All, delay);
            }

            yield return new WaitForSeconds(0.2f);

            if (UnityEngine.Random.Range(0, 20) > 10)
            {
                for (int i = 0; i < actor.BodyHandler.BodyParts.Count; i++)
                {
                    if (i >= (int)Define.BodyPart.Hip && i <= (int)Define.BodyPart.Head) continue;
                    if (i == (int)Define.BodyPart.LeftFoot || 
                        i == (int)Define.BodyPart.RightFoot ||
                        i == (int)Define.BodyPart.Ball) continue;

                    actor.BodyHandler.BodyParts[i].transform.rotation = Quaternion.Euler(20, 0, 0);
                }
            }
            else
            {
                for (int i = 0; i < actor.BodyHandler.BodyParts.Count; i++)
                {
                    if (i >= (int)Define.BodyPart.Hip && i <= (int)Define.BodyPart.Head) continue;
                    if (i == (int)Define.BodyPart.LeftFoot ||
                        i == (int)Define.BodyPart.RightFoot ||
                        i == (int)Define.BodyPart.Ball) continue;
                    actor.BodyHandler.BodyParts[i].transform.rotation = Quaternion.Euler(-20, 0, 0);
                }
            }
            yield return null;
        }

        // ���� ����
        _hasShock = false;
        StartCoroutine(ResetBodySpring());
        photonView.RPC("TransferDebuffToPlayer", RpcTarget.MasterClient, (int)InteractableObject.Damage.Default);

        //photonView.RPC("Stun", RpcTarget.All, 0.5f);
        StartCoroutine(Stun(0.5f));
        
        actor.actorState = Actor.ActorState.Stand;
        actor.debuffState &= ~Actor.DebuffState.Shock;
        DestroyEffect("Lightning_aura");

        actor.InvokeStatusChangeEvent();
        _audioClip = null;
    }

    [PunRPC]
    void StopShock()
    {
        StopCoroutine("Shock");
        // ���� ����
        _hasShock = false;
        StartCoroutine(ResetBodySpring());
        photonView.RPC("Stun", RpcTarget.All, 0.5f);
        actor.actorState = Actor.ActorState.Stand;
        actor.debuffState &= ~Actor.DebuffState.Shock;
        photonView.RPC("DestroyEffect", RpcTarget.All, "Lightning_aura");

        actor.InvokeStatusChangeEvent();
        _audioClip = null;
    }


    [PunRPC]
    IEnumerator Stun(float delay)
    {
        _hasStun = true;
        yield return new WaitForSeconds(delay);
        yield return RestoreBodySpring();
        _hasStun = false;
        actor.actorState = Actor.ActorState.Stand;
        actor.debuffState &= ~Actor.DebuffState.Stun;

        actor.InvokeStatusChangeEvent();
        DestroyEffect("Stun_loop");
    }

    [PunRPC]
    public void DestroyEffect(string name)
    {
        GameObject go = GameObject.Find($"{name}");
        Managers.Resource.Destroy(go);
        effectObject = null;
    }

    [PunRPC]
    public void StunCreate()
    {
        //Effects/Stun_loop ���� 
        EffectObjectCreate("Effects/Stun_loop");
    }

    public void BurnCreate()
    {
        EffectObjectCreate("Effects/Fire_large");
    }

    public void ShockCreate()
    {
        EffectObjectCreate("Effects/Lightning_aura");
    }

    public void PowerUpCreate()
    {
        EffectObjectCreate("Effects/Aura_acceleration");
    }

    public void IceCubeCreate()
    {
        EffectObjectCreate("Effects/IceCube");
    }

    public void IceSmokeCreate()
    {
        EffectObjectCreate("Effects/Fog_frost");
    }

    [PunRPC]
    public void PoisonCreate()
    {
        HasDrunk = true;
        EffectObjectCreate("Effects/Fog_poison");
    }

    public void WetCreate()
    {
        EffectObjectCreate("Effects/Wet");
    }

    void EffectObjectCreate(string path)
    {
        effectObject = Managers.Resource.PhotonNetworkInstantiate($"{path}");
        effectObject.transform.position = playerTransform.position;
    }

    [PunRPC]
    public void MoveEffect()
    {
        //LateUpdate���� �ʰ� ������ �Ǿ NullReference�� ���� ���� if ���� �־���
        if (effectObject != null && effectObject.name == "Stun_loop")
            effectObject.transform.position = new Vector3(playerTransform.position.x, playerTransform.position.y + 1, playerTransform.position.z);
        else if (effectObject != null && effectObject.name == "Fog_frost")
        {
            effectObject.transform.position = new Vector3(playerTransform.position.x, playerTransform.position.y - 2, playerTransform.position.z);
        }

    }
    [PunRPC]
    public void PlayerEffect()
    {
        if (effectObject != null)
            effectObject.transform.position = playerTransform.position;
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

        //����� ü���� 0���� ������ Death��
        if (tempHealth <= 0f)
        {
            EnterUnconsciousState();
            KillPlayer();
        }
        else
        {
            //�������°� �ƴҶ� ���� �̻��� �������� ������ ����
            if (actor.actorState != Actor.ActorState.Unconscious)
            {
                if (realDamage >= _knockoutThreshold)
                {
                    if (actor.debuffState == Actor.DebuffState.Ice) //�����̻� �Ŀ� �߰�
                        return;
                    actor.actorState = Actor.ActorState.Unconscious;
                    photonView.RPC("StunCreate", RpcTarget.All);
                    EnterUnconsciousState();
                }
            }
        }
        actor.Health = Mathf.Clamp(tempHealth, 0f, actor.MaxHealth);

        _healthDamage = 0f;
    }
    [PunRPC]
    IEnumerator InvulnerableState(float time)
    {
        invulnerable = true;
        yield return new WaitForSeconds(time);
        invulnerable = false;
    }

    void KillPlayer()
    {
        StartCoroutine(ResetBodySpring());
        actor.actorState = Actor.ActorState.Dead;
        _isDead = true;
        actor.InvokeDeathEvent();
    }

    void EnterUnconsciousState()
    {
        //������ ����Ʈ�� ���� ���� �߰�

        actor.debuffState = Actor.DebuffState.Stun;
        StartCoroutine(ResetBodySpring());
        actor.Grab.GrabResetTrigger();
        actor.BodyHandler.LeftHand.PartRigidbody.collisionDetectionMode = CollisionDetectionMode.Discrete;
        actor.BodyHandler.LeftForearm.PartRigidbody.collisionDetectionMode = CollisionDetectionMode.Discrete;
        actor.BodyHandler.RightHand.PartRigidbody.collisionDetectionMode = CollisionDetectionMode.Discrete;
        actor.BodyHandler.RightForearm.PartRigidbody.collisionDetectionMode = CollisionDetectionMode.Discrete;
    }

    [PunRPC]
    void SetJointSpring(float percentage)
    {
        JointDrive angularXDrive;
        JointDrive angularYZDrive;
        int j = 0;

        //������ ȸ���� ��� ���� �����ÿ� �ۼ�Ƽ���� 0�����ؼ� ���
        for (int i = 0; i < actor.BodyHandler.BodyParts.Count; i++)
        {
            if (i == (int)Define.BodyPart.Hip)
                continue;

            angularXDrive = actor.BodyHandler.BodyParts[i].PartJoint.angularXDrive;
            angularXDrive.positionSpring = _xPosSpringAry[j] * percentage;
            actor.BodyHandler.BodyParts[i].PartJoint.angularXDrive = angularXDrive;

            angularYZDrive = actor.BodyHandler.BodyParts[i].PartJoint.angularYZDrive;
            angularYZDrive.positionSpring = _yzPosSpringAry[j] * percentage;
            actor.BodyHandler.BodyParts[i].PartJoint.angularYZDrive = angularYZDrive;

            j++;
        }

    }

    public IEnumerator ResetBodySpring()
    {
        photonView.RPC("SetJointSpring", RpcTarget.All, 0f);
        yield return null;
    }

    public IEnumerator RestoreBodySpring()
    {
        float startTime = Time.time;
        float springLerpDuration = 0.07f;

        while (Time.time < startTime + springLerpDuration)
        {
            float elapsed = Time.time - startTime;
            float percentage = elapsed / springLerpDuration;
            photonView.RPC("SetJointSpring", RpcTarget.All, percentage);
            yield return null;
        }
    }

    public IEnumerator RestoreBodySpring(float _springLerpTime = 1f)
    {
        float startTime = Time.time;
        float springLerpDuration = _springLerpTime;

        while (Time.time - startTime < springLerpDuration)
        {
            float elapsed = Time.time - startTime;
            float percentage = elapsed / springLerpDuration;
            photonView.RPC("SetJointSpring", RpcTarget.All, percentage);
            yield return null;
        }
    }

    [PunRPC]
    public void TransferDebuffToPlayer(int DamageType)
    {
        ChangeDamageModifier((int)Define.BodyPart.LeftFoot, DamageType);
        ChangeDamageModifier((int)Define.BodyPart.RightFoot, DamageType);
        ChangeDamageModifier((int)Define.BodyPart.LeftLeg, DamageType);
        ChangeDamageModifier((int)Define.BodyPart.RightLeg, DamageType);
        ChangeDamageModifier((int)Define.BodyPart.Head, DamageType);
        ChangeDamageModifier((int)Define.BodyPart.LeftHand, DamageType);
        ChangeDamageModifier((int)Define.BodyPart.RightHand, DamageType);
    }



    private void ChangeDamageModifier(int bodyPart, int DamageType)
    {
        switch ((Define.BodyPart)bodyPart)
        {
            case Define.BodyPart.LeftFoot:
                actor.BodyHandler.LeftFoot.PartInteractable.damageModifier = (InteractableObject.Damage)DamageType;
                break;
            case Define.BodyPart.RightFoot:
                actor.BodyHandler.RightFoot.PartInteractable.damageModifier = (InteractableObject.Damage)DamageType;
                break;
            case Define.BodyPart.LeftLeg:
                actor.BodyHandler.LeftLeg.PartInteractable.damageModifier = (InteractableObject.Damage)DamageType;
                break;
            case Define.BodyPart.RightLeg:
                actor.BodyHandler.RightLeg.PartInteractable.damageModifier = (InteractableObject.Damage)DamageType;
                break;
            case Define.BodyPart.LeftThigh:
                break;
            case Define.BodyPart.RightThigh:
                break;
            case Define.BodyPart.Hip:
                break;
            case Define.BodyPart.Waist:
                break;
            case Define.BodyPart.Chest:
                break;
            case Define.BodyPart.Head:
                actor.BodyHandler.Head.PartInteractable.damageModifier = (InteractableObject.Damage)DamageType;
                break;
            case Define.BodyPart.LeftArm:
                break;
            case Define.BodyPart.RightArm:
                break;
            case Define.BodyPart.LeftForeArm:
                break;
            case Define.BodyPart.RightForeArm:
                break;
            case Define.BodyPart.LeftHand:
                actor.BodyHandler.LeftHand.PartInteractable.damageModifier = (InteractableObject.Damage)DamageType;
                break;
            case Define.BodyPart.RightHand:
                actor.BodyHandler.RightHand.PartInteractable.damageModifier = (InteractableObject.Damage)DamageType;
                break;
        }
    }


}