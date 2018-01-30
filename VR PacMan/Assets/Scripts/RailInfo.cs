using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RailInfo : MonoBehaviour
{
    [SerializeField] private float m_xClamp = 0.0f;
    [SerializeField] private float m_zClamp = 0.0f;

    public float xClamp { get { return m_xClamp; } }
    public float zClamp { get { return m_zClamp; } }

    void Start()
    {
        if (gameObject.tag == "HorizontalRail")
        {
            m_zClamp = transform.position.z;
        }
        else if (gameObject.tag == "VerticalRail")
        {
            m_xClamp = transform.position.x;
        }
    }
}
