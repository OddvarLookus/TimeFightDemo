using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditsCluster : MonoBehaviour
{
	[SerializeField] float minArea;
	[SerializeField] float maxArea;
	[SerializeField] float creditsDensity;
	
	[SerializeField] GameObject creditPrefab;

	protected void Start()
	{
		InitializeCluster();
	}
    
	void InitializeCluster()
	{
		float randArea = Random.Range(minArea, maxArea);
		
		int numCredits = Mathf.RoundToInt(randArea * creditsDensity);
		for(int i = 0; i < numCredits; i++)
		{
			Vector3 randPos = new Vector3(Random.Range(-randArea, randArea), Random.Range(-randArea, randArea), Random.Range(-randArea, randArea));
			GameObject crdt = Instantiate(creditPrefab);
			Transform crdtTr = crdt.transform;
			crdtTr.parent = transform.parent;
			crdtTr.position = transform.position + randPos;
		}
		Destroy(this.gameObject);
		
	}
    
	protected void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(transform.position, minArea);
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere(transform.position, maxArea);
	}
}
