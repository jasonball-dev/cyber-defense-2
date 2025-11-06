using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class SpawnerScript : MonoBehaviour
{
    [SerializeField] private WaveManagerMultiplayer _waveManagerMultiplayer;
    public GameObject Enemy;

    // Vars for Spawner
    public bool Activated = true;
    public float SpawnRate = 2f;
    private float NextSpawnTime = 0f;

    private void Start()
    {
        _waveManagerMultiplayer.AddSpawner(gameObject.GetComponent<SpawnerScript>());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            SpawnEnemy();
        }

        if (Time.time >= NextSpawnTime && Activated)
        {
            SpawnEnemy();
            NextSpawnTime = Time.time + SpawnRate;
        }
    }

    private void SpawnEnemy()
    {
        var newEnemy = Instantiate(Enemy, gameObject.transform.position, Quaternion.identity);
        newEnemy.GetComponent<NetworkObject>().Spawn();
    }
}