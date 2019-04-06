using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Xml;

public class ControllerMenuInputManager : MonoBehaviour
{
    Selectable currentSelectable, lastSelectable;

    [SerializeField]
    Button backButton, previousPageButton, nextPageButton;

    [SerializeField]
    Selectable defaultSelectable;

    [SerializeField]
    Text pageName, pageText;

    string[] pageTexts, pageNames;

    int currentPage = 0;

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
        if (defaultSelectable != null)
            defaultSelectable.Select();
        XmlDocument howToPlayDoc = new XmlDocument();
        howToPlayDoc.Load(Application.streamingAssetsPath + "/HowToPlayXML.xml");
        XmlNodeList textNodes = howToPlayDoc.SelectNodes("//Page");
        pageTexts = new string[textNodes.Count];
        pageNames = new string[textNodes.Count];
        for(int i = 0; i < pageTexts.Length; i++)
        {
            pageTexts[i] = textNodes[i].Attributes["Text"].Value;
            pageNames[i] = textNodes[i].Attributes["Name"].Value;
        }
    }

    private void Update()
    {
        GameObject currentSelectedGO = EventSystem.current.currentSelectedGameObject;
        if (currentSelectedGO == null)
        {
            if (lastSelectable != null)
            {
                lastSelectable.Select();
                currentSelectedGO = lastSelectable.gameObject;
            }
            else
                return;
        }
        currentSelectable = currentSelectedGO.GetComponent<Selectable>();
        if (currentSelectable is Slider && Input.GetAxis("Horizontal") != 0f)
        {
            (currentSelectable as Slider).value += Input.GetAxis("Horizontal");
        }
        else if (currentSelectable is InputField && Input.GetAxisRaw("Vertical") != 0f)
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

    public void NewPage(int next)
    {
        currentPage += Mathf.Clamp(next, -1, 1);
        if(currentPage <= 0)
        {
            previousPageButton.gameObject.SetActive(false);
            backButton.Select();
        }
        else
        {
            previousPageButton.gameObject.SetActive(true);
        }
        if (currentPage >= pageTexts.Length - 1)
        {
            nextPageButton.gameObject.SetActive(false);
            backButton.Select();
        }
        else
        {
            nextPageButton.gameObject.SetActive(true);
        }
        pageName.text = pageNames[currentPage];
        pageText.text = pageTexts[currentPage];
    }

    public void LeaveHowToPlay()
    {
        currentPage = 0;
        NewPage(0);
    }

    public void Select(GameObject newSelectableGO)
    {
        newSelectableGO.GetComponent<Selectable>().Select();
    }
}