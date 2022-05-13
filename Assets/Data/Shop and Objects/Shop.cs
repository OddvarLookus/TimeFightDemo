using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class Shop : MonoBehaviour, IInteractable
{
	
	[SerializeField] BuyableObjectInfos[] objectInfos;
	
	[Header("Buyable Object Prefab")]
	[AssetsOnly] [SerializeField] GameObject buyableObjectPrefab;
	
	[Header("Object Positions")]
	[SerializeField] Transform pos0, pos1, pos2;
	Vector3 localPos0, localPos1, localPos2;
	
	bool hasInteracted = false;
	public void Interact(PlayerController pc)
	{
		//if(!hasInteracted)
		//{
		//	bool fromFront = Vector3.Dot((pc.transform.position - transform.position).normalized, transform.forward) >= 0f;
			
		//	ThrowObjects(fromFront);
		//	hasInteracted = true;
		//}
		
	}
	
	protected void Start()
	{
		localPos0 = pos0.localPosition;
		localPos1 = pos1.localPosition;
		localPos2 = pos2.localPosition;
		
		StartShopSoundTimer();
		ThrowObjects();
	}
	
	void ThrowObjects()
	{
		
		pos0.localPosition = localPos0;
		pos1.localPosition = localPos1;
		pos2.localPosition = localPos2;
		
		InstantiateBuyableObjectAtPos(pos0.position, 0);
		InstantiateBuyableObjectAtPos(pos1.position, 1);
		InstantiateBuyableObjectAtPos(pos2.position, 2);

	}
	
	void InstantiateBuyableObjectAtPos(Vector3 pos, int idx)
	{
		GameObject g = Instantiate(buyableObjectPrefab);
		Transform tr = g.transform;
		tr.SetParent(null);
		tr.position = pos;
		tr.GetComponent<BuyableObject>().SetObjectInfos(objectInfos[idx]);
	}
	
	public string GetInteractionDescription()
	{
		return "";
	}
	
	//SOUNDS
	[Header("SOUNDS")]
	[SerializeField] SoundsPack shopSound;
	[SerializeField] float shopSoundTime;
	void StartShopSoundTimer()
	{
		StaticAudioStarter.instance.StartAudioEmitter(transform.position, shopSound.GetRandomSound(), shopSound.GetRandomPitch());
		StartCoroutine(ShopSoundCoroutine(shopSoundTime));
	}
	
	IEnumerator ShopSoundCoroutine(float time)
	{
		yield return new WaitForSeconds(time);
		StartShopSoundTimer();
	}
	
}
