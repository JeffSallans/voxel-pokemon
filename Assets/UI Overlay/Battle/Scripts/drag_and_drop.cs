using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class drag_and_drop : MonoBehaviour
{
    public GameObject canvas;
    public GameObject handArea;

    private bool isDragging = false;

    // Start is called before the first frame update
    void Start()
    {
        canvas = GameObject.Find("Main Canvas");
        handArea = GameObject.Find("Player Hand");
    }

    public void StartDrag()
    {
        isDragging = true;
    }

    public void EndDrag()
    {
        isDragging = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (isDragging)
        {
            transform.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            transform.SetParent(handArea.transform, true);
            transform.SetParent(canvas.transform, true);
        }
        
    }
}
