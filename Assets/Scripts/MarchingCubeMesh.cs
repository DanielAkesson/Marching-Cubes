using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
[RequireComponent(typeof(MeshCollider))]
[RequireComponent(typeof(MeshFilter))]
public class MarchingCubeMesh : MonoBehaviour
{
    //properties shit
    private float _isoSurface;
    public float IsoSurface { get { return _isoSurface; } set { _isoSurface = value; dirty_flag = true; } }

    //Mesh shit
    private MeshFilter meshFilter;
    private MeshCollider meshCollider;
    private Mesh mesh;
    private Vector3[] verts;
    private int[] tris;

    //Compressing
    private Vector3[] smallVerts;
    private int[] smallTris;

    //Shader shit
    private const int groupSize = 1;
    private ComputeShader shader;
    private ComputeBuffer pointCloud_buf;
    private ComputeBuffer vert_buf;
    private ComputeBuffer tris_buf;

    //paint shit
    private float[] PointCloud;
    private bool dirty_flag = true;

    private int TempLog = 0;

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(0, 0, 1, 0.5f);
        Gizmos.DrawCube(transform.position + Vector3.one * 4, new Vector3(8, 8, 8));
    }
    void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshCollider = GetComponent<MeshCollider>();
        //Inizialize
        mesh = new Mesh();
        meshFilter.mesh = mesh;
        shader = ComputeShader.Instantiate(Resources.Load<ComputeShader>("Shaders/MarchingCubeShader"));
        int dim = 8 * groupSize;
        PointCloud = new float[(dim + 1) * (dim + 1) * (dim + 1)];
        verts = new Vector3[dim * dim * dim * 15];
        tris = new int[dim * dim * dim * 15];

        //Set up shader
        shader.SetFloat("IsoSurface", _isoSurface);
        pointCloud_buf = new ComputeBuffer(PointCloud.Length * sizeof(float), sizeof(float), ComputeBufferType.Default);
        vert_buf = new ComputeBuffer(verts.Length * sizeof(float) * 3, sizeof(float) * 3, ComputeBufferType.Default);
        tris_buf = new ComputeBuffer(tris.Length * sizeof(int), sizeof(int), ComputeBufferType.Default);
        shader.SetBuffer(shader.FindKernel("CSMain"), "PointCloud", pointCloud_buf);
        shader.SetBuffer(shader.FindKernel("CSMain"), "Verts", vert_buf);
        shader.SetBuffer(shader.FindKernel("CSMain"), "Tris", tris_buf);
    }
    private void Update()
    {
        if (!dirty_flag)
            return;

        dirty_flag = false;
        UpdateShader();
        vert_buf.GetData(verts);
        tris_buf.GetData(tris);
        CleanVerts();

        //Set values
        meshFilter.sharedMesh.Clear();
        meshFilter.sharedMesh.vertices = smallVerts;
        meshFilter.sharedMesh.triangles = smallTris;
        meshFilter.sharedMesh.RecalculateNormals();
        meshCollider.sharedMesh = meshFilter.sharedMesh;
    }
    public void OnDestroy()
    {
        pointCloud_buf.Release();
        vert_buf.Release();
        tris_buf.Release();
    }
    private void UpdateShader()
    {
        shader.SetFloat("IsoSurface", _isoSurface);
        pointCloud_buf.SetData(PointCloud);
        shader.Dispatch(shader.FindKernel("CSMain"), groupSize, groupSize, groupSize);
    }
    private void CleanVerts()
    {
        int lastRealIndex = 0;
        for(int i=0;i<verts.Length;i++)
        {
            if (verts[i].x >= 0)
            {
                verts[lastRealIndex] = verts[i];
                lastRealIndex++;
            }
        }

        //Make smaller
        smallVerts = new Vector3[lastRealIndex];
        smallTris = new int[lastRealIndex];
        Array.Copy(verts, 0, smallVerts, 0, lastRealIndex);
        Array.Copy(tris, 0, smallTris, 0, lastRealIndex);
    }
    public void PaintPointCPU(float amount, Vector3 point)
    {
        int dim = 8 * groupSize + 1;
        Vector3 localSpace = point - transform.position;
        Vector3Int localSpaceRound = new Vector3Int(Mathf.RoundToInt(localSpace.x), Mathf.RoundToInt(localSpace.y), Mathf.RoundToInt(localSpace.z));
        if (localSpaceRound.x < 0 || localSpaceRound.x >= dim || localSpaceRound.y < 0 || localSpaceRound.y >= dim || localSpaceRound.z < 0 || localSpaceRound.z >= dim)
            return;

        int index = (int)localSpaceRound.x + dim * (int)localSpaceRound.y + dim * dim * (int)localSpaceRound.z;
        PointCloud[index] += amount;
        dirty_flag = true;
    }
    public void PaintGPU(float amount, Vector3 point)
    {
        int dim = 8 * groupSize + 1;
        Vector3 localSpace = point - transform.position;
        Vector3Int localSpaceRound = new Vector3Int(Mathf.RoundToInt(localSpace.x), Mathf.RoundToInt(localSpace.y), Mathf.RoundToInt(localSpace.z));
        if (localSpaceRound.x < 0 || localSpaceRound.x >= dim || localSpaceRound.y < 0 || localSpaceRound.y >= dim || localSpaceRound.z < 0 || localSpaceRound.z >= dim)
            return;

        int index = (int)localSpaceRound.x + dim * (int)localSpaceRound.y + dim * dim * (int)localSpaceRound.z;
        PointCloud[index] += amount;
        dirty_flag = true;
    }
}
