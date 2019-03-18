using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankCreationManager : MonoBehaviour
{
    [SerializeField]
    GameObject testObject;

    int baseIndex = 0, towerIndex = 0, currentCollection = 0;

    Mesh[][] allMeshes = new Mesh[2][];

    Material[][][] allMats = new Material[2][][];

    private void Awake()
    {
        Object[] loadedBaseMeshes = Resources.LoadAll("Meshes/Bases");
        Object[] loadedTowerMeshes = Resources.LoadAll("Meshes/Towers");
        
        print(loadedBaseMeshes.Length);

        allMeshes[0] = new Mesh[loadedBaseMeshes.Length];
        allMeshes[1] = new Mesh[loadedTowerMeshes.Length];

        for (int i = 0; i < loadedBaseMeshes.Length; i++)
            allMeshes[0][i] = (loadedBaseMeshes[i] as GameObject).GetComponent<MeshFilter>().sharedMesh;

        for (int i = 0; i < loadedTowerMeshes.Length; i++)
            allMeshes[1][i] = (loadedTowerMeshes[i] as GameObject).GetComponent<MeshFilter>().sharedMesh;

        for (int i = 0; i < allMeshes.Length; i++)
            print(allMeshes[i].Length);

        testObject.GetComponent<MeshFilter>().mesh = allMeshes[0][0];
    }

    public void ViewNext(int next)
    {

    }

    public void ViewMesh(int collection, int index)
    {

    }

    public void ChangeCollection(int next)
    {

    }
}