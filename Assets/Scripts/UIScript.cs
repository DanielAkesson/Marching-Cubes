using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIScript : MonoBehaviour
{
    public Slider BrushSize, BrushStrength, IsoLevel;
    public Text BrushSizeText, BrushStrengthText, IsoLevelText;
    public PointCloudManagerNew PointClound;
    public CameraBrush Brush;

    public void Start()
    {
        PointClound.SetIsoSurfaceLevel(IsoLevel.value); IsoLevelText.text = $"Iso-Level: {IsoLevel.value}";
        Brush.Amount = BrushStrength.value; BrushStrengthText.text = $"Brush Strength: {BrushStrength.value}";
        Brush.BrushSize = BrushSize.value; BrushSizeText.text = $"Brush Size: {BrushSize.value}";
        //Registers
        BrushSize.onValueChanged.AddListener(delegate { Brush.BrushSize = BrushSize.value; BrushSizeText.text = $"Brush Size: {BrushSize.value}"; });
        BrushStrength.onValueChanged.AddListener(delegate { Brush.Amount = BrushStrength.value; BrushStrengthText.text = $"Brush Strength: {BrushStrength.value}"; });
        IsoLevel.onValueChanged.AddListener(delegate { PointClound.SetIsoSurfaceLevel(IsoLevel.value); IsoLevelText.text = $"Iso-Level: {IsoLevel.value}"; });
    }
}
