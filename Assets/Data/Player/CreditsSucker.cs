using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CreditsSucker : MonoBehaviour
{
	int currentCredits = 0;
	int maxCredits = 500;
	int currentLevel = 0;
	[SerializeField] AnimationCurve LevelUpCurve;
    
    [SerializeField] float minSuckRadius, maxSuckRadius;
    [SerializeField] float maxPlayerSpeed;

    float suckRadius;

    PlayerController playerController;
    private void Awake()
    {
	    playerController = GetComponent<PlayerController>();
	    
    }

	protected void Start()
	{
		GameUIManager.instance.SetCreditsLabel(currentCredits);
		CrupsIndicator.instance.RefreshCrupsIndicator((float)currentCredits / (float)maxCredits, currentLevel);
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
	
	void CalculateLevelUp()
	{
		int creditsExceeding = currentCredits - maxCredits;
		
		if(creditsExceeding >= 0)//LEVEL UP
		{
			currentLevel += 1;
			maxCredits = Mathf.FloorToInt(LevelUpCurve.Evaluate((float)currentLevel));
			currentCredits = creditsExceeding;
			
			if(currentCredits >= maxCredits)
			{
				CalculateLevelUp();
			}
		}
		else//NO LEVEL UP
		{
			
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
