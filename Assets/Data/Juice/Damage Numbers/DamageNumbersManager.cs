using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class DamageNumbersManager : MonoBehaviour
{
	
	public static DamageNumbersManager instance;
	Transform cameraTr;
	
    
	void Awake()
    {
	    instance = this;
	    cameraTr = Camera.main.transform;
	    
	    InitializeDamageNumbers();
    }

	
	[SerializeField] int maxDamageNumbers;
	[AssetsOnly] [SerializeField] GameObject damageNumberPrefab;
	Queue<DamageNumber> damageNumbers = new Queue<DamageNumber>();
	
	void InitializeDamageNumbers()
	{
		for(int i = 0; i < maxDamageNumbers; i++)
		{
			GameObject nNumberGO = Instantiate(damageNumberPrefab);
			nNumberGO.transform.SetParent(transform);
			damageNumbers.Enqueue(nNumberGO.GetComponent<DamageNumber>());
		}
	}
	
	public void SpawnDamageNumber(float nNum, Vector3 numberPos)
	{
		DamageNumber nDamageNumber = damageNumbers.Dequeue();
		nDamageNumber.SetNumber(nNum, numberPos, cameraTr);
		damageNumbers.Enqueue(nDamageNumber);
	}
	
    
}
