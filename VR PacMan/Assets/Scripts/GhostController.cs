using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostController : MonoBehaviour
{
    private Renderer m_renderer = null;
    private Color m_normalColor = Color.black;

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
}
