using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Rendering;

[RequireComponent(typeof(MeshCollider))]
[RequireComponent(typeof(MeshFilter))]
public class MarchingCubeMeshNew : MonoBehaviour
{

    //properties shit
    public float IsoSurface { get { return marchShader.IsoSurface; } set { marchShader.IsoSurface = value; marchDirtyFlag = true; } }

    //Mesh shit
    private MeshFilter meshFilter;
    private MeshCollider meshCollider;
    private Mesh mesh;
    private Vector3[] optimizedVerts;
    private int[] optimizedTris;

    //Shader shit
    PaintPointCloudShaderAPI paintShader;
    MarchingCubeShaderAPI marchShader;
    private float[] pointCloud;
    private Vector3[] verts;
    private int[] tris;
    private bool marchDirtyFlag = false;

    //Unity callbacks
    void OnDrawGizmos()
    {
        Gizmos.color = new Color(0, 0, 1, 0.05f);
        Gizmos.DrawCube(transform.position + Vector3.one * 4, new Vector3(8, 8, 8));
    }
    void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshCollider = GetComponent<MeshCollider>();

        //Inizialize
        mesh = new Mesh();
        meshFilter.sharedMesh = mesh;
        paintShader = new PaintPointCloudShaderAPI(transform.position);
        marchShader = new MarchingCubeShaderAPI(out pointCloud, out verts, out tris);
    }
    private void Update()
    {
        if(marchDirtyFlag)
        {
            marchDirtyFlag = false;
            UpdateMarch();
            CleanVerts();
            meshFilter.sharedMesh.Clear();
            meshFilter.sharedMesh.vertices = optimizedVerts;
            meshFilter.sharedMesh.triangles = optimizedTris;
            meshFilter.sharedMesh.RecalculateNormals();
            meshCollider.sharedMesh = meshFilter.sharedMesh;
        }
    }
    public void OnDestroy()
    {
        paintShader.Release();
        marchShader.Release();
    }
    //March functions
    private void UpdateMarch()
    {
        marchShader.MarchCloud(ref pointCloud, ref verts, ref tris);
    }
    private void CleanVerts()
    {
        int lastRealIndex = 0;
        for (int i = 0; i < verts.Length; i++)
        {
            if (verts[i].x >= 0)
            {
                verts[lastRealIndex] = verts[i];
                lastRealIndex++;
            }
        }

        //Make smaller
        optimizedVerts = new Vector3[lastRealIndex];
        optimizedTris = new int[lastRealIndex];
        Array.Copy(verts, optimizedVerts, lastRealIndex);
        Array.Copy(tris, optimizedTris, lastRealIndex);
    }
   
    //Paint functions
    public void PaintGPU(Vector3 brushWorldPoint, float brushRadius, float amount)
    {
        paintShader.PaintCloud(ref pointCloud, brushWorldPoint, brushRadius, amount);
        marchDirtyFlag = true;
    }
    public void PaintPointCloud(Vector3Int point, float amount)
    {
        int size = 8; // this is not alwas the case this is hard coded for a 8x8 chuck pattern
        int id = point.x + ((size + 1) * point.y) + ((size + 1) * (size + 1) * point.z);
        pointCloud[id] += amount;
        marchDirtyFlag = true;
    }
    public void SetPointCloud(Vector3Int point, float value)
    {
        int size = 8; // this is not alwas the case this is hard coded for a 8x8 chuck pattern
        int id = point.x + ((size + 1) * point.y) + ((size + 1) * (size + 1) * point.z);
        pointCloud[id] = value;
        marchDirtyFlag = true;
    }
}
