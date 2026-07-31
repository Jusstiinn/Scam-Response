using System;
using System.Collections;
using UnityEngine;

public class NPCWaitingTimer : MonoBehaviour
{
    [SerializeField, Min(0.1f)] private float waitDuration = 18f;
    [SerializeField] private Vector2 returnCooldownRange = new Vector2(5f, 10f);

    private Coroutine timerRoutine;

    public void Begin(Action onTimeout)
    {
        Stop();
        timerRoutine = StartCoroutine(WaitRoutine(onTimeout));
    }

    public void Stop()
    {
        if (timerRoutine != null)
        {
            StopCoroutine(timerRoutine);
            timerRoutine = null;
        }
    }

    public float GetRandomReturnCooldown()
    {
        return UnityEngine.Random.Range(returnCooldownRange.x, returnCooldownRange.y);
    }

    private IEnumerator WaitRoutine(Action onTimeout)
    {
        yield return new WaitForSeconds(waitDuration);
        timerRoutine = null;
        onTimeout?.Invoke();
    }
}
