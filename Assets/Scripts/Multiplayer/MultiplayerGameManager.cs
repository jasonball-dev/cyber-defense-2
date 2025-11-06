using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Multiplayer
{
    public class MultiplayerGameManager : Singleton<MultiplayerGameManager>
    {
        [SerializeField] private string environment = "production";
        [SerializeField] private int maxNumberOfConnections = 2;
        [SerializeField] private string gameSceneName = "Cyber Defense";
        public string PlayerName { get; set; }

        private string _lobbyId;
        private Action _startButtonCallback;

        // todo 
        // make join process more reliable

        private UnityTransport Transport => NetworkManager.Singleton.gameObject.GetComponent<UnityTransport>();

        private async Task<string> InitializeUnityGameServices()
        {
            print("playername:" + PlayerName);
            if (UnityServices.State != ServicesInitializationState.Uninitialized) return "already initialized";

            Debug.Log("[MultiplayerGameManager] Start init of GameServices");

            var options = new InitializationOptions()
                .SetEnvironmentName(environment);

            options.SetProfile(PlayerName);

            await UnityServices.InitializeAsync(options);

            if (!AuthenticationService.Instance.IsSignedIn)
                await AuthenticationService.Instance.SignInAnonymouslyAsync();

            return "initialized";
        }

        public async Task<LobbyHostData> OpenLobbyNetwork(string lobbyName, Action startButtonCallback)
        {
            _startButtonCallback = startButtonCallback;
            await InitializeUnityGameServices();

            var optionsLobby = new CreateLobbyOptions
            {
                IsPrivate = false,
            };

            // todo
            // clean up lobby name & check if name already in use 
            var lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, 2, optionsLobby);

            Debug.Log($"[MultiplayerGameManager] Lobby created with ID: {lobby.Id}");

            var callbacks = new LobbyEventCallbacks();
            callbacks.LobbyChanged += OnLobbyChanged;

            await LobbyService.Instance.SubscribeToLobbyEventsAsync(lobby.Id, callbacks);

            StartCoroutine(HeartbeatLobbyCoroutine());

            _lobbyId = lobby.Id;
            var lobbyHostData = new LobbyHostData
            {
                Name = lobby.Name,
                Id = lobby.Id,
            };
            return lobbyHostData;
        }

        public async Task<string> JoinSpecificLobbyNetwork(string id)
        {
            Debug.Log($"[MultiplayerGameManager] Try to join lobby with id {id}");
            try
            {
                await LobbyService.Instance.JoinLobbyByIdAsync(id);

                //if needed to further update lobby data, then implement it here

                var callbacks = new LobbyEventCallbacks();
                callbacks.LobbyChanged += OnLobbyChanged;

                await LobbyService.Instance.SubscribeToLobbyEventsAsync(id, callbacks);

                return "subscribed";
            }
            catch (LobbyServiceException e)
            {
                Debug.Log($"$\"[MultiplayerGameManager]  Exception in join: {e.Message}");
                return "failed";
            }
        }

        public async Task<List<LobbyHostData>> RefreshLobbyListNetwork()
        {
            await InitializeUnityGameServices();

            var options = new QueryLobbiesOptions
            {
                Filters = new List<QueryFilter>
                {
                    new(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT)
                }
            };

            var response = await LobbyService.Instance.QueryLobbiesAsync(options);

            var listOfLobbiesForUi = new List<LobbyHostData>();
            foreach (var lobby in response.Results)
            {
                Debug.Log(
                    $"[MultiplayerGameManager] Lobby: {lobby.Name}, Players: {lobby.Players.Count}/{lobby.MaxPlayers}");
                listOfLobbiesForUi.Add(new LobbyHostData { Name = lobby.Name, Id = lobby.Id });
            }

            return listOfLobbiesForUi;
        }

        private async void OnLobbyChanged(ILobbyChanges changes)
        {
            try
            {
                if (changes.Data.Changed)
                {
                    Debug.Log("[MultiplayerGameManager] new data for lobby arrived");
                    var changesInLobbies = changes.Data.Value;

                    foreach (
                        var joinCode in
                        changesInLobbies.Select(dataBlock => dataBlock.Value.Value.Value))
                    {
                        Debug.Log($"[MultiplayerGameManager] join code {joinCode} just arrived");

                        await JoinRelay(joinCode);
                    }
                }

                // exact one player is allowed and needed so this logic is fine for now
                if (changes.PlayerJoined.Changed)
                {
                    print("new player joined");
                    _startButtonCallback.Invoke();
                }
            }
            catch (Exception e)
            {
                Debug.Log($"[MultiplayerGameManager] Exception in Lobby changes: {e.Message}");
            }
        }

        public async Task<RelayHostData> StartGameAfterClientsJoinedNetwork()
        {
            Debug.Log(
                $"[MultiplayerGameManager] Relay server starting with max connections: {maxNumberOfConnections}");

            var allocation = await Relay.Instance.CreateAllocationAsync(maxNumberOfConnections);

            var relayHostData = new RelayHostData
            {
                Key = allocation.Key,
                Port = (ushort)allocation.RelayServer.Port,
                AllocationID = allocation.AllocationId,
                AllocationIDBytes = allocation.AllocationIdBytes,
                IPv4Address = allocation.RelayServer.IpV4,
                ConnectionData = allocation.ConnectionData
            };

            relayHostData.JoinCode = await Relay.Instance.GetJoinCodeAsync(relayHostData.AllocationID);

            Transport.SetRelayServerData(
                relayHostData.IPv4Address,
                relayHostData.Port,
                relayHostData.AllocationIDBytes,
                relayHostData.Key,
                relayHostData.ConnectionData
            );

            MetaGameManager.Instance.SetGameJoinCode(relayHostData.JoinCode);

            var sendData = new Dictionary<string, DataObject>
            {
                {
                    "RelayJoinCode", new DataObject(DataObject.VisibilityOptions.Public, relayHostData.JoinCode)
                }
            };

            await LobbyService.Instance.UpdateLobbyAsync(_lobbyId,
                new UpdateLobbyOptions { Data = sendData });

            Debug.Log($"[MultiplayerGameManager] new relay generated join code {relayHostData.JoinCode}");

            NetworkManager.Singleton.StartHost();

            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneManager_OnLoadEventCompleted;
            Debug.Log("LOADING SCENE");
            NetworkManager.Singleton.SceneManager.LoadScene(gameSceneName, LoadSceneMode.Single);

            return relayHostData;
        }

        private async Task<RelayJoinData> JoinRelay(string joinCode)
        {
            Debug.Log($"[MultiplayerGameManager] Client should join game with join code {joinCode}");

            var allocation = await Relay.Instance.JoinAllocationAsync(joinCode);

            var relayJoinData = new RelayJoinData
            {
                Key = allocation.Key,
                Port = (ushort)allocation.RelayServer.Port,
                AllocationID = allocation.AllocationId,
                AllocationIDBytes = allocation.AllocationIdBytes,
                ConnectionData = allocation.ConnectionData,
                HostConnectionData = allocation.HostConnectionData,
                IPv4Address = allocation.RelayServer.IpV4,
                JoinCode = joinCode
            };

            Transport.SetRelayServerData(relayJoinData.IPv4Address, relayJoinData.Port, relayJoinData.AllocationIDBytes,
                relayJoinData.Key, relayJoinData.ConnectionData, relayJoinData.HostConnectionData);

            Debug.Log($"[MultiplayerGameManager] Client joined relay successfully");

            NetworkManager.Singleton.StartClient();
            //NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneManager_OnLoadEventCompleted;

            return relayJoinData;
        }

        private void SceneManager_OnLoadEventCompleted(
            string sceneName,
            LoadSceneMode loadSceneMode,
            List<ulong> clientsCompleted,
            List<ulong> clientsTimedOut
        )
        {
            Debug.Log(
                $"[MultiplayerGameManager] Scene {sceneName} loaded. Completed clients: {string.Join(", ", clientsCompleted)}");

            if (clientsTimedOut.Count > 0)
            {
                Debug.LogWarning($"Some clients timed out during scene load: {string.Join(", ", clientsTimedOut)}");
            }

            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted -= SceneManager_OnLoadEventCompleted;
        }

        private void OnDisable()
        {
            if (NetworkManager.Singleton != null)
            {
                NetworkManager.Singleton.SceneManager.OnLoadEventCompleted -= SceneManager_OnLoadEventCompleted;
            }
        }

        private IEnumerator HeartbeatLobbyCoroutine()
        {
            var delay = new WaitForSecondsRealtime(20);
            while (true)
            {
                LobbyService.Instance.SendHeartbeatPingAsync(_lobbyId);
                yield return delay;
            }
        }

        private void SafeLocalPlayerObject()
        {
            var clientId = NetworkManager.Singleton.LocalClientId;
            var playerObject = NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(clientId);

            if (playerObject != null)
            {
                if (playerObject.gameObject.scene.name != "DontDestroyOnLoad")
                {
                    DontDestroyOnLoad(playerObject);
                }

                Debug.Log($"[MultiplayerGameManager] Local Player Object: {playerObject.name} preserved");
            }
            else
            {
                Debug.LogWarning("[MultiplayerGameManager] Player object not found.");
            }
        }
    }
}