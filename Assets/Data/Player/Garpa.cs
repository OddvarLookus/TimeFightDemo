using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class Garpa : MonoBehaviour
{
    
	[SceneObjectsOnly] [SerializeField] CameraController camController;
	[SceneObjectsOnly] [SerializeField] PlayerController playerController;
	[SerializeField] float garpaSpeed;
	[SerializeField] float garpaRotSpeed;
	[SerializeField] float garpaSizeAddition;
	[SerializeField] float minGarpaDistance;
    
	[Header("Garpa Alert")]
	[MinValue(0f)] [SerializeField] float minGarpaAlertDist;
	[MinValue(0f)] [SerializeField] float maxGarpaAlertDist;
	bool prevGarpaAlert = false;
	bool garpaAlert = false;
	public bool GetGarpaAlert(){return garpaAlert;}
    
	[AssetsOnly] [SerializeField] GameObject garpaTeleportNotifierPrefab;
	Transform garpaTeleportNotifier;
	
	[Header("Garpa Pointer")]
	[SceneObjectsOnly] [SerializeField] LineRenderer pointerLR;
	
	protected void Start()
	{
		//INSTANTIATE TELEPORT NOTIFIER
		GameObject nTeleportNotif = Instantiate(garpaTeleportNotifierPrefab);
		garpaTeleportNotifier = nTeleportNotif.transform;
		garpaTeleportNotifier.SetParent(transform.parent, false);
		garpaTeleportNotifier.transform.position = transform.position;
		garpaTeleportNotifier.transform.localScale = new Vector3(0f, 0f, 1f);
		
		//START POINTER
		StartCoroutine(NearestEnemyRefreshCoroutine());
	}
    
	protected void Update()
	{
		if(!GameManager.instance.IsGamePaused())
		{
			GarpaMovementBehavior();
		}
		
		
		ComputeGarpaAlert();
		GarpaAlertAnimation();
		
		RefreshPointer();
		
		prevGarpaAlert = garpaAlert;
	}
    
	Vector3 targetGarpaPos = Vector3.zero;
	bool garpaNextToPlayer = false;
	void GarpaMovementBehavior()
	{
	
		if(camController.GetCameraMode() == CameraController.CameraMode.ENEMYLOCK)
		{
			Transform lockedEnemy = camController.GetCurrentlyLockedEnemy();
			if(lockedEnemy != null)
			{
				if((transform.position - lockedEnemy.position).magnitude < 5f)
				{
					float garpaMadArea = lockedEnemy.localScale.x + garpaSizeAddition;
					Vector3 randGarpaPos = new Vector3(Random.Range(-garpaMadArea, garpaMadArea), Random.Range(-garpaMadArea, garpaMadArea), Random.Range(-garpaMadArea, garpaMadArea));
					randGarpaPos = Vector3.ClampMagnitude(randGarpaPos, garpaMadArea);
				
					transform.position = lockedEnemy.position + randGarpaPos;
				}
				else
				{
					transform.position = Vector3.Lerp(transform.position, lockedEnemy.position, garpaSpeed * Time.deltaTime);
				}
			}

			
		}
		else if(camController.GetCameraMode() == CameraController.CameraMode.FREELOOK)
		{
			//get nearest position in a radius around the player to position the garpa
			Vector3 nearestPos = (transform.position - playerController.transform.position).normalized;
			nearestPos *= minGarpaDistance;
			nearestPos += playerController.transform.position;
			
			//when garpa is next to the player, rotate around the player
			float distGarpaToPlayer = (transform.position - playerController.transform.position).magnitude;
			if(distGarpaToPlayer <= minGarpaDistance + 0.5f)
			{
				if(garpaNextToPlayer == false)
				{
					targetGarpaPos = transform.position - playerController.transform.position;
					garpaNextToPlayer = true;
				}
				
				targetGarpaPos = targetGarpaPos.normalized * minGarpaDistance;
				targetGarpaPos = Quaternion.AngleAxis(garpaRotSpeed * Time.deltaTime, Vector3.up) * targetGarpaPos;
				nearestPos = targetGarpaPos + playerController.transform.position;
				
			}
			else
			{
				garpaNextToPlayer = false;
			}
			
			transform.position = Vector3.Lerp(transform.position, nearestPos, garpaSpeed * Time.deltaTime);
		}
		
	}
	
	#region GARPA TELEPORT SUPPORT
	void ComputeGarpaAlert()
	{
		if(camController.GetCameraMode() == CameraController.CameraMode.ENEMYLOCK)
		{
			//Check The Distance for the alert
			Vector3 garpaToPlayer = playerController.transform.position - transform.position;
			float garpaDist = garpaToPlayer.magnitude;
			if(garpaDist >= minGarpaAlertDist && garpaDist <= maxGarpaAlertDist)
			{
				//Garpa Is Alert
				garpaAlert = true;
				garpaTeleportNotifier.position = camController.GetCurrentlyLockedEnemy().position + new Vector3(0f, 4.5f, 0f);
				garpaTeleportNotifier.LookAt(camController.transform.position, camController.transform.up);
			}
			else
			{
				garpaAlert = false;
			}
		}
		else
		{
			garpaAlert = false;
		}
	}
	
	void GarpaAlertAnimation()
	{
		if(garpaAlert != prevGarpaAlert)
		{
			LeanTween.cancel(garpaTeleportNotifier.gameObject);
		}
		
		if(garpaAlert == true && prevGarpaAlert == false)//just became alert
		{
			LeanTween.scale(garpaTeleportNotifier.gameObject, new Vector3(1.6f, 1.6f, 1f), 0.3f).setEase(LeanTweenType.easeInOutQuart);
		}
		else if(garpaAlert == false && prevGarpaAlert == true)//just became not alert
		{
			LeanTween.scale(garpaTeleportNotifier.gameObject, new Vector3(0f, 0f, 1f), 0.3f).setEase(LeanTweenType.easeInOutQuart);
		}
	}
	
	public Vector3 AdviceTeleportOffset()
	{
		float teleportDist = minGarpaAlertDist / 7f;
		Vector3 enemyToPlayer = playerController.transform.position - camController.GetCurrentlyLockedEnemy().position;
		enemyToPlayer = enemyToPlayer.normalized * teleportDist;
		return enemyToPlayer;
	}
	
	public Transform GetLockedEnemy()
	{
		return camController.GetCurrentlyLockedEnemy();
	}
	#endregion
	
	#region GARPA POINTER
	
	Transform nearestEnemy;
	IEnumerator NearestEnemyRefreshCoroutine()
	{
		yield return new WaitForSeconds(0.2f);
		
		if(camController.GetCameraMode() == CameraController.CameraMode.FREELOOK)
		{
			if(EnemySystemManager.instance.GetFewEnemiesRemain() && EnemySystemManager.instance.GetEnemiesNum() > 0)
			{
				nearestEnemy = EnemySystemManager.instance.GetNearestEnemy(transform.position);
			}
			else
			{
				nearestEnemy = null;
			}
		}
		else
		{
			nearestEnemy = null;
		}

		
		StartCoroutine(NearestEnemyRefreshCoroutine());
	}
	
	void RefreshPointer()
	{
		if(nearestEnemy != null)
		{
			if(pointerLR.enabled == false)
			{
				pointerLR.enabled = true;
			}
			
			//compute the last position
			Vector3 lastPos;
			Vector3 rayDir = nearestEnemy.position - transform.position;
			float rayDist = rayDir.magnitude;
			rayDir = rayDir.normalized;
			RaycastHit rhit;
			bool hit = Physics.Raycast(transform.position, rayDir, out rhit, rayDist, ~0, QueryTriggerInteraction.Ignore);
			if(hit)
			{
				lastPos = rhit.point;
			}
			else
			{
				lastPos = nearestEnemy.position;
			}
			
			pointerLR.SetPosition(0, transform.position);
			pointerLR.SetPosition(1, lastPos);
		}
		else//nearest enemy is null
		{
			if(pointerLR.enabled == true)
			{
				pointerLR.enabled = false;
			}
			
		}
	}
	
	#endregion
	
	protected void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.white;
		Gizmos.DrawWireSphere(playerController.transform.position, minGarpaAlertDist);
		Gizmos.DrawWireSphere(playerController.transform.position, maxGarpaAlertDist);
	}
	
    
}
