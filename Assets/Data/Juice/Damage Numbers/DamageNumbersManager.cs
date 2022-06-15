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
	
	public void SpawnDamageNumber(float nNum, Vector3 numberPos, NumberTypes numberType = NumberTypes.NORMAL)
	{
		DamageNumber nDamageNumber = damageNumbers.Dequeue();
		
		if(numberType == NumberTypes.NORMAL)
		{
			nDamageNumber.SetNumber(nNum, numberPos, cameraTr, true, numberType);
		}
		else if(numberType == NumberTypes.EXPLOSION_DAMAGE)
		{
			nDamageNumber.SetNumber(nNum, numberPos, cameraTr, true, numberType, 6f);
		}
		
		damageNumbers.Enqueue(nDamageNumber);
	}
	
    
}

[System.Serializable]
public enum NumberTypes
{
	NORMAL = 0,
	EXPLOSION_DAMAGE = 1
}


