using System;
using UnityEngine;

namespace ModularRadioSystem
{
	internal static class Helpers
	{
		internal static bool IsPrefab(this Transform transform)
		{
			return !transform.gameObject.activeInHierarchy && transform.gameObject.activeSelf && transform.root == transform;
		}
	}
}
