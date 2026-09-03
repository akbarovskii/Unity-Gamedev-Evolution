using UnityEngine;

public class FixedPointInFront : MonoBehaviour
{
   
    [SerializeField] private Transform targetObject;

    [Header("Position")]
    [SerializeField] private float fixedX = 0f; 
    [SerializeField] private float fixedY = 0f; 
    [SerializeField] private float distanceAhead = 10f;

    private void LateUpdate()
    {
        if (targetObject == null) return;

       
        Vector3 newPosition = new Vector3(
            fixedX,                           
            fixedY,                          
            targetObject.position.z + distanceAhead 
        );

        transform.position = newPosition;
    }


    private void OnDrawGizmos()
    {
        if (targetObject != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, 0.3f);

            Gizmos.color = Color.green;
            Vector3 targetPos = new Vector3(fixedX, fixedY, targetObject.position.z + distanceAhead);
            Gizmos.DrawLine(targetObject.position, targetPos);
        }
    }
}
