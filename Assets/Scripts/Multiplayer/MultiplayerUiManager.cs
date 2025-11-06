using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Multiplayer
{
    public class MultiplayerAccessor : MonoBehaviour
    {
        [SerializeField] private TMP_InputField lobbyNameInputFieldField;
        [SerializeField] private GameObject lobbyItemPrefab;
        [SerializeField] private GameObject lobbyScrollContentParent;
        [SerializeField] private GameObject hostMenuUi;
        [SerializeField] private GameObject lobbyMenuUi;
        [SerializeField] private GameObject lobbyWaitingText;

        private readonly Dictionary<GameObject, LobbyUiData> _lobbyUiMapping = new();

        // Step 1 (On Host)
        public async void OpenLobby()
        {
            try
            {
                hostMenuUi.SetActive(false);

                var lobbyName = lobbyNameInputFieldField.text;

                if (lobbyName.Length == 0 || lobbyName == "Enter Lobbyname")
                {
                    lobbyName = "Lobby-" + UnityEngine.Random.Range(1000, 100000);
                }

                var startLobbyResult = await MultiplayerGameManager.Instance.OpenLobbyNetwork(
                    lobbyName,
                    ActivateStartButtonCallback);

                lobbyMenuUi.transform.Find("LobbyName").GetComponent<TextMeshProUGUI>().text = startLobbyResult.Name;
                lobbyMenuUi.transform.Find("StartButton").gameObject.SetActive(false);
                lobbyMenuUi.SetActive(true);
            }
            catch (Exception e)
            {
                Debug.Log($"[MultiplayerAccessor] Exception {e.Message}");
            }
        }

        // Step 2 (On Client)
        public async void RefreshLobbyList()
        {
            try
            {
                var lobbies = await MultiplayerGameManager.Instance.RefreshLobbyListNetwork();
                _lobbyUiMapping.Clear();

                foreach (var lobby in lobbies)
                {
                    var lobbyItem = GameObject.Instantiate(lobbyItemPrefab, lobbyScrollContentParent.transform);
                    lobbyItem.GetComponentInChildren<Button>().onClick.AddListener(JoinSpecificLobby);
                    lobbyItem.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = lobby.Name;

                    var data = new LobbyUiData { Id = lobby.Id, Name = lobby.Name };
                    _lobbyUiMapping.Add(lobbyItem, data);
                }
            }
            catch (Exception e)
            {
                Debug.Log($"[MultiplayerAccessor] Exception {e.Message}");
            }
        }

        // Step 3 (On Client) Join specific lobby
        private async void JoinSpecificLobby()
        {
            try
            {
                var clickedButton = EventSystem.current.currentSelectedGameObject;
                var prefabRoot = clickedButton.transform.parent.gameObject;
                var lobbyId = _lobbyUiMapping[prefabRoot].Id;

                hostMenuUi.SetActive(false);
                lobbyWaitingText.SetActive(true);
                await MultiplayerGameManager.Instance.JoinSpecificLobbyNetwork(lobbyId);
            }
            catch (Exception e)
            {
                Debug.Log($"[MultiplayerAccessor] Exception {e.Message}");
            }
        }

        // Step 4 (On Host) Start Game for everyone
        public async void StartGameAfterClientsJoined()
        {
            try
            {
                await MultiplayerGameManager.Instance.StartGameAfterClientsJoinedNetwork();
            }
            catch (Exception e)
            {
                Debug.Log($"[MultiplayerAccessor] Exception {e.Message}");
            }
        }

        private void ActivateStartButtonCallback()
        {
            lobbyMenuUi.transform.Find("StartButton").gameObject.SetActive(true);
        }

        private class LobbyUiData
        {
            public string Name;
            public string Id;
        }
    }
}