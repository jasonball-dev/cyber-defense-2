using Unity.Netcode;
using UnityEngine;

public class EnemyDeathScript : NetworkBehaviour
{
    public GameObject impactParticle;

    public void LocalDieTrigger()
    {
        if (!IsServer) return;
        TriggerDeathClientRpc();
    }

    [ClientRpc]
    private void TriggerDeathClientRpc()
    {
        Instantiate(impactParticle, transform.position, Quaternion.identity);

        foreach (var rendererLocal in GetComponentsInChildren<Renderer>())
        {
            rendererLocal.enabled = false;
        }

        foreach (var colliderLocal in GetComponentsInChildren<Collider>())
        {
            colliderLocal.enabled = false;
        }

        if (IsServer)
        {
            Destroy(gameObject, 1);
        }
    }
}