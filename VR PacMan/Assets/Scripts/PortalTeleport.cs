using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalTeleport : MonoBehaviour 
{
	[SerializeField] private Transform _teleDestination = null;

	public Vector3 teleDestination
	{
		get
		{
			return _teleDestination.position;
		}
	}
}
