using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Sirenix.OdinInspector;

public class CrupsIndicator : MonoBehaviour
{
	public static CrupsIndicator instance;
    
	[SceneObjectsOnly] [SerializeField] TextMeshProUGUI levelLabel;
	[SceneObjectsOnly] [SerializeField] Image crupsImage;
    
	protected void Awake()
	{
		instance = this;
	}
	
	public void RefreshCrupsIndicator(float crupsFactor)
	{
		crupsImage.fillAmount = crupsFactor;
		
	}
    
	
    
}
