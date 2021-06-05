using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MainMenuController : MonoBehaviour
{
    [Header("References")]
    private GameObject activeGameObject;
    [SerializeField] private GameObject main;
    [SerializeField] private GameObject level;
    private Dictionary<MainMenuState, GameObject> statesObjects;

    [Header("Parameters")]
    [SerializeField] private MainMenuState state = MainMenuState.Main;

    private void Awake()
    {
        statesObjects = new Dictionary<MainMenuState, GameObject>();
        statesObjects.Add(MainMenuState.Main, main);
        statesObjects.Add(MainMenuState.Level, level);
        activeGameObject = statesObjects[state];
        SwitchState((int)state);
    }

    public void SwitchState(int stateId)
    {
        state = (MainMenuState)stateId;
        activeGameObject.SetActive(false);
        activeGameObject = statesObjects[state];
        activeGameObject.SetActive(true);
    }
}
