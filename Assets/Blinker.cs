using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum BlinkerState
{
    BLINK,
    FORCE_ON,
    FORCE_OFF
}

[System.Serializable]
public class BlinkerEvent : UnityEvent
{

}


public class Blinker : MonoBehaviour
{

    [SerializeField]
    public BlinkerEvent onTurnOn;

    [SerializeField]
    public BlinkerEvent onTurnOff;

    public float minWait = 0.001f;
    public float maxWait = 0.2f;

    public BlinkerState state;

    // Use this for initialization
    void Start()
    {
        StartCoroutine(process());
    }

    public void Blink()
    {
        state = BlinkerState.BLINK;
    }

    public void ForceOn()
    {
        state = BlinkerState.FORCE_ON;
    }

    public void ForceOff()
    {
        state = BlinkerState.FORCE_OFF;
    }

    private IEnumerator process()
    {
        while (true)
        {
            switch (state)
            {
                case BlinkerState.BLINK:
                    onTurnOn.Invoke();
                    yield return new WaitForSeconds(Random.Range(minWait, maxWait));
                    onTurnOff.Invoke();
                    yield return new WaitForSeconds(Random.Range(minWait, maxWait));
                    break;
                case BlinkerState.FORCE_ON:
                    onTurnOn.Invoke();
                    yield return new WaitUntil(() => state != BlinkerState.FORCE_ON);
                    break;
                case BlinkerState.FORCE_OFF:
                    onTurnOff.Invoke();
                    yield return new WaitUntil(() => state != BlinkerState.FORCE_OFF);
                    break;
            }
        }
    }
}
