using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ModularRadioSystem
{
	public class InteractionRaycast : MonoBehaviour
	{
		public RaycastHit hitInfo;
		public bool hasHit;
		public float rayDistance = 1.35f;
		public LayerMask layerMask;

		private void Start()
		{
			hitInfo = default(RaycastHit);
			layerMask = LayerMask.GetMask(new string[]
			{
				"Dashboard"
			});
		}

		private void FixedUpdate()
		{
			if (Camera.main != null)
				hasHit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo, rayDistance, layerMask);
		}

		public bool GetHit(Collider collider) => hasHit && hitInfo.collider == collider;
		public bool GetHitAny(Collider[] colliders) => hasHit && colliders.Any((Collider collider) => collider == hitInfo.collider);
		public bool GetHitAny(List<Collider> colliders) => hasHit && colliders.Any((Collider collider) => collider == hitInfo.collider);
	}
}
