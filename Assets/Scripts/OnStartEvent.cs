using System.Collections;
using UnityEngine;
using UnityEngine.Events;

[AddComponentMenu("Reflect/Template/On Start Event")]
public class OnStartEvent : MonoBehaviour
{
    [SerializeField] float _delay = 1f;
    public UnityEvent onStart, onDelayedStart;

    private void Start()
    {
        onStart?.Invoke();
        StartCoroutine(DelayedStart(_delay));
    }

    private IEnumerator DelayedStart(float delay)
    {
        yield return new WaitForSeconds(delay);
        onDelayedStart?.Invoke();
    }
}