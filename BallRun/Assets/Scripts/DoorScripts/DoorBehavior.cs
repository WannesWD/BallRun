using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DoorBehavior : MonoBehaviour
{
    [SerializeField] private Vector3 _doorDisplacementDirection = Vector3.down;
    [SerializeField] private float _doorSpeed = 5f;
    [SerializeField] private float _doorDisplacementMagnitude = 40f;


    private Vector3 _closedDoorPos = Vector3.zero;
    private Vector3 _openDoorPos = Vector3.zero;
    private float _timeAfterOpened = 0f;
    private float _lerpT = 0f;
    private bool _opened = false;
    private bool _lockDoor = false;
    private void Start()
    {
        _closedDoorPos = transform.position;
        _openDoorPos = transform.position + (_doorDisplacementDirection * _doorDisplacementMagnitude);
    }
    private void Update()
    {
        if (_doorSpeed == 0f) return;

        //move door if opened
        if(_opened && !_lockDoor)
        {
            transform.position = Vector3.Lerp(_closedDoorPos, _openDoorPos, _lerpT);

            if (_timeAfterOpened >= _doorSpeed) 
            {
                _lockDoor = true;
                return;
            }

            _timeAfterOpened += Time.deltaTime;
            _lerpT = _timeAfterOpened / _doorSpeed;
        }
    }

    public void OpenDoor()
    {
        _opened = true;
    }
}
