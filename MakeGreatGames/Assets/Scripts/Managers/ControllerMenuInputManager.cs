using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

[Serializable]
struct Selectables
{
    [SerializeField]
    Selectable[] selectables;
}

public class ControllerMenuInputManager : MonoBehaviour
{
    [SerializeField]
    Selectables[] allElements;

    int[] currentSelectedElements;

    int currentElementArr = 0;

    static ControllerMenuInputManager instance;

    public static ControllerMenuInputManager Instance
    {
        get { return instance; }
    }

    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(this);
        instance = this;
    }

    private void Start()
    {
        currentSelectedElements = new int[allElements.Length];
    }

    private void Update()
    {

    }

    public void ChangeCurrentElementArr()
    {

    }
}