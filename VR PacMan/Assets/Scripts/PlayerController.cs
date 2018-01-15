﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody _rb = null;
    private WaitForSeconds _chaseModeTime;
    private Coroutine _chaseModeCoroutine;

    [SerializeField] private float _speed = 5.0f;
    [SerializeField] private Transform _mainCam = null;

    public bool chaseMode
    {
        private set;
        get;
    }

    // Use this for initialization
    void Start()
    {
        if (!_mainCam)
            _mainCam = Camera.main.transform;

        _rb = GetComponent<Rigidbody>();

        _chaseModeTime = new WaitForSeconds(10.0f);
        chaseMode = false;
    }

    // Update is called once per frame
    void Update()
    {
        _rb.velocity = _mainCam.forward * _speed;
        Debug.Log(chaseMode);
    }

    void OnTriggerEnter(Collider c)
    {
        if (c.gameObject.tag == "Power Pellet")
        {
            Destroy(c.gameObject);
            _chaseModeCoroutine = StartCoroutine(ChaseMode());
        }
    }

    private IEnumerator ChaseMode()
    {
        chaseMode = true;
        yield return _chaseModeTime;
        chaseMode = false;
    }
}
