using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DoorTrigger : MonoBehaviour
{
    [SerializeField] private GameObject _door;
    [SerializeField] private UnityEvent _onTargetHit;
    private DoorBehavior _doorBehavior = null;
    private void Start()
    {
        DoorBehavior doorBehavior = _door.GetComponent<DoorBehavior>();

        if (doorBehavior) _doorBehavior = doorBehavior;
    }

    //kill ball and open door if trigger hit 
    private void OnTriggerEnter(Collider other)
    {
        if (_doorBehavior == null) return;
        ValuableItemBehavior valuableItem = other.gameObject.GetComponent<ValuableItemBehavior>();
        if (valuableItem)
        {
            Destroy(other.gameObject);
            _doorBehavior.OpenDoor();
            _onTargetHit?.Invoke();
        }
    }

}
