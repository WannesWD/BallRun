using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PickupBehavior : MonoBehaviour
{
    [SerializeField] private UnityEvent _onThrow;

    private ValuableItemBehavior _attachedItem = null;

    public ValuableItemBehavior getAttachedItem()
    {
        return _attachedItem;
    }

    //attach item when it gets in range
    private void OnTriggerEnter(Collider other)
    {
        ValuableItemBehavior valuableItem = other.gameObject.GetComponent<ValuableItemBehavior>();
        if (valuableItem)
        {
            valuableItem.HandlePickup();
            _attachedItem = valuableItem;
        }
    }

    public void ThrowItem()
    {
        if (_attachedItem == null) return;
        
        _attachedItem.ThrowItem();
        _onThrow?.Invoke();
        _attachedItem = null;
    }
}
