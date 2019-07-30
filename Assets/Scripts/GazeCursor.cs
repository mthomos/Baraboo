﻿using UnityEngine;

/*
 * GazeCursor script
 * Script for controlling apperance and placement of the cursor
 * Also using gaze to get the object that the user is looking at.
 */

public class GazeCursor : MonoBehaviour
{
    public Color FocusedColor = Color.red;
    //Private Variables
    private GameObject FocusedObject = null; // The object which user is staring at
    //Cached variables
    private Renderer cursorMeshRenderer; // Using this to disable cursor
    private RaycastHit hitInfo;
    private Vector3 gazeOrigin;
    private Vector3 gazeDirection;
    private Camera mainCamera;
    public bool focusedManipualtedObjectChanged = false;
    public HandsTrackingController handsTrackingController;

    void Start ()
    {
        //Initialize cached variables
        cursorMeshRenderer = gameObject.GetComponent<Renderer>();
        mainCamera = Camera.main;
    }

    void Update()
    {

        gazeOrigin = mainCamera.transform.position;
        gazeDirection = mainCamera.transform.forward;
        if (Physics.Raycast(gazeOrigin, gazeDirection, out hitInfo))
        {
            cursorMeshRenderer.enabled = true;
            gameObject.transform.position = hitInfo.point;
            gameObject.transform.rotation = Quaternion.FromToRotation(Vector3.up, hitInfo.normal);
            // Check if user focused on a menu object
            if (hitInfo.collider.gameObject.CompareTag("UI"))
            {
                FocusedObject = hitInfo.collider.gameObject;
                EventManager.TriggerEvent("gaze_ui");
            }
            else if (hitInfo.collider.gameObject.CompareTag("User"))
            {
                if (FocusedObject == hitInfo.collider.gameObject)
                {
                    if (!handsTrackingController.handDetected())
                        UtilitiesScript.Instance.ChangeColorOutline(FocusedObject, FocusedColor);
                    focusedManipualtedObjectChanged = false;
                }
                else
                {
                    FocusedObject = hitInfo.collider.gameObject;
                    UtilitiesScript.Instance.ChangeColorOutline(FocusedObject, FocusedColor);
                    focusedManipualtedObjectChanged = true;
                }
            }
        }
        else
        {
            cursorMeshRenderer.enabled = false;
            UtilitiesScript.Instance.DisableOutline(FocusedObject);
        }
    }

    public GameObject GetFocusedObject()
    {
        return FocusedObject;
    }
}
