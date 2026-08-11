using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class AutomaticDoubleDoorTrigger : MonoBehaviour
{
    [SerializeField] private DoubleSlidingDoor slidingDoor;

    [Header("Accepted Tags")]
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private string npcTag = "NPC";

    [Header("Closing")]
    [SerializeField, Min(0f)] private float closeDelay = 0.5f;

    private readonly Dictionary<Transform, int> colliderCounts =
        new Dictionary<Transform, int>();

    private Coroutine closeRoutine;

    private void Awake()
    {
        BoxCollider triggerCollider = GetComponent<BoxCollider>();
        triggerCollider.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        Transform validObject = FindTaggedObject(other.transform);

        if (validObject == null)
            return;

        if (colliderCounts.ContainsKey(validObject))
            colliderCounts[validObject]++;
        else
            colliderCounts.Add(validObject, 1);

        CancelScheduledClose();

        if (slidingDoor != null && !slidingDoor.IsOpen)
            slidingDoor.OpenDoor();
    }

    private void OnTriggerExit(Collider other)
    {
        Transform validObject = FindTaggedObject(other.transform);

        if (validObject == null ||
            !colliderCounts.ContainsKey(validObject))
        {
            return;
        }

        colliderCounts[validObject]--;

        if (colliderCounts[validObject] <= 0)
            colliderCounts.Remove(validObject);

        RemoveDestroyedObjects();

        if (colliderCounts.Count == 0)
        {
            CancelScheduledClose();
            closeRoutine = StartCoroutine(CloseAfterDelay());
        }
    }

    private IEnumerator CloseAfterDelay()
    {
        yield return new WaitForSeconds(closeDelay);

        RemoveDestroyedObjects();

        if (colliderCounts.Count == 0)
            slidingDoor?.CloseDoor();

        closeRoutine = null;
    }

    private Transform FindTaggedObject(Transform startingTransform)
    {
        Transform current = startingTransform;

        while (current != null)
        {
            if (current.CompareTag(playerTag) ||
                current.CompareTag(npcTag))
            {
                return current;
            }

            current = current.parent;
        }

        return null;
    }

    private void RemoveDestroyedObjects()
    {
        List<Transform> objectsToRemove = new List<Transform>();

        foreach (KeyValuePair<Transform, int> entry in colliderCounts)
        {
            if (entry.Key == null || entry.Value <= 0)
                objectsToRemove.Add(entry.Key);
        }

        foreach (Transform objectToRemove in objectsToRemove)
            colliderCounts.Remove(objectToRemove);
    }

    private void CancelScheduledClose()
    {
        if (closeRoutine == null)
            return;

        StopCoroutine(closeRoutine);
        closeRoutine = null;
    }

    private void OnDisable()
    {
        colliderCounts.Clear();
        CancelScheduledClose();
    }
}