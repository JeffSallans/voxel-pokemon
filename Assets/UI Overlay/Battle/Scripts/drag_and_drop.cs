using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class drag_and_drop : MonoBehaviour
{
    ptDisplayElectricEnergy ptDisplay;

    public GameObject canvas;
    public GameObject handArea;

    private bool isDragging = false;
    private GameObject dropzone;
    private GameObject dropArea;
    private GameObject startParent;
    private GameObject pt_active_energy_lightning;
    private Vector2 startPos;
    private bool isOverDropZone = false;

    // Start is called before the first frame update
    void Start()
    {
        canvas = GameObject.Find("Main Canvas");
        handArea = GameObject.Find("Player Hand");
        dropArea = GameObject.Find("Drop Zone");
        pt_active_energy_lightning = GameObject.Find("pt_active_energy_lightning");
    }

    void PlayCard()
    {
        ptDisplay = pt_active_energy_lightning.GetComponent<ptDisplayElectricEnergy>();
        ptDisplay.AddEnergy();
        //UIText = pt_active_energy_lightning.GetComponent<Text>();
        Destroy(this.gameObject);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Enter");
        isOverDropZone = true;
        dropzone = collision.gameObject;

    }

    void OnTriggerExit2D(Collider2D collision)
    {
        Debug.Log("Exit");
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
            PlayCard();
            //transform.position = dropArea.transform.position;
            //transform.SetParent(dropArea.transform, false);
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
            transform.SetParent(canvas.transform, true);
        }
        
    }
}
