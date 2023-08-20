using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Required when Using UI elements.

[ExecuteInEditMode]
public class StrabismSliderController : MonoBehaviour
{
    public Slider sliderAxisX;
    public Slider sliderAxisY;
    
    public Material material3D;

    // Start is called before the first frame update
    void Start()
    {
        sliderAxisX.value = 0;
        sliderAxisX.onValueChanged.AddListener(delegate { UpdateSliderX(sliderAxisX.value); });
        UpdateSliderX(sliderAxisX.value);
        sliderAxisY.onValueChanged.AddListener(delegate { UpdateSliderY(sliderAxisY.value); });
        UpdateSliderY(sliderAxisY.value);
    }
    public void UpdateSliderX(float value) { material3D.SetFloat("_AxisX", value); }
    public void UpdateSliderY(float value) { material3D.SetFloat("_AxisY", value); }
}
