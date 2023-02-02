using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class CovieGen : MonoBehaviour
{
    public CovieBuild   covieParameters;

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

        (mesh, material) = CovieBuild.Generate(covieParameters);
    }


}
