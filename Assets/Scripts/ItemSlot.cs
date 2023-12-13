using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemSlot : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        // if a DraggableItem is dropped onto this (open) ItemSlot, set its parent to this ItemSlot
        if (IsEmpty())
        {
            GameObject dropped = eventData.pointerDrag;
            DraggableItem draggableItem = dropped.GetComponent<DraggableItem>();
            draggableItem.parentAfterDrag = transform;
        }
    }

    // public method for checking the ItemSlot's emptiness
    public bool IsEmpty()
    {
        return (transform.childCount == 0);
    }
}
