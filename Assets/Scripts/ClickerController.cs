﻿using UnityEngine;
using UnityEngine.XR.WSA.Input;

/*
 * ClickerController script
 * Script for receiving click and double click events from Clicker
 */

public class ClickerController : MonoBehaviour
{
    private float lastClickTime = .0f;
    private bool clickTriggered;
    private bool doubleClickTriggered;
    public bool clickerEnabled;


    private void Start()
    {
        if (clickerEnabled)
            InteractionManager.InteractionSourcePressed += InteractionManager_InteractionSourcePressed;
    }

    private void Update()
    {
        if(clickerEnabled)
            CheckForClicker();
    }
    private void OnDestroy()
    {
        InteractionManager.InteractionSourcePressed -= InteractionManager_InteractionSourcePressed;
    }


    private void CheckForClicker()
    {
        if (doubleClickTriggered == true)
        {
            clickTriggered = false;
            doubleClickTriggered = false;
            EventManager.TriggerEvent("double_click");
        }
        if (doubleClickTriggered == false && clickTriggered == true)
        {
            clickTriggered = false;
            EventManager.TriggerEvent("click");
        }
    }

    private void InteractionManager_InteractionSourcePressed(InteractionSourcePressedEventArgs args)
    {
        if (args.state.source.kind == InteractionSourceKind.Controller)
        {
            if (!clickTriggered) //First click
            {
                clickTriggered = true;
                lastClickTime = Time.time;
            }
            else //Second click
            {
                if (Time.time - lastClickTime < 0.6f)
                {
                    clickTriggered = false;
                    doubleClickTriggered = true;
                }
            }
        }
    }

    public void enableClicker()
    {
        clickerEnabled = false;
        InteractionManager.InteractionSourcePressed += InteractionManager_InteractionSourcePressed;
    }

    public void disableClicker()
    {
        clickerEnabled = false;
        InteractionManager.InteractionSourcePressed -= InteractionManager_InteractionSourcePressed;
    }

}
