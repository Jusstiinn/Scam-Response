using System;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class NpcNavAgent : MonoBehaviour
{
    private NavMeshAgent agent;
    private Action arrival;
    public float CurrentSpeed => agent != null ? agent.velocity.magnitude : 0f;
    private void Awake() => agent = GetComponent<NavMeshAgent>();
    private void Update()
    {
        if (arrival == null || agent.pathPending || !agent.isOnNavMesh) return;
        if (agent.remainingDistance <= agent.stoppingDistance && (!agent.hasPath || agent.velocity.sqrMagnitude < 0.01f))
        { var cb = arrival; arrival = null; cb.Invoke(); }
    }
    public void MoveTo(Transform destination, Action onArrived = null)
    {
        if (destination == null || !agent.isOnNavMesh) return;
        arrival = onArrived; agent.isStopped = false; agent.SetDestination(destination.position);
    }
    public void Follow(Transform target) { if (target != null && agent.isOnNavMesh) agent.SetDestination(target.position); }
    public void Warp(Vector3 position) { if (agent.isOnNavMesh) agent.Warp(position); else transform.position = position; }
}
