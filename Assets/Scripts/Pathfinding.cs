using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Pathfinding : MonoBehaviour
{
    public Astronaut astronaut; 
    public Transform target;
    public LineRenderer pathRenderer;

    private Grid grid;
    private List<Node> currentPath;

    void Awake()
    {
        grid = GetComponent<Grid>();
        pathRenderer.positionCount = 0;
    }

    void Update()
    {
        Vector3 startPos = new Vector3(
                (float)astronaut.current.posX,
                (float)astronaut.current.posY,
                (float)astronaut.current.posZ
            );
            
            FindPath(startPos, target.position);
    }

    public void FindPath(Vector3 startPos, Vector3 targetPos)
    {
        Node startNode = grid.NodeFromWorldPoint(startPos);
        Node targetNode = grid.NodeFromWorldPoint(targetPos);

        if (startNode == null || targetNode == null || targetNode.bIsWall)
        {
            Debug.LogError("Invalid start or target position");
            pathRenderer.positionCount = 0; // Clear path visualization
            return;
        }

        Heap<Node> openSet = new Heap<Node>(grid.MaxSize);
        HashSet<Node> closedSet = new HashSet<Node>();
        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            Node currentNode = openSet.RemoveFirst();
            closedSet.Add(currentNode);

            if (currentNode == targetNode)
            {
                currentPath = RetracePath(startNode, targetNode);
                UpdatePathVisualization();
                return; 
            }

            foreach (Node neighbor in grid.GetNeighboringNodes(currentNode))
            {
                if (neighbor.bIsWall || closedSet.Contains(neighbor))
                    continue;

                int newMovementCostToNeighbor = currentNode.gCost + GetDistance(currentNode, neighbor) + neighbor.movementPenalty;

                if (newMovementCostToNeighbor < neighbor.gCost || !openSet.Contains(neighbor))
                {
                    neighbor.gCost = newMovementCostToNeighbor;
                    neighbor.hCost = GetDistance(neighbor, targetNode);
                    neighbor.parent = currentNode;

                    if (!openSet.Contains(neighbor))
                        openSet.Add(neighbor);
                    else
                        openSet.UpdateItem(neighbor);
                }
            }
        }

        // If no path found, clear visualization
        pathRenderer.positionCount = 0;
    }

    List<Node> RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        path.Reverse();

        return path;
    }

    void UpdatePathVisualization()
    {
        pathRenderer.positionCount = currentPath.Count;
        Vector3[] pathPositions = new Vector3[currentPath.Count];

        for (int i = 0; i < currentPath.Count; i++)
        {
            pathPositions[i] = currentPath[i].worldPosition + Vector3.up * 0.5f;
        }
        pathRenderer.SetPositions(pathPositions);
    }

    int GetDistance(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.iGridX - nodeB.iGridX);
        int dstY = Mathf.Abs(nodeA.iGridY - nodeB.iGridY);

        return (dstX > dstY) ?
            14 * dstY + 10 * (dstX - dstY) :
            14 * dstX + 10 * (dstY - dstX);
    }
}
