using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RailInfo : MonoBehaviour
{
	[SerializeField] private float _xClamp = 0.0f;
	[SerializeField] private float _zClamp = 0.0f;

    public float xClamp { get { return _xClamp; } }
    public float zClamp { get { return _zClamp; } }

    void Start()
    {
        if (gameObject.tag == "HorizontalRail")
        {
            _zClamp = transform.position.z;
        }
        else if (gameObject.tag == "VerticalRail")
        {
            _xClamp = transform.position.x;
        }
    }
}
