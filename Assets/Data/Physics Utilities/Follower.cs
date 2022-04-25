using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class Follower : MonoBehaviour
{
	
	[SceneObjectsOnly] [SerializeField] Transform transformToFollow;
	[SerializeField] Vector3 offset;
	enum FollowType{UPDATE = 0, FIXED_UPDATE = 1}
	[SerializeField] FollowType followType;
	
	
	protected void Update()
	{
		if(followType == FollowType.UPDATE)
		{
			Follow();
		}
	}
	
	protected void FixedUpdate()
	{
		if(followType == FollowType.FIXED_UPDATE)
		{
			Follow();
		}
	}
	
	void Follow()
	{
		Vector3 newPos = transformToFollow.position + offset;
		transform.position = newPos;
	}
	
	protected void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawWireCube(transformToFollow.position + offset, new Vector3(0.2f, 0.2f, 0.2f));
	}
	
}
