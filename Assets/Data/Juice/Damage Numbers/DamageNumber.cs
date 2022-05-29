using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamageNumber : MonoBehaviour
{
	TextMeshProUGUI label;
	void Awake()
    {
	    label = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
	    label.text = "";
	    label.gameObject.SetActive(false);
    }

	LTSeq animSequence;
	Transform cameraTr;
	Vector3 randomMovementVec = Vector3.zero;
	//LTDescr scaleTween0, scaleTween1;
	public void SetNumber(float nNum, Vector3 nPos, Transform nCameraTr, bool randomMovement = true)
	{
		if(randomMovement)
		{
			randomMovementVec = new Vector3(Random.Range(-3f, 3f), Random.Range(3f, 8f), Random.Range(-3f, 3f));
		}
		
		transform.position = nPos;
		cameraTr = nCameraTr;
		label.gameObject.SetActive(true);
		label.text = (Mathf.FloorToInt(nNum * 50f)).ToString();
		
		Vector3 maxScale = new Vector3(2.3f, 2.3f, 1f);
		Vector3 initScale = new Vector3(0f, 0f, 1f);
		
		if(animSequence != null)
		{
			LeanTween.cancel(this.gameObject);
		}
		
		animSequence = LeanTween.sequence();
		animSequence.append(LeanTween.scale(this.gameObject, maxScale, 0.2f).setEase(LeanTweenType.easeOutQuad));
		animSequence.append(LeanTween.scale(this.gameObject, initScale, 0.4f).setEase(LeanTweenType.easeInQuad));
		animSequence.append(() => 
		{
			label.gameObject.SetActive(false);
		});
		
	}
	
	protected void Update()
	{
		if(label.gameObject.activeSelf)
		{
			transform.LookAt(cameraTr, cameraTr.up);
			
			if(randomMovementVec != Vector3.zero)//deceleration
			{
				transform.position += randomMovementVec * Time.deltaTime;
				randomMovementVec = Vector3.Lerp(randomMovementVec, Vector3.zero, Time.deltaTime);
				if(randomMovementVec.sqrMagnitude <= 0.1f)
				{
					randomMovementVec = Vector3.zero;
				}
			}
		}
	}
	
	
    
}
