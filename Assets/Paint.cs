using UnityEngine;

public class Paint : MonoBehaviour
{
    public PointCloudManagerNew Canvas;
    public float BrushSize;
    public float Amount;

    public void Start()
    {
        Canvas.PaintIsoSurfaceSphere(transform.position, BrushSize, Amount);
    }
}
