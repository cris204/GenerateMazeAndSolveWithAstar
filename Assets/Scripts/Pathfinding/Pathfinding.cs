using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{

    Grid grid;

    private void Awake()
    {
        grid = GetComponent<Grid>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)) {
            FindPath(grid.playerPoint.position, grid.finalPoint.position);
        }
    }

    private void FindPath(Vector3 startPosition, Vector3 targetPosition)
    {
        Node startNode = grid.GetNodeFromWorldPoint(startPosition);
        Node finalNode = grid.GetNodeFromWorldPoint(targetPosition);

        List<Node> openSet = new List<Node>();
        HashSet<Node> closeSet = new HashSet<Node>();

        openSet.Add(startNode);

        while (openSet.Count > 0) {

            Node currentNode = openSet[0];
            for (int i = 1; i < openSet.Count; i++) {
                if(openSet[i].fCost < currentNode.fCost ||
                    openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost) {

                    currentNode = openSet[i];
                }
            }

            openSet.Remove(currentNode);
            closeSet.Add(currentNode);

            if(currentNode == finalNode) {
                RetracePath(startNode, finalNode);
                return;
            }

            foreach(Node neighbour in grid.GetNeighbours(currentNode)) {
                if(!neighbour.walkable || closeSet.Contains(neighbour)) {
                    continue;
                }

                int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);

                if(newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour)) {
                    neighbour.gCost = newMovementCostToNeighbour;
                    neighbour.hCost = GetDistance(neighbour, finalNode);
                    neighbour.parent = currentNode;
                    
                    if (!openSet.Contains(neighbour)) {
                        openSet.Add(neighbour);
                    }
                }

            }
        }
    }

    private void RetracePath(Node startNode, Node finalNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = finalNode;

        while(currentNode != startNode) {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }

        path.Reverse();

        grid.path = path;

    }

    public int GetDistance(Node nodeA, Node nodeB)
    {
        int distanceX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int distanceY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        if(distanceX > distanceY) {
            return 14 * distanceY + 10*(distanceX - distanceY);
        }
        return 14 * distanceX + 10*(distanceY - distanceX);

    }
}