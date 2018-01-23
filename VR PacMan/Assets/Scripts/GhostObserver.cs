using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostObserver : MonoBehaviour
{

    [SerializeField] private PlayerController m_player = null;

    private bool m_ghostsBlueMode = false;
    public List<GhostController> m_ghosts = new List<GhostController>();

    // Use this for initialization
    void Start()
    {
        if (!m_player)
        {
            Debug.Log("No player attached. Adding default.");
            m_player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        }

        foreach (GameObject _ghost in GameObject.FindGameObjectsWithTag("Ghost"))
        {
            m_ghosts.Add(_ghost.GetComponent<GhostController>());
        }
    }

    // Update is called once per frame
    void Update()
    {
        CheckPlayerStatus();
    }

    private void CheckPlayerStatus()
    {
        if (m_player.chaseMode && !m_ghostsBlueMode)
        {
            m_ghostsBlueMode = true;
            SetGhostBlueMode(m_ghostsBlueMode);
        }
        else if (!m_player.chaseMode && m_ghostsBlueMode)
        {
            m_ghostsBlueMode = false;
            SetGhostBlueMode(m_ghostsBlueMode);
        }
    }

    private void SetGhostBlueMode(bool _state)
    {
        if (m_ghosts.Count > 0)
        {
            foreach (GhostController _ghost in m_ghosts)
            {
                _ghost.GoBlue(_state);
            }
        }
    }
}
