using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class ProjectileScript : NetworkBehaviour
{
    public GameObject impactParticle;

    void Start()
    {
        Destroy(gameObject, 15f);
        GameManager.Instance.ProjectilesShoot++;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            print("enemy on trigger");
            GameManager.Instance.EnemiesKilled++;
            other.gameObject.GetComponent<EnemyDeathScript>().LocalDieTrigger();
        }

        Destroy(gameObject);
    }

    // world collisions
    void OnCollisionEnter(Collision collision)
    {
        print("on collision");
        if (collision.gameObject.GetComponent<AStarEnemyScript>() != null ||
            collision.gameObject.GetComponent<PlayerActions>() != null) return;

        Instantiate(impactParticle, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}