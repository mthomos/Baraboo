﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class UiController : MonoBehaviour
{
    //Public Variables-For Editor
    public GameObject menuPrefab;
    public GameObject settingsPrefab;
    public GameObject playPrefab;
    public GameObject resultsPrefab;
    public GameObject aboutPrefab;
    public FileManager fileManager;
    public ObjectPlacer placer;
    public GazeCursor gazeCursor;
    public TextMesh DebugText;
    public HandsTrackingController handsTrackingController;
    public FlowController flowController;
    public float menuDistance;

    // Settings
    private List<int> settings;
    private bool audioFeedbackEnabled = false;
    private bool clickerEnabled = false;
    private bool rightHandEnabled = true;
    private bool leftHandEnabled = true;
    private bool treeIsShort = true;

    // Menu
    private GameObject menuScreen;
    private GameObject settingsScreen;
    private GameObject playScreen;
    private GameObject resultsScreen;
    private GameObject aboutScreen;
    private GameObject currentMenu;
    private int inMenu = -1; // Menu index

    // Training
    private bool trainingMode = false;

    private void Start()
    {
        //Load Settings
        settings = fileManager.LoadSettings();
        for (int i = 0; i < settings.Count; i++)
        {
            if (i == 0)
            {
                if (settings[i] > 0)
                    audioFeedbackEnabled = true;
            }
            else if (i == 2)
            {
                if (settings[i] > 0)
                    clickerEnabled = true;
            }
        }
    }

    private void tapUiReceived()
    {
        if (trainingMode)
        {
            //Reset UI
            TextToSpeech.Instance.StartSpeaking("Training finished");
            ObjectCollectionManager.Instance.ClearScene();
            UtilitiesScript.Instance.enableObject(currentMenu);
            if (resultsScreen == null) //Create Results menu
                resultsScreen = Instantiate(resultsPrefab, currentMenu.transform.position, currentMenu.transform.rotation);
            else
                UtilitiesScript.Instance.enableObject(resultsScreen);
            UtilitiesScript.Instance.disableObject(currentMenu);
            currentMenu = resultsScreen;
            TextMesh suc = currentMenu.transform.Find("Successes").gameObject.GetComponent<TextMesh>();
            TextMesh failures = currentMenu.transform.Find("Failures").gameObject.GetComponent<TextMesh>();
            suc.text = "Succeses : " + flowController.success;
            failures.text = "Failures: " + flowController.fail;
            inMenu = 3;

            flowController.finishGame();
            return;
        }

        GameObject tappedObj = gazeCursor.getFocusedObject();
        if (tappedObj.CompareTag("UI"))
        {
            if (inMenu == 0) //Start Menu
            {
                if (tappedObj.name == "StartButton")
                {
                    moveToPlayScreen();
                }
                else if (tappedObj.name == "SettingsButton")
                {
                    moveToSettingsScreen();
                }
                else if (tappedObj.name == "AboutButton")
                {
                    moveToAboutScreen();
                }
            }
            else if (inMenu == 1) //Setting Menu
            {
                if (tappedObj.name == "AudioFeedbackButton")
                {
                    audioFeedbackEnabled = (!audioFeedbackEnabled);
                    if (audioFeedbackEnabled)
                        tappedObj.GetComponentInChildren<TextMesh>().text = "Audio Feedback:" + "\n" + "On";
                    else
                        tappedObj.GetComponentInChildren<TextMesh>().text = "Audio Feedback:" + "\n" + "Off";
                }
                else if (tappedObj.name == "ClickerButton")
                {
                    clickerEnabled = !(clickerEnabled);
                    if (audioFeedbackEnabled)
                        tappedObj.GetComponentInChildren<TextMesh>().text = "Clicker Enabled:" + "\n" + "On";
                    else
                        tappedObj.GetComponentInChildren<TextMesh>().text = "Clicker Enabled:" + "\n" + "Off";
                }
                else if (tappedObj.name == "BackButton")
                {
                    returnToStartMenu();
                }
            }
            else if (inMenu == 2) // Play Menu
            {
                if (tappedObj.name == "PlayButton")
                {
                    //Prepare UI
                    UtilitiesScript.Instance.disableObject(currentMenu);
                    DebugText.text = "Place your hand in right angle pose for 2 seconds ";
                    TextToSpeech.Instance.StartSpeaking(DebugText.text);
                    // Prepare Logic
                    flowController.startPlaying();
                }
                else if (tappedObj.name == "RightHandButton")
                {
                    rightHandEnabled = (!rightHandEnabled);
                    if (rightHandEnabled)
                        tappedObj.GetComponentInChildren<TextMesh>().text = "Right Hand:" + "\n" + "Yes";
                    else
                        tappedObj.GetComponentInChildren<TextMesh>().text = "Right Hand:" + "\n" + "No";
                }
                else if (tappedObj.name == "LeftHandButton")
                {
                    leftHandEnabled = (!leftHandEnabled);
                    if (rightHandEnabled)
                        tappedObj.GetComponentInChildren<TextMesh>().text = "Left Hand:" + "\n" + "Yes";
                    else
                        tappedObj.GetComponentInChildren<TextMesh>().text = "Left Hand:" + "\n" + "No";
                }

                else if (tappedObj.name == "SizeTreeButton")
                {
                    treeIsShort = (!treeIsShort);
                    if (treeIsShort)
                        tappedObj.GetComponentInChildren<TextMesh>().text = "Tree Height:" + "\n" + "Short";
                    else
                        tappedObj.GetComponentInChildren<TextMesh>().text = "Tree Height:" + "\n" + "Tall";
                }
                else if (tappedObj.name == "BackButton")
                {
                    returnToStartMenu();
                }
            }
            else if (inMenu == 4) // About Menu
            {
                if (tappedObj.name == "BackButton")
                {
                    returnToStartMenu();
                }
            }
        }
    }

    private void moveToAboutScreen()
    {
        UtilitiesScript.Instance.disableObject(currentMenu);
        if (aboutScreen == null) //Create Play menu
            aboutScreen = Instantiate(aboutPrefab, currentMenu.transform.position, currentMenu.transform.rotation);
        else
            UtilitiesScript.Instance.enableObject(aboutScreen);
        currentMenu = aboutScreen;
        inMenu = 4;
    }

    private void moveToPlayScreen()
    {
        UtilitiesScript.Instance.disableObject(currentMenu);
        if (playScreen == null) //Create Play menu
            playScreen = Instantiate(playPrefab, currentMenu.transform.position, currentMenu.transform.rotation);
        else
            UtilitiesScript.Instance.enableObject(playScreen);
        currentMenu = playScreen;
        inMenu = 2;
    }

    private void moveToSettingsScreen()
    {
        UtilitiesScript.Instance.disableObject(currentMenu);
        if (settingsScreen == null) //Create Settings menu
            settingsScreen = Instantiate(settingsPrefab, currentMenu.transform.position, currentMenu.transform.rotation);
        else
            UtilitiesScript.Instance.enableObject(settingsScreen);
        currentMenu = settingsScreen;
        inMenu = 1;
    }

    private void returnToStartMenu()
    {
        UtilitiesScript.Instance.disableObject(currentMenu);
        currentMenu = menuScreen;
        UtilitiesScript.Instance.enableObject(currentMenu);
        inMenu = 0;
    }

    public void createUI()
    {
        //First listen for taps
        gazeCursor.setGenericUse();
        EventManager.StartListening("tap", tapUiReceived);
        //Appear the menu in front of user
        placer.HideGridEnableOcclulsion();
        menuScreen = Instantiate(menuPrefab);
        Vector3 pos = Camera.main.transform.position + Camera.main.transform.forward * menuDistance;
        menuScreen.transform.position = pos;
        //Fix menu direction
        Vector3 directionToTarget = Camera.main.transform.position - pos;
        directionToTarget.y = 0.0f;
        if (directionToTarget.sqrMagnitude > 0.005f)
            menuScreen.transform.rotation = Quaternion.LookRotation(-directionToTarget);

        inMenu = 0;
        currentMenu = menuScreen;
    }

    public void calibrationMaxPose()
    {
        DebugText.text = "Raise your hand as high as you can. When ready open your palm";
        TextToSpeech.Instance.StartSpeaking(DebugText.text);
    }

    public void printText(string text)
    {
        DebugText.text = text;
    }

    public void prepareUserManipulation(bool righthand)
    {
        string text;
        if (righthand)
            text = "Next manipulation is with the right hand.";
        else
            text = "Next manipulation is with the left hand.";
        printText(text);
        TextToSpeech.Instance.StartSpeaking(text);
    }
}
