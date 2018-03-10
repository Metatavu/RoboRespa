using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class RaycastObjectDetectorEvent : UnityEvent
{

}

public class RaycastObjectDetector : MonoBehaviour
{
    public float interactionDistance = 3f;

    public LayerMask layers;

    [SerializeField]
    public RaycastObjectDetectorEvent onObjectEnter;

    [SerializeField]
    public RaycastObjectDetectorEvent onObjectExit;

    private bool lastState = false;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;

        Debug.DrawRay(transform.position, transform.forward * interactionDistance);

        if (Physics.Raycast(transform.position, transform.forward, out hit, interactionDistance, layers))
        {
            if (lastState)
            {
                onObjectEnter.Invoke();
            }
            lastState = true;
        } else
        {
            if (!lastState)
            {
                onObjectExit.Invoke();
            }
            lastState = false;
        }
    }

}
