using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    [Header("Grid Settings")]
    public LayerMask WallMask;
    public Vector2 vGridWorldSize = new Vector2(210, 130);
    public float fNodeRadius = 0.5f;
    public float fDistanceBetweenNodes = 0.1f;
    public TerrainType[] walkableRegions;

    [Header("Debug")]
    public bool showGridGizmos = true;

    public Node[,] NodeArray { get; private set; }
    LayerMask walkableMask;
    Dictionary<int, int> walkableRegionsDict = new Dictionary<int, int>();

    float fNodeDiameter;
    int iGridSizeX, iGridSizeY;

    public int MaxSize => iGridSizeX * iGridSizeY;
    
    private Pathfinding pathfinding;

    void Awake()
    {
        fNodeDiameter = fNodeRadius * 2;
        CalculateGridSize();
        InitializeWalkableRegions();
        CreateGrid();
        pathfinding = GetComponent<Pathfinding>();
    }

    void CalculateGridSize()
    {
        iGridSizeX = Mathf.RoundToInt(vGridWorldSize.x / fNodeDiameter);
        iGridSizeY = Mathf.RoundToInt(vGridWorldSize.y / fNodeDiameter);
    }

    void InitializeWalkableRegions()
    {
        foreach (TerrainType region in walkableRegions)
        {
            walkableMask |= region.terrainMask;
            walkableRegionsDict.Add((int)Mathf.Log(region.terrainMask.value, 2), region.terrainPenalty);
        }
    }

    void CreateGrid()
    {
        NodeArray = new Node[iGridSizeX, iGridSizeY];
        Vector3 gridBottomLeft = transform.position - 
            Vector3.right * vGridWorldSize.x/2 - 
            Vector3.forward * vGridWorldSize.y/2;

        for (int x = 0; x < iGridSizeX; x++)
        {
            for (int y = 0; y < iGridSizeY; y++)
            {
                Vector3 worldPoint = gridBottomLeft + 
                    Vector3.right * (x * fNodeDiameter + fNodeRadius) + 
                    Vector3.forward * (y * fNodeDiameter + fNodeRadius);

                bool isWall = CheckForWall(worldPoint);
                int penalty = isWall ? 0 : CalculateTerrainPenalty(worldPoint);

                NodeArray[x, y] = new Node(isWall, worldPoint, x, y, penalty);
            }
        }
    }

    bool CheckForWall(Vector3 worldPoint)
    {
        return Physics.CheckSphere(worldPoint, fNodeRadius, WallMask);
    }

    int CalculateTerrainPenalty(Vector3 worldPoint)
    {
        Ray ray = new Ray(worldPoint + Vector3.up * 50, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, 100, walkableMask))
        {
            if (walkableRegionsDict.TryGetValue(hit.collider.gameObject.layer, out int penalty))
            {
                return penalty;
            }
        }
        return 0;
    }

    public List<Node> GetNeighboringNodes(Node node)
    {
        List<Node> neighbors = new List<Node>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0) continue;

                int checkX = node.iGridX + x;
                int checkY = node.iGridY + y;

                if (IsWithinGrid(checkX, checkY))
                {
                    neighbors.Add(NodeArray[checkX, checkY]);
                }
            }
        }

        return neighbors;
    }

    bool IsWithinGrid(int x, int y)
    {
        return x >= 0 && x < iGridSizeX && y >= 0 && y < iGridSizeY;
    }

    public Node NodeFromWorldPoint(Vector3 worldPosition)
    {
        Vector3 localPos = worldPosition - transform.position;
        
        float percentX = (localPos.x + vGridWorldSize.x/2) / vGridWorldSize.x;
        float percentY = (localPos.z + vGridWorldSize.y/2) / vGridWorldSize.y;
        
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.FloorToInt(percentX * (iGridSizeX - 1));
        int y = Mathf.FloorToInt(percentY * (iGridSizeY - 1));
        
        return NodeArray[x, y];
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(vGridWorldSize.x, 1, vGridWorldSize.y));

        if (NodeArray != null && showGridGizmos)
        {
            // Get the current path from Pathfinding, if available
            List<Node> pathNodes = null;
            if (pathfinding == null)
                pathfinding = GetComponent<Pathfinding>();
            if (pathfinding != null)
                pathNodes = pathfinding.GetCurrentPath();

            foreach (Node node in NodeArray)
            {
                // Default color
                Gizmos.color = node.bIsWall ? Color.red : Color.white;

                // If this node is in the current path, color it green
                if (pathNodes != null && pathNodes.Contains(node))
                    Gizmos.color = Color.green;

                Gizmos.DrawCube(node.worldPosition, Vector3.one * (fNodeDiameter - fDistanceBetweenNodes));
            }
        }
    }


    [System.Serializable]
    public class TerrainType
    {
        public LayerMask terrainMask;
        public int terrainPenalty;
    }
}
