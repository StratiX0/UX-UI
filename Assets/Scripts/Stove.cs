using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Stove : MonoBehaviour
{
    public int temperature;
    public int maxTemperature;
    public Slider temperatureSlider;
    public TextMeshProUGUI temperatureText;
    
    public void SetTemperature(int value)
    {
        temperature = value;
    }
    
    public int GetTemperature()
    {
        return temperature;
    }
    
    public void ScaleTemperature()
    {
        float value = temperatureSlider.value;
        temperature = (int)(maxTemperature * value);
        temperatureText.text = temperature.ToString();
    }
}
