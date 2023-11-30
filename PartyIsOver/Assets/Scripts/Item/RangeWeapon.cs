using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeWeapon : Item
{
    public override void Use()
    {
        Fire();
    }

    void Fire()
    {
        //��������� �����ؾ���
        ProjectileBase projectile =Instantiate(ItemData.Projectile,Owner.Grab.RangeWeaponSkin.position, Quaternion.LookRotation(-Owner.BodyHandler.Chest.PartTransform.up +new Vector3(0f,0.37f,0f)));
        projectile.Shoot(this);
        Owner.PlayerController.PlayerEffectSound("PlayerEffect/Cartoon-UI-040");
    }
    void FireFix()
    {
        //��������� �����ؾ���
        //ProjectileBase projectile = Managers.Resource.PhotonNetworkInstantiate(ItemData.Projectile, Owner.Grab.RangeWeaponSkin.position, Quaternion.LookRotation(-Owner.BodyHandler.Chest.PartTransform.up + new Vector3(0f, 0.37f, 0f)));
        //projectile.Shoot(this);
        //Owner.PlayerController.PlayerEffectSound("PlayerEffect/Cartoon-UI-040");
    }

}
