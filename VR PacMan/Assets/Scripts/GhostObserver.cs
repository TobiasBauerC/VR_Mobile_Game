using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostObserver : MonoBehaviour
{

	[SerializeField] private PlayerController _player = null;

	private bool _ghostsBlueMode = false;
    public List<GhostController> _ghosts = new List<GhostController>();

    // Use this for initialization
    void Start()
    {
        if (!_player)
        {
            Debug.Log("No player attached. Adding default.");
            _player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        }

        foreach (GameObject _ghost in GameObject.FindGameObjectsWithTag("Ghost"))
        {
            _ghosts.Add(_ghost.GetComponent<GhostController>());
        }
    }

    // Update is called once per frame
    void Update()
    {
        CheckPlayerStatus();
    }

    private void CheckPlayerStatus()
    {
        if (_player.chaseMode && !_ghostsBlueMode)
        {
            _ghostsBlueMode = true;
            SetGhostBlueMode(_ghostsBlueMode);
        }
        else if (!_player.chaseMode && _ghostsBlueMode)
        {
            _ghostsBlueMode = false;
            SetGhostBlueMode(_ghostsBlueMode);
        }
    }

	private void SetGhostBlueMode(bool state)
    {
        if (_ghosts.Count > 0)
        {
            foreach (GhostController _ghost in _ghosts)
            {
                _ghost.GoBlue(state);
            }
        }
    }

    public void ResetGhosts()
    {
        if (_ghosts.Count > 0)
        {
            foreach (GhostController _ghost in _ghosts)
            {
                _ghost.Respawn();
            }
        }
    }
}
