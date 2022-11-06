using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Used to remove the fog effect from a specific camera
/// 
/// Source: https://answers.unity.com/questions/11100/is-it-possible-to-disable-fog-on-a-per-camera-basi.html
/// </summary>
[RequireComponent(typeof(Camera))]

public class CameraFogSetting : MonoBehaviour
{
    /// <summary>
    /// Set to false to remove the fog effect from the given camera
    /// </summary>
    [SerializeField] bool enableFog = true;
    bool previousFogState;
    void OnPreRender()
    {
        previousFogState = RenderSettings.fog;
        RenderSettings.fog = enableFog;
    }
    void OnPostRender()
    {
        RenderSettings.fog = previousFogState;
    }
}
