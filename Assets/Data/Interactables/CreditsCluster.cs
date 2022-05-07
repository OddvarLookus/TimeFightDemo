using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditsCluster : MonoBehaviour
{
	[SerializeField] float minDistance;
	[SerializeField] float maxDistance;
	[SerializeField] int creditsNum;
	[SerializeField] bool enableDebugLines;
	
	[SerializeField] GameObject creditPrefab;

	protected void Start()
	{
		InitializeCluster();
	}
    
	List<Vector3> points = new List<Vector3>();
	void InitializeCluster()
	{
		if(!Application.isEditor)
		{
			enableDebugLines = false;
		}
		
		Vector3 nRandPos = transform.position;
		Vector3 prevPos = nRandPos;
		for(int i = 0; i < creditsNum; i++)
		{
			float randDist = Random.Range(minDistance, maxDistance);
			nRandPos = nRandPos + (new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized * randDist);
			
			GameObject crdt = Instantiate(creditPrefab);
			Transform crdtTr = crdt.transform;
			crdtTr.parent = transform.parent;
			crdtTr.position = nRandPos;
			
			points.Add(nRandPos);
			prevPos = nRandPos;
			
		}
		
		if(!enableDebugLines)
		{
			Destroy(this.gameObject);
		}
		
		
	}
	
	protected void Update()
	{
		for(int i = 0; i < points.Count; i++)
		{
			int prevIdx = i - 1;
			if(prevIdx < 0)
			{
				prevIdx = 0;
			}
			Debug.DrawLine(points[prevIdx], points[i], Color.red, 0.1f);
		}
	}
    
	protected void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(transform.position, minDistance);
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere(transform.position, maxDistance);
	}
}
