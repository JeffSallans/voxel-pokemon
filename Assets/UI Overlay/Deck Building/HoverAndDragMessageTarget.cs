using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;

public class HoverAndDragMessageTarget : MonoBehaviour
{
    private List<DropEvent> _allPossibleEvents;

    // functions that can be called via the messaging system

    public void OnDragHelper(Card card)
    {
        _allPossibleEvents = GameObject.FindObjectsOfType<DropEvent>().ToList();
        OnDrag(card, _allPossibleEvents);
    }

    protected virtual void OnDrag(Card card, List<DropEvent> _events)
    {

    }

    /// <summary>
    /// When an object is hovering over a custom target
    /// </summary>
    public virtual void OnHoverEnter(HoverAndDragEvent _event)
    {

    }

    public virtual void OnHoverExit(HoverAndDragEvent _event)
    {

    }

    public void OnDropHelper(HoverAndDragEvent _event)
    {
        OnDrop(_event, _allPossibleEvents);
    }

    /// <summary>
    /// When an object is dropped on a custom target
    /// </summary>
    protected virtual void OnDrop(HoverAndDragEvent _event, List<DropEvent> _events)
    {
        // Remove all drop areas
        if (_events != null)
        {
            _events.Where(e => e.targetAnimator != null && e != _event.dropEvent)
                .ToList()
                .ForEach(e =>
                {
                    e.targetAnimator.SetTrigger("onDropStop");
                });
        }
    }

    /// <summary>
    /// Card should call this when the drag has stopped
    /// </summary>
    /// <param name="card"></param>
    public void OnDragStopHelper(Card card)
    {
        OnDragStop(card, _allPossibleEvents);
    }

    /// <summary>
    /// When an object is dropped not on a custom target
    /// </summary>
    protected virtual void OnDragStop(Card card, List<DropEvent> _events)
    {
        // Remove all drop areas
        if (_events != null)
        {
            _events.Where(e => e.targetAnimator != null)
                .ToList()
                .ForEach(e =>
                {
                    e.targetAnimator.SetTrigger("onDropStop");
                });
        }
    }
}
