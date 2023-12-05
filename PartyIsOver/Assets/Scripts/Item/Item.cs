using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using static Define;

public class Item : MonoBehaviourPun
{
    public Actor Owner;  
    public InteractableObject InteractableObject;
    public ItemData ItemData;


    public Transform OneHandedPos;
    public Transform TwoHandedPos;
    public Transform Head;
    public Transform Body;

    // Start is called before the first frame update
    void Start()
    {
        InteractableObject = GetComponent<InteractableObject>();
        InteractableObject.damageModifier = InteractableObject.Damage.Default;
        GetComponent<Rigidbody>().mass = 10f;


        Body = transform.GetChild(0);
        Head = transform.GetChild(1);
        OneHandedPos = transform.GetChild(2);
        if(transform.GetChild(3) != null && (ItemData.ItemType == ItemType.TwoHanded 
            || ItemData.ItemType == ItemType.Ranged || ItemData.ItemType == ItemType.Gravestone))
        {
            TwoHandedPos = transform.GetChild(3);
        }
    }

    // Update is called once per frame
    void Update()
    {
    
    }

    
    public virtual void Use()
    {
        //���ǻ��
        if (PhotonNetwork.LocalPlayer.IsMasterClient)
            Owner.StatusHandler.AddDamage(ItemData.UseDamageType, 0f, null);

        Destroy(gameObject,1f);
        //�վ ���鶩 ��ũ��Ʈ �ϳ� �� �İ� Item�� ��ӹ޾Ƽ� Use�� ���������ϴ� �Լ��� �������̵�
        //����� ���鶧 ItemData ��ũ��Ʈ���� Projectile�� �Ϲ� ���Ÿ������� ����ü�� ���� �� �� �ְ� �ϰų�
        //ItemData ��ũ��Ʈ���� Projectile�� ���� ���Ÿ��� ������� ���� ���Ӱ� �۾��ϴ� ������ ����
        
    }

    public void ThrowItem()
    {
        ProjectileBase projectile = Managers.Pool.Pop(ItemData.Projectile.gameObject).GetComponent<ProjectileBase>();

        Vector3 forward = -Owner.BodyHandler.Chest.PartTransform.up;
        projectile.transform.position = Owner.BodyHandler.RightHand.transform.position;

        
        projectile.transform.rotation = Quaternion.LookRotation(forward + new Vector3(0f, 0.37f, 0f));
        projectile.Shoot(this);
        Owner.PlayerController.PlayerEffectSound("PlayerEffect/Cartoon-UI-040");

        projectile.GetComponent<InteractableObject>().ChangeUseTypeTrigger(0.08f, -1f);

    }
}

