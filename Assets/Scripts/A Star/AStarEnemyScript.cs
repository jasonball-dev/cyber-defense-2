using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;
using CodeMonkey.Utils;
using Unity.Netcode;

public class AStarEnemyScript : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float stoppingDistance = 1f;
    [SerializeField] private float updateInterval = 2.0f;

    private List<Vector3> pathVectorList;
    private int currentPathIndex;
    private float pathUpdateTimer;

    private GameObject[] _player = new GameObject[2];

    private Pathfinding pathfinding;

    void Start()
    {
        if (!NetworkManager.Singleton.IsServer) return;
        _player = GameObject.FindGameObjectsWithTag("Player");

        pathUpdateTimer = updateInterval;

        pathfinding = new Pathfinding(17, 17, new Vector3(-70, 0, -70));
    }

    void Update()
    {
        if (!NetworkManager.Singleton.IsServer) return;

        pathUpdateTimer -= Time.deltaTime;

        if (pathUpdateTimer <= 0f)
        {
            foreach (var player in _player)
            {
                Debug.Log("playername in enemy: " + player.name);
            }

            // TODO
            // performance - recycle path found
            // check if player is alive  
            // use -> _player[0].GameObject().GetComponent<PlayerActions>().IsPlayerAlive();
            var player1 = _player[0] != null ? SetTargetPosition(_player[0].transform.position) : Int32.MaxValue;
            var player2 = _player.Length == 2 && _player[1] != null
                ? SetTargetPosition(_player[1].transform.position)
                : Int32.MaxValue;
            if ((player1 < player2 && _player[0].GameObject().GetComponent<PlayerActions>().IsPlayerAlive()) || !_player[1].GameObject().GetComponent<PlayerActions>().IsPlayerAlive())
            {
                SetTargetPosition(_player[0].transform.position);
            }
            else
            {
                SetTargetPosition(_player[1].transform.position);
            }

            pathUpdateTimer = updateInterval;
        }

        HandleMovement();
    }

    private int SetTargetPosition(Vector3 targetPosition)
    {
        currentPathIndex = 0;
        //Debug.Log("Transform: " + new Vector3(x, 0, z).ToString() + ", Target: " + targetPosition.ToString());
        //Debug.Log("Transform: " + transform.position.ToString() + ", Target: " + targetPosition.ToString());
        pathVectorList = pathfinding.FindPath(transform.position, targetPosition);

        //Debug.Log("pathVectorList has been set: 1" + pathVectorList[0].ToString());
        //Debug.Log("pathVectorList has been set: 2" + pathVectorList[1].ToString());
        //Debug.Log("pathVectorList has been set: 3" + pathVectorList[2].ToString());
        //Debug.Log("pathVectorList has been set: Final" + pathVectorList[pathVectorList.Count - 1].ToString());

        if (pathVectorList != null && pathVectorList.Count > 1)
        {
            pathVectorList.RemoveAt(0);
            return pathVectorList!.Count;
        }

        return Int32.MaxValue;
    }

    private void HandleMovement()
    {
        Debug.Log("now setting target");
        if (pathVectorList != null)
        {
            Vector3 targetPosition = pathVectorList[currentPathIndex];
            //Debug.Log("current target:" + targetPosition.ToString());

            if (Vector3.Distance(transform.position, targetPosition) > stoppingDistance)
            {
                Vector3 moveDir = (targetPosition - transform.position).normalized;

                // Set the rotation to look towards the target position
                if (moveDir != Vector3.zero) // Prevent rotation errors when moveDir is zero
                {
                    Quaternion targetRotation = Quaternion.LookRotation(moveDir);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
                }

                transform.position += moveDir * speed * Time.deltaTime;
            }
            else
            {
                currentPathIndex++;
                if (currentPathIndex >= pathVectorList.Count)
                {
                    pathVectorList = null;
                }
            }
        }
    }
}