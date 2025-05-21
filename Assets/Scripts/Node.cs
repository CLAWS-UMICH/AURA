using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : IHeapItem<Node> {

    public int iGridX;//X Position in the Node Array
    public int iGridY;//Y Position in the Node Array
    public bool bIsWall;//Tells the program if this node is being obstructed.
    public Vector3 worldPosition;//The world position of the node.
    public int movementPenalty; // Weight given to a walkable surface

    public int gCost;//The cost of moving to the next square.
    public int hCost;//The distance to the goal from this node.
    public Node parent;//For the AStar algoritm, will store what node it previously came from so it cn trace the shortest path.

    // public int FCost { get { return igCost + ihCost; } }//Quick get function to add G cost and H Cost, and since we'll never need to edit FCost, we dont need a set function.
    public int FCost => gCost + hCost;  // Updated property

    int heapIndex; // Index within the heap structure
    public int HeapIndex { get => heapIndex; set => heapIndex = value; } // variable that allows setting and getting of the heapIndex

    public int CompareTo(Node other)
    {
        int compare = FCost.CompareTo(other.FCost);
        if (compare == 0)
        {
            compare = hCost.CompareTo(other.hCost);  // Changed from ihCost
        }
        return -compare;
    }

    public Node(bool isWall, Vector3 worldPos, int gridX, int gridY, int penalty)
    {
        bIsWall = isWall;
        worldPosition = worldPos; 
        iGridX = gridX;
        iGridY = gridY;
        movementPenalty = penalty;
    }

    // public Node(bool a_bIsWall, Vector3 a_vPos, int a_igridX, int a_igridY, int _penalty)//Constructor
    // {
    //     bIsWall = a_bIsWall;//Tells the program if this node is being obstructed.
    //     vPosition = a_vPos;//The world position of the node.
    //     iGridX = a_igridX;//X Position in the Node Array
    //     iGridY = a_igridY;//Y Position in the Node Array
    //     movementPenalty = _penalty; // Weight given to a walkable area
    // }
}