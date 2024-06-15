using UnityEngine;

public class FollowMesh : MonoBehaviour
{
    // The target mesh to follow
    public Transform targetMesh;

    // Distance to maintain from the target
    public float followDistance = 5.0f;

    // Speed at which the object follows the target
    public float followSpeed = 5.0f;

    void Update()
    {
        if (targetMesh == null)
        {
            Debug.LogWarning("Target mesh not assigned.");
            return;
        }

        // Calculate the desired position based on the target position and follow distance
        Vector3 direction = (transform.position - targetMesh.position).normalized;
        Vector3 desiredPosition = targetMesh.position + direction * followDistance;

        // Move towards the desired position smoothly
        transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);
    }
}
