using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class CovieGen : MonoBehaviour
{
    public bool     autoRefresh;
    public Covie    covieParameters;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    [Button("Generate")]
    void Generate()
    {
        Mesh        mesh;
        Material    material;

        (mesh, material) = covieParameters.Generate();

        MeshFilter meshFilter = GetComponent<MeshFilter>();
        if (meshFilter == null)
        {
            meshFilter = gameObject.AddComponent<MeshFilter>();
        }
        meshFilter.sharedMesh = mesh;

        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer == null) 
        { 
            meshRenderer= gameObject.AddComponent<MeshRenderer>();
        }
        meshRenderer.sharedMaterial = material;
    }

    private void OnValidate()
    {
        if (autoRefresh) Generate();
    }

    private void OnDrawGizmosSelected()
    {
        // Display normals
        Mesh mesh = GetComponent<MeshFilter>().sharedMesh;
        if (mesh == null) return;

        var prevMatrix = Gizmos.matrix;

        var vertex = mesh.vertices;
        var normal = mesh.normals;
        var tris = mesh.triangles;

        /*Gizmos.matrix = transform.localToWorldMatrix;
        for (int i = 0; i < vertex.Length; i++)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(vertex[i], vertex[i] + normal[i]);
        }//*/

        /*int tri_start = 0;
        int tri_range = 2;
        for (int tri_idx = tri_start; tri_idx < Mathf.Min(tri_start + tri_range, tris.Length / 3); tri_idx++)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(vertex[tris[tri_idx * 3 + 0]], vertex[tris[tri_idx * 3 + 1]]);
            Gizmos.DrawLine(vertex[tris[tri_idx * 3 + 1]], vertex[tris[tri_idx * 3 + 2]]);
            Gizmos.DrawLine(vertex[tris[tri_idx * 3 + 2]], vertex[tris[tri_idx * 3 + 0]]);
        }//*/

        Gizmos.matrix = prevMatrix;
    }
}
