using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostNode : MonoBehaviour 
{
	public LayerMask _ghostNodeMask;
	public List<GhostNode> _connectedGhostNodes = new List<GhostNode>();

	private WaitForSeconds _disableWait = new WaitForSeconds(1.0f);

	// Use this for initialization
	void Start () 
	{
		GetGhostNode(Vector3.forward);
		GetGhostNode(Vector3.right);
		GetGhostNode(Vector3.back);
		GetGhostNode(Vector3.left);
		StartCoroutine("DisableCollider");
	}

	void Update()
	{
		if(Input.GetKeyDown(KeyCode.Space))
		{
			GetNextGhostNode(null);
		}
	}

	public GhostNode GetNextGhostNode(GhostNode lastGhostNode)
	{
		int newNodeIndex = (int) Random.Range(0, _connectedGhostNodes.Count);

		if(_connectedGhostNodes[newNodeIndex] != lastGhostNode)
			return _connectedGhostNodes[newNodeIndex];
		else
			return GetNextGhostNode(lastGhostNode);
	}
	
	private void GetGhostNode(Vector3 direction)
	{
		RaycastHit hit;

		if(Physics.Raycast(transform.position, direction, out hit, Mathf.Infinity, _ghostNodeMask))
		{
			_connectedGhostNodes.Add(hit.collider.GetComponent<GhostNode>());
		}
	}

	private IEnumerator DisableCollider()
	{
		yield return _disableWait;
		GetComponent<Collider>().enabled = false;
	}
}
