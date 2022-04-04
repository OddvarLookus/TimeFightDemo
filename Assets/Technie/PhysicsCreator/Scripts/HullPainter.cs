﻿using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Technie.PhysicsCreator
{
	public class HullMapping
	{
		public Hull sourceHull;

		public Collider generatedCollider;
		public MeshCollider[] autoGeneratedColliders; // null if no auto generated colliders

		public HullPainterChild targetChild; // null if non-child
		public HullPainterChild[] targetAutoGeneratedChilds; // null if no auto generated colliders

		public void AddAutoChild(HullPainterChild newChild, MeshCollider newCollider)
		{
			if (newChild != null)
			{
				List<HullPainterChild> childList = new List<HullPainterChild>();
				if (targetAutoGeneratedChilds != null)
					childList.AddRange(targetAutoGeneratedChilds);

				if (!childList.Contains(newChild))
				{
					childList.Add(newChild);
					this.targetAutoGeneratedChilds = childList.ToArray();
				}
			}


			if (newCollider != null)
			{
				List<MeshCollider> colliderList = new List<MeshCollider>();
				if (autoGeneratedColliders != null)
					colliderList.AddRange(autoGeneratedColliders);

				if (!colliderList.Contains(newCollider))
				{
					colliderList.Add(newCollider);
					this.autoGeneratedColliders = colliderList.ToArray();
				}
			}
		}
	}

	public class HullPainter : MonoBehaviour
	{
		public PaintingData paintingData;
		public HullData hullData;

		private List<HullMapping> hullMapping;

		private Mesh debugMesh;

		void OnDestroy()
		{
#if UNITY_EDITOR
			SceneView.RepaintAll();
#endif
		}

		public void CreateColliderComponents (Mesh[] autoHulls)
		{
			CreateHullMapping ();

			foreach (Hull hull in paintingData.hulls)
			{
				UpdateCollider(hull);
			}

			// Do auto-hulls last so that the components are always last on the game object
			foreach (Hull hull in paintingData.hulls)
			{
				CreateAutoHulls(hull, autoHulls);
			}
		}

		/** Remove all colliders, leave child objects and HullPainterChild components for later recreation */
		public void RemoveAllColliders ()
		{
			// Don't immediately refresh the hull mappings, as that could create new child objects and we're just trying to delete existing ones

			if (hullMapping != null)
			{
				// Destroy just the generated colliders
				foreach (HullMapping mapping in hullMapping)
				{
					DestroyImmediateWithUndo(mapping.generatedCollider);

					if (mapping.autoGeneratedColliders != null)
					{
						foreach (MeshCollider col in mapping.autoGeneratedColliders)
						{
							DestroyImmediateWithUndo(col);
						}
					}
				}

				// Delete all mappings that aren't child mappings (as they will have no collider now)
				for (int i=hullMapping.Count-1; i>=0; i--)
				{
					if (hullMapping[i].targetChild != null)
					{
						hullMapping.RemoveAt(i);
					}
				}
			}
		}

		/** Removes all generated components - colliders and child objects with child colliders and HullPainterChild on them */
		public void RemoveAllGenerated()
		{
			CreateHullMapping();

			foreach (HullMapping mapping in hullMapping)
			{
				DestroyImmediateWithUndo(mapping.generatedCollider);
				if (mapping.targetChild != null)
				{
					DestroyImmediateWithUndo(mapping.targetChild.gameObject);
				}

				if (mapping.autoGeneratedColliders != null)
				{
					foreach (MeshCollider col in mapping.autoGeneratedColliders)
					{
						DestroyImmediateWithUndo(col);
					}
				}

				if (mapping.targetAutoGeneratedChilds != null)
				{
					foreach (HullPainterChild child in mapping.targetAutoGeneratedChilds)
					{
						GameObject childObj = child.gameObject;

						DestroyImmediateWithUndo(child);

						// If the child has no children and is now empty, destroy the object as well
						if (childObj.transform.childCount == 0 && childObj.GetComponents<Component>().Length == 1) // length=1 means only transform left
							DestroyImmediateWithUndo(childObj);
					}
				}
			}
		}

		private static bool IsDeletable(GameObject obj)
		{
			Component[] allComps = obj.GetComponents<Component>();

			int numIgnorable = 0;

			foreach (Component comp in allComps)
			{
				if (comp is Transform
					|| comp is Collider
					|| comp is HullPainter
					|| comp is HullPainterChild)
				{
					numIgnorable++;
				}
			}

			return allComps.Length == numIgnorable;
		}

		private static void DestroyImmediateWithUndo(Object obj)
		{
			if (obj == null)
				return;
#if UNITY_EDITOR
			Undo.DestroyObjectImmediate(obj);
#else
			GameObject.DestroyImmediate(obj);
#endif
		}

		private void CreateHullMapping()
		{
			if (hullMapping == null)
			{
				hullMapping = new List<HullMapping>();
			}

			// Remove any invalid entries from the hull mapping
			//	null entries are garbage and can be dropped
			//	null source hull means the hull has been deleted and this mapping is no longer relevant
			//	missing *both* generated collider *and* target child means there's no data to point at, so might as well remove it and regen from scratch
			for (int i = hullMapping.Count - 1; i >= 0; i--)
			{
				HullMapping mapping = hullMapping[i];
				if (mapping == null
					|| mapping.sourceHull == null
					|| (mapping.generatedCollider == null && mapping.targetChild == null))
				{
					hullMapping.RemoveAt(i);
				}
			}

			// Check to see if any existing mappings need updating (hull.type doesn't match Collider type, or child type no longer matches)

			foreach (Hull hull in paintingData.hulls)
			{
				// First map non-auto hulls
				if (IsMapped(hull))
				{
					// We already have a mapping for this, but is it still of the correct type?
					
					Collider value = FindExistingCollider(hullMapping, hull);

					bool isHullOk = (hull.type == HullType.ConvexHull && value is MeshCollider);
					bool isBoxOk = (hull.type == HullType.Box && value is BoxCollider);
					bool isCapsuleOk = (hull.type == HullType.Capsule && value is CapsuleCollider);
					bool isSphereOk = (hull.type == HullType.Sphere && value is SphereCollider);
					bool isFaceOk = (hull.type == HullType.Face && value is MeshCollider);
					bool isFaceBoxOk = (hull.type == HullType.FaceAsBox && value is BoxCollider);
					bool isAutoOk = (hull.type == HullType.Auto && value is MeshCollider && hull.autoMeshes != null && hull.autoMeshes.Length > 0);

					bool isColliderTypeOk = (isHullOk || isBoxOk || isCapsuleOk || isSphereOk || isFaceOk || isFaceBoxOk || isAutoOk);
					bool isChildTypeOk = value == null
										|| ((hull.isChildCollider) == (value.transform.parent == this.transform))
										|| (hull.type == HullType.Auto); // Auto hulls will fix any isChildCollider mismatches later

					if (isColliderTypeOk && isChildTypeOk)
					{
						// All good
					}
					else
					{
						// Mismatch - hull.type doesn't match collider type
						// Delete the collider and remove the mapping
						// This hull will then be orphaned, and a new collider added back in accordingly
						DestroyImmediateWithUndo(value);
						RemoveMapping(hull);
					}
				}
			}

			// Connect orphans
			//
			// Find hulls without a Collider
			// Find Colliders without hulls
			// Try and map the two together

			// First find orphans - hull, colliders or childs that aren't already mapped

			List<Hull> orphanedHulls = new List<Hull>();
			List<Collider> orphanedColliders = new List<Collider>();
			List<HullPainterChild> orphanedChilds = new List<HullPainterChild>();

			foreach (Hull h in paintingData.hulls)
			{
				if (!IsMapped(h))
				{
					orphanedHulls.Add(h);
				}
			}
			
			foreach (Collider c in FindLocal<Collider>())
			{
				if (!IsMapped(c))
				{
					orphanedColliders.Add(c);
				}
			}

			foreach (HullPainterChild c in FindLocal<HullPainterChild>())
			{
				if (!IsMapped(c))
				{
					orphanedChilds.Add(c);
				}
			}

			// Try and connect orphaned hulls with orphaned colliders

			for (int i = orphanedHulls.Count - 1; i >= 0; i--)
			{
				Hull h = orphanedHulls[i];

				bool matchedHull = false;

				for (int j = orphanedColliders.Count - 1; j >= 0; j--)
				{
					Collider c = orphanedColliders[j];
					MeshCollider meshCol = c as MeshCollider;
					BoxCollider boxCol = c as BoxCollider;
					CapsuleCollider capCol = c as CapsuleCollider;
					SphereCollider sphereCol = c as SphereCollider;

					// Find the HullPainterChild adjacent to the collider (if a child collider)
					HullPainterChild child = null;
					if (c.transform.parent == this.transform)
					{
						child = c.gameObject.GetComponent<HullPainterChild>();
					}

					// todo needs better handling
					bool isMatchingChild = h.isChildCollider && c.transform.parent == this.transform;
					bool isMatchingAuto = child != null && child.isAutoHull && h.type == HullType.Auto && meshCol != null && h.ContainsAutoMesh(meshCol.sharedMesh);
					if (isMatchingAuto)
					{
						HullMapping autoMapping = FindMapping(h);
						if (autoMapping == null)
						{
							autoMapping = new HullMapping();
							autoMapping.sourceHull = h;
							hullMapping.Add(autoMapping);
						}
						
						autoMapping.AddAutoChild(child, c as MeshCollider);

						child.parent = this;

						// These are no longer orphaned, so remove them from these lists
						orphanedColliders.RemoveAt(j);
						orphanedChilds.Remove(child);

						// Hull no longer orphaned, so flag to remove it once we've finished trying other colliders
						matchedHull = true;
					}
					else if (isMatchingChild)
					{
						bool isMatchingBox = h.type == HullType.Box && c is BoxCollider && Approximately(h.collisionBox.collisionBox.center, boxCol.center) && Approximately(h.collisionBox.collisionBox.size, boxCol.size);
						bool isMatchingSphere = h.type == HullType.Sphere && c is SphereCollider && h.collisionSphere != null && Approximately(h.collisionSphere.center, sphereCol.center) && Approximately(h.collisionSphere.radius, sphereCol.radius);
						bool isMatchingCapsule = h.type == HullType.Capsule && c is CapsuleCollider && Approximately(h.collisionCapsule.capsuleCenter, capCol.center) && (int)h.collisionCapsule.capsuleDirection == capCol.direction && Approximately(h.collisionCapsule.capsuleRadius, capCol.radius) && Approximately(h.collisionCapsule.capsuleHeight, capCol.radius);
						bool isMatchingConvexHull = h.type == HullType.ConvexHull && c is MeshCollider && meshCol.sharedMesh == h.collisionMesh;
						bool isMatchingFace = h.type == HullType.Face && c is MeshCollider && meshCol.sharedMesh == h.faceCollisionMesh;
						bool isMatchingFaceAsBox = h.type == HullType.FaceAsBox && c is BoxCollider && Approximately(h.faceBoxCenter, boxCol.center) && Approximately(h.faceBoxSize, boxCol.size);

						if (isMatchingBox || isMatchingSphere || isMatchingCapsule || isMatchingConvexHull || isMatchingFace || isMatchingFaceAsBox)
						{
							// Found a pair, so add a mapping and remove the orphans
							AddMapping(h, c, child);

							// These are no longer orphaned, so remove them from these lists
							orphanedHulls.RemoveAt(i);
							orphanedColliders.RemoveAt(j);
							
							// Remove the no-longer orphaned child
							for (int k=0; k<orphanedChilds.Count; k++)
							{
								if (orphanedChilds[k] == child)
								{
									orphanedChilds.RemoveAt(k);
									break;
								}
							}

							break;
						}
					}
				}

				// If the hull has been matched then we can remove it from the orphaned list
				if (matchedHull)
				{
					orphanedHulls.RemoveAt(i);
				}
			}

			
			// We've tried to connect hulls to existing colliders, now try and connect hulls to existing HullPainterChilds
			// These will be child without a collider (as otherwise they'd have be picked up earlier)
			for (int i = orphanedHulls.Count - 1; i >= 0; i--)
			{
				Hull h = orphanedHulls[i];

				if (!h.isChildCollider)
					continue;

				for (int j = orphanedChilds.Count - 1; j >= 0; j--)
				{
					HullPainterChild child = orphanedChilds[j];
					HullMapping mapping = FindMapping(child);

					if (mapping != null && mapping.sourceHull != null)
					{
						// Found a match for hull-mapping-child

						// Ensure this still has a collider
						if (mapping.generatedCollider == null)
						{
							// Recreate the collider of the correct type with the existing hull-mapping-child

							RecreateChildCollider(mapping);
						}

						orphanedHulls.RemoveAt(i);
						orphanedChilds.RemoveAt(j);
						break;
					}
				}
			}

			// Try and match up orphaned auto childs with hulls
			// These will be ones without a collider (those will be picked up earlier)
			// FIXME: Not working atm. Fix and see if we actually need it
			
			for (int i = orphanedHulls.Count - 1; i >= 0; i--)
			{
				Hull h = orphanedHulls[i];

				if (!h.isChildCollider || h.type != HullType.Auto)
					continue;

				bool matchedHull = false;
				for (int j = orphanedChilds.Count - 1; j >= 0; j--)
				{
					HullPainterChild child = orphanedChilds[j];


					if (child.isAutoHull && child.gameObject.name.StartsWith(h.name))
					{
						HullMapping mapping = FindMapping(h);
						if (mapping == null)
						{
							mapping = new HullMapping();
							mapping.sourceHull = h;
							hullMapping.Add(mapping);
						}
						mapping.AddAutoChild(child, null);

						orphanedChilds.RemoveAt(j);
						matchedHull = true;
					}
				}

				if (matchedHull)
					orphanedHulls.RemoveAt(i);
			}
			

			// Create colliders for any hull mapping children without colliders
			foreach (HullMapping mapping in hullMapping)
			{
				if (mapping.targetChild != null && mapping.generatedCollider == null)
					RecreateChildCollider(mapping);
			}

			// Create child components for child colliders without them
			foreach (HullMapping mapping in hullMapping)
			{
				if (mapping.targetChild == null && mapping.generatedCollider != null && mapping.generatedCollider.transform.parent == this.transform)
				{
					// Mapping has a child collider but no HullPainterChild
					// Recreate the child component
					HullPainterChild newChild = AddComponent<HullPainterChild>(mapping.generatedCollider.gameObject);
					newChild.parent = this;
					mapping.targetChild = newChild;
				}
			}

			// Create colliders for any left over hulls

			foreach (Hull h in orphanedHulls)
			{
				if (h.type == HullType.Box)
				{
					CreateCollider<BoxCollider>(h);
				}
				else if (h.type == HullType.Sphere)
				{
					CreateCollider<SphereCollider>(h);
				}
				else if (h.type == HullType.ConvexHull)
				{
					CreateCollider<MeshCollider>(h);
				}
				else if (h.type == HullType.Face)
				{
					CreateCollider<MeshCollider>(h);
				}
				else if (h.type == HullType.FaceAsBox)
				{
					CreateCollider<BoxCollider>(h);
				}
				else if (h.type == HullType.Capsule)
				{
					CreateCollider<CapsuleCollider>(h);
				}
			}
			
			// Delete any left over colliders
			// TODO: This probably isn't properly undo-aware

			foreach (Collider c in orphanedColliders)
			{
				if (c == null)
					continue;

				if (c.gameObject == this.gameObject)
				{
					DestroyImmediateWithUndo(c);
				}
				else
				{
					// Child collider - delete collider, HullPainterChild (if any) and GameObject (if empty)

					GameObject go = c.gameObject;
					DestroyImmediateWithUndo(c);
					DestroyImmediateWithUndo(go.GetComponent<HullPainterChild>());
					if (IsDeletable(go))
					{
						DestroyImmediateWithUndo(go);
					}
				}
			}

			// Delete any left over hull painter childs
			// TODO: This probably isn't undo-aware
			
			foreach (HullPainterChild child in orphanedChilds)
			{
				if (child == null)
					continue;

				// Delete child, collider (if any) and GameObject (if empty)
				GameObject go = child.gameObject;
				DestroyImmediateWithUndo(child);
				DestroyImmediateWithUndo(go.GetComponent<Collider>());
				if (IsDeletable(go))
				{
					DestroyImmediateWithUndo(go);
				}
			}

			// Sanity check - all hull mappings should have a collider of the right type now
		//	foreach (HullMapping mapping in hullMapping)
		//	{
		//		if (mapping.generatedCollider == null)
		//			Debug.LogWarning("Null collider for hull: " + mapping.sourceHull.name);
		//	}
		}

		private static bool Approximately(Vector3 lhs, Vector3 rhs)
		{
			return Mathf.Approximately (lhs.x, rhs.x) && Mathf.Approximately (lhs.y, rhs.y) && Mathf.Approximately (lhs.z, rhs.z);
		}
		private static bool Approximately(float lhs, float rhs)
		{
			return Mathf.Approximately(lhs, rhs);
		}

		private void CreateCollider<T>(Hull sourceHull) where T : Collider
		{
			if (sourceHull.isChildCollider)
			{
			//	GameObject newChild = new GameObject(sourceHull.name);
				GameObject newChild = CreateGameObject(sourceHull.name);
				newChild.transform.SetParent(this.transform, false);
				newChild.transform.localPosition = Vector3.zero;
				newChild.transform.localRotation = Quaternion.identity;
				newChild.transform.localScale = Vector3.one;

				HullPainterChild childPainter = AddComponent<HullPainterChild>(newChild);
				childPainter.parent = this;

				T col = AddComponent<T>(newChild);
				AddMapping(sourceHull, col, childPainter);
			}
			else
			{
				T col = AddComponent<T>(this.gameObject);
				AddMapping(sourceHull, col, null);
			}
		}

		private void RecreateChildCollider(HullMapping mapping)
		{
			if (mapping == null || mapping.sourceHull == null || !mapping.sourceHull.isChildCollider)
				return;

			if (mapping.sourceHull.type == HullType.Box)
			{
				RecreateChildCollider<BoxCollider>(mapping);
			}
			else if (mapping.sourceHull.type == HullType.Sphere)
			{
				RecreateChildCollider<SphereCollider>(mapping);
			}
			else if (mapping.sourceHull.type == HullType.ConvexHull)
			{
				RecreateChildCollider<MeshCollider>(mapping);
			}
			else if (mapping.sourceHull.type == HullType.Face)
			{
				RecreateChildCollider<MeshCollider>(mapping);
			}
			else if (mapping.sourceHull.type == HullType.FaceAsBox)
			{
				RecreateChildCollider<BoxCollider>(mapping);
			}
			else if (mapping.sourceHull.type == HullType.Capsule)
			{
				RecreateChildCollider<CapsuleCollider>(mapping);
			}
		}

		private void RecreateChildCollider<T>(HullMapping mapping) where T : Collider
		{
			if (mapping.sourceHull == null || !mapping.sourceHull.isChildCollider)
				return;
			
			T col = AddComponent<T>(mapping.targetChild.gameObject);
			mapping.generatedCollider = col;
		}

		// Updates the existing collider for this hull
		// A collider of the correct type wil already exist, we just need to sync up the latest position/size/etc. data from the hull to the collider
		private void UpdateCollider(Hull hull)
		{
			Collider c = null;

			if (hull.type == HullType.Box)
			{
				BoxCollider boxCollider = FindExistingCollider(hullMapping, hull) as BoxCollider;
				boxCollider.center = hull.collisionBox.collisionBox.center;
				boxCollider.size = hull.collisionBox.collisionBox.size + (hull.enableInflation ? Vector3.one * hull.inflationAmount : Vector3.zero);
				if (hull.isChildCollider)
				{
					boxCollider.transform.localPosition = hull.collisionBox.boxPosition;
					boxCollider.transform.localRotation = hull.collisionBox.boxRotation;
				}
				c = boxCollider;
			}
			else if (hull.type == HullType.Sphere)
			{
				SphereCollider sphereCollider = FindExistingCollider(hullMapping, hull) as SphereCollider;
				sphereCollider.center = hull.collisionSphere.center;
				sphereCollider.radius = hull.collisionSphere.radius + (hull.enableInflation ? hull.inflationAmount : 0.0f);
				c = sphereCollider;
			}
			else if (hull.type == HullType.Capsule)
			{
				CapsuleCollider capsuleCollider = FindExistingCollider(hullMapping, hull) as CapsuleCollider;
				capsuleCollider.center = hull.collisionCapsule.capsuleCenter;
				capsuleCollider.direction = (int)hull.collisionCapsule.capsuleDirection;
				capsuleCollider.radius = hull.collisionCapsule.capsuleRadius;
				capsuleCollider.height = hull.collisionCapsule.capsuleHeight;
				if (hull.isChildCollider)
				{
					capsuleCollider.transform.localPosition = hull.collisionCapsule.capsulePosition;
					capsuleCollider.transform.localRotation = hull.collisionCapsule.capsuleRotation;
				}
				c = capsuleCollider;
			}
			else if (hull.type == HullType.ConvexHull)
			{
				MeshCollider meshCollider = FindExistingCollider(hullMapping, hull) as MeshCollider;
				meshCollider.sharedMesh = hull.collisionMesh;
				meshCollider.convex = true;
#if !UNITY_2018_4_OR_NEWER
				meshCollider.inflateMesh = hull.enableInflation;
				meshCollider.skinWidth = hull.inflationAmount;
#endif
				c = meshCollider;
			}
			else if (hull.type == HullType.Face)
			{
				MeshCollider faceCollider = FindExistingCollider(hullMapping, hull) as MeshCollider;
				faceCollider.sharedMesh = hull.faceCollisionMesh;
				faceCollider.convex = true;
#if !UNITY_2018_4_OR_NEWER
				faceCollider.inflateMesh = hull.enableInflation;
				faceCollider.skinWidth = hull.inflationAmount;
#endif
				c = faceCollider;
			}
			else if (hull.type == HullType.FaceAsBox)
			{
				BoxCollider boxCollider = FindExistingCollider(hullMapping, hull) as BoxCollider;
				boxCollider.center = hull.faceBoxCenter;
				boxCollider.size = hull.faceBoxSize + (hull.enableInflation ? Vector3.one * hull.inflationAmount : Vector3.zero);
				if (hull.isChildCollider)
				{
					boxCollider.transform.localRotation = hull.faceAsBoxRotation;
				}
				c = boxCollider;
			}
			else if (hull.type == HullType.Auto)
			{
				// ..?
			}

			if (c != null)
			{
				c.material = hull.material;
				c.isTrigger = hull.isTrigger;

				// Sync the child object's name with the hull
				if (hull.isChildCollider)
				{
					c.gameObject.name = hull.name;
				}
			}
		}

		public void SetAllTypes (HullType newType)
		{
			foreach (Hull h in paintingData.hulls)
			{
				h.type = newType;
			}
		}

		public void SetAllMaterials (PhysicMaterial newMaterial)
		{
			foreach (Hull h in paintingData.hulls)
			{
				h.material = newMaterial;
			}
		}

		public void SetAllAsChild(bool isChild)
		{
			foreach (Hull h in paintingData.hulls)
			{
				h.isChildCollider = isChild;
			}
		}

		public void SetAllAsTrigger(bool isTrigger)
		{
			foreach (Hull h in paintingData.hulls)
			{
				h.isTrigger = isTrigger;
			}
		}

		private List<T> FindLocal<T>() where T : Component
		{
			List<T> localComps = new List<T>();

			localComps.AddRange(this.gameObject.GetComponents<T>());

			for (int i=0; i<transform.childCount; i++)
			{
				localComps.AddRange(transform.GetChild(i).GetComponents<T>());
			}

			return localComps;
		}

		private bool IsMapped(Hull hull)
		{
			if (hullMapping == null)
				return false;

			foreach (HullMapping map in hullMapping)
			{
				if (map.sourceHull == hull)
					return true;
			}
			return false;
		}

		private bool IsMapped(Collider col)
		{
			if (hullMapping == null)
				return false;

			foreach (HullMapping map in hullMapping)
			{
				if (map.generatedCollider == col)
					return true;
			}
			return false;
		}

		private bool IsMapped(HullPainterChild child)
		{
			if (hullMapping == null)
				return false;

			foreach (HullMapping map in hullMapping)
			{
				if (map.targetChild == child)
					return true;
			}
			return false;
		}

		private void AddMapping(Hull hull, Collider col, HullPainterChild painterChild)
		{
			HullMapping newMapping = new HullMapping()
			{
				sourceHull = hull,
				generatedCollider = col,
				targetChild = painterChild
			};

			this.hullMapping.Add(newMapping);
		}

		private void RemoveMapping(Hull hull)
		{
			for (int i=0; i<hullMapping.Count; i++)
			{
				if (hullMapping[i].sourceHull == hull)
				{
					hullMapping.RemoveAt(i);
					return;
				}
			}
		}

		private HullMapping FindMapping(HullPainterChild child)
		{
			if (hullMapping == null)
				return null;

			foreach (HullMapping h in hullMapping)
			{
				if (h.targetChild == child)
					return h;
			}
			return null;
		}
		/*
		private HullMapping FindAutoMapping(HullPainterChild childToFind)
		{
			if (hullMapping == null)
				return null;
			foreach (HullMapping h in hullMapping)
			{
				if (h.targetAutoGeneratedChilds != null)
				{
					foreach (HullPainterChild c in h.targetAutoGeneratedChilds)
					{
						if (c == childToFind)
							return h;
					}
				}
			}
			return null;
		}
		*/
		private HullMapping FindMapping(Hull hull)
		{
			if (hullMapping == null)
				return null;

			foreach (HullMapping h in hullMapping)
			{
				if (h.sourceHull == hull)
					return h;
			}
			return null;
		}

		public Hull FindSourceHull(HullPainterChild child)
		{
			if (hullMapping == null)
			{
				// TODO: Hull mapping should be serialised, when it is remove this message as it'll only exist to catch un-upgraded assets
			//	Debug.LogError("No hull mapping present!");
				return null;
			}

			foreach (HullMapping h in hullMapping)
			{
				// Check the target child (for normal hulls)
				if (h.targetChild == child)
					return h.sourceHull;

				// Check the target child array (for auto hulls)
				if (h.targetAutoGeneratedChilds != null)
				{
					foreach (HullPainterChild hC in h.targetAutoGeneratedChilds)
					{
						if (hC == child)
							return h.sourceHull;
					}
				}
			}

			return null;
		}
		
		private static Collider FindExistingCollider(List<HullMapping> mappings, Hull hull)
		{
			foreach (HullMapping map in mappings)
			{
				if (map.sourceHull == hull)
				{
					return map.generatedCollider;
				}
			}
			return null;
		}

		private void CreateAutoHulls(Hull hull, Mesh[] autoHulls)
		{
			if (hull.type != HullType.Auto)
				return;


			HullMapping mapping = FindMapping(hull);
			if (mapping == null)
			{
				mapping = new HullMapping();
				mapping.sourceHull = hull;
				hullMapping.Add(mapping);
			}

			Mesh[] clippedHulls = hull.autoMeshes;

			// Ensure we have as many collider components as we need

			List<MeshCollider> autoColliders = new List<MeshCollider>();

			// Resize the colliders array to match
			/*
			System.Array.Resize(ref mapping.autoGeneratedColliders, mapping.targetAutoGeneratedChilds.Length);
			
			// Fill in any null holes in the colliders array
			for (int i = 0; i < mapping.targetAutoGeneratedChilds.Length; i++)
			{
				if (mapping.autoGeneratedColliders[i] == null)
				{
					MeshCollider newMeshCol = mapping.targetAutoGeneratedChilds[i].gameObject.AddComponent<MeshCollider>();
					newMeshCol.convex = true;
					mapping.autoGeneratedColliders[i] = newMeshCol;
				}
			}
			*/
			//	if (mapping.autoGeneratedColliders != null)
			//		autoColliders.AddRange(mapping.autoGeneratedColliders);

			// Populate the autoColliders list to match the childs array, creating new components if we need to
			if (mapping.targetAutoGeneratedChilds != null)
			{
				for (int i = 0; i < mapping.targetAutoGeneratedChilds.Length; i++)
				{
					if (mapping.autoGeneratedColliders != null && i < mapping.autoGeneratedColliders.Length)
					{
						autoColliders.Add(mapping.autoGeneratedColliders[i]);
					}
					else
					{
						MeshCollider newMeshCol = mapping.targetAutoGeneratedChilds[i].gameObject.AddComponent<MeshCollider>();
						newMeshCol.convex = true;
						autoColliders.Add(newMeshCol);
					}
				}
			}

			for (int i= autoColliders.Count-1; i>=0; i--)
			{
				// Delete this collider if the child flag doesn't match up with what we have
				bool colliderIsChild = (autoColliders[i].transform != this.transform);
				if (colliderIsChild != this.transform && hull.isChildCollider)
				{
					if (colliderIsChild)
						GameObject.DestroyImmediate(autoColliders[i].gameObject);
					else
						GameObject.DestroyImmediate(autoColliders[i]);
					autoColliders.RemoveAt(i);
				}
			}

			// Create collider components and apply the clipped hulls to them

			for (int i=0; i< clippedHulls.Length; i++)
			{
				Mesh auto = clippedHulls[i];

				MeshCollider colliderToUse;

				if (i < autoColliders.Count)
				{
					colliderToUse = autoColliders[i];
				}
				else if (hull.isChildCollider)
				{
					GameObject child = CreateGameObject("New child");
					child.transform.SetParent(this.transform, false);

					HullPainterChild painterChild = child.AddComponent<HullPainterChild>();
					painterChild.parent = this;
					painterChild.isAutoHull = true;

					colliderToUse = child.AddComponent<MeshCollider>();

					autoColliders.Add(colliderToUse);
				}
				else
				{
					colliderToUse = this.gameObject.AddComponent<MeshCollider>();

					autoColliders.Add(colliderToUse);
				}

			//	mapping.autoGeneratedColliders[i] = colliderToUse;

				colliderToUse.sharedMesh = auto;
				colliderToUse.convex = true;
				colliderToUse.isTrigger = hull.isTrigger;
				colliderToUse.material = hull.material;

				// TODO: Inflation?
				// ..
			}

			// Rename all the collider objects by index (because the indices may have changed since last time)
			if (hull.isChildCollider)
			{
				for (int i = 0; i < autoColliders.Count; i++)
				{
					autoColliders[i].gameObject.name = string.Format("{0}.{1}", hull.name, (i + 1));
				}
			}

			// Rebuild the list of HullPainterChild components
			// Rebuilding from scratch ensures that they're in the same order as the autoGeneratedColliders array
			List<HullPainterChild> childMarkers = new List<HullPainterChild>();
			foreach (MeshCollider col in autoColliders)
			{
				childMarkers.Add(col.GetComponent<HullPainterChild>());
			}

			mapping.autoGeneratedColliders = autoColliders.ToArray();
			mapping.targetAutoGeneratedChilds = childMarkers.ToArray();

			/*
			GameObject boundsObj = new GameObject("Painted Bounds");
			boundsObj.transform.SetParent(this.transform, false);
			MeshCollider boundsCol = boundsObj.AddComponent<MeshCollider>();
			boundsCol.sharedMesh = hull.collisionMesh;
			boundsCol.convex = true;
			*/
			/*
			GameObject auto0Obj = new GameObject("Input Auto Hull 0");
			auto0Obj.transform.SetParent(this.transform, false);
			MeshCollider auto0Col = auto0Obj.AddComponent<MeshCollider>();
			auto0Col.sharedMesh = autoHulls[0];
			auto0Col.convex = true;
			*/
		}


		/*
		private static void CreatePreviewMesh(Mesh mesh, string name)
		{
			GameObject obj = new GameObject();
			obj.name = name;

		//	MeshFilter filter = obj.AddComponent<MeshFilter>();
		//	filter.sharedMesh = mesh;
		//	MeshRenderer r = obj.AddComponent<MeshRenderer>();
			// todo set material

			MeshCollider col = obj.AddComponent<MeshCollider>();
			col.sharedMesh = mesh;

		}
		*/




		private static GameObject CreateGameObject(string goName)
		{
			GameObject go = new GameObject(goName);
#if UNITY_EDITOR
			Undo.RegisterCreatedObjectUndo(go, "Created "+goName);
#endif
			return go;
		}

		private static T AddComponent<T>(GameObject targetObj) where T : Component
		{
#if UNITY_EDITOR
			return (T)Undo.AddComponent(targetObj, typeof(T));
#else
			return targetObj.AddComponent<T>();
#endif
		}

		void OnDrawGizmos()
		{
			/*
			if (debugMesh != null)
			{
				Vector3[] vertices = debugMesh.vertices;
				int[] indices = debugMesh.triangles;

				int i = 90;

				Vector3 p0 = vertices[indices[i]];
				Vector3 p1 = vertices[indices[i + 1]];
				Vector3 p2 = vertices[indices[i + 2]];

				Vector3 e0 = (p1 - p0);
				Vector3 e1 = (p2 - p0);


				Vector3 normal = Vector3.Cross(e0.normalized, e1.normalized);

				Gizmos.color = Color.white;
				Gizmos.DrawSphere(p0, 0.02f);
				Gizmos.DrawSphere(p1, 0.02f);
				Gizmos.DrawSphere(p2, 0.02f);

				Gizmos.color = Color.green;
				Gizmos.DrawLine(p0, p0 + e0);
				Gizmos.DrawLine(p0, p0 + e1);

				Gizmos.color = Color.blue;
				Gizmos.DrawLine(p0, p0 + normal);
			}
			*/
		}
	}

} // namespace Technie.PhysicsCreator

