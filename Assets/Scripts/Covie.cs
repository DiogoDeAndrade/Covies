using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Covie
{
    [Header("Geometry")]
    public float    genHeight = 1.75f;
    [Range(1.0f, 100.0f)]
    public float    age = 20.0f;
    public int      randomSeed = 1234;
    [Range(0.0f, 1.0f)]
    public float    legProportion = 0.5f;
    [Range(0.0f, 1.0f)]
    public float    legWidthProportion = 1.0f;
    [Range(0.0f, 1.0f)]
    public float    bodyProportion = 0.4f;
    [Range(0.0f, 1.0f)]
    public float    headProportion = 0.1f;
    [MinMaxSlider(0.0f, 1.0f), Tooltip("Radius of character (Z radius, X radius)")]
    public Vector2  radius = new Vector2(0.25f, 0.5f);
    [Range(-1.0f, 1.0f)]
    public float    fatFactor = 0.0f;
    [Header("Material")]
    public Color    skinColor = Color.white;
    public bool     usePants;
    public Color    pantColor = Color.blue;
    [Range(0.0f, 1.0f)]
    public float    pantProportion = 0.8f;
    public bool     useShirt;
    public Color    shirtColor = Color.green;
    public Color    hairColor = Color.black;
    public bool     hairShine = false;
    [Range(0.0f, 1.0f)]
    public float    hairLengthBack = 0.7f;
    [Range(0.0f, 0.25f)]
    public float    hairLengthFront = 0.0f;
    [Range(0.15f, 0.5f)]
    public float    hairStart = 0.15f;

    public (Mesh, Material) Generate()
    {
        System.Random   rnd = new System.Random(randomSeed);
        int             subdiv = 16;

        List<Vector3>   vertices = new List<Vector3>();
        List<Vector2>   uv = new List<Vector2>();
        List<int>       triangles = new List<int>();
        int             baseIndex;
        float           angleInc, angle;
        float           ageScale = Mathf.Pow(Mathf.Clamp01(age / 20.0f), 0.5f);
        float           scale = Mathf.Lerp(0.2f, 1.0f, ageScale);
        float           actualHeight = genHeight * scale;
        float           actualFatFactor = Mathf.Lerp(fatFactor + 0.5f, fatFactor, age / 10.0f);

        // Build bottom cap
        vertices.Add(Vector3.zero);
        uv.Add(new Vector2(0.5f, 1.0f));

        baseIndex = vertices.Count;
        angleInc = (Mathf.PI * 2.0f) / subdiv;
        angle = 0.0f;

        Vector3 legRadius = legWidthProportion * radius * scale;
        for (int i = 0; i < subdiv; i++)
        {
            vertices.Add(new Vector3(legRadius.y * Mathf.Sin(angle), 0.0f, legRadius.x * Mathf.Cos(angle)));
            uv.Add(new Vector2((float)i / (float)subdiv, 1.0f));
            angle += angleInc;
        }

        for (int i = 0; i < subdiv; i++)
        {
            triangles.Add(0);
            triangles.Add(baseIndex + (i + 1) % subdiv);
            triangles.Add(baseIndex + i);
        }//*/

        // Build "legs" (bottom part, just a cylinder) - Must have one more segment because of the UV wrap
        baseIndex = vertices.Count;
        angleInc = (Mathf.PI * 2.0f) / subdiv;
        angle = 0.0f;
        for (int i = 0; i <= subdiv; i++)
        {
            var v = new Vector3(legRadius.y * Mathf.Sin(angle), 0.0f, legRadius.x * Mathf.Cos(angle));
            vertices.Add(v);
            uv.Add(new Vector2(i / (float)subdiv, 1.0f));
            angle += angleInc;
        }
        angle = 0.0f;
        for (int i = 0; i <= subdiv; i++)
        {
            var v = new Vector3(legRadius.y * Mathf.Sin(angle), legProportion * actualHeight, legRadius.x * Mathf.Cos(angle));
            vertices.Add(v);
            angle += angleInc;
            uv.Add(new Vector2(i / (float)subdiv, 0.75f));
        }

        for (int i = 0; i < subdiv; i++)
        {
            triangles.Add(baseIndex + i);
            triangles.Add(baseIndex + i + 1 + (subdiv + 1));
            triangles.Add(baseIndex + i + (subdiv + 1));

            triangles.Add(baseIndex + i);
            triangles.Add(baseIndex + i + 1);
            triangles.Add(baseIndex + i + 1 + (subdiv + 1));
        }

        // Build body (middle part, thin or fat cylinder) - Also needs one more segment because of the UV wrap
        int     bodySubdivs = 5;
        float   y = legProportion * actualHeight;
        float   yInc = (bodyProportion * actualHeight) / (bodySubdivs - 1);

        baseIndex = vertices.Count;
        angleInc = (Mathf.PI * 2.0f) / subdiv;

        for (int j = 0; j < bodySubdivs; j++)
        {
            angle = 0.0f;
            float t = (j / (float)(bodySubdivs - 1));
            float uvV = 0.75f - 0.35f * t;
            float fat = Mathf.Sin(t * Mathf.PI);

            Vector3 segmentRadius = (radius + (radius * actualFatFactor) * fat) * scale;

            for (int i = 0; i <= subdiv; i++)
            {
                var v = new Vector3(segmentRadius.y * Mathf.Sin(angle), y, segmentRadius.x * Mathf.Cos(angle));
                vertices.Add(v);
                uv.Add(new Vector2(i / (float)subdiv, uvV));
                angle += angleInc;
            }

            y += yInc;
        }

        for (int i = 0; i < subdiv; i++)
        {
            int innerIndex = baseIndex - (subdiv + 1);
            int outerIndex = baseIndex;

            triangles.Add(innerIndex + i);
            triangles.Add(outerIndex + i + 1);
            triangles.Add(outerIndex + i);

            triangles.Add(innerIndex + i);
            triangles.Add(innerIndex + i + 1);
            triangles.Add(outerIndex + i + 1);
        }

        for (int j = 0; j < bodySubdivs - 1; j++)
        {
            int segmentIndex = baseIndex + j * (subdiv + 1);
            for (int i = 0; i < subdiv; i++)
            {
                triangles.Add(segmentIndex + i);
                triangles.Add(segmentIndex + i + 1 + (subdiv + 1));
                triangles.Add(segmentIndex + i + (subdiv + 1));

                triangles.Add(segmentIndex + i);
                triangles.Add(segmentIndex + i + 1);
                triangles.Add(segmentIndex + i + 1 + (subdiv + 1));
            }
        }

        // Build head (top part, cylinder) - Also needs one more segment because of the UV wrap
        int     headSubdivs = 8;
        Vector3 headRadius = (radius * 0.25f + radius * actualFatFactor * 0.05f) * scale;
        
        y = (legProportion + bodyProportion) * actualHeight;
        yInc = (headProportion * actualHeight) / (headSubdivs - 1);

        baseIndex = vertices.Count;
        angleInc = (Mathf.PI * 2.0f) / subdiv;

        for (int j = 0; j < headSubdivs; j++)
        {
            angle = 0.0f;
            float t = (j / (float)(headSubdivs - 1));
            float uvV = 0.4f - 0.4f * t;
            float fat = Mathf.Sin(t * Mathf.PI);
            if (j < 2) fat = 0.0f;

            Vector3 segmentRadius = headRadius + headRadius * fat;

            for (int i = 0; i <= subdiv; i++)
            {
                var v = new Vector3(segmentRadius.y * Mathf.Sin(angle), y, segmentRadius.x * Mathf.Cos(angle));
                vertices.Add(v);
                uv.Add(new Vector2(i / (float)subdiv, uvV));
                angle += angleInc;
            }

            y += yInc;
        }

        for (int i = 0; i < subdiv; i++)
        {
            int innerIndex = baseIndex - (subdiv + 1);
            int outerIndex = baseIndex;

            triangles.Add(innerIndex + i);
            triangles.Add(outerIndex + i + 1);
            triangles.Add(outerIndex + i);

            triangles.Add(innerIndex + i);
            triangles.Add(innerIndex + i + 1);
            triangles.Add(outerIndex + i + 1);
        }

        for (int j = 0; j < headSubdivs - 1; j++)
        {
            int segmentIndex = baseIndex + j * (subdiv + 1);
            for (int i = 0; i < subdiv; i++)
            {
                triangles.Add(segmentIndex + i);
                triangles.Add(segmentIndex + i + 1 + (subdiv + 1));
                triangles.Add(segmentIndex + i + (subdiv + 1));

                triangles.Add(segmentIndex + i);
                triangles.Add(segmentIndex + i + 1);
                triangles.Add(segmentIndex + i + 1 + (subdiv + 1));
            }
        }

        // Top of head
        baseIndex = vertices.Count;
        vertices.Add(new Vector3(0.0f, actualHeight * (legProportion + bodyProportion + headProportion), 0.0f));
        uv.Add(new Vector2(0.5f, 0.0f));

        int topIndex = baseIndex - (subdiv + 1);
        for (int i = 0; i < subdiv; i++)
        {
            triangles.Add(baseIndex);
            triangles.Add(topIndex + i);
            triangles.Add(topIndex + i + 1);
        }

        // Create mesh
        Mesh mesh = new Mesh();
        mesh.name = "CovieMesh";
        mesh.SetVertices(vertices);
        mesh.SetUVs(0, uv);
        mesh.SetTriangles(triangles, 0);

        MeshTools.ComputeNormalsAndTangentSpaceWelded(mesh, true);

        int width = 256;
        int height = 256;

        Bitmap albedoBitmap = new Bitmap(width, height);
        Bitmap metallicBitmap = new Bitmap(width, height);
        albedoBitmap.Fill(skinColor);
        metallicBitmap.Fill(new Color(0, 0, 0, 0));
        if (usePants)
        {
            albedoBitmap.RectAlpha(0, (int)(0.75f * height), width, (int)(0.25f * height * pantProportion), pantColor);
        }
        if (useShirt)
        {
            albedoBitmap.RectAlpha(0, (int)(0.40f * height) - 2, width, (int)(height * 0.35f) + 2, shirtColor);
        }

        // Head
        int pixelHeightHead = (int)(height * 0.4f) - 5;
        albedoBitmap.RectAlpha(0, 0, width, pixelHeightHead, skinColor);
        // Hair
        Color actualHairColor = Color.Lerp(hairColor, Color.white, (age - 40) / 20);
        float actualHairLengthFront = Mathf.Lerp(hairLengthFront, 0, (age - 45) / 25);
        float actualHairLengthBack = Mathf.Lerp(hairLengthBack, 0, (age - 50) / 25);
        float actualHairStart = Mathf.Lerp(hairStart, 0.3f, (age - 45) / 25);
        Color metallicHair = (hairShine) ? (new Color(0.75f, 0.0f, 0.0f, 0.5f)) : (new Color(0.25f, 0.0f, 0.0f, 0.25f));
        metallicHair = Color.Lerp(metallicHair, new Color(0.75f, 0.0f, 0.0f, 0.5f), (age - 40) / 20);

        albedoBitmap.RectAlpha((int)(width * actualHairStart), 0, (int)(width * (1.0f - actualHairStart * 2.0f)), (int)(pixelHeightHead * actualHairLengthBack), actualHairColor);
        metallicBitmap.Rect((int)(width * actualHairStart), 0, (int)(width * (1.0f - actualHairStart * 2.0f)), (int)(pixelHeightHead * actualHairLengthBack), metallicHair);
        if (actualHairLengthFront > 0.0f)
        {
            albedoBitmap.RectAlpha(0, 0, width, (int)(pixelHeightHead * actualHairLengthFront), actualHairColor);
            metallicBitmap.Rect(0, 0, width, (int)(pixelHeightHead * actualHairLengthFront), metallicHair);
        }

        Texture2D albedoTexture = albedoBitmap.ToTexture("CovieAlbedo", FilterMode.Point, TextureWrapMode.Repeat, TextureWrapMode.Clamp);
        Texture2D metallicTexture = metallicBitmap.ToTexture("CovieMetallic", FilterMode.Bilinear);

        Material material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        material.name = "CovieMaterial";

        material.SetFloat("_Smoothness", 1.0f);
        material.SetTexture("_BaseMap", albedoTexture);
        material.SetTexture("_MetallicGlossMap", metallicTexture);

        material.EnableKeyword("_METALLICSPECGLOSSMAP");

        return (mesh, material);
    }

    static public Color HexToColor(string color)
    {
        Color ret;
        if (ColorUtility.TryParseHtmlString(color, out ret))
        {
            return ret;
        }

        return Color.white;
    }
}
