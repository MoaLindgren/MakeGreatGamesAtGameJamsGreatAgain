using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ControllerMenuInputManager : MonoBehaviour
{
    [SerializeField]
    GameObject defaultSelectable;

    Selectable currentSelectable;

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
        currentSelectable = defaultSelectable.GetComponent<Selectable>();
        EventSystem.current.SetSelectedGameObject(currentSelectable.gameObject);
    }

    private void Update()
    {
        print(currentSelectable.name);
        currentSelectable = EventSystem.current.currentSelectedGameObject.GetComponent<Selectable>();
        if (Input.GetButtonDown("Submit") && currentSelectable is Button)
        {
            (currentSelectable as Button).onClick.Invoke();
        }
        else if(currentSelectable is Slider && Input.GetAxis("Horizontal") != 0f)
        {
            (currentSelectable as Slider).value += Input.GetAxis("Horizontal");
        }
        else if (Input.GetButtonDown("Cancel"))
        {

        }
    }
}