#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using UnityEngine;

public class PathfindingNode : MonoBehaviour
{
    [SerializeField] private PathfindingNode[] connections;

    public PathfindingNode[] GetConnections()
    {
        return connections;
    }

    public void AddConnection(PathfindingNode other)
    {
        PathfindingNode[] newConnections = new PathfindingNode[connections.Length + 1];
        Array.Copy(connections, newConnections, connections.Length);
        newConnections[^1] = other;
    }
}

#if UNITY_EDITOR
// TODO replace with gizmos
[CustomEditor(typeof(PathfindingNode))]
public class PathfindingNodeEditor : Editor
{
    void OnSceneGUI()
    {
        PathfindingNode node = (PathfindingNode)target;
        Handles.color = new Color(1, 1, 0);
        Handles.DrawSolidDisc(node.transform.position, Vector3.forward, 0.25f);

        foreach (PathfindingNode connected in node.GetConnections())
        {
            Handles.color = new Color(1, 0.64f, 0);
            Handles.DrawLine(node.transform.position, connected.transform.position);
            Handles.color = new Color(0, 0.64f, 1);
            Handles.DrawSolidDisc(connected.transform.position, Vector3.forward, 0.15f);
        }

        // Vector3 averagePosition = Vector3.zero;

        // foreach (PathfindingNode node in targets)
        // {
        //     Handles.color = new Color(1, 1, 0);
        //     Handles.DrawSolidDisc(node.transform.position, Vector3.forward, 0.25f);
        //     averagePosition += node.transform.position;
        // }

        // averagePosition /= targets.Length;

        // if (Handles.Button(averagePosition, Quaternion.identity, 0.5f, 1, Handles.CircleHandleCap))
        //     foreach (PathfindingNode node in targets)
        //         foreach (PathfindingNode node2 in targets)
        //             node.AddConnection(node2);
    }
}
#endif
