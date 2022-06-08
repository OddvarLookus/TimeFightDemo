using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine.UI;
using System;

public class ProgressionLayer : SerializedMonoBehaviour
{
	[Header("Upgrades Definition")]
	[SerializeReference] List<Upgrade> allUpgrades = new List<Upgrade>();
    
	[Header("References")]
	
	[SceneObjectsOnly] [SerializeField] GameObject cardsParent;
	
	[OnValueChanged("OnCardsTrChanged")]
	[SceneObjectsOnly] [SerializeField] Transform[] cardsTr;
	[DisableIf("@true")] [SerializeField] Image[] cardsImages;
	void OnCardsTrChanged()
	{
		cardsImages = new Image[cardsTr.Length];
		for(int i = 0; i < cardsTr.Length; i++)
		{
			cardsImages[i] = cardsTr[i].GetComponent<Image>();
		}
	}
	
	//UPGRADE BEHAVIOR
	public static ProgressionLayer instance;
	[Header("Raycasting")]
	EventSystem evSystem;
	
	protected void Awake()
	{
		instance = this;
		evSystem = GameObject.FindObjectOfType<EventSystem>();
	}
	
	[HideInInspector] public Action<Upgrade> OnUpgradeChosen;
	List<Upgrade> upgradesYouCanChoose = new List<Upgrade>();
	bool choosingUpgrade = false;
	Transform hooveredCard = null;
	public void StartUpgradeChoice(Action<Upgrade> nOnUpgradeChosen)
	{
		Debug.Log("UPGRADE CHOICE STARTED");
		OnUpgradeChosen = nOnUpgradeChosen;
		
		cardsParent.SetActive(true);
		Time.timeScale = 0f;
		
		upgradesYouCanChoose.Clear();
		List<Upgrade> upgradeRolls = new List<Upgrade>();//copy the upgrade list
		for(int i = 0; i < allUpgrades.Count; i++)
		{
			upgradeRolls.Add(allUpgrades[i]);
		}
		for(int i = 0; i < cardsTr.Length; i++)//roll the random and put it on the card
		{
			int roll = UnityEngine.Random.Range(0, upgradeRolls.Count);
			cardsImages[i].sprite = upgradeRolls[roll].upgradeSprite;
			upgradesYouCanChoose.Add(upgradeRolls[roll]);
			upgradeRolls.RemoveAt(roll);
		}
		
		choosingUpgrade = true;
		
	}
	
	void UpgradeChooseBehavior()
	{
		if(choosingUpgrade)
		{
			PointerEventData pEvData = new PointerEventData(evSystem);
			pEvData.position = Input.mousePosition;
			List<RaycastResult> uiRaycastResults = new List<RaycastResult>();
			evSystem.RaycastAll(pEvData, uiRaycastResults);
			
			if(uiRaycastResults.Count > 0)//the raycast hit something
			{
				if(Input.GetMouseButtonDown(0))
				{
					bool contains = false;
					int clickedCardIdx = int.MaxValue;
					for(int i = 0; i < cardsTr.Length; i++)
					{
						if(uiRaycastResults[0].gameObject.transform == cardsTr[i])
						{
							contains = true;
							clickedCardIdx = i;
						}
					}
					if(contains)//you have clicked one of the cards
					{
						cardsParent.SetActive(false);
						Time.timeScale = 1f;
						
						OnUpgradeChosen(upgradesYouCanChoose[clickedCardIdx]);
					}
				}
			}
		}
		
	}
	
	protected void Update()
	{
		UpgradeChooseBehavior();
	}
	
	
}

[System.Serializable]
public class Upgrade
{
	public string upgradeName;
	public Sprite upgradeSprite;
}

[System.Serializable]
public class StatsUpgrade : Upgrade
{
	[Header("Stats")]
	public float damageBonus;
	public float attackSpeedBonus;
	public float luckBonus;
}

//ALL THE TECNICHE
[System.Serializable]
public class TechniqueUpgrade : Upgrade
{
	
}

[System.Serializable]
public class DemolitionGlovesUpgrade : TechniqueUpgrade
{
	public float damageAgainstAsteroidsBonus;
}





