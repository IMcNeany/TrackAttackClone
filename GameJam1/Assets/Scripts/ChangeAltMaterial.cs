using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeAltMaterial : MonoBehaviour
{
    public List<MeshRenderer> meshes = new List<MeshRenderer>();
    
    public void SetMaterials(Material mat)
    {
        foreach(MeshRenderer mesh in meshes)
        {
            Material[] mats = mesh.materials;
            mats[1] = mat;

            mesh.materials = mats;
        }
    }
}
