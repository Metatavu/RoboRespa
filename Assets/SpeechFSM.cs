using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

[System.Serializable]
public class SpeechFsmEvent : UnityEvent
{

}

public class SpeechFSM : MonoBehaviour
{
    [SerializeField]
    public SpeechFsmEvent startEvent;

    [SerializeField]
    public SpeechFsmEvent stopEvent;

    public bool listenToInput;

    public string buttonName;

    // Use this for initialization
    void Start()
    {
        StartCoroutine(process());
    }

    public void SetListenToInput(bool mode) // Required to hook into events
    {
        listenToInput = mode;
    }

    private IEnumerator process()
    {
        while (true)
        {
            yield return new WaitUntil(() => Input.GetButtonDown(buttonName));
            if (!listenToInput)
            {
                continue;
            }
            startEvent.Invoke();
            yield return new WaitForSeconds(0.01f);
            yield return new WaitUntil(() => Input.GetButtonDown(buttonName));
            Debug.Log("stopEvent");
            stopEvent.Invoke();
            yield return new WaitForSeconds(0.01f);
        }
    }
}
