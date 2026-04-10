using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public Vector3 position;
    public List<Node> neighbors = new List<Node>();

    public Node(Vector3 pos)
    {
        position = pos;
    }
}