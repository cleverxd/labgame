using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class NavmeshBuilder : MonoBehaviour
{
    public NavMeshSurface navmesh; // Reference to the NavMeshSurface

    public IEnumerator ClearAndRebuildNavmesh()
    {
        NavMeshAgent[] agents = FindObjectsOfType<NavMeshAgent>();
        foreach (var agent in agents)
        {
            agent.enabled = false;
        }

        // Clear the existing NavMesh
        NavMesh.RemoveAllNavMeshData();

        // Wait for a frame to ensure everything is processed
        yield return new WaitForEndOfFrame();

        // Rebuild the NavMesh
        navmesh.BuildNavMesh();

        // Re-enable the agents
        foreach (var agent in agents)
        {
            agent.enabled = true;
        }
    }
}
