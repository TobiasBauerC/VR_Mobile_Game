using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostController : MonoBehaviour
{
	private Renderer _renderer = null;
	private Color _normalColor = Color.black;

	[SerializeField] private Transform _spawnPos = null;

    // Use this for initialization
    void Start()
    {
        _renderer = GetComponent<Renderer>();
        _normalColor = _renderer.material.color;
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
