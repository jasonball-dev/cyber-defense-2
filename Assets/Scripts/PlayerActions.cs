using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerActions : NetworkBehaviour
{
    [SerializeField] private GameObject projectile;
    [SerializeField] private AudioClip shootSound;
    private Rigidbody _playerRigidbody;
    private AudioSource _audioSource;
    private bool _initInNewScene = false;

    private readonly NetworkVariable<Vector3> _networkPosition = new(
        default,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner
    );

    private readonly NetworkVariable<Quaternion> _networkRotation = new NetworkVariable<Quaternion>(
        default,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner
    );

    private readonly NetworkVariable<bool> _isReady = new NetworkVariable<bool>(
        false,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner
    );

    private readonly NetworkVariable<bool> _playerAlive = new NetworkVariable<bool>(
        true,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner
    );

    private bool _playerDeadAndProcessesDeathProcessed = false;
    private Vector3 _movement;

    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (IsLocalPlayer)
        {
            if (!_initInNewScene && SceneManager.GetActiveScene().buildIndex == 1)
            {
                _initInNewScene = true;

                MetaGameManager.Instance.LocalPlayerGameObject = gameObject;

                Debug.Log($"player instantiated: IsOwner: {IsOwner}, IsServer: {IsServer}");

                // todo
                // fix spawn points 
                var spawnPointMarkerObject =
                    GameObject.Find(NetworkManager.IsServer ? "SPAWNPOINT1" : "SPAWNPOINT2");

                var pos = spawnPointMarkerObject.transform.position;
                pos.y = 18.5f;

                transform.position = pos;

                Camera.main!.GetComponent<CamFollow>().SetTarget(gameObject.transform);

                _playerRigidbody = GetComponent<Rigidbody>();

                _playerRigidbody.freezeRotation = true;
                _playerRigidbody.isKinematic = false;
                _playerRigidbody.interpolation = RigidbodyInterpolation.None;

                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;

                _isReady.Value = true;
            }

            _networkPosition.Value = transform.position;

            if (Input.GetKeyDown(KeyCode.Space) && IsPlayerAlive())
            {
                ShootProjectileOnLocalInstance();
                SpawnProjectileClientRpc();
            }
        }
        else
        {
            transform.position = _networkPosition.Value;
            transform.rotation = _networkRotation.Value;
        }


        if (_isReady.Value && IsOwner && IsLocalPlayer)
        {
            if (_playerAlive.Value)
            {
                var moveForward = Input.GetAxis("Vertical");
                var strafe = Input.GetAxis("Horizontal");
                _movement = (transform.forward * moveForward + transform.right * strafe) * 0.6f;
            }

            var mouseX = Input.GetAxis("Mouse X");
            var rotation = mouseX * 5;

            transform.Rotate(0, rotation, 0);
            if (IsOwner) _networkRotation.Value = transform.rotation;
        }

        if (Input.GetKeyDown(KeyCode.K) && IsServer)
        {
            foreach (var enemy in GameObject.FindGameObjectsWithTag("Enemy"))
            {
                Destroy(enemy);
            }
        }
    }

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            _networkPosition.Value = transform.position;
        }
    }

    void FixedUpdate()
    {
        if (_isReady.Value && IsOwner && IsLocalPlayer && _playerAlive.Value)
            _playerRigidbody.MovePosition(_playerRigidbody.position + _movement);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!_isReady.Value || !IsLocalPlayer) return;

        if (collision.gameObject.CompareTag("Enemy"))
        {
            print("enemy has hit player");
            GameManager.Instance.Health--;
        }

        _playerRigidbody.linearVelocity = Vector3.zero;
        _playerRigidbody.angularVelocity = Vector3.zero;
    }

    public void PlayerDeath()
    {
        if (!_isReady.Value || !IsLocalPlayer || !_playerAlive.Value) return;
        _playerAlive.Value = false;

        // SetTransparency(gameObject, (float)0.5); <- does not work

        //todo 
        //set lower alpha chanel 
    }

    public bool IsPlayerAlive()
    {
        return _playerAlive.Value;
    }


    private void ShootProjectileOnLocalInstance()
    {
        // todo
        // reactivate sound
        _audioSource.PlayOneShot(shootSound);

        var positionForProjectile = new Vector3(this.transform.position.x, this.transform.position.y + 1,
            this.transform.position.z);

        var localProjective = Instantiate(this.projectile, positionForProjectile,
            this.transform.rotation * Quaternion.Euler(0, -90, 0));
        localProjective.tag = "bullet";

        // ignore local player
        Physics.IgnoreCollision(localProjective.GetComponent<Collider>(), gameObject.GetComponent<Collider>());

        localProjective.GetComponent<Rigidbody>().AddRelativeForce(new Vector3
            (900f, 0, 0));
    }

    void SetTransparency(GameObject obj, float alpha)
    {
        print("set transparency");

        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();

        foreach (Renderer rend in renderers)
        {
            if (rend.material != null)
            {
                Material mat = rend.material;
                Color color = mat.color;
                color.a = alpha;
                mat.color = color;

                // Configure the shader for transparency
                mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                mat.SetInt("_ZWrite", 0);
                mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
            }
        }
    }


    [Rpc(SendTo.NotOwner)]
    private void SpawnProjectileClientRpc()
    {
        ShootProjectileOnLocalInstance();
    }
}