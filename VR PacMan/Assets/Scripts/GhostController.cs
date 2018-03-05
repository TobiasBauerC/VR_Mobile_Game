using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]

public class GhostController : MonoBehaviour
{
	private Renderer _renderer = null;
	private Color _normalColor = Color.black;
	private GhostNode _lastGhostNode = null;

	private bool _moving = true;

	[SerializeField] private Rigidbody _rb = null;
	[SerializeField] private Transform _spawnPos = null;
	[SerializeField] private GhostNode _currentNode = null;
	[SerializeField] private float _minDistanceToNode = 1.0f;
	[SerializeField] private float _speed = 2.5f;

    // Use this for initialization
    void Start()
    {
		if(!_rb)
			_rb = GetComponent<Rigidbody>();

        _renderer = GetComponent<Renderer>();
        _normalColor = _renderer.material.color;
		LookAtTarget();
	}

	void Update()
	{
		_moving = !GameManager.instance.paused;

		if(Vector3.Distance(transform.position, GetRelativeNodePosition(_currentNode.transform.position)) <= _minDistanceToNode)
		{
			Vector3 newPos = _currentNode.transform.position;
			newPos.y = transform.position.y;
			transform.position = newPos;
			_moving = false;
			GetNextNode();
		}
	}

	void FixedUpdate()
	{
		
		if(_moving)
		{
			_rb.velocity = transform.forward * _speed;
		}
	}

	private void GetNextNode()
	{
		GhostNode tempGhostNode = _currentNode;
		_currentNode = tempGhostNode.GetNextGhostNode(_lastGhostNode);
		_lastGhostNode = tempGhostNode;
		LookAtTarget();
		_moving = true;
	}

	private void LookAtTarget()
	{
		Vector3 lookAtPosition = _currentNode.transform.position;
		lookAtPosition.y = transform.position.y;
		transform.LookAt(lookAtPosition);
	}

	private Vector3 GetRelativeNodePosition(Vector3 nodePosition)
	{
		Vector3 newPos = nodePosition;
		newPos.y = transform.position.y;
		return newPos;
	}

    public void GoBlue(bool state)
    {
        if (state)
        {
            _renderer.material.color = Color.blue;
        }
        else
        {
            _renderer.material.color = _normalColor;
        }
    }

    public void Respawn()
    {
        Vector3 newPos = transform.position;
        newPos.x = _spawnPos.position.x;
        newPos.z = _spawnPos.position.z;
        transform.position = newPos;
        GoBlue(false);
    }
}
