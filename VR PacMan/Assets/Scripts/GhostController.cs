using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]

public class GhostController : MonoBehaviour
{
	private Renderer _renderer;
    private Color _normalColor;
    private GhostNode _lastGhostNode;
    private GhostNode _firstGhostNode;
    private Vector3 _spawnPos;

	private bool _moving = true;

    public bool isBlue { get; set; }

	[SerializeField] private Rigidbody _rb;
	[SerializeField] private GhostNode _currentNode;
	[SerializeField] private float _minDistanceToNode = 1.0f;
	[SerializeField] private float _speed = 2.5f;

    // Use this for initialization
    void Start()
    {
		if(!_rb)
			_rb = GetComponent<Rigidbody>();

        _firstGhostNode = _currentNode;
        _spawnPos = transform.position;
        _renderer = GetComponent<Renderer>();
        _normalColor = _renderer.material.color;
		LookAtTarget();
	}

	void Update()
	{
		_moving = !GameManager.instance.paused;

        if (!_moving)
            _rb.isKinematic = true;

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
            _rb.isKinematic = false;
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
            isBlue = true;
            _renderer.material.color = Color.blue;
        }
        else
        {
            isBlue = false;
            _renderer.material.color = _normalColor;
        }
    }

    public void Respawn()
    {
        _moving = false;
        Vector3 newPos = transform.position;
        newPos.x = _spawnPos.x;
        newPos.z = _spawnPos.z;
        transform.position = newPos;
        GoBlue(false);
        _currentNode = _firstGhostNode;
        LookAtTarget();
    }
}
