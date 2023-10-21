using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionHandeler : MonoBehaviour
{
    public float damageMinimumVelocity = 0.25f;

    public Actor actor;

    private Transform rootTransform;


    // Start is called before the first frame update
    void Start()
    {
        if (actor == null)
        {
            rootTransform = transform.root;
            actor = rootTransform.GetComponent<Actor>();
        }
        else
        {
            rootTransform = actor.transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void DamageCheck(Collision collision)
    {
        InteractableObject collisionInteractable = collision.transform.GetComponent<InteractableObject>();
        Transform collisionTransform = collision.transform;
        Rigidbody collisionRigidbody = collision.rigidbody;
        Collider collisionCollider = collision.collider;
        Vector3 relativeVelocity = collision.relativeVelocity;
        float velocityMagnitude = relativeVelocity.magnitude;

        


        float num = 0f;
        float num2 = 0f;
        for (int i = 0; i < collision.contactCount; i++)
        {
            ContactPoint contact = collision.GetContact(i);

            //�浹�� �༮�� rigidbody�� ������ �� ��ü�� mass�� �ְ� ������ ����Ʈ�� 40�� ����
            num = ((!collisionRigidbody) ? 40f : collisionRigidbody.mass);

            //�浹������ ��ֺ��Ͷ� relativeVelocity(�浹�� �� ��ü���� ������� �̵��ӵ�)�� ���ϰ� ��ȯ
            num2 = Vector3.Dot(contact.normal, relativeVelocity) * Mathf.Clamp(num, 0f, 40f) / 1100f;

            //������ ����� �ٲ�
            if (num2 < 0f)
            {
                num2 = 0f - num2;
            }


            //������ ����
            num2 = Mathf.RoundToInt(num2);
            if (num2 > 0f && velocityMagnitude > damageMinimumVelocity)
            {
                if (collisionInteractable != null)
                {
                    actor.statusHandeler.AddDamage(collisionInteractable.damageModifier, num2, collisionCollider.gameObject);
                }
                else
                {
                    actor.statusHandeler.AddDamage(InteractableObject.Damage.Default, num2, collisionCollider.gameObject);
                }
            }
        }

}
