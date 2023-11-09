using UnityEngine;

public class TargetingHandler : MonoBehaviour
{
    private float detectionRadius = 1.2f;
    public LayerMask layerMask;
    public float maxAngle = 90f; // 180���� ���� (90��)���� ����

    Collider _nearestCollider;

    BodyHandler _bodyHandler;
    InteractableObject[] _interactableObjects = new InteractableObject[30];
    InteractableObject _nearestObject;

    // Start is called before the first frame update
    void Start()
    {
        _bodyHandler = GetComponent<BodyHandler>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public InteractableObject SearchTarget(Grab.Side side)
    {
        //���߿� ��������� ���� ȿ����
        Collider[] colliders = new Collider[40];
        _nearestCollider = null;
        _nearestObject = null;

        Transform chestTransform = _bodyHandler.Chest.transform;
        
        
        //���麤��
        Vector3 chestForward = -chestTransform.up;

        //üũ�� ���� ����
        Vector3 detectionDirection;
        if (side == Grab.Side.Left)
            detectionDirection = -chestTransform.right;
        else
            detectionDirection = chestTransform.right;



        // �� �ȿ� �ݶ��̴� ����
        int colliderCount = Physics.OverlapSphereNonAlloc(chestTransform.position, detectionRadius, colliders, layerMask);


        if (colliderCount <= 0 )
        {
            return null;
        }



        // �ٶ󺸴� ���� 180�� �̳��� �ݶ��̴� �� interatableObject ���������� Ȯ��
        for (int i = 0; i < colliderCount; i++)
        {
            Vector3 toCollider = colliders[i].transform.position - chestTransform.position;
            float angle = Vector3.Angle(chestForward, toCollider);
            float angle2 = Vector3.Angle(detectionDirection, toCollider);


            if (angle <= maxAngle && angle2 <= 110f && colliders[i].GetComponent<InteractableObject>())
            {
                if (_nearestObject == null || toCollider.magnitude < (_nearestObject.transform.position - chestTransform.position).magnitude)
                {
                    _nearestCollider = colliders[i];
                    _nearestObject = colliders[i].GetComponent<InteractableObject>();
                }

            }
        }

        
        if(_nearestCollider == null)
        {
            return null;
        }


        return _nearestObject;
    }

    public Vector3 FindClosestCollisionPoint(Collider collider)
    {
        if (collider == null)
        {
            Debug.Log("Ÿ�ٿ� �ݶ��̴��� ����");
            return Vector3.zero;
        }

        Vector3 start = _bodyHandler.Chest.transform.position; 
        Vector3 direction = (collider.transform.position - start).normalized;
        float distance = Vector3.Distance(start, collider.transform.position);

        RaycastHit hit;

        if (Physics.Raycast(start, direction, out hit, distance, layerMask))
        {
            return hit.point;
        }
        else
        {
            Debug.Log("Ÿ�ٿ� ������ ����");
            return Vector3.zero;
        }
    }
}
