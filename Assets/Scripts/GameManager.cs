using System.Collections;
using System.Collections.Generic;
using Multiplayer;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public GameObject gameOver, heart0, heart1, heart2, heart3, quitButton;

    private int _health = 3;

    public int Health
    {
        get => _health;
        set
        {
            _health = value;
            Debug.Log("set value: " + _health);

            if (_health == 0)
            {
                PlayerDeath();
            }
        }
    }

    public int EnemiesKilled { get; set; } = 0;
    public int ProjectilesShoot { get; set; } = 0;

    private WaveManagerMultiplayer _waveManagerMultiplayer;
    [SerializeField] private WaveNotification notifier;

    void Start()
    {
        Debug.Log("Game Manager started");
        Health = 4;
        heart0.gameObject.SetActive(true);
        heart1.gameObject.SetActive(true);
        heart2.gameObject.SetActive(true);
        heart3.gameObject.SetActive(true);
        gameOver.gameObject.SetActive(false);
        quitButton.gameObject.SetActive(false);
    }

    //todo 
    //alpha layer
    private void PlayerDeath()
    {
        print("local player death");
        StartCoroutine(PlayerDeathUi());

        var localPlayer = MetaGameManager.Instance.LocalPlayerGameObject;
        localPlayer.GetComponent<PlayerActions>().PlayerDeath();

        ScoreUpload.Instance.SendScore(
            MetaGameManager.Instance.LocalPlayerProfileName,
            EnemiesKilled
        );

        var quitButtonManager = GameObject.Find("QuitButtonDeathManager");
        var quitButtonDeathManagerScript = quitButtonManager.GetComponent<QuitButtonDeathManager>();
        quitButtonDeathManagerScript.SetLocalPlayerDead();
    }

    private IEnumerator PlayerDeathUi()
    {
        notifier.ShowMessage("You have died :(", 2);
        yield return new WaitForSeconds(2);
        notifier.ShowMessage($"Killed {EnemiesKilled} robots with {ProjectilesShoot} projectiles", 4);
        yield return new WaitForSeconds(4);
        notifier.ShowMessage($"Your character is frozen until the game is finished", 2);
        yield return new WaitForSeconds(2);
        notifier.ShowMessage($"After that you can quit.", 2);
        yield return new WaitForSeconds(2);
        notifier.ShowMessage("Thanks for playing!", 4);
        yield return new WaitForSeconds(4);
    }

    void Update()
    {
        switch (Health)
        {
            case 4:
                heart0.gameObject.SetActive(true);
                heart1.gameObject.SetActive(true);
                heart2.gameObject.SetActive(true);
                heart3.gameObject.SetActive(true);
                break;
            case 3:
                heart0.gameObject.SetActive(true);
                heart1.gameObject.SetActive(true);
                heart2.gameObject.SetActive(true);
                heart3.gameObject.SetActive(false);
                break;
            case 2:
                heart0.gameObject.SetActive(true);
                heart1.gameObject.SetActive(true);
                heart2.gameObject.SetActive(false);
                heart3.gameObject.SetActive(false);
                break;
            case 1:
                heart0.gameObject.SetActive(true);
                heart1.gameObject.SetActive(false);
                heart2.gameObject.SetActive(false);
                heart3.gameObject.SetActive(false);
                break;
            case 0:
                heart0.gameObject.SetActive(false);
                heart1.gameObject.SetActive(false);
                heart2.gameObject.SetActive(false);
                heart3.gameObject.SetActive(false);
                break;
            default:
                heart0.gameObject.SetActive(false);
                heart1.gameObject.SetActive(false);
                heart2.gameObject.SetActive(false);
                heart3.gameObject.SetActive(false);
                gameOver.gameObject.SetActive(true);
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                break;
        }
    }
}