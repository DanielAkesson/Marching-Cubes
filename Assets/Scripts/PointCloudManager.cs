using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointCloudManager : MonoBehaviour
{
    public MarchingCubeMesh Prefab8X8;

    private float isoSurface;
    private Dictionary<Vector3Int, MarchingCubeMesh> meshGrid = new Dictionary<Vector3Int, MarchingCubeMesh>();
    public void PaintIsoSurfaceSphere(Vector3 point, float brushSize, float amount)
    {
        int maxBrush = (int)(brushSize / 2f) + 1;
        Vector3 localSpace = point - transform.position;
        Vector3Int localSpaceRound = new Vector3Int(Mathf.RoundToInt(localSpace.x), Mathf.RoundToInt(localSpace.y), Mathf.RoundToInt(localSpace.z));
        for (int x = -maxBrush; x <= maxBrush; x++)
        {
            for (int y = -maxBrush; y <= maxBrush; y++)
            {
                for (int z = -maxBrush; z <= maxBrush; z++)
                {
                    Vector3 currentPoint = new Vector3(localSpaceRound.x + x, localSpaceRound.y + y, localSpaceRound.z + z);
                    float distFromCenter = (localSpace - currentPoint).magnitude;
                    PaintPoint( amount * Mathf.Clamp01((maxBrush - distFromCenter) / maxBrush), currentPoint);
                }
            }
        }        
    }

    public void PaintPoint(float amount, Vector3 point)
    {
        Vector3Int grid = new Vector3Int((int)(point.x / 8f), (int)(point.y / 8f), (int)(point.z / 8f));

        for(int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                for (int z = -1; z <= 1; z++)
                {
                    Vector3Int offset = new Vector3Int(x, y, z);
                    if (!meshGrid.ContainsKey(grid + offset))
                    {
                        meshGrid.Add(grid + offset, Instantiate(Prefab8X8, (grid + offset) * 8, Quaternion.identity, transform));
                        meshGrid[grid + offset].IsoSurface = isoSurface;
                    }
                    meshGrid[grid + offset].PaintPointCPU(amount, point);
                }
            }
        }          
    }

    public void SetIsoSurfaceLevel(float level)
    {
        isoSurface = level;
        foreach (MarchingCubeMesh VMF in meshGrid.Values)
            VMF.IsoSurface = level;
    }
}
