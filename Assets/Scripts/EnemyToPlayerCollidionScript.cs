using UnityEngine;

public class EnemyCollidionScript : MonoBehaviour
{
    private void OnTriggerEnter(Collider externalCollider)
    {
        if (!externalCollider.gameObject.CompareTag("Player") ||
            !externalCollider.gameObject.GetComponent<PlayerActions>().IsLocalPlayer) return;
        print("enemy hat hit player");
        GameManager.Instance.Health--;
    }
}