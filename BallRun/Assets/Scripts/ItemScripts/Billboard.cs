using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    [SerializeField] private Transform _camera;

    private void Awake()
    {
        _camera = FindObjectOfType<Camera>().transform;
    }

    //makes sure billboard is always looking at the give camera
    private void LateUpdate()
    {
        transform.LookAt(transform.position + _camera.forward);
    }
}
