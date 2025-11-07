using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBrush : MonoBehaviour
{
    public PointCloudManagerNew Canvas;
    public float BrushSize;
    public float Amount;
    public LayerMask PaintLayer;

    private float distance = 30;
    private Vector3 screenPos;
    private Camera cam;

    private void Start()
    {
        cam = GetComponent<Camera>();
    }

    void Update()
    {
        Ray lazer = cam.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(lazer, out RaycastHit hit, float.MaxValue, PaintLayer);
        if(hit.collider != null && Input.GetMouseButton(0))
        {
            Canvas.PaintIsoSurfaceSphere(hit.point, BrushSize, Amount);
        }
    }

    void LateUpdate()
    {
        MoveUpdate();
    }

    private void MoveUpdate()
    {
        //Spin with mouse
        Vector3 screenPosDelta = Vector3.zero;
        if (Input.GetMouseButton(1))
            screenPosDelta = (screenPos - Input.mousePosition) * 0.1f;
        screenPos = Input.mousePosition;

        //Position
        transform.position += screenPosDelta.x * transform.right;
        transform.position += screenPosDelta.y * transform.up;

        //Rotation
        transform.LookAt(Canvas.gameObject.transform.position, Vector3.up);

        //Distance
        distance -= Input.mouseScrollDelta.y;
        if (distance < 1) distance = 1;
        transform.position = Canvas.gameObject.transform.position + (-transform.forward * distance);
    }
}
