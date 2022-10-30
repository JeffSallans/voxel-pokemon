using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Used for player to recenter the third person camera
/// </summary>
public class CameraCenterControls : MonoBehaviour
{
    private Cinemachine.CinemachineFreeLook freeLook;

    // Start is called before the first frame update
    void Start()
    {
        freeLook = GameObject.FindObjectOfType<Cinemachine.CinemachineFreeLook>();
    }

    // Update is called once per frame
    void Update()
    {
        if (freeLook && Input.GetKeyDown(KeyCode.Q))
        {
            freeLook.m_RecenterToTargetHeading.m_enabled = true;
        }

        if (freeLook && Input.GetKeyUp(KeyCode.Q))
        {
            freeLook.m_RecenterToTargetHeading.m_enabled = false;
        }
    }
}
