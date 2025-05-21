using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    public Astronaut astronaut; 
    // public Transform target;
    public LineRenderer pathRenderer;

    private Grid grid;
    private List<Node> currentPath;
    private Vector3 currentTargetPosition;

    void Awake()
    {
        grid = GetComponent<Grid>();
        // pathRenderer = GetComponent<LineRenderer>();
        // InitializeLineRenderer();
        pathRenderer.positionCount = 0;
    }

    public void SetTarget(Vector3 targetPosition)
    {
        currentTargetPosition = targetPosition;
        CalculatePath(currentTargetPosition);
    }

    public void CalculatePath(Vector3 targetWorldPosition)
    {
        Vector3 startPos = GetAstronautWorldPosition();
        Debug.Log($"Start position: {startPos}");
        FindPath(startPos, targetWorldPosition);
    }

    Vector3 GetAstronautWorldPosition()
    {
        return new Vector3(
            (float)astronaut.current.posX,
            0,
            (float)astronaut.current.posZ // Ensure Z uses posZ
        );
    }

    void InitializeLineRenderer()
    {
        pathRenderer.startWidth = 0.3f;
        pathRenderer.endWidth = 0.3f;
        pathRenderer.material = new Material(Shader.Find("Sprites/Default")) { 
            color = Color.cyan 
        };
        pathRenderer.positionCount = 0;
    }

    // void Update()
    // {
    //     Vector3 startPos = new Vector3(
    //         (float)astronaut.current.posX,
    //         (float)astronaut.current.posY,
    //         (float)astronaut.current.posZ
    //     );
        
    //     FindPath(startPos, target.position);
    // }

    public void FindPath(Vector3 startPos, Vector3 targetPos)
    {
        Node startNode = grid.NodeFromWorldPoint(startPos);
        Node targetNode = grid.NodeFromWorldPoint(targetPos);
        
        if (startNode == null) Debug.LogError($"Start node is null at position {startPos}");
        if (targetNode == null) Debug.LogError($"Target node is null at position {targetPos}");

        if (!ValidateNodes(startNode, targetNode))
        {
            Debug.LogError("Node validation failed");
            pathRenderer.positionCount = 0;
            return;
        }

        Debug.Log("Starting A* search...");
        ResetNodeCosts();
        AStarSearch(startNode, targetNode);
    }

    bool ValidateNodes(Node start, Node target)
    {
        if (start == null)
        {
            Debug.LogError($"Invalid start position (outside grid bounds?)");
            return false;
        }
        
        if (target == null)
        {
            Debug.LogError($"Invalid target position (outside grid bounds?)");
            return false;
        }

        if (target.bIsWall)
        {
            Debug.LogError("Target node is blocked by wall");
            return false;
        }

        return true;
    }

    void ResetNodeCosts()
    {
        foreach (Node node in grid.NodeArray)
        {
            node.gCost = int.MaxValue;
            node.hCost = 0;
            node.parent = null;
        }
    }

    void AStarSearch(Node startNode, Node targetNode)
    {
        Heap<Node> openSet = new Heap<Node>(grid.MaxSize);
        HashSet<Node> closedSet = new HashSet<Node>();
        
        startNode.gCost = 0;
        startNode.hCost = GetDistance(startNode, targetNode);
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

            ProcessNeighbors(currentNode, targetNode, openSet, closedSet);
        }
        
        Debug.LogWarning("No path exists between points");
        pathRenderer.positionCount = 0;
    }

    void ProcessNeighbors(Node current, Node target, Heap<Node> openSet, HashSet<Node> closedSet)
    {
        foreach (Node neighbor in grid.GetNeighboringNodes(current))
        {
            if (neighbor.bIsWall || closedSet.Contains(neighbor)) 
                continue;

            int newCost = current.gCost + GetDistance(current, neighbor) + neighbor.movementPenalty;
            
            if (newCost < neighbor.gCost || !openSet.Contains(neighbor))
            {
                neighbor.gCost = newCost;
                neighbor.hCost = GetDistance(neighbor, target);
                neighbor.parent = current;

                if (!openSet.Contains(neighbor))
                    openSet.Add(neighbor);
                else 
                    openSet.UpdateItem(neighbor);
            }
        }
    }

    List<Node> RetracePath(Node start, Node end)
    {
        List<Node> path = new List<Node>();
        Node current = end;

        while (current != start && current != null)
        {
            path.Add(current);
            current = current.parent;
        }
        
        if (current == null)
        {
            Debug.LogError("Path broken - null parent detected");
            return new List<Node>();
        }

        path.Reverse();
        return path;
    }

    void UpdatePathVisualization()
    {
        if (currentPath == null || currentPath.Count == 0) return;

        Vector3[] positions = new Vector3[currentPath.Count];
        for (int i = 0; i < currentPath.Count; i++)
        {
            positions[i] = currentPath[i].worldPosition + Vector3.up * 0.5f;
        }
        
        pathRenderer.positionCount = positions.Length;
        pathRenderer.SetPositions(positions);
    }

    int GetDistance(Node a, Node b)
    {
        int dx = Mathf.Abs(a.iGridX - b.iGridX);
        int dy = Mathf.Abs(a.iGridY - b.iGridY);
        
        return dx > dy ? 
            14 * dy + 10 * (dx - dy) : 
            14 * dx + 10 * (dy - dx);
    }

    public List<Node> GetCurrentPath()
    {
        return currentPath;
    }
}
