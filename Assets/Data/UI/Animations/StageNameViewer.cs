using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using TMPro;

public class StageNameViewer : MonoBehaviour
{
    
	[SerializeField] float stayTime = 1f;
	[SerializeField] GameObject label;
	
	public void StartStageView(string stageName)
	{
		label.SetActive(true);
		label.GetComponent<TextMeshProUGUI>().text = stageName;
		StartCoroutine(StageViewRemoveCoroutine());
	}
	
	IEnumerator StageViewRemoveCoroutine()
	{
		yield return new WaitForSeconds(stayTime);
		label.SetActive(false);
		
	}
    
}
