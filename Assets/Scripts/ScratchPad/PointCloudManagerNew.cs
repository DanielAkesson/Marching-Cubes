using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointCloudManagerNew : MonoBehaviour
{
    public MarchingCubeMeshNew Prefab8X8;

    private float isoSurface;
    private Dictionary<Vector3Int, MarchingCubeMeshNew> meshGrid = new Dictionary<Vector3Int, MarchingCubeMeshNew>();
    public void PaintIsoSurfaceSphere(Vector3 brushPoint, float brushRadius, float amount)
    {
        int halfExtend = Mathf.FloorToInt(brushRadius / 8f) + 2;
        Vector3Int Chunk = new Vector3Int((int)(brushPoint.x / 8f), (int)(brushPoint.y / 8f), (int)(brushPoint.z / 8f));

        for (int x = -halfExtend; x <= halfExtend; x++)
            for (int y = -halfExtend; y <= halfExtend; y++)
                for (int z = -halfExtend; z <= halfExtend; z++)
                    PaintChunk(Chunk + new Vector3Int(x,y,z), brushPoint, brushRadius, amount); 
    }
    public void PaintChunk(Vector3Int chunkID, Vector3 brushPoint, float brushRadius, float amount)
    {
        if (!meshGrid.ContainsKey(chunkID))
        {
            meshGrid.Add(chunkID, Instantiate(Prefab8X8, (chunkID) * 8, Quaternion.identity, transform));
            meshGrid[chunkID].IsoSurface = isoSurface;
        }
        meshGrid[chunkID].PaintGPU(brushPoint, brushRadius, amount);
    }

    public void SetIsoSurfaceLevel(float level)
    {
        isoSurface = level;
        foreach (MarchingCubeMeshNew VMF in meshGrid.Values)
            VMF.IsoSurface = level;
    }
}
