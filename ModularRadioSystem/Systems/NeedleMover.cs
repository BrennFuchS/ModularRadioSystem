using System;
using UnityEngine;

namespace ModularRadioSystem
{
	public class NeedleMover : MonoBehaviour
	{
		public Transform needle;
		public Tuner tuner;
		public float needleMaxMove79 = 0.024f;
		public float needleMaxMove82 = 0.0125f;
		public float needleMaxMove86 = 0.014f;
		public float needleMaxMove925 = 0.032f;
		public Vector3 needleMoveDirection = new Vector3(0f, 0f, 1f);
		private float move79;
		private float move82;
		private float move86;
		private float move925;

		private void Update()
		{
			move79 = Mathf.Clamp(tuner.frequency - 75f, 0f, 4f) / 4f * needleMaxMove79;
			move82 = Mathf.Clamp(tuner.frequency - 79f, 0f, 3f) / 3f * needleMaxMove82;
			move86 = Mathf.Clamp(tuner.frequency - 82f, 0f, 4f) / 4f * needleMaxMove86;
			move925 = Mathf.Clamp(tuner.frequency - 86f, 0f, 9f) / 9f * needleMaxMove925;
			needle.localPosition = needleMoveDirection * (move79 + move82 + move86 + move925);
		}
	}
}
