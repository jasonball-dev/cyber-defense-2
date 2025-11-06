using System;
using Unity.Netcode;
using UnityEngine;

public class QuitButtonDeathManager : NetworkBehaviour
{
    [SerializeField] private GameObject gameQuitButton;
    private readonly NetworkVariable<int> _deadPlayerCount = new NetworkVariable<int>(0);


    // gets called if a client dies 
    public void SetLocalPlayerDead()
    {
        if (IsServer)
        {
            _deadPlayerCount.Value++;

            if (_deadPlayerCount.Value == 2)
            {
                ActivateQuitButtonClientRpc();
            }
        }
        else
        {
            NotifyDeathServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void NotifyDeathServerRpc()
    {
        _deadPlayerCount.Value++;

        if (_deadPlayerCount.Value == 2)
        {
            ActivateQuitButtonClientRpc();
        }
    }

    // should be called if both players are dead  
    [ClientRpc]
    private void ActivateQuitButtonClientRpc()
    {
        gameQuitButton.gameObject.SetActive(true);
    }
}