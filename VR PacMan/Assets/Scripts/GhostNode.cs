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

	public GhostNode GetNextGhostNode(GhostNode lastGhostNode)
	{
		int newNodeIndex = (int) Random.Range(0, _connectedGhostNodes.Count);

		if(_connectedGhostNodes[newNodeIndex] != lastGhostNode)
			return _connectedGhostNodes[newNodeIndex];
		else
			return GetNextGhostNode(lastGhostNode);
	}

    public GhostNode GetNextPacManGhostNode(GhostNode lastGhostNode)
    {
        GhostNode closestNode = null;
        float closestDistance = Mathf.Infinity;
        Vector3 playerPos = GameManager.instance.pacManTransform.position;

        foreach(GhostNode node in _connectedGhostNodes)
        {
            if (node == lastGhostNode)
                continue;
            float distance = Vector3.Distance(node.transform.position, playerPos);
            if(distance < closestDistance)
            {
                closestNode = node;
                closestDistance = distance;
            }
        }

        return closestNode;
    }

    public GhostNode GetFurthesrPacManGhostNode(GhostNode lastGhostNode)
    {
        GhostNode closestNode = null;
        float closestDistance = 0;
        Vector3 playerPos = GameManager.instance.pacManTransform.position;

        foreach (GhostNode node in _connectedGhostNodes)
        {
            if (node == lastGhostNode)
                continue;
            float distance = Vector3.Distance(node.transform.position, playerPos);
            if (distance > closestDistance)
            {
                closestNode = node;
                closestDistance = distance;
            }
        }

        return closestNode;
    }

    public GhostNode GetNextPacManFlankGhostNode(GhostNode lastGhostNode)
    {
        GhostNode closestNode = null;
        float closestDistance = Mathf.Infinity;
        Vector3 playerFlankPos = GameManager.instance.pacManTransform.position + (Camera.main.transform.forward * 10.0f);

        foreach (GhostNode node in _connectedGhostNodes)
        {
            if (node == lastGhostNode)
                continue;
            float distance = Vector3.Distance(node.transform.position, playerFlankPos);
            if (distance < closestDistance)
            {
                closestNode = node;
                closestDistance = distance;
            }
        }

        return closestNode;
    }

	private void GetGhostNode(Vector3 direction)
	{
		RaycastHit hit;

		if(Physics.Raycast(transform.position, direction, out hit, Mathf.Infinity, _ghostNodeMask))
		{
			if(hit.collider.gameObject.layer == 10)
				_connectedGhostNodes.Add(hit.collider.GetComponent<GhostNode>());
		}
	}

	private IEnumerator DisableCollider()
	{
		yield return _disableWait;
		GetComponent<Collider>().enabled = false;
	}
}
