using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using Unity.Netcode;
using UnityEngine;
using Debug = UnityEngine.Debug;


public class WaveManagerMultiplayer : NetworkBehaviour
{
    [SerializeField] private WaveNotification notifier;

    private int _currentWaveIndex;
    private int _countLocallyKilledEnemies;
    private int _countProjectilesFired;
    private bool _gameRunning = true;

    private readonly List<SpawnerScript> _spawner = new();

    static WaveManagerMultiplayer()
    {
        var rng = new System.Random();
    }

    private void Start()
    {
        DontDestroyOnLoad(this);
        Debug.Log("[WaveManagerMultiplayer] Stared");
        if (IsServer) StartCoroutine(StartWaveExecution());
    }

    private IEnumerator StartWaveExecution()
    {
        for (var i = 15; i > 0; i--)
        {
            if (i % 2 == 0) BroadcastMessageToClientsClientRpc("Start in " + i, 1);
            yield return new WaitForSeconds(1f);
        }

        BroadcastMessageToClientsClientRpc("Let's go", 4);

        while (_gameRunning)
        {
            var wave = Wave.GenerateWave();
            if (_currentWaveIndex != 0)
            {
                BroadcastMessageToClientsClientRpc($"Let's move on", 2);
                yield return new WaitForSeconds(5);
            }

            BroadcastMessageToClientsClientRpc($"Wave {_currentWaveIndex + 1} started", 2);
            _currentWaveIndex++;
            yield return ExecuteWave(wave);
            yield return new WaitForSeconds(2);
            BroadcastMessageToClientsClientRpc($"Wave finished", 2);
            yield return new WaitForSeconds(5);
            ;
            // todo
            // check if still at least one player is alive (idk if case if last player dies on last enemy and this one aswell is relevant)
        }
    }

    private IEnumerator ExecuteWave(Wave wave)
    {
        var randomizedSpawners = new List<SpawnerScript>(_spawner);
        Utils.ShuffleList(randomizedSpawners);

        for (var i = 0; i < wave.spawnerCount && i < randomizedSpawners.Count; i++)
        {
            randomizedSpawners[i].SpawnRate = wave.spawnRate;
            randomizedSpawners[i].Activated = true;
        }

        yield return new WaitForSeconds(wave.lengthOfWave);

        foreach (var spawner in _spawner)
        {
            spawner.Activated = false;
        }

        print("wave waiting to kill last enemies ");

        yield return new WaitUntil(() => GameObject.FindGameObjectsWithTag("Enemy").Length == 0);

        print("wave over");

        yield return 1;
    }

    public void AddSpawner(SpawnerScript incSpawner)
    {
        _spawner.Add(incSpawner);
        Debug.Log("add spawner: " + _spawner.Count);
    }


    [ClientRpc]
    private void BroadcastMessageToClientsClientRpc(string message, int duration)
    {
        Debug.Log($"[CLIENT] Broadcast from server: {message}");
        notifier.ShowMessage(message, duration);
    }
}