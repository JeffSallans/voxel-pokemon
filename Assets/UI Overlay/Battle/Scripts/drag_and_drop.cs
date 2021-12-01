using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class drag_and_drop : MonoBehaviour
{
    public GameObject canvas;
    public GameObject handArea;

    private bool isDragging = false;
    private GameObject dropzone;
    private GameObject dropArea;
    private GameObject startParent;
    private Vector2 startPos;
    private bool isOverDropZone = false;

    // Start is called before the first frame update
    void Start()
    {
        canvas = GameObject.Find("Main Canvas");
        handArea = GameObject.Find("Player Hand");
        dropArea = GameObject.Find("Drop Zone");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        isOverDropZone = true;
        dropzone = collision.gameObject;

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        isOverDropZone = false;
        dropzone = null;
    }
    public void StartDrag()
    {
        isDragging = true;
        startParent = transform.parent.gameObject;
        startPos = transform.position;
    }

    public void EndDrag()
    {
        isDragging = false;
        if (isOverDropZone)
        {
            transform.position = dropArea.transform.position;
            transform.SetParent(dropArea.transform, false);
        }
        else
        {
            transform.position = startPos;
            transform.SetParent(startParent.transform, false);
        }
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
