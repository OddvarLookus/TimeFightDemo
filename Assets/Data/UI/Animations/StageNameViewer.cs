using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using TMPro;

public class StageNameViewer : MonoBehaviour
{
    
	[SerializeField] float stayTime = 1f;
	[SerializeField] GameObject label;
	
	[Header("Tweens Easing")]
	[SerializeField] LeanTweenType appearEasing;
	[SerializeField] LeanTweenType disappearEasing;
	
	public void StartStageView(string stageName)
	{
		label.SetActive(true);
		label.GetComponent<TextMeshProUGUI>().text = stageName;
		
		label.transform.localScale = new Vector3(0f, 0f, 1f);
		LeanTween.scale(label, new Vector3(1.4f, 1.4f, 1.4f), stayTime - 0.4f).setEase(appearEasing);
		StartCoroutine(StageViewRemoveCoroutine());
	}
	
	IEnumerator StageViewRemoveCoroutine()
	{
		yield return new WaitForSeconds(stayTime);
		
		LeanTween.scale(label, new Vector3(6f, 0f, 1f), 0.2f).setEase(disappearEasing).setOnComplete(
			()=>
			{
				label.SetActive(false);
			}
		);
		
		
	}
    
}
