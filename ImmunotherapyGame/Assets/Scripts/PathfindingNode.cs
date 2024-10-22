using UnityEngine;

public class PathfindingNode : MonoBehaviour
{
    [SerializeField] private PathfindingNode[] connections;

    public PathfindingNode[] GetConnections()
    {
        return connections;
    }
}