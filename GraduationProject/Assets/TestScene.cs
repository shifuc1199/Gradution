/*****************************
Created by 师鸿博
*****************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScene : MonoBehaviour
{
    public Material mat;
    public Texture2D tex;
    public CanvasRenderer reder;
    public float a;
    public float b;
    public float c;
    public float d;
    public float e;
    // Start is called before the first frame update
    void Start()
    {
        reder = GetComponent<CanvasRenderer>();

        reder.SetMaterial(mat, tex);


    }

    // Update is called once per frame
    void Update()
    {
        Mesh mesh = new Mesh();

        Vector3[] verts = new Vector3[6]
        {
            new Vector3(0, 0, 0),
            new Vector3(0, a, 0),
            new Vector3(b, 20, 0),
            new Vector3(c/2+7, -c, 0),
            new Vector3(-d/2-7, -d, 0),
            new Vector3(-e, 20, 0),
        };
        Vector2[] uv = new Vector2[6]
       {
           new Vector2(0, 0),
            new Vector2(1, 1),
            new Vector2(1, 1),
            new Vector2(1, 1),
            new Vector2(1, 1),
            new Vector2(1, 1),
       };

        int[] tris = new int[15] { 0, 1, 5, 0, 1, 2, 0, 2, 3, 0, 3, 4, 0, 4, 5 };
        mesh.vertices = verts;
        mesh.uv = uv;
        mesh.triangles = tris;
        reder.SetMesh(mesh);


    }
}
