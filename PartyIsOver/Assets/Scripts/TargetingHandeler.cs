using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetingHandeler : MonoBehaviour
{
    public float detectionRadius = 3f;
    public LayerMask layerMask;
    public float maxAngle = 90f; // 180���� ���� (90��)���� ����

    Collider _nearestCollider;


    InteractableObject[] _interactableObjects = new InteractableObject[30];
    InteractableObject _nearestObject;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public Collider SearchTarget()
    {
        Collider[] colliders = new Collider[30];
        _nearestCollider = null;
        _nearestObject = null;

        // ĳ���Ͱ� ���� �ٶ󺸴� ���� ����
        Vector3 detectionDirection = transform.forward;

        // �� �ȿ� �ݶ��̴� ����
        int colliderCount = Physics.OverlapSphereNonAlloc(transform.position, detectionRadius, colliders, layerMask);


        if (colliderCount <= 0 )
        {
            return null;
        }



        // �ٶ󺸴� ���� 180�� �̳��� �ݶ��̴� �� interatableObject ���������� Ȯ��
        for (int i = 0; i < colliderCount; i++)
        {
            Vector3 toCollider = colliders[i].transform.position - transform.position;
            float angle = Vector3.Angle(detectionDirection, toCollider);

            if (angle <= maxAngle && colliders[i].GetComponentInChildren<InteractableObject>())
            {
                if (_nearestObject == null || toCollider.magnitude < (_nearestObject.transform.position - transform.position).magnitude)
                {

                    Debug.Log("Collider " + colliders[i].name);

                    _nearestCollider = colliders[i];


                }

            }
        }

        // �ش� �ݶ��̴��� ���� interatableObject ���� ����
        //_interactableObjects = _nearestCollider.GetComponentsInChildren<InteractableObject>();



        //���� ����� interatable ������Ʈ ã�� �����ʿ�





        return _nearestCollider;
    }

}
