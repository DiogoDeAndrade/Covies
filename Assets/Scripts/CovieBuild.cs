using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CovieBuild
{
    public float    genHeight = 1.75f;
    [Range(0.0f, 1.0f)]
    public float    legProportion = 0.5f;
    [Range(0.0f, 1.0f)]
    public float    bodyProportion = 0.4f;
    [Range(0.0f, 1.0f)]
    public float    headProportion = 0.1f;
    [MinMaxSlider(0.0f, 1.0f)]
    public Vector2  radius = new Vector2(0.5f, 0.25f);

    static public (Mesh, Material) Generate(CovieBuild p)
    {
        Mesh mesh = new Mesh();




        Material material = new Material(Shader.Find("Universal Render Pipeline/Lit"));

        return (mesh, material);
    }
}
