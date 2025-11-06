using UnityEngine;

public class CamFollow : MonoBehaviour
{
    public Transform playerTransform;

    void Update()
    {
        if (playerTransform == null) return;
        transform.position = playerTransform.position + new Vector3(0, 2.6f, -0.2f);
        transform.rotation = playerTransform.rotation;
    }

    // Player sets target at the beginning
    public void SetTarget(Transform target)
    {
        playerTransform = target;
    }
}