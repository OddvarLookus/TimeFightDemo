using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class StaticExplosionsManager : MonoBehaviour
{
	[AssetsOnly] [SerializeField] GameObject explosionPrefab;
	[SerializeField] int maxExplosionsNum;
	Queue<Explosion> explosions = new Queue<Explosion>();
    
	public static StaticExplosionsManager instance;
	
	protected void Start()
	{
		instance = this;
		
		InitializeExplosions();
	}
    
	void InitializeExplosions()
	{
		GameObject nExplosion = Instantiate(explosionPrefab);
		Transform nExplosionTr = nExplosion.transform;
		Explosion nex = nExplosion.GetComponent<Explosion>();
		nExplosionTr.SetParent(transform);
		explosions.Enqueue(nex);
	}
	
	public void RequestExplosionAt(Vector3 npos, float nsize, float ndamage, int layerMask = ~0)
	{
		Explosion ne = explosions.Dequeue();
		ne.StartExplosion(npos, nsize, ndamage, layerMask);
		explosions.Enqueue(ne);
	}
	
    
    
}
