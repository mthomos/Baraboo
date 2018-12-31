﻿using System.Collections.Generic;
using System.Linq;
using HoloToolkit.Unity;
using UnityEngine;

public class ObjectCollectionManager : Singleton<ObjectCollectionManager>
{
    //Public Variables - For Editor
    public GameObject TreePrefab;
    public GameObject FruitPrefab;
    public GameObject BoxPrefab;
    public GameObject GatePrefab;
    public Vector3 TreeSize;
    public Vector3 FruitSize;
    public Vector3 BoxSize;
    public Vector3 GateSize;
    public Vector3 CalibrationPointSize;
    // Private Variables
    public FlowController flowController;
    private GameObject createdTree;
    private GameObject createdBox;
    private GameObject createdGate;
    private float ScaleFactor;
    private List<GameObject> ActiveHolograms = new List<GameObject>();
    private bool boxCreated, treeCreated;

    public void CreateTree(Vector3 positionCenter, Quaternion rotation)
    {
        // Stay center in the square but move down to the ground
        Vector3 position = positionCenter - new Vector3(0, TreeSize.y * .5f, 0);
        GameObject newObject = Instantiate(TreePrefab, position, rotation);
        newObject.name = "Tree";
        if (newObject != null)
        {
            newObject.transform.parent = gameObject.transform;
            newObject.tag = "Dummy";
            newObject.transform.localScale = RescaleToSameScaleFactor(TreePrefab);
            ActiveHolograms.Add(newObject);
            createdTree = newObject;
            SetProps();
        }
    }

    public void CreateBox(Vector3 positionCenter, Quaternion rotation)
    {
        // Stay center in the square but move down to the ground
        var position = positionCenter + new Vector3(0, BoxSize.y * .25f, 0);
        GameObject newObject = Instantiate(BoxPrefab, position, rotation);
        newObject.name = "Box";
        if (newObject != null)
        {
            newObject.transform.parent = gameObject.transform;
            newObject.tag = "Dummy";
            newObject.transform.localScale = RescaleToSameScaleFactor(BoxPrefab);
            ActiveHolograms.Add(newObject);
            createdBox = newObject;
            boxCreated = true;
            if (treeCreated && boxCreated)
                flowController.PrepareNextManipulation();
        }
    }

    public void CreateGate(Vector3 positionCenter, Quaternion rotation)
    {
        // Stay center in the square but move down to the ground
        var position = positionCenter + new Vector3(0, GateSize.y * .25f, 0);
        GameObject newObject = Instantiate(GatePrefab, position, rotation);
        newObject.name = "Gate";
        if (newObject != null)
        {
            newObject.transform.parent = gameObject.transform;
            newObject.tag = "Dummy";
            newObject.transform.localScale = RescaleToSameScaleFactor(GatePrefab);
            ActiveHolograms.Add(newObject);
            createdGate = newObject;
            boxCreated = true;
            if (treeCreated && boxCreated)
                flowController.PrepareNextManipulation();
        }
    }

    private Vector3 RescaleToSameScaleFactor(GameObject objectToScale)
    {
        if (ScaleFactor == 0f) CalculateScaleFactor();
        return objectToScale.transform.localScale * ScaleFactor;
    }

    private Vector3 RescaleToDesiredSizeProportional(GameObject objectToScale, Vector3 desiredSize)
    {
        float scaleFactor = CalcScaleFactorHelper(objectToScale, desiredSize);
        return objectToScale.transform.localScale * scaleFactor;
    }

    private void CalculateScaleFactor()
    {
        float maxScale = float.MaxValue;
        var ratio = CalcScaleFactorHelper(TreePrefab, TreeSize);
        if (ratio < maxScale) maxScale = ratio;
        ScaleFactor = maxScale;
    }

    private float CalcScaleFactorHelper(GameObject obj, Vector3 desiredSize)
    {
        float maxScale = float.MaxValue;
        var curBounds = GetBoundsForAllChildren(obj).size;
        var difference = desiredSize - curBounds;
        float ratio;

        if (difference.x > difference.y && difference.x > difference.z)
            ratio = desiredSize.x / curBounds.x;
        else if (difference.y > difference.x && difference.y > difference.z)
            ratio = desiredSize.y / curBounds.y;
        else
            ratio = desiredSize.z / curBounds.z;

        if (ratio < maxScale)
            maxScale = ratio;

        return maxScale;
    }

    private Bounds GetBoundsForAllChildren(GameObject findMyBounds)
    {
        Bounds result = new Bounds(Vector3.zero, Vector3.zero);

        foreach (var curRenderer in findMyBounds.GetComponentsInChildren<Renderer>())
        {
            if (result.extents == Vector3.zero)
                result = curRenderer.bounds;
            else
                result.Encapsulate(curRenderer.bounds);
        }
        return result;
    }

    public void SetProps()
    {
        GameObject child;
        for (int i = 0; i < createdTree.transform.childCount; i++)
        {
            child = createdTree.transform.GetChild(i).gameObject;
            child.AddComponent<AppleScript>();
            ActiveHolograms.Add(child);
        }
        treeCreated = true;
        if (treeCreated && boxCreated)
            flowController.PrepareNextManipulation();
    }
    
    public GameObject getLowestFruit(float threshold)
    {
        List<GameObject> list  = new List<GameObject>();
        if (createdTree == null)
            return null;
        for (int i = 0; i < createdTree.transform.childCount; i++)
        {
            if ( createdTree.transform.GetChild(i).gameObject.transform.position.y > threshold)
                list.Add(createdTree.transform.GetChild(i).gameObject);
        }

        List <GameObject> a = list.OrderBy(item => item.transform.position.y).ToList();
        if (a[0] != null)
            return a[0];
        else
            return null;
    }

    public void ClearScene()
    {
        foreach (GameObject i in ActiveHolograms)
            Destroy(i);
        ActiveHolograms.Clear();
    }

    public void appearBox(int counter, Vector3 initPos)
    {
        /*
        Vector3 handRef = initPos - Camera.main.transform.position;
        float magHR = Mathf.Sqrt(Mathf.Pow(handRef.x, 2) + Mathf.Pow(handRef.z, 2));
        float cosf = handRef.x / magHR;
        float sinf = handRef.z / magHR;
        if (counter%2==0)
        {
            handRef.x = magHR * (-sinf);
            handRef.z = magHR * (cosf);
        }
        else
        {
            handRef.x = magHR * sinf;
            handRef.z = magHR * (-cosf);
        }
        handRef.y = initPos.y - 0.5f;
        createdBox.SetActive(true);
        Vector3 newPos = handRef + Camera.main.transform.position;
        createdBox.transform.position = newPos;
        */
        createdBox.SetActive(true);
    }

    public void disappearBox()
    {
        for (int a = 0; a < createdBox.transform.childCount; a++)
            createdBox.transform.GetChild(a).gameObject.SetActive(false);
        createdBox.SetActive(false);
    }

    public GameObject getCreatedBox()
    {
        return createdBox;
    }

    public GameObject getCreatedGate()
    {
        return createdGate;
    }
}
