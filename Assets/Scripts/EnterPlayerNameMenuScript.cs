using Multiplayer;
using TMPro;
using UnityEngine;

public class EnterPlayerNameMenuScript : MonoBehaviour
{
    [SerializeField] private TMP_InputField playerInputField;

    public void OnClickNext()
    {
        var playerName = playerInputField.text;
        if (playerName.Length == 0)
        {
            playerName = "Player-" + UnityEngine.Random.Range(1000, 100000);
        }

        MultiplayerGameManager.Instance.PlayerName = playerName;
        MetaGameManager.Instance.LocalPlayerProfileName = playerName;
    }
}