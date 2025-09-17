using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed = 5f;

    void LateUpdate()
    {
        // Try to find Pokki dynamically if not yet assigned
        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                target = player.transform;
            }
        }

        // Follow logic
        if (target != null)
        {
            Vector3 targetPosition = new Vector3(target.position.x, target.position.y, -10f);
            transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);
        }
    }
}
