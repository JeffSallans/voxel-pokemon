using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HoverAndDragMessageTarget : MonoBehaviour
{
    // functions that can be called via the messaging system

    /// <summary>
    /// When an object is hovering over a custom target
    /// </summary>
    public virtual void OnHoverEnter(HoverAndDragEvent _event)
    {

    }

    public virtual void OnHoverExit(HoverAndDragEvent _event)
    {

    }

    /// <summary>
    /// When an object is dropped on a custom target
    /// </summary>
    public virtual void OnDrop(HoverAndDragEvent _event)
    {

    }
}
