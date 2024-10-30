using System.Collections.Generic;
using UnityEngine;
using Yohash.PriorityQueue;

public class Pathfinder : MonoBehaviour
{
    [SerializeField] private PathfindingNode[] pathfindingNodes;
    [SerializeField] private float maxPathNodeDistance = 1;

    private Dictionary<PathfindingNode, PriorityQueueNode> nodes = new Dictionary<PathfindingNode, PriorityQueueNode>();
    private PriorityQueueNode currentNode;
    private PriorityQueueNode destination;
    private Stack<PriorityQueueNode> path = new Stack<PriorityQueueNode>();

    private FastPriorityQueue<PriorityQueueNode> openSet;
    private Dictionary<PriorityQueueNode, PriorityQueueNode> cameFrom = new Dictionary<PriorityQueueNode, PriorityQueueNode>();
    private Dictionary<PriorityQueueNode, float> costSoFar = new Dictionary<PriorityQueueNode, float>();

    void Start()
    {
        if (openSet == null)
            UpdateNodes();
    }

    private void UpdateNodes()
    {
        foreach (PathfindingNode node in pathfindingNodes)
            nodes[node] = new PriorityQueueNode(node);

        openSet = new FastPriorityQueue<PriorityQueueNode>(nodes.Count);
    }

    void FixedUpdate()
    {
        if (destination == null)
            return;

        PriorityQueueNode nearest = currentNode;
        float nearestDist = nearest == null ? float.MaxValue : Vector2.Distance(transform.position, nearest.GetPathfindingNode().transform.position);

        foreach (PathfindingNode node in pathfindingNodes)
        {
            float dist = Vector2.Distance(transform.position, node.transform.position);

            if (dist < nearestDist)
            {
                nearest = nodes[node];
                nearestDist = dist;
            }
        }

        if (path.Count > 0 && nearest == path.Peek())
        {
            if (nearestDist <= maxPathNodeDistance)
                currentNode = path.Pop();
        }
        else if (currentNode != nearest)
        {
            currentNode = nearest;
            CalculatePath();
        }
    }

    public PathfindingNode GetCurrentNode()
    {
        if (currentNode == null)
            return null;

        return currentNode.GetPathfindingNode();
    }

    public PathfindingNode GetNextNode()
    {
        if (path.TryPeek(out PriorityQueueNode node))
            return node.GetPathfindingNode();

        return null;
    }

    public PathfindingNode GetDestination()
    {
        if (destination == null)
            return null;

        return destination.GetPathfindingNode();
    }

    public Stack<PriorityQueueNode> GetPath()
    {
        return path;
    }

    public void SetPathfindingNodes(PathfindingNode[] pfNodes)
    {
        pathfindingNodes = pfNodes;
        UpdateNodes();
    }

    public void SetDestination(PathfindingNode dest)
    {
        if (dest == null)
        {
            destination = null;
            return;
        }

        if (destination != null && destination.GetPathfindingNode() == dest)
            return;

        currentNode ??= nodes[GetNearestNode(transform.position)];
        destination = nodes[dest];
        CalculatePath();
    }

    public PathfindingNode GetNearestNode(Vector2 position)
    {
        PathfindingNode nearest = null;
        float nearestDist = float.MaxValue;

        foreach (PathfindingNode node in pathfindingNodes)
        {
            float dist = Vector2.Distance(position, node.transform.position);

            if (dist < nearestDist)
            {
                nearest = node;
                nearestDist = dist;
            }
        }

        return nearest;
    }

    public float DistanceToNode(PathfindingNode node)
    {
        Stack<PriorityQueueNode> pathCache = new Stack<PriorityQueueNode>(path);
        PriorityQueueNode destinationCache = destination;
        destination = nodes[node];
        currentNode ??= nodes[GetNearestNode(transform.position)];
        CalculatePath();
        float distance = 0;

        while (path.Count > 1)
        {
            PathfindingNode current = path.Pop().GetPathfindingNode();
            PathfindingNode next = path.Peek().GetPathfindingNode();
            distance += Vector2.Distance(current.transform.position, next.transform.position);
        }

        path = pathCache;
        destination = destinationCache;
        return distance;
    }

    private void CalculatePath()
    {
        openSet.Clear();
        cameFrom.Clear();
        costSoFar.Clear();
        openSet.Enqueue(currentNode, 0);
        costSoFar[currentNode] = 0;

        while (openSet.Count > 0)
        {
            PriorityQueueNode current = openSet.Dequeue();

            if (current == destination)
            {
                ReconstructPath(current);
                break;
            }
            
            foreach (PathfindingNode nextPathfindingNode in current.GetPathfindingNode().GetConnections())
            {
                PriorityQueueNode next = nodes[nextPathfindingNode];
                float newCost = costSoFar[current] + Vector2.Distance(current.GetPathfindingNode().transform.position, next.GetPathfindingNode().transform.position);

                if (costSoFar.ContainsKey(next) && newCost >= costSoFar[next])
                    continue;

                costSoFar[next] = newCost;
                float priority = newCost + Vector2.Distance(next.GetPathfindingNode().transform.position, destination.GetPathfindingNode().transform.position);
                openSet.Enqueue(next, priority);
                cameFrom[next] = current;
            }
        }
    }

    private void ReconstructPath(PriorityQueueNode current)
    {
        path.Clear();
        path.Push(current);

        while (cameFrom.ContainsKey(current))
        {
            current = cameFrom[current];
            path.Push(current);
        }
    }
}

public class PriorityQueueNode : FastPriorityQueueNode
{
    private PathfindingNode pathfindingNode;

    public PriorityQueueNode(PathfindingNode nodeIn)
    {
        pathfindingNode = nodeIn;
    }

    public PathfindingNode GetPathfindingNode()
    {
        return pathfindingNode;
    }

    public void SetPathfindingNode(PathfindingNode node)
    {
        pathfindingNode = node;
    }
}
