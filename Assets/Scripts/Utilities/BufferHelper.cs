using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System;

public class BufferHelper : MonoBehaviour
{

    public static int DEFAULT_BUFFER_DURATION = 8;


    [SerializeField] InputActionReference actionReference;
    [SerializeField] bool isHoldable = false;
    [SerializeField] int bufferDuration = DEFAULT_BUFFER_DURATION;

    int window = 0;
    public bool Buffered => window > 0;


    private void Update()
    {        
        if (actionReference.action.WasPerformedThisFrame() || isHoldable && actionReference.action.IsPressed())
        {
            BufferInput();
        }
    }
    private void FixedUpdate()
    {
        if (window > 0)
        {
            window--;
        }
    } 

    public void BufferInput()
    {
        window = bufferDuration;
    }

    public void Consume()
    {
        window = 0;
    }
    public void OverrideDuration(int duration)
    {
        bufferDuration = duration;
    }

    public void ResetOverrideDuration()
    {
        bufferDuration = DEFAULT_BUFFER_DURATION;
    }
}
