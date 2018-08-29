﻿using HoloToolkit.Unity;
using UnityEngine;

public class TagAlongForDisplay : MonoBehaviour
{
    public float TagalongDistance = 2.0f;
    public float PositionUpdateSpeed = 10f;
    public float SmoothingFactor = 0.6f;

    protected Interpolator interpolator;

    void Start()
    {
        interpolator = gameObject.GetComponent<Interpolator>();
        interpolator.SmoothLerpToTarget = true;
        interpolator.SmoothPositionLerpRatio = SmoothingFactor;
    }

    void Update()
    {
        Vector3 tagalongTargetPosition;
        tagalongTargetPosition = Camera.main.transform.position + Camera.main.transform.forward * TagalongDistance;
        interpolator.PositionPerSecond = PositionUpdateSpeed;
        interpolator.SetTargetPosition(tagalongTargetPosition);

        Vector3 directionToTarget = Camera.main.transform.position - transform.position;

        directionToTarget.y = 0.0f;

        // If we are right next to the camera the rotation is undefined. 
        if (directionToTarget.sqrMagnitude < 0.005f)
            return;

        transform.rotation = Quaternion.LookRotation(-directionToTarget);
    }
}