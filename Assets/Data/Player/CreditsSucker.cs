﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

public class CreditsSucker : MonoBehaviour
{
	static int currentCredits = 0;
	static int maxCredits = 500;
	static int currentLevel = 0;
	[SerializeField] AnimationCurve LevelUpCurve;
    
    [SerializeField] float minSuckRadius, maxSuckRadius;
    [SerializeField] float maxPlayerSpeed;

	[SerializeField] SoundsPack levelUpSound;
	
    float suckRadius;

	PlayerController playerController;
	PlayerStatsManager playerStatsManager;
    private void Awake()
    {
	    playerController = GetComponent<PlayerController>();
	    playerStatsManager = GetComponent<PlayerStatsManager>();
    }
	
	
	
	protected void Start()
	{
		maxCredits = Mathf.FloorToInt(LevelUpCurve.Evaluate((float)currentLevel));
		
		GameUIManager.instance.SetCreditsLabel(currentCredits);
		CrupsIndicator.instance.RefreshCrupsIndicator((float)currentCredits / (float)maxCredits, currentLevel);
	}
	
	public void ResetLevels()
	{
		currentCredits = 0;
		currentLevel = 0;
		maxCredits = Mathf.FloorToInt(LevelUpCurve.Evaluate((float)currentLevel));
	}
	
    // Update is called once per frame
    void Update()
    {
        UpdateSuckZone();
        SuckCredits();

    }
    void UpdateSuckZone()
    {
        float t = playerController.GetVelocity().magnitude / maxPlayerSpeed;
        suckRadius = Mathf.Lerp(minSuckRadius, maxSuckRadius, t);
    }
    void SuckCredits()
    {
        Collider[] collHits;
        collHits = Physics.OverlapSphere(transform.position, suckRadius, ~0, QueryTriggerInteraction.Collide);
        for (int i = 0; i < collHits.Length; i++) 
        {
            if (collHits[i].gameObject.TryGetComponent(out Credit credit))
            {
	            credit.Attract(transform, this);
            }
        }

    }

    public void AddCredits(int _creditsToAdd)
	{
	    currentCredits += _creditsToAdd;
	    CalculateLevelUp();
	    //currentCredits = Mathf.Clamp(currentCredits, 0, int.MaxValue);
		
	    CrupsIndicator.instance.RefreshCrupsIndicator((float)currentCredits / (float)maxCredits, currentLevel);
        GameUIManager.instance.SetCreditsLabel(currentCredits);
    }
	
	int creditsExceeding = 0;
	bool choosingUpgrade = false;
	void CalculateLevelUp()
	{
		if(!choosingUpgrade)
		{
			creditsExceeding = currentCredits - maxCredits;
		
			if(creditsExceeding >= 0)//LEVEL UP
			{
				currentLevel += 1;
				StaticAudioStarter.instance.StartAudioEmitter(transform.position, levelUpSound.GetRandomSound(), levelUpSound.GetRandomPitch());
				//EXECUTE POWER UP ROUTINE
				Cursor.lockState = CursorLockMode.None;
				GameManager.instance.SetCanPauseGame(false);
				choosingUpgrade = true;
				ProgressionLayer.instance.StartUpgradeChoice(OnUpgradeChosen);
			}
			else//NO LEVEL UP
			{
			
			}
		}
		
	}
	
	void OnUpgradeChosen(Upgrade chosenUpgrade)
	{
		//set the upgraded stats
		playerStatsManager.AddUpgrade(chosenUpgrade);
		Cursor.lockState = CursorLockMode.Locked;
		GameManager.instance.SetCanPauseGame(true);
		choosingUpgrade = false;
		//continue level up logic
		maxCredits = Mathf.FloorToInt(LevelUpCurve.Evaluate((float)currentLevel));
		currentCredits = creditsExceeding;
			
		if(currentCredits >= maxCredits)
		{
			CalculateLevelUp();
		}
	}
	
	public bool TryBuy(int cost)
	{
		if(cost <= currentCredits)//i can afford
		{
			currentCredits -= cost;
			GameUIManager.instance.SetCreditsLabel(currentCredits);
			return true;
		}
		else//2 poor
		{
			return false;
		}
	}
	
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, minSuckRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, maxSuckRadius);
    }

}
