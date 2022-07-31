using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Used to show a cursor without the mouse leaving the screen
/// </summary>
public class OverlayCursor : MonoBehaviour
{
    /// <summary>
    /// Set to false when making a exe
    /// </summary>
    public bool inDebugMode = false;

    /// <summary>
    /// Set to true when the cursor should show
    /// </summary>
    public bool showCursor = true;

    public GameObject cursorObject = null;

    private float xOffset = 0;

    private float yOffset = 0;

    public float zOffset = 0;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = inDebugMode;

        var rectTran = cursorObject.GetComponent<RectTransform>().rect;
        xOffset = rectTran.width / 2f * cursorObject.transform.lossyScale.x;
        yOffset = rectTran.height / 2f * cursorObject.transform.lossyScale.y;
    }

    // Update is called once per frame
    void Update()
    {
        if (showCursor)
        {
            cursorObject.SetActive(true);
            var canvasCamera = GameObject.Find("Canvas Camera").GetComponent<Camera>();
            var mousePosition = canvasCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 100f - (zOffset * cursorObject.transform.lossyScale.z)));
            cursorObject.transform.position = mousePosition + new Vector3(xOffset, -yOffset, zOffset * cursorObject.transform.lossyScale.z);
        }
        else
        {
            cursorObject.SetActive(false);
        }
    }
}
