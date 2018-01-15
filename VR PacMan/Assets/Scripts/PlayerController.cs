using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody _rb = null;

    [SerializeField] private float _speed = 5.0f;
    [SerializeField] private Transform _mainCam = null;

    // Use this for initialization
    void Start()
    {
        if (!_mainCam)
        {
            _mainCam = Camera.main.transform;
        }

        _rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        _rb.velocity = _mainCam.forward * _speed;
    }
}
