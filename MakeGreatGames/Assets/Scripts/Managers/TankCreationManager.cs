using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TankCreationManager : MonoBehaviour
{
    [SerializeField]
    GameObject tankPrefab, confirmExitMenu;

    MeshFilter prefabTankBaseMesh, prefabTankTowerMesh, previewTankBaseMesh, previewTankTowerMesh;

    int baseIndex = 0, towerIndex = 0, currentCollection = 0, baseMatIndex = 0, towerMatIndex = 0;

    Mesh[][] allMeshes = new Mesh[2][];

    Material[][][] allMats = new Material[2][][];

    private void Awake()
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

        prefabTankBaseMesh.GetComponent<TankScript>().enabled = false;

        GameObject previewTank = Instantiate(tankPrefab);

        previewTankBaseMesh = previewTank.GetComponentInChildren<MeshFilter>();
        previewTankTowerMesh = previewTankBaseMesh.GetComponentInChildren<MeshFilter>();

        foreach (ParticleSystem p in previewTankBaseMesh.GetComponentsInChildren<ParticleSystem>())
            p.Stop();

        previewTank.GetComponentInChildren<Canvas>().gameObject.SetActive(false);
    }

    public void ChangeMat(bool next)
    {

    }

    public void ChangeBaseMesh(bool next)
    {

    }

    public void ChangeTowerMesh(bool next)
    {

    }

    public void ReturnButtonPressed()
    {
        if (
            prefabTankBaseMesh.sharedMesh != previewTankBaseMesh.mesh
            || prefabTankTowerMesh.sharedMesh != previewTankTowerMesh.mesh
            || prefabTankBaseMesh.GetComponent<MeshRenderer>().material != previewTankBaseMesh.GetComponent<MeshRenderer>().material
            || prefabTankTowerMesh.GetComponent<MeshRenderer>().material != previewTankTowerMesh.GetComponent<MeshRenderer>().material
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