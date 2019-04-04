using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ControllerMenuInputManager : MonoBehaviour
{
    Selectable currentSelectable, lastSelectable;

    [SerializeField]
    Button backButton;

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

    private void Update()
    {
        GameObject currentSelectedGO = EventSystem.current.currentSelectedGameObject;
        if (currentSelectedGO == null)
        {
            lastSelectable.Select();
            currentSelectedGO = lastSelectable.gameObject;
        }
        currentSelectable = currentSelectedGO.GetComponent<Selectable>();
        if (currentSelectable is Slider && Input.GetAxis("Horizontal") != 0f)
        {
            (currentSelectable as Slider).value += Input.GetAxis("Horizontal");
        }
        else if(currentSelectable is InputField && Input.GetAxisRaw("Vertical") != 0f)
        {
            (currentSelectable as InputField).DeactivateInputField();
        }
        else if (Input.GetButtonDown("Cancel"))
        {
            backButton.onClick.Invoke();
        }
        lastSelectable = currentSelectable;
    }

    public void SetBackButton(GameObject newBackButton)
    {
        backButton = newBackButton.GetComponent<Button>();
    }
}