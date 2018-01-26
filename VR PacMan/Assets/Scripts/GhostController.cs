using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostController : MonoBehaviour
{
    private Renderer m_renderer = null;
    private Color m_normalColor = Color.black;

    [SerializeField] private Transform m_spawnPos = null;

    // Use this for initialization
    void Start()
    {
        m_renderer = GetComponent<Renderer>();
        m_normalColor = m_renderer.material.color;
    }

    public void GoBlue(bool _state)
    {
        if (_state)
        {
            m_renderer.material.color = Color.blue;
        }
        else
        {
            m_renderer.material.color = m_normalColor;
        }
    }

    public void Respawn()
    {
        Vector3 newPos = transform.position;
        newPos.x = m_spawnPos.position.x;
        newPos.z = m_spawnPos.position.z;
        transform.position = newPos;
        GoBlue(false);
    }
}
