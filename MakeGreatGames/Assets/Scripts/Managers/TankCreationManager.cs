﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TankCreationManager : MonoBehaviour//, IPointerUpHandler, IPointerDownHandler
{
    [SerializeField]
    GameObject tankPrefab, confirmExitMenu;

    [SerializeField]
    Button rotLeftButton, rotRightButton;

    MeshFilter prefabTankBaseMesh, prefabTankTowerMesh, previewTankBaseMesh, previewTankTowerMesh;

    int baseIndex = 0, towerIndex = 0, baseMatIndex = 0, towerMatIndex = 0;

    Mesh[][] allMeshes = new Mesh[2][];

    Dictionary<string, Material[]>[] allMats = new Dictionary<string, Material[]>[2];

    GameObject previewTank;

    float rotAmount = 0f;

    private void Start()
    {
        Object[] loadedBaseMeshes = Resources.LoadAll("Meshes/Bases");
        Object[] loadedTowerMeshes = Resources.LoadAll("Meshes/Towers");

        allMeshes[0] = new Mesh[loadedBaseMeshes.Length];
        allMeshes[1] = new Mesh[loadedTowerMeshes.Length];

        for (int i = 0; i < loadedBaseMeshes.Length; i++)
            allMeshes[0][i] = (loadedBaseMeshes[i] as GameObject).GetComponent<MeshFilter>().sharedMesh;

        for (int i = 0; i < loadedTowerMeshes.Length; i++)
            allMeshes[1][i] = (loadedTowerMeshes[i] as GameObject).GetComponent<MeshFilter>().sharedMesh;

        prefabTankBaseMesh = tankPrefab.GetComponentInChildren<MeshFilter>();
        prefabTankTowerMesh = prefabTankBaseMesh.GetComponentInChildren<MeshFilter>();

        previewTank = Instantiate(tankPrefab);

        previewTankBaseMesh = previewTank.GetComponentInChildren<MeshFilter>();
        previewTankTowerMesh = previewTankBaseMesh.GetComponentInChildren<MeshFilter>();

        previewTankBaseMesh.GetComponentInParent<TankScript>().enabled = false;

        foreach (ParticleSystem p in previewTankBaseMesh.GetComponentsInChildren<ParticleSystem>())
            p.Stop();

        previewTank.GetComponentInChildren<Canvas>().gameObject.SetActive(false);

        allMats[0] = new Dictionary<string, Material[]>();
        allMats[1] = new Dictionary<string, Material[]>();

        foreach (Mesh baseMesh in loadedBaseMeshes)
        {
            allMats[0].Add(baseMesh.name, (Material[])Resources.LoadAll("Materials/Bases/" + baseMesh.name, typeof(Material)));
        }
        foreach (Mesh towerMesh in loadedTowerMeshes)
        {
            allMats[1].Add(towerMesh.name, (Material[])Resources.LoadAll("Materials/Towers/" + towerMesh.name, typeof(Material)));
        }

    }

    private void Update()
    {
        if (rotAmount != 0f)
            previewTank.transform.Rotate(0f, rotAmount * Time.deltaTime, 0f);
    }

    /*

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
            rotAmount = 0f;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
            if (eventData.selectedObject == rotLeftButton)
                rotAmount = 50f;
            else if (eventData.selectedObject == rotLeftButton)
                rotAmount = -50f;
    }

    */

    public void ChangeBaseMat(int next)
    {

    }

    public void ChangeTowerMat(int next)
    {
        towerMatIndex = 0;
    }

    public void ChangeBaseMesh(int next)
    {
        baseMatIndex = 0;
    }

    public void ChangeTowerMesh(int next)
    {

    }

    public void RotateTank(float amount)
    {
        rotAmount = amount;
    }

    public void ReturnButtonPressed()
    {
        if (
            prefabTankBaseMesh.sharedMesh != previewTankBaseMesh.sharedMesh
            || prefabTankTowerMesh.sharedMesh != previewTankTowerMesh.sharedMesh
            || prefabTankBaseMesh.GetComponent<MeshRenderer>().sharedMaterial != previewTankBaseMesh.GetComponent<MeshRenderer>().sharedMaterial
            || prefabTankTowerMesh.GetComponent<MeshRenderer>().sharedMaterial != previewTankTowerMesh.GetComponent<MeshRenderer>().sharedMaterial
           )
        {
            ShowConfirmExitMenu(true);
        }
        else
            ReturnToMainMenu();
    }

    public void ShowConfirmExitMenu(bool show)
    {
        confirmExitMenu.SetActive(show);
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void SaveSettings()
    {
        prefabTankBaseMesh.sharedMesh = previewTankBaseMesh.mesh;
        prefabTankBaseMesh.GetComponent<MeshRenderer>().material = previewTankBaseMesh.GetComponent<MeshRenderer>().material;
        prefabTankTowerMesh.sharedMesh = prefabTankTowerMesh.mesh;
        prefabTankTowerMesh.GetComponent<MeshRenderer>().material = previewTankTowerMesh.GetComponent<MeshRenderer>().material;
    }
}