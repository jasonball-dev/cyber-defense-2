using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;
using CodeMonkey;

public class TestingAStar : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private CharacterPathfindingMovementHandler characterPathfinding;
    private Pathfinding pathfinding;
    //private Grid<PathNode> grid; // test
    public Transform playerT;

    private void Start() {
        if (playerT == null) //test
        {
            playerT = GameObject.FindWithTag("Player").transform;
        }

        //pathfinding = new Pathfinding(17, 17, new Vector3(-70, 0, -70));
        //grid = new Grid<PathNode>(17, 17, 10f, new Vector3(-70, 0, -70), (Grid<PathNode> g, int x, int z) => new PathNode(g, x, z)); // test
    }

    public void Update() {
        if (Input.GetMouseButtonDown(0)) {
            //pathfinding.GetGrid().GetXZ(playerT.position, out int x, out int z);
            pathfinding.GetGrid().GetXZ(new Vector3(35, 21, -60), out int startX, out int startZ);
            pathfinding.GetGrid().GetXZ(new Vector3(35, 0, -17), out int targX, out int targZ);
            //List<PathNode> path = pathfinding.FindPath(10, 0, x, z); 
            List<PathNode> path = pathfinding.FindPath(startX, startZ, targX, targZ); 
            if (path != null) {
                for (int i = 0; i < path.Count - 1; i++) {
                    Debug.DrawLine(new Vector3(path[i].x - 7, 0, path[i].z - 7) * 10f + Vector3.one * 5f, new Vector3(path[i+1].x - 7, 0, path[i+1].z - 7) * 10f + Vector3.one * 5f, Color.green, 5f);
                }
            }

            List<Vector3> vectorPath = new List<Vector3>();
            foreach (PathNode pathNode in path) {
                vectorPath.Add(new Vector3(pathNode.x, 0, pathNode.z) * pathfinding.grid.GetCellSize() + Vector3.one * pathfinding.grid.GetCellSize() * .5f);
            }
            Debug.Log("vectorPath has been set: 1" + vectorPath[0].ToString());
            Debug.Log("vectorPath has been set: 2" + vectorPath[1].ToString());
            Debug.Log("vectorPath has been set: 3" + vectorPath[2].ToString());
            Debug.Log("vectorPath has been set: Final" + vectorPath[vectorPath.Count - 1].ToString());

            //characterPathfinding.SetTargetPosition(player.transform.position);
        }
    }

    /* private void Update() {
        //Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        //if (Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, layerMask)) {
            if (Input.GetMouseButtonDown(0)) {
                // grid.SetGridObject(raycastHit.point, new PathNode(grid, 99, 99)); // test
                pathfinding.GetGrid().GetXZ(player.transform.position, out int x, out int z);
                List<PathNode> path = pathfinding.FindPath(0, 0, x, z);
                if (path != null) {
                    for (int i = 0; i < path.Count - 1; i++) {
                        Debug.DrawLine(new Vector3(path[i].x, 0, path[i].z) * 10f + Vector3.one * 5f, new Vector3(path[i+1].x, 0, path[i+1].z) * 10f + Vector3.one * 5f, Color.green, 5f);
                    }
                }
                characterPathfinding.SetTargetPosition(player.transform.position);
            }
        //}
    } */

    // Old testing, dont delete
    /*private Grid<bool> grid;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private LayerMask layerMask;

    private void Start() {
        grid = new Grid<bool>(20, 10, 8f, Vector3.zero);
    }

    private void Update() {

    }
    */

    /*
    private void Start() {
        grid = new Grid(15, 15, 10f, new Vector3(-100, 0));
    }

    private void Update() {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, layerMask)) {
            if (Input.GetMouseButtonDown(0)) {
                grid.SetValue(raycastHit.point, 56);
            }
        }
    }
    */
}
